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

using Quantler.Data;
using Quantler.Orders;
using Quantler.Performance;
using Quantler.Securities;
using Quantler.Tracker;
using System;
using System.Collections.Generic;
using System.Reflection;
using Quantler.Messaging;

namespace Quantler.Interfaces
{
    /// <summary>
    /// Interface implementation of a quant fund
    /// </summary>
    public interface IQuantFund
    {
        #region Public Properties

        /// <summary>
        /// Returns the period in time needed for backfilling as set by any of the modules
        /// </summary>
        TimeSpan BackFillingPeriod { get; set; }

        /// <summary>
        /// Get currently set benchmark for this quant fund
        /// </summary>
        Benchmark Benchmark { get; }

        /// <summary>
        /// Gets the current quant fund state.
        /// </summary>
        FundState State { get; }

        /// <summary>
        /// Gets the fund identifier.
        /// </summary>
        string FundId { get; }

        /// <summary>
        /// True if this agent is in the process of loading historical data
        /// </summary>
        bool IsBackfilling { get; }

        /// <summary>
        /// Gets a value indicating whether [force tick].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [force tick]; otherwise, <c>false</c>.
        /// </value>
        bool IsForceTick { get; }

        /// <summary>
        /// True if this quant fund is running and processing data
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Returns all modules used by this quant fund
        /// </summary>
        IModule[] Modules { get; }

        /// <summary>
        /// Name of this quant fund
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the currently active order tickets.
        /// </summary>
        OrderTicket[] OrderTickets { get; }

        /// <summary>
        /// Returns known pending orders for this quant fund
        /// </summary>
        PendingOrder[] PendingOrders { get; }

        /// <summary>
        /// Returns the portfolio for which this quant fund is part of
        /// </summary>
        IPortfolio Portfolio { get; }

        /// <summary>
        /// Current positions hold by this quant fund
        /// </summary>
        PositionTracker Positions { get; }

        /// <summary>
        /// Current trading results calculated for this quant fund (isolated)
        /// </summary>
        Result Results { get; }

        /// <summary>
        /// Date and time on which this quant fund was set to start
        /// </summary>
        DateTime StartedDTUtc { get; }

        /// <summary>
        /// Security universe for this quant fund
        /// </summary>
        Universe Universe { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Gets the current fund-state for the specified security.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <returns></returns>
        SecurityState GetState(Security security);

        /// <summary>
        /// Called when data is received.
        /// </summary>
        /// <param name="data">The data.</param>
        void OnData(DataUpdates data);

        /// <summary>
        /// Called at the end of each trading day.
        /// </summary>
        void OnEndOfDay();

        /// <summary>
        /// Called when a margin call has occured.
        /// </summary>
        /// <param name="tickets">The tickets.</param>
        void OnMarginCall(List<SubmitOrderTicket> tickets);

        /// <summary>
        /// Called when an order ticket event has occured.
        /// </summary>
        /// <param name="orderticketevent">The orderticketevent.</param>
        void OnOrderTicketEvent(OrderTicketEvent orderticketevent);

        /// <summary>
        /// Called when a quant fund is terminated.
        /// </summary>
        void OnTermination();

        /// <summary>
        /// Sets the fund state for the corresponding security.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <param name="security">The security.</param>
        /// <param name="state">The state.</param>
        void SetState(IModule module, Security security, SecurityState state);

        /// <summary>
        /// Start the current agent and allow it to process data
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the current agent from running
        /// </summary>
        void Stop();

        /// <summary>
        /// Liquidates this quant fund (and all its holdings). Waits for all orders to be processed
        /// </summary>
        void Liquidate();

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Initialize(Assembly assembly, AddFundMessage fundinfo);

        /// <summary>
        /// Processes the order ticket according to the order flow for a quant fund.
        /// </summary>
        /// <param name="orderticket">The orderticket.</param>
        /// <param name="useflow"></param>
        OrderTicket ProcessTicket(OrderTicket orderticket, bool useflow = true);

        #endregion Public Methods
    }
}