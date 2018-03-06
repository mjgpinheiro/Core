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

namespace Quantler.Messaging.Event
{
    /// <summary>
    /// Order info event message
    /// </summary>
    /// <seealso cref="EventMessage" />
    public class OrderInfoMessage : EventMessage
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the base currency of this order
        /// </summary>
        public CurrencyType Currency { get; set; }

        /// <summary>
        /// Gets or sets the direction of this order
        /// </summary>
        public Direction Direction { get; set; }

        /// <summary>
        /// Gets or sets the quant fund identifier.
        /// </summary>
        public string FundId { get; set; }

        /// <summary>
        /// Gets or sets the name of the fund.
        /// </summary>
        public string FundName { get; set; }

        /// <summary>
        /// Gets or sets the identifier of this order
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the last modified date and time in local exchange time.
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Gets or sets the last modified date and time in utc time.
        /// </summary>
        public DateTime LastModifiedUtc { get; set; }

        /// <summary>
        /// Gets or sets the limit price of this order
        /// </summary>
        public decimal LimitPrice { get; set; }

        /// <summary>
        /// Gets or sets the type of the order.
        /// </summary>
        public OrderType OrderType { get; set; }

        /// <summary>
        /// Gets or sets the portfolio identifier.
        /// </summary>
        public string PortfolioId { get; set; }

        /// <summary>
        /// Gets or sets the quantity for this order
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets the status of this order
        /// </summary>
        public OrderState Status { get; set; }

        /// <summary>
        /// Gets or sets the stop price of this order
        /// </summary>
        public decimal StopPrice { get; set; }

        /// <summary>
        /// Gets or sets the ticker symbol
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Gets or sets the average fill price.
        /// </summary>
        public decimal AverageFillPrice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this order type is simulated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is simulated; otherwise, <c>false</c>.
        /// </value>
        public bool IsSimulated { get; set; }

        /// <summary>
        /// Type of message
        /// </summary>
        public override EventMessageType Type => EventMessageType.OrderInfo;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Create new order informatio instance
        /// </summary>
        /// <param name="pendingorder"></param>
        /// <param name="fundname"></param>
        /// <param name="portfolioid"></param>
        /// <returns></returns>
        public static OrderInfoMessage Create(PendingOrder pendingorder, string portfolioid, string fundname)
        {
            return new OrderInfoMessage
            {
                AverageFillPrice = pendingorder.AverageFillPrice,
                Currency = pendingorder.Security.BaseCurrency,
                Direction = pendingorder.Order.Direction,
                FundId = pendingorder.FundId,
                FundName = fundname,
                Id = pendingorder.OrderId,
                LastModified = pendingorder.Order.CreatedUtc.ConvertTo(TimeZone.Utc, pendingorder.Security.Exchange.TimeZone),
                LastModifiedUtc = pendingorder.Order.CreatedUtc,
                LimitPrice = pendingorder.Order.LimitPrice,
                OrderType = pendingorder.Order.Type,
                PortfolioId = portfolioid,
                Quantity = pendingorder.OrderQuantity,
                Status = pendingorder.OrderState,
                StopPrice = pendingorder.Order.StopPrice,
                Symbol = pendingorder.Security.Ticker.Name,
                IsSimulated = pendingorder.IsSimulated
            };
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Get order unique id
        /// </summary>
        /// <returns></returns>
        protected override string GetUniqueId() =>
            PortfolioId + FundId + Id;

        #endregion Protected Methods
    }
}