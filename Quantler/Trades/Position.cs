#region License Header

/*
* QUANTLER.COM - Quant Fund Development Platform
* Quantler Core Trading Engine. Copyright 2018 Quantler B.V.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

#endregion License Header

using MoreLinq;
using Quantler.Interfaces;
using Quantler.Orders;
using Quantler.Securities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quantler.Trades
{
    /// <summary>
    /// Position holder for simulated trading
    /// TODO: some values can be recalculated when adjusting this position instead of recalculating it (faster to do on adjust) => is improvement
    /// </summary>
    public class Position
    {
        #region Private Fields

        /// <summary>
        /// Base currency to relate to (account currency)
        /// </summary>
        private readonly CurrencyType _baseCurrency;

        /// <summary>
        /// The display currency 
        /// </summary>
        private readonly CurrencyType _displayCurrency;

        /// <summary>
        /// Associated fund id
        /// </summary>
        private readonly string _fundId;

        /// <summary>
        /// Associated fills
        /// </summary>
        private readonly List<Fill> _fills = new List<Fill>();

        /// <summary>
        /// Current amount of interest paid
        /// TODO: implement interest
        /// </summary>
        private Dictionary<Fill, decimal> _interestPaid = new Dictionary<Fill, decimal>();

        /// <summary>
        /// Holder for min and max prices
        /// </summary>
        private readonly Dictionary<Fill, MmHolder> _mmPrices = new Dictionary<Fill, MmHolder>();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Default implementation for keeping track of current position values
        /// </summary>
        /// <param name="fundid"></param>
        /// <param name="displaycurrency"></param>
        /// <param name="basecurrency"></param>
        public Position(string fundid, CurrencyType displaycurrency, CurrencyType basecurrency, Security security)
        {
            _displayCurrency = displaycurrency;
            _baseCurrency = basecurrency;
            _fundId = fundid;
            Security = security;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Associated fills
        /// </summary>
        public IEnumerable<Fill> AssociatedFills => _fills;

        /// <summary>
        /// Weighted average price of this position
        /// </summary>
        public decimal AveragePrice => AssociatedFills.WeightedAverage(x => x.FillPrice, x => x.FillQuantity);

        /// <summary>
        /// Current security price
        /// </summary>
        public decimal CurrentPrice => Security.Price;

        /// <summary>
        /// Direction of current position
        /// </summary>
        public Direction Direction => AssociatedFills.All(x => x.Direction == Direction.Long) ? Direction.Long : Direction.Short;

        /// <summary>
        /// Quantity needed to be flat
        /// </summary>
        public decimal FlatQuantity => -Quantity;

        /// <summary>
        /// PnL without costs
        /// </summary>
        public decimal GrossProfit => AveragePrice * UnsignedQuantity;

        /// <summary>
        /// True if position is flat
        /// </summary>
        public bool IsFlat => Quantity == 0;

        /// <summary>
        /// True if position is long
        /// </summary>
        public bool IsLong => Quantity > 0;

        /// <summary>
        /// True if position is short
        /// </summary>
        public bool IsShort => Quantity < 0;

        /// <summary>
        /// Date and time this position was last modified
        /// </summary>
        public DateTime LastModifiedUtc
        {
            private set;
            get;
        }

        /// <summary>
        /// Margin used (total value)
        /// </summary>
        public decimal MarginInUse => TotalValue;

        /// <summary>
        /// PnL with costs incurred
        /// </summary>
        public decimal NetProfit => GrossProfit - TotalFees;

        /// <summary>
        /// Current position quantity
        /// </summary>
        public decimal Quantity => Direction == Direction.Short ? -AssociatedFills.Sum(x => x.FillQuantity) : AssociatedFills.Sum(x => x.FillQuantity);

        /// <summary>
        /// Associated position security
        /// </summary>
        public Security Security { get; }

        /// <summary>
        /// Total amount of commissions paid for this position
        /// </summary>
        public decimal TotalCommission => AssociatedFills.Sum(x => x.FillFee);

        /// <summary>
        /// Total amount of fees paid for this position (commission and interest)
        /// </summary>
        public decimal TotalFees => TotalCommission + TotalInterest;

        /// <summary>
        /// Total amount of interest paid for this position
        /// </summary>
        public decimal TotalInterest => 0m;

        /// <summary>
        /// Total value of this position in account base currency
        /// </summary>
        public decimal TotalValue => Security.ConvertValue(AssociatedFills.Sum(x => x.FillValue), _baseCurrency);

        /// <summary>
        /// Absolute size of posiion
        /// </summary>
        public decimal UnsignedQuantity => Math.Abs(Quantity);

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Process fill to adjust position
        /// Returns null if this fill does not close anything, thus not creating a trade
        /// TODO: protect from misuse (internal)?
        /// TODO: set creation of trades in the tradecomposite logic for extensibility
        /// </summary>
        /// <param name="fill"></param>
        /// <returns></returns>
        public Trade Adjust(Fill fill)
        {
            //Set last modified
            LastModifiedUtc = fill.UtcTime;

            //Some cleaning up
            ClearOldPrices();

            //Some information holders
            Fill[] opened = new Fill[0];
            Fill[] closed = { fill };

            //Check  if we have no position at all
            if (IsFlat)
            {
                _fills.Clear();
                _fills.Add(fill);
            }

            //Check current position if we might have flipped it
            else if (fill.Direction != Direction && fill.FillQuantity >= UnsignedQuantity)
            {
                //Get open fill
                opened = _fills.ToArray();

                //Get closed fill
                closed = new [] { Fill.AdjustedFill(fill, fill.FillPrice, UnsignedQuantity) };

                //Set new fill
                _fills.Clear();
                fill = Fill.AdjustedFill(fill, fill.FillPrice, fill.FillQuantity - UnsignedQuantity);
                _fills.Add(fill);

                //Return trade
                return new Trade(_fundId, _displayCurrency, opened, closed, GetPrice(true, opened.Union(closed)), GetPrice(false, opened.Union(closed)));
            }

            //Check if we are adding to an existing position (FIFO)
            else if (fill.Direction == Direction)
            {
                _fills.Add(fill);
                return null;
            }

            //Else we need to adjust according to fill and remove current fills from this position (FIFO)
            decimal sizeneeded = fill.FillQuantity;
            List<Fill> opening = new List<Fill>();
            for (int i = _fills.Count - 1; i >= 0; i--)
            {
                //Get items
                var currentfill = _fills[i];
                if (currentfill.FillQuantity > sizeneeded)
                {
                    //We need to split it
                    decimal leftover = currentfill.FillQuantity - sizeneeded;
                    _fills[i] = Fill.AdjustedFill(currentfill, currentfill.FillPrice, leftover);
                    currentfill = Fill.AdjustedFill(currentfill, currentfill.FillPrice, sizeneeded);
                }
                else
                    //Remove this fill
                    _fills.RemoveAt(i);

                //Add to currently closing fill
                opening.Add(currentfill);
                sizeneeded -= currentfill.FillQuantity;

                //Check if we are ok
                if (sizeneeded == 0)
                    break;
            }

            //Return this filled trade
            opened = opening.ToArray();
            return new Trade(_fundId, _displayCurrency, opened, closed, GetPrice(true, opened.Union(closed)), GetPrice(false, opened.Union(closed)));
        }

        /// <summary>
        /// Adjust current prices
        /// TODO: protect from misue (internal)?
        /// TODO: set in flow
        /// </summary>
        public void AdjustPrices()
        {
            //Get midpoint price
            decimal price = Security.Price;

            //Set for all prices
            _mmPrices.ForEach(x =>
            {
                x.Value.MinPrice = x.Value.MinPrice > price ? price : x.Value.MinPrice;
                x.Value.MaxPrice = x.Value.MaxPrice < price ? price : x.Value.MaxPrice;
            });
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Remove old prices, those who are no longer in use
        /// </summary>
        private void ClearOldPrices() =>
            _mmPrices.Select(x => x.Key)
                    .Where(x => !_fills.Contains(x))
                    .Select(x => _mmPrices.Remove(x));

        /// <summary>
        /// Get the min or max price during the holding period of this fill
        /// </summary>
        /// <param name="ismin"></param>
        /// <param name="fills"></param>
        /// <returns></returns>
        private decimal GetPrice(bool ismin, IEnumerable<Fill> fills) =>
            ismin ? fills.Select(x => _mmPrices[x]).Min(x => x.MinPrice) :
                    fills.Select(x => _mmPrices[x]).Max(x => x.MaxPrice);

        #endregion Private Methods

        #region Private Classes

        /// <summary>
        /// Holds information for min and max prices during holding a fill
        /// </summary>
        private class MmHolder
        {
            #region Public Properties

            /// <summary>
            /// Measured max price
            /// </summary>
            public decimal MaxPrice { get; set; }

            /// <summary>
            /// Measured min price
            /// </summary>
            public decimal MinPrice { get; set; }

            #endregion Public Properties
        }

        #endregion Private Classes
    }
}