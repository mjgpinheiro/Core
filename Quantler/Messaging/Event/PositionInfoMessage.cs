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
using Quantler.Trades;
using System;
using Quantler.Account.Cash;
using Quantler.Securities;

namespace Quantler.Messaging.Event
{
    /// <summary>
    /// Position update event message
    /// </summary>
    /// <seealso cref="EventMessage" />
    public class PositionInfoMessage : EventMessage
    {
        #region Public Properties

        /// <summary>
        /// Average position price (weighted average)
        /// </summary>
        public decimal AveragePrice { get; set; }

        /// <summary>
        /// Base currency of position
        /// </summary>
        public CurrencyType Currency { get; set; }

        /// <summary>
        /// Current asset price
        /// </summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// Current position direction
        /// </summary>
        public Direction Direction { get; set; }

        /// <summary>
        /// Last date and time this position was modified
        /// </summary>
        public DateTime LastModifiedUtc { get; set; }

        /// <summary>
        /// Position margin in use
        /// </summary>
        public decimal MarginInUse { get; set; }

        /// <summary>
        /// Current open profit and loss
        /// </summary>
        public decimal NetProfit { get; set; }

        /// <summary>
        /// Current position quantity
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Position ticker symbol
        /// </summary>
        public string Ticker { get; set; }

        /// <summary>
        /// Current position weight in the portfolio
        /// </summary>
        public decimal CurrentWeight { get; set; }

        /// <summary>
        /// Position weight in the universe
        /// </summary>
        public decimal UniverseWeight { get; set; }

        /// <summary>
        /// Message type
        /// </summary>
        public override EventMessageType Type => EventMessageType.PositionInfo;

        #endregion Public Properties

        #region Private Properties

        /// <summary>
        /// Associated fund id
        /// </summary>
        private string FundId { get; set; }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Create position info message instance
        /// </summary>
        /// <param name="fundid"></param>
        /// <param name="pos"></param>
        /// <param name="universe"></param>
        /// <param name="funds"></param>
        /// <returns></returns>
        public static PositionInfoMessage Create(string fundid, Position pos, Universe universe, CalculatedFunds funds)
        {
            return new PositionInfoMessage
            {
                AveragePrice = pos.AveragePrice,
                Currency = pos.Security.BaseCurrency,
                CurrentPrice = pos.CurrentPrice,
                Direction = pos.Direction,
                FundId = fundid,
                LastModifiedUtc = pos.LastModifiedUtc,
                MarginInUse = pos.MarginInUse,
                NetProfit = pos.NetProfit,
                Quantity = pos.Quantity,
                Ticker = pos.Security.Ticker.Name,
                CurrentWeight = pos.TotalValue / funds.Equity,
                UniverseWeight = universe.GetWeight(pos.Security)
            };
        }

        /// <summary>
        /// True if this message equals another message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override bool Equals(EventMessage message)
        {
            if (message is PositionInfoMessage)
            {
                var instance = message as PositionInfoMessage;
                return instance.NetProfit == NetProfit &&
                        instance.MarginInUse == MarginInUse &&
                        instance.UniqueId == UniqueId;
            }
            else
                return false;
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Generate unique id
        /// </summary>
        /// <returns></returns>
        protected override string GetUniqueId() => FundId + Ticker;

        #endregion Protected Methods
    }
}