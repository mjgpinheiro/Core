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

using Quantler.Account;
using Quantler.Account.Cash;
using Quantler.Broker;
using Quantler.Orders;
using Quantler.Trades;
using System;
using System.Collections.Generic;

namespace Quantler.Interfaces
{
    /// <summary>
    /// Broker connection interface
    /// </summary>
    public interface BrokerConnection
    {
        #region Public Events

        /// <summary>
        /// Occurs when [account balance change].
        /// </summary>
        event EventHandler<AccountAction> BalanceChange;

        /// <summary>
        /// Occurs when [order state change].
        /// </summary>
        event EventHandler<OrderTicketEvent> OrderStateChange;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Gets the type of the broker.
        /// </summary>
        BrokerType BrokerType { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        bool IsConnected { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is ready.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is ready; otherwise, <c>false</c>.
        /// </value>
        bool IsReady { get; }

        /// <summary>
        /// Gets the latency in milliseconds.
        /// </summary>
        int LatencyInMS { get; }

        /// <summary>
        /// Gets a value indicating whether this is a live feed with live updates.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [live feed]; otherwise, <c>false</c>.
        /// </value>
        bool LiveFeed { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Cancels the order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        bool CancelOrder(PendingOrder order);

        /// <summary>
        /// Connects this instance.
        /// </summary>
        void Connect();

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Gets the account funds currently known.
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<CashPosition> GetAccountFunds();

        /// <summary>
        /// Gets the currently active orders.
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<PendingOrder> GetActiveOrders();

        /// <summary>
        /// Gets the position overview.
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<Position> GetPositionOverview();

        /// <summary>
        /// Submits the order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        bool SubmitOrder(PendingOrder order);

        /// <summary>
        /// Tests the connection latency between the current instance and the broker.
        /// </summary>
        void TestLatency();

        /// <summary>
        /// Updates the order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        bool UpdateOrder(PendingOrder order);

        #endregion Public Methods
    }
}