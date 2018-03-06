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

using Quantler.Securities;
using System;

namespace Quantler.Orders
{
    /// <summary>
    /// Ticket for creating a new order, order type is derived from the provided information
    /// </summary>
    public class SubmitOrderTicket : OrderTicket
    {
        #region Private Fields

        /// <summary>
        /// The order type
        /// </summary>
        private OrderType _orderType;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Submit new order
        /// </summary>
        /// <param name="fundid"></param>
        /// <param name="security"></param>
        public SubmitOrderTicket(string fundid, Security security)
            : base(fundid, security, -1) => Type = OrderTicketType.Submit;

        /// <summary>
        /// Submit new order based on input
        /// </summary>
        /// <param name="fundid">The fund.</param>
        /// <param name="security">The security.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="stopprice">The stopprice.</param>
        /// <param name="limitprice">The limitprice.</param>
        /// <param name="ordertype">The ordertype.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="exchange">The exchangeModel.</param>
        public SubmitOrderTicket(string fundid, Security security, decimal quantity, decimal stopprice = 0m, decimal limitprice = 0m, OrderType ordertype = OrderType.Market, string comment = "", string exchange = "")
            : base(fundid, security, -1)
        {
            Quantity = quantity;
            StopPrice = stopprice;
            LimitPrice = limitprice;
            _orderType = ordertype;
            Comment = comment;
            ExchangeName = exchange;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Comments associated to this order
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Exchange at which this order should be processed (optional)
        /// </summary>
        public string ExchangeName { get; set; }

        /// <summary>
        /// Date and time at which this order should expire
        /// </summary>
        public DateTime? Expiry { get; set; }

        /// <summary>
        /// Associated fill policy
        /// </summary>
        public FillPolicy FillPolicy { get; set; }

        /// <summary>
        /// Limit price (optional)
        /// </summary>
        public decimal LimitPrice { get; set; }

        /// <summary>
        /// Derived order type
        /// </summary>
        public OrderType OrderType
        {
            get => _orderType == OrderType.Market ? DeriveOrderType : _orderType;
            set => _orderType = value;
        }

        /// <summary>
        /// Quantity of order
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Stop price for this order (optional)
        /// </summary>
        public decimal StopPrice { get; set; }

        /// <summary>
        /// Time in force for the order
        /// </summary>
        public TimeInForce TimeInForce { get; set; }

        #endregion Public Properties

        #region Private Properties

        /// <summary>
        /// Derives the type of the order.
        /// </summary>
        /// <returns></returns>
        private OrderType DeriveOrderType =>
                StopPrice == 0 && LimitPrice == 0 ? OrderType.Market :
                StopPrice > 0 && LimitPrice > 0 ? OrderType.StopLimit :
                LimitPrice > 0 ? OrderType.Limit :
                StopPrice > 0 ? OrderType.StopMarket :
                OrderType.Market;

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Create a new limit order
        /// </summary>
        /// <param name="fundid">The fund.</param>
        /// <param name="security">The security.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="limitprice">The limitprice.</param>
        /// <param name="comment"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public static SubmitOrderTicket LimitOrder(string fundid, Security security, decimal quantity, decimal limitprice, string comment = "", string exchange = "") =>
            new SubmitOrderTicket(fundid, security, quantity, limitprice: limitprice, ordertype: OrderType.Limit, comment: comment, exchange: exchange);

        /// <summary>
        /// Markets the on close order.
        /// </summary>
        /// <param name="fundid">The fund.</param>
        /// <param name="security">The security.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="exchange">The exchangeModel.</param>
        /// <returns></returns>
        public static SubmitOrderTicket MarketOnCloseOrder(string fundid, Security security, decimal quantity, string comment = "", string exchange = "") =>
            new SubmitOrderTicket(fundid, security, quantity, ordertype: OrderType.MarketOnClose, comment: comment, exchange: exchange);

        /// <summary>
        /// Markets the on open order.
        /// </summary>
        /// <param name="fundid">The fund.</param>
        /// <param name="security">The security.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="exchange">The exchangeModel.</param>
        /// <returns></returns>
        public static SubmitOrderTicket MarketOnOpenOrder(string fundid, Security security, decimal quantity, string comment = "", string exchange = "") =>
            new SubmitOrderTicket(fundid, security, quantity, ordertype: OrderType.MarketOnOpen, comment: comment, exchange: exchange);

        /// <summary>
        /// Create a new market order
        /// </summary>
        /// <param name="fundid">The fund.</param>
        /// <param name="security">The security.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="comment"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public static SubmitOrderTicket MarketOrder(string fundid, Security security, decimal quantity, string comment = "", string exchange = "") =>
            new SubmitOrderTicket(fundid, security, quantity, comment: comment, exchange: exchange);

        /// <summary>
        /// Create a new stop limit order
        /// </summary>
        /// <param name="fundid">The fund.</param>
        /// <param name="security">The security.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="limitprice">The limitprice.</param>
        /// <param name="stopprice">The stopprice.</param>
        /// <param name="comment"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public static SubmitOrderTicket StopLimitOrder(string fundid, Security security, decimal quantity, decimal limitprice, decimal stopprice, string comment = "", string exchange = "") =>
            new SubmitOrderTicket(fundid, security, quantity, limitprice: limitprice, stopprice: stopprice, ordertype: OrderType.StopLimit);

        /// <summary>
        /// Create a new stop order
        /// </summary>
        /// <param name="fundid">The fund.</param>
        /// <param name="security">The security.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="stopprice">The stopprice.</param>
        /// <param name="comment"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public static SubmitOrderTicket StopMarketOrder(string fundid, Security security, decimal quantity, decimal stopprice, string comment = "", string exchange = "") =>
            new SubmitOrderTicket(fundid, security, quantity, stopprice, ordertype: OrderType.StopMarket);

        /// <summary>
        /// Cancel this submit order
        /// </summary>
        public void Cancel() => State = OrderTicketState.Processed;

        #endregion Public Methods
    }
}