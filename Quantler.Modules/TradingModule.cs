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
using System.Collections.Generic;
using System.Linq;

namespace Quantler.Modules
{
    /// <summary>
    /// Used for modules that have trading capabilities
    /// </summary>
    public abstract class TradingModule : Module
    {
        #region Protected Properties

        /// <summary>
        /// Gets the pending orders.
        /// </summary>
        protected PendingOrder[] PendingOrders => GetPendingOrders(x => !x.Order.State.IsDone()).ToArray();

        #endregion Protected Properties

        #region Private Properties

        /// <summary>
        /// Gets the associated quant fund.
        /// </summary>
        private IQuantFund QuantFund { get; set; }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Cancel all open orders related to the specified security.
        /// </summary>
        /// <param name="security">The security.</param>
        public void CancelOpenOrders(Security security) =>
            PendingOrders.Where(x => x.Security == security).Cancel();

        /// <summary>
        /// Cancels a specific order by order id.
        /// </summary>
        /// <param name="orderid">The orderid.</param>
        public void CancelOrder(long orderid) =>
            PendingOrders.Where(x => x.OrderId == orderid).Cancel();

        /// <summary>
        /// Gets the order by identifier.
        /// </summary>
        /// <param name="orderid">The orderid.</param>
        /// <returns></returns>
        public PendingOrder GetOrderById(long orderid) =>
            QuantFund.PendingOrders.FirstOrDefault(x => x.OrderId == orderid);

        /// <summary>
        /// Gets the pending orders.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <returns></returns>
        public IEnumerable<PendingOrder> GetPendingOrders(Func<PendingOrder, bool> search) =>
            QuantFund.PendingOrders.Where(search);

        /// <summary>
        /// Sets the associated quantfund.
        /// </summary>
        /// <param name="quantfund">The quantfund.</param>
        public override void SetQuantFund(IQuantFund quantfund)
        {
            if (QuantFund == null && quantfund != null)
            {
                QuantFund = quantfund;
                SetQuantFund(quantfund);
            }
        }

        /// <summary>
        /// Submits the order ticket for processing.
        /// </summary>
        /// <param name="orderticket">The orderticket.</param>
        /// <returns></returns>
        public OrderTicket SubmitOrderTicket(OrderTicket orderticket)
        {
            //Can only send order tickets if this order ticket is for an existing position
            if (Position[orderticket.Security].IsFlat)
            {
                orderticket.SetResponse(OrderTicketResponse.Error(orderticket.OrderId, OrderTicketResponseErrorCode.UnsupportedRequestType, $"Supporting modules cannot open new positions, only signal modules can."));
                return orderticket;
            }
            else
                return QuantFund.ProcessTicket(orderticket);
        }

        #endregion Public Methods
    }
}