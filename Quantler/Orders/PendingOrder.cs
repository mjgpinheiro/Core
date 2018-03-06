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
using System.Linq;

namespace Quantler.Orders
{
    /// <summary>
    /// Pending order implementation
    /// </summary>
    public class PendingOrder
    {
        #region Private Fields

        /// <summary>
        /// Associated fills
        /// </summary>
        private readonly List<Fill> _fills = new List<Fill>();

        /// <summary>
        /// Current broker connection
        /// </summary>
        private readonly OrderTicketHandler _orderTicketHandler;

        /// <summary>
        /// The portfolio
        /// </summary>
        private readonly IPortfolio _portfolio;

        /// <summary>
        /// Recent order updates holder
        /// </summary>
        private readonly List<OrderTicket> _orderupdates = new List<OrderTicket>();

        /// <summary>
        /// Attached order
        /// </summary>
        private Order _attachedOrder;

        /// <summary>
        /// Locking object for order mutation actions (cancel/update)
        /// </summary>
        private readonly object _ordermutationlock = new object();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initialize new pending order
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="fundid"></param>
        /// <param name="order"></param>
        /// <param name="comment"></param>
        /// <param name="createdutc"></param>
        /// <param name="issimulated">If true, order behavior is simulated</param>
        public PendingOrder(IPortfolio portfolio, string fundid, Order order, string comment, DateTime createdutc, bool issimulated = false)
        {
            _portfolio = portfolio;
            _orderTicketHandler = portfolio.OrderTicketHandler;
            FundId = fundid;
            Order = order;
            CreatedUtc = createdutc;
            IsSimulated = issimulated;
            Comment = comment;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Get the weighted average price at which this order was filled
        /// </summary>
        public decimal AverageFillPrice =>
            OrderFills.WeightedAverage(x => x.FillPrice, x => x.FillQuantity);

        /// <summary>
        /// If this order was canceled, it will have a cancel order ticket
        /// </summary>
        public OrderTicket CancelOrderTicket
        {
            private set;
            get;
        }

        /// <summary>
        /// Comments attached to this pending order
        /// </summary>
        public string Comment { get; }

        /// <summary>
        /// Date and time in utc this pending order was created
        /// </summary>
        public DateTime CreatedUtc { get; }

        /// <summary>
        /// Current value filled, in base currency
        /// </summary>
        public decimal FilledValue =>
            Security.ConvertValue(OrderFills.Sum(x => x.FillValue), _portfolio.BrokerAccount.Currency);

        /// <summary>
        /// Fund id for which this pending order belongs to
        /// </summary>
        public string FundId { get; }

        /// <summary>
        /// True, if this order has been send to the broker
        /// </summary>
        public bool IsOrderSend =>
            (int)Order.State >= 1;

        /// <summary>
        /// Internally simulated order behavior
        /// </summary>
        public bool IsSimulated { get; }

        /// <summary>
        /// Associated order
        /// </summary>
        public Order Order
        {
            private set => _attachedOrder = value;
            get => _attachedOrder.Clone();
        }

        /// <summary>
        /// Returns the sum of fees applied to fills
        /// </summary>
        public decimal OrderFees =>
            _fills.Sum(x => x.FillFee);

        /// <summary>
        /// Sum of the order that has been filled so far
        /// </summary>
        public decimal OrderFilledQuantity =>
            OrderFills.Sum(x => x.FillQuantity);

        /// <summary>
        /// Return current fills
        /// </summary>
        public Fill[] OrderFills =>
            _fills.ToArray();

        /// <summary>
        /// Order id attached to this order
        /// </summary>
        public long OrderId => Order.InternalId;

        /// <summary>
        /// Associated order quantity
        /// </summary>
        public decimal OrderQuantity =>
            Order.Quantity;

        /// <summary>
        /// Current order state
        /// </summary>
        public OrderState OrderState =>
            Order.State;

        /// <summary>
        /// Order updates received
        /// </summary>
        public OrderTicket[] OrderUpdates =>
            _orderupdates.ToArray();

        /// <summary>
        /// Security associated with this order
        /// </summary>
        public Security Security =>
            Order.Security;

        /// <summary>
        /// Returns total order value (based on the latest price received)
        /// </summary>
        public decimal Value =>
            Security.ConvertValue(Order.UnsignedQuantity * Security.Price, _portfolio.BrokerAccount.Currency);

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Add fill to this pending order
        /// TODO: needs to be internal and protected
        /// </summary>
        /// <param name="fill"></param>
        public void AddFill(Fill fill)
        {
            lock (_ordermutationlock)
            {
                _fills.Add(fill);
            }
        }

        /// <summary>
        /// Cancel this order, will be processed right away
        /// </summary>
        /// <returns></returns>
        public OrderTicket Cancel()
        {
            if (CancelOrderTicket == null)
            {
                //Set cancel order ticket
                CancelOrderTicket = new CancelOrderTicket(FundId, Security, OrderId);

                //Process
                _orderTicketHandler.Process(CancelOrderTicket);

                //Return
                return CancelOrderTicket;
            }
            else
                return CancelOrderTicket;
        }

        /// <summary>
        /// Update this order, will be processed right away
        /// </summary>
        /// <param name="updateticket"></param>
        /// <returns></returns>
        public OrderTicket Update(OrderTicket updateticket)
        {
            //Get update order ticket
            UpdateOrderTicket ticket;
            if (updateticket.Type == OrderTicketType.Update)
                ticket = updateticket as UpdateOrderTicket;
            else
            {
                updateticket.SetResponse(OrderTicketResponse.Error(OrderId, OrderTicketResponseErrorCode.InvalidRequest, $"Could not process update request, unexpected type cast"));
                return updateticket;
            }

            //TODO: this should take place at order ticket handler
            lock (_ordermutationlock)
                //Set to known updates
                _orderupdates.Add(updateticket);

            //Send update order request to handler
            _orderTicketHandler.Process(updateticket);
            return updateticket;
        }

        /// <summary>
        /// Update this order, will be processed right away
        /// </summary>
        /// <param name="orderupdates">The orderupdates.</param>
        /// <returns></returns>
        public OrderTicket Update(Action<OrderUpdate> orderupdates)
        {
            //Set new update order ticket
            UpdateOrderTicket nticket = new UpdateOrderTicket(FundId, Security, OrderId);

            //Set changes we request
            nticket.Process(orderupdates);

            //Send update ticket to pending order
            return Update(nticket);
        }

        /// <summary>
        /// Update current order
        /// TODO: needs to be internal and protected
        /// </summary>
        /// <param name="order"></param>
        public void UpdateOrder(Order order)
        {
            //Check if have the correct order
            if (order == null || order.InternalId != Order.InternalId)
                throw new Exception("Order does not correspond to current attached order");

            lock (_ordermutationlock)
            {
                Order = order;
            }
        }

        #endregion Public Methods
    }
}