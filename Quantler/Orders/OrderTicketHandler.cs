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

using Quantler.Account.Cash;
using Quantler.Data;
using Quantler.Interfaces;
using System;
using System.Collections.Generic;

namespace Quantler.Orders
{
    /// <summary>
    /// Handler for new order tickets
    /// </summary>
    /// <seealso cref="Quantler.Interfaces.QTask" />
    public interface OrderTicketHandler : QTask
    {
        #region Public Properties

        /// <summary>
        /// Associated broker connection
        /// </summary>
        BrokerConnection BrokerConnection { get; }

        /// <summary>
        /// Gets the fund manager.
        /// </summary>
        CashManager CashManager { get; }

        /// <summary>
        /// Gets the last interal order identifier.
        /// </summary>
        int LastInteralOrderId { get; }

        /// <summary>
        /// Gets the market order fill timeout.
        /// </summary>
        TimeSpan MarketOrderFillTimeout { get; set; }

        /// <summary>
        /// Gets the current order count.
        /// </summary>
        int OrderCount { get; }

        /// <summary>
        /// Gets the order tracker.
        /// </summary>
        OrderTracker OrderTracker { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Cancels the open orders.
        /// </summary>
        /// <param name="orders">The orders.</param>
        /// <returns></returns>
        List<PendingOrder> CancelOpenOrders(PendingOrder[] orders);

        /// <summary>
        /// Thread entry for executing all current order tickets
        /// </summary>
        void Execute();

        /// <summary>
        /// Gets the next internal order identifier.
        /// </summary>
        /// <returns></returns>
        long GetNextInternalOrderId();

        /// <summary>
        /// Gets the open orders.
        /// </summary>
        /// <returns></returns>
        List<Order> GetOpenOrders();

        /// <summary>
        /// Gets the order by broker identifier.
        /// </summary>
        /// <param name="brokerid">The brokerid.</param>
        /// <returns></returns>
        Order GetOrderByBrokerId(string brokerid);

        /// <summary>
        /// Gets the order by identifier.
        /// </summary>
        /// <param name="orderid">The orderid.</param>
        /// <returns></returns>
        Order GetOrderById(long orderid);

        /// <summary>
        /// Gets the orders trough search.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <returns></returns>
        IEnumerable<Order> GetOrders(Func<Order, bool> search = null);

        /// <summary>
        /// Gets if there is sufficient capital for the specified pendingorder.
        /// </summary>
        /// <param name="pendingorder">The pendingorder.</param>
        /// <returns></returns>
        bool GetSufficientCapitalForOrder(PendingOrder pendingorder);

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="portfolio">The portfolio.</param>
        void Initialize(IPortfolio portfolio);

        /// <summary>
        /// On data point received (for simulated order types)
        /// </summary>
        /// <param name="dataupdates">The data updates.</param>
        void OnData(DataUpdates dataupdates);

        /// <summary>
        /// Processes the specified ticket.
        /// </summary>
        /// <param name="ticket">The ticket.</param>
        /// <returns></returns>
        OrderTicket Process(OrderTicket ticket);

        /// <summary>
        /// Removes the order.
        /// </summary>
        /// <param name="orderid">The orderid.</param>
        /// <param name="comment">The comment.</param>
        /// <returns></returns>
        void RemoveOrder(long orderid, string comment = null);

        /// <summary>
        /// Waits for order to be processed by the broker.
        /// </summary>
        /// <param name="orderid">The orderid.</param>
        /// <returns></returns>
        bool WaitForOrder(long orderid);

        #endregion Public Methods
    }
}