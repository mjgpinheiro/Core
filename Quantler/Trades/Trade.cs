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

using Quantler.Interfaces;
using Quantler.Orders;
using Quantler.Securities;
using System;
using System.Linq;

namespace Quantler.Trades
{
    /// <summary>
    /// Trade, open and close fill combined
    /// </summary>
    public class Trade
    {
        #region Public Constructors

        /// <summary>
        /// Create a trade using multiple fills
        /// </summary>
        /// <param name="displaycurrency"></param>
        /// <param name="open"></param>
        /// <param name="close"></param>
        /// <param name="minprice"></param>
        /// <param name="maxprice"></param>
        public Trade(CurrencyType displaycurrency, Fill[] open, Fill[] close, decimal minprice, decimal maxprice) =>
            ProcessFills("", displaycurrency, open, close, minprice, maxprice);

        /// <summary>
        /// Create a trade using multiple fills
        /// </summary>
        /// <param name="fundid"></param>
        /// <param name="displaycurrency"></param>
        /// <param name="open"></param>
        /// <param name="close"></param>
        /// <param name="minprice"></param>
        /// <param name="maxprice"></param>
        public Trade(string fundid, CurrencyType displaycurrency, Fill[] open, Fill[] close, decimal minprice, decimal maxprice) =>
            ProcessFills(fundid, displaycurrency, open, close, minprice, maxprice);

        /// <summary>
        /// Create a trade for a trading agent
        /// </summary>
        /// <param name="fundid"></param>
        /// <param name="displaycurrency"></param>
        /// <param name="open"></param>
        /// <param name="close"></param>
        /// <param name="minprice"></param>
        /// <param name="maxprice"></param>
        public Trade(string fundid, CurrencyType displaycurrency, Fill open, Fill close, decimal minprice, decimal maxprice) =>
            ProcessFills(fundid, displaycurrency, new[] { open }, new[] { close }, minprice, maxprice);

        /// <summary>
        /// Create a fill without knowing the origins trading agent for this order
        /// </summary>
        /// <param name="displaycurrency"></param>
        /// <param name="open"></param>
        /// <param name="close"></param>
        /// <param name="minprice"></param>
        /// <param name="maxprice"></param>
        public Trade(CurrencyType displaycurrency, Fill open, Fill close, decimal minprice, decimal maxprice) =>
            ProcessFills("", displaycurrency, new[] { open }, new[] { close }, minprice, maxprice);

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Price at which this trade was closed
        /// </summary>
        public decimal ClosedPrice
        {
            private set;
            get;
        }

        /// <summary>
        /// Date and time this trade was closed in utc
        /// </summary>
        public DateTime ClosedUtc
        {
            private set;
            get;
        }

        /// <summary>
        /// Gets the total value of this trade in account base currency, during close
        /// </summary>
        public decimal ClosedValue
        {
            private set;
            get;
        }

        /// <summary>
        /// Gets the currency conversion used for converting from base to display currency
        /// </summary>
        public decimal CurrencyConversion
        {
            private set;
            get;
        }

        /// <summary>
        /// Direction of this trade
        /// </summary>
        public Direction Direction
        {
            private set;
            get;
        }

        /// <summary>
        /// The display currency used
        /// </summary>
        public CurrencyType DisplayCurrency
        {
            private set;
            get;
        }

        /// <summary>
        /// Duration this trade was open for
        /// </summary>
        public TimeSpan Duration
        {
            private set;
            get;
        }

        /// <summary>
        /// Associated fees for this trade
        /// </summary>
        public decimal Fees
        {
            private set;
            get;
        }

        /// <summary>
        /// Associated fund id
        /// </summary>
        public string FundId
        {
            private set;
            get;
        }

        /// <summary>
        /// PnL excluding fees for this trade
        /// </summary>
        public decimal GrossPnL
        {
            private set;
            get;
        }

        /// <summary>
        /// Maximum Adverse Excursion
        /// </summary>
        public decimal MAE
        {
            private set;
            get;
        }

        /// <summary>
        /// Maximum Favorable Excursion
        /// </summary>
        public decimal MFE
        {
            private set;
            get;
        }

        /// <summary>
        /// PnL including fees for this trade
        /// </summary>
        public decimal NetPnL
        {
            private set;
            get;
        }

        /// <summary>
        /// Date and time this trade was opened in utc
        /// </summary>
        public DateTime OpenedUtc
        {
            private set;
            get;
        }

        /// <summary>
        /// Price at which this trade was opened
        /// </summary>
        public decimal OpenPrice
        {
            private set;
            get;
        }

        /// <summary>
        /// Associated size for this trade
        /// </summary>
        public decimal Quantity
        {
            private set;
            get;
        }

        /// <summary>
        /// Associated security for this trade
        /// </summary>
        public Security Security
        {
            private set;
            get;
        }

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// Process input
        /// </summary>
        /// <param name="fundid"></param>
        /// <param name="displaycurrency"></param>
        /// <param name="open"></param>
        /// <param name="close"></param>
        /// <param name="minprice"></param>
        /// <param name="maxprice"></param>
        private void ProcessFills(string fundid, CurrencyType displaycurrency, Fill[] open, Fill[] close, decimal minprice, decimal maxprice)
        {
            //Perform checks
            if (open.Sum(x => x.FillQuantity) != close.Sum(x => x.FillQuantity))
                throw new ArgumentException("Cannot process uneven fill size for creating a trade");
            else if (open.Length == 0 || close.Length == 0)
                throw new ArgumentException("Cannot process trade due to missing fills");

            //Try get security
            Security = open[0].Security;

            //Add input
            FundId = fundid;
            ClosedPrice = close.WeightedAverage(x => x.FillPrice, x => x.FillQuantity);
            ClosedUtc = close.Max(x => x.UtcTime);
            Direction = open.First().Direction;
            Fees = Security.ConvertValue(open.Sum(x => x.FillFee) + close.Sum(x => x.FillFee), displaycurrency);
            OpenedUtc = open.Min(x => x.UtcTime);
            OpenPrice = open.WeightedAverage(x => x.FillPrice, x => x.FillQuantity);
            Quantity = close.Sum(x => x.FillQuantity);
            Duration = ClosedUtc - OpenedUtc;
            GrossPnL = Security.ConvertValue((Direction == Direction.Long ? ClosedPrice - OpenPrice : OpenPrice - ClosedPrice) * Quantity, displaycurrency);
            NetPnL = GrossPnL - Fees;
            MAE = Direction == Direction.Long ? minprice : maxprice;
            MFE = Direction == Direction.Short ? minprice : maxprice;
            ClosedValue = Security.ConvertValue((ClosedPrice * Quantity), displaycurrency);
            DisplayCurrency = displaycurrency;
            CurrencyConversion = Security.ConvertValue(1, displaycurrency);
        }

        #endregion Private Methods
    }
}