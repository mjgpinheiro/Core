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

using System;
using Quantler.Securities;

namespace Quantler.Orders
{
    /// <summary>
    /// Update order ticket
    /// </summary>
    public class UpdateOrderTicket : OrderTicket
    {
        #region Public Constructors

        /// <summary>
        /// Initialize new order update ticket
        /// </summary>
        /// <param name="fundid"></param>
        /// <param name="security"></param>
        /// <param name="orderid"></param>
        public UpdateOrderTicket(string fundid, Security security, long orderid)
            : base(fundid, security, orderid) => Type = OrderTicketType.Update;

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Comments associated to this order for reporting purposes
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Limit price, if used
        /// </summary>
        public decimal? LimitPrice { get; set; }

        /// <summary>
        /// Quantity of order
        /// </summary>
        public decimal? Quantity { get; set; }

        /// <summary>
        /// Stop price of order, if used
        /// </summary>
        public decimal? StopPrice { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Create new order update
        /// </summary>
        /// <param name="pendingorder"></param>
        /// <param name="updateaction"></param>
        /// <returns></returns>
        public static UpdateOrderTicket Create(PendingOrder pendingorder, Action<OrderUpdate> updateaction)
        {
            //Set to return
            UpdateOrderTicket toreturn = new UpdateOrderTicket(pendingorder.FundId, pendingorder.Security, pendingorder.OrderId);

            //Set updates
            toreturn.Process(updateaction);

            //Return
            return toreturn;
        }

        /// <summary>
        /// Cancel this update order request
        /// </summary>
        public void Cancel() => State = OrderTicketState.Processed;

        /// <summary>
        /// Process order update
        /// </summary>
        /// <param name="updates"></param>
        public void Process(Action<OrderUpdate> updates)
        {
            //Get data
            var result = new OrderUpdate();
            updates(result);

            //Set information
            Comment = result.Comment;
            LimitPrice = result.LimitPrice;
            Quantity = result.Quantity;
            StopPrice = result.StopPrice;
        }

        #endregion Public Methods
    }
}