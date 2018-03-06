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
using Quantler.Securities;
using System;
using System.Collections.Generic;

namespace Quantler.Orders.Type
{
    /// <summary>
    /// Base order class, order types are implemented from this base class
    /// </summary>
    public abstract class OrderImpl : Order
    {
        #region Protected Constructors

        /// <summary>
        /// Base order implementation
        /// </summary>
        /// <param name="security"></param>
        /// <param name="fundid"></param>
        /// <param name="quantity"></param>
        /// <param name="createdutc"></param>
        /// <param name="exchange"></param>
        /// <param name="comment"></param>
        protected OrderImpl(Security security, string fundid, decimal quantity, DateTime createdutc, string exchange, string comment = "")
        {
            FundId = fundid;
            Security = security;
            Quantity = quantity;
            Comment = comment;
            ExchangeName = exchange;
            CreatedUtc = createdutc;
            Type = OrderType.Market; //Default fall back to market order
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderImpl"/> class.
        /// </summary>
        protected OrderImpl()
        {
            
        }

        #endregion Protected Constructors

        #region Public Properties

        /// <summary>
        /// Broker based order id
        /// </summary>
        public List<string> BrokerId { get; set; }

        /// <summary>
        /// Any comments associated to this order
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Time and date this order was created (in utc)
        /// </summary>
        public DateTime CreatedUtc { get; set; }

        /// <summary>
        /// Direction of order (long = buy, short = sell)
        /// </summary>
        public Direction Direction => Quantity > 0 ? Direction.Long : Direction.Short;

        /// <summary>
        /// Associated exchangeModel on which this order should be matched (optional)
        /// </summary>
        public string ExchangeName { get; set; }

        /// <summary>
        /// If applicable, when this order should expire
        /// </summary>
        public DateTime? Expiry { get; set; }

        /// <summary>
        /// Fill policy that was used, currently only supports immediate fills
        /// </summary>
        public FillPolicy FillPolicy => FillPolicy.Immediate;

        /// <summary>
        /// Associated fund id for this order
        /// </summary>
        public string FundId { get; set; }

        /// <summary>
        /// Internal order id
        /// </summary>
        public long InternalId { get; set; }

        /// <summary>
        /// Limit price, if applicable to order type
        /// </summary>
        public decimal LimitPrice { get; set; }

        /// <summary>
        /// Current order quantity
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Associated security
        /// </summary>
        public Security Security { get; set; }

        /// <summary>
        /// Order state
        /// </summary>
        public OrderState State { get; set; }

        /// <summary>
        /// Current stop price, if applicable to this order type
        /// </summary>
        public decimal StopPrice { get; set; }

        /// <summary>
        /// Currently only GTC orders are supported
        /// </summary>
        public TimeInForce TimeInForceSpec => TimeInForce.GTC;

        /// <summary>
        /// Current order type
        /// </summary>
        public OrderType Type { get; set; }

        /// <summary>
        /// Quantity unsigned
        /// </summary>
        public decimal UnsignedQuantity => Math.Abs(Quantity);

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Clone the current order object
        /// </summary>
        public abstract Order Clone();

        /// <summary>
        /// Update current order
        /// </summary>
        /// <param name="ticket"></param>
        public virtual void Update(UpdateOrderTicket ticket)
        {
            //Check if we have the correct order to update
            if (ticket.OrderId != InternalId)
                throw new ArgumentException("Invalid order for update");

            //Check size adjustments
            if (ticket.Quantity.HasValue)
                Quantity = ticket.Quantity.Value;

            //Check comment
            if (!string.IsNullOrWhiteSpace(ticket.Comment))
                Comment = ticket.Comment;
        }

        /// <summary>
        /// Update order state
        /// </summary>
        /// <param name="state"></param>
        public void Update(OrderState state) =>
            State = state;

        /// <summary>
        /// Set the internal order id
        /// </summary>
        /// <param name="orderid"></param>
        internal void SetInternalOrderId(long orderid) =>
            InternalId = orderid;

        /// <summary>
        /// Copies base Order properties to the specified order
        /// </summary>
        /// <param name="order">The target of the copy</param>
        protected void CopyTo(OrderImpl order)
        {
            order.InternalId = InternalId;
            order.Type = Type;
            order.Security = Security;
            order.BrokerId = BrokerId;
            order.Comment = Comment;
            order.CreatedUtc = CreatedUtc;
            order.ExchangeName = ExchangeName;
            order.Expiry = Expiry;
            order.FundId = FundId;
            order.LimitPrice = LimitPrice;
            order.Quantity = Quantity;
            order.StopPrice = StopPrice;
            order.State = State;
        }

        #endregion Public Methods
    }
}