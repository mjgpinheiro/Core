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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quantler.Messaging.Event
{
    /// <summary>
    /// Pending order event message update
    /// </summary>
    /// <seealso cref="EventMessage" />
    public class PendingOrderInfoMessage : EventMessage
    {
        #region Public Properties

        /// <summary>
        /// Base currency type of pending order
        /// </summary>
        public CurrencyType Currency { get; set; }

        /// <summary>
        /// Order direction
        /// </summary>
        public Direction Direction { get; set; }

        /// <summary>
        /// Distance between current price and market hit (if limit or stop order)
        /// </summary>
        public decimal Distance { get; set; }

        /// <summary>
        /// Associated fund id
        /// </summary>
        public string FundId { get; set; }

        /// <summary>
        /// Associated fund name
        /// </summary>
        public string FundName { get; set; }

        /// <summary>
        /// Associated id of the pending order
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// If true, this order has been filled
        /// </summary>
        public bool IsFilled { get; set; }

        /// <summary>
        /// Last date and time this pending order was modified
        /// </summary>
        public DateTime LastModifiedUtc { get; set; }

        /// <summary>
        /// Pending order limit price set
        /// </summary>
        public decimal LimitPrice { get; set; }

        /// <summary>
        /// Associated order id
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// Derived order type
        /// </summary>
        public OrderType OrderType { get; set; }

        /// <summary>
        /// Associated portfolio id
        /// </summary>
        public string PortfolioId { get; set; }

        /// <summary>
        /// Set quantity for this order
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Associated stop price
        /// </summary>
        public decimal StopPrice { get; set; }

        /// <summary>
        /// Symbol for this order
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Message info type
        /// </summary>
        public override EventMessageType Type => EventMessageType.PendingOrderInfo;

        /// <summary>
        /// Historical pending order updates
        /// </summary>
        public Dictionary<long, string> Updates { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Generate a pending order event info message
        /// </summary>
        /// <param name="pendingorder"></param>
        /// <returns></returns>
        public static PendingOrderInfoMessage Generate(PendingOrder pendingorder, string portfolioid, string fundname)
        {
            return new PendingOrderInfoMessage
            {
                Currency = pendingorder.Security.BaseCurrency,
                Direction = pendingorder.Order.Direction,
                Distance = pendingorder.Order.Type != OrderType.Market ?
                            pendingorder.Order.Type == OrderType.Limit ? (pendingorder.Order.Direction == Direction.Long ? pendingorder.Order.LimitPrice - pendingorder.Security.BidPrice : pendingorder.Security.AskPrice - pendingorder.Order.LimitPrice) :
                            pendingorder.Order.Type == OrderType.StopMarket ? (pendingorder.Order.Direction == Direction.Long ? pendingorder.Order.StopPrice - pendingorder.Security.BidPrice : pendingorder.Security.AskPrice - pendingorder.Order.StopPrice) :
                            pendingorder.Order.Type == OrderType.StopLimit ? (pendingorder.Order.Direction == Direction.Long ? pendingorder.Order.StopPrice - pendingorder.Security.BidPrice : pendingorder.Security.AskPrice - pendingorder.Order.StopPrice) :
                            0m : 0m,
                FundId = pendingorder.FundId,
                FundName = fundname,
                Id = pendingorder.OrderId,
                IsFilled = pendingorder.OrderFilledQuantity == pendingorder.Order.UnsignedQuantity,
                LastModifiedUtc = pendingorder.OrderUpdates.Length == 0 ? pendingorder.CreatedUtc : pendingorder.OrderUpdates.Max(x => x.CreatedUtc),
                LimitPrice = pendingorder.Order.LimitPrice,
                OrderId = pendingorder.OrderId,
                OrderType = pendingorder.Order.Type,
                PortfolioId = portfolioid,
                Quantity = pendingorder.Order.Quantity,
                StopPrice = pendingorder.Order.StopPrice,
                Symbol = pendingorder.Security.Ticker.Name,
                Updates = pendingorder.OrderUpdates.ToDictionary(x => x.CreatedUtc.ToUnixTime(), x => x.Type.ToString())
            };
        }

        /// <summary>
        /// Check if this message equals any prev. message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override bool Equals(EventMessage message)
        {
            if (message is PendingOrderInfoMessage instance)
            {
                return instance.UniqueId == UniqueId &&
                        instance.Updates.Count == Updates.Count &&
                        instance.Distance == Distance;
            }
            else
                return false;
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Generate unique id for this message
        /// </summary>
        /// <returns></returns>
        protected override string GetUniqueId() =>
            FundId + Id.ToString();

        #endregion Protected Methods
    }
}