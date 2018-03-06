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

using NLog;
using Quantler.Account;
using Quantler.Account.Cash;
using Quantler.Broker.Model;
using Quantler.Data;
using Quantler.Messaging;
using Quantler.Orders;
using Quantler.Performance;
using System.Reflection;

namespace Quantler.Interfaces
{
    /// <summary>
    /// Portfolio information holder
    /// </summary>
    public interface IPortfolio
    {
        #region Public Properties

        /// <summary>
        /// Gets the associated actions scheduler.
        /// </summary>
        ActionsScheduler ActionsScheduler { get; }

        /// <summary>
        /// Gets the associated account.
        /// </summary>
        BrokerAccount BrokerAccount { get; }

        /// <summary>
        /// Gets the broker connection.
        /// </summary>
        BrokerConnection BrokerConnection { get; }

        /// <summary>
        /// Associated broker model
        /// </summary>
        BrokerModel BrokerModel { get; }

        /// <summary>
        /// Fund manager for this portfolio
        /// </summary>
        CashManager CashManager { get; }

        /// <summary>
        /// Gets the clock.
        /// </summary>
        WorldClock Clock { get; }

        /// <summary>
        /// Gets the currency model used.
        /// </summary>
        Currency Currency { get; }

        /// <summary>
        /// Gets the event runner.
        /// </summary>
        EventRunner EventRunner { get; }

        /// <summary>
        /// Handler for exceptions created by either the framework or client's modules
        /// </summary>
        ExceptionHandler ExceptionHandler { get; }

        /// <summary>
        /// Unique identifier for this portfolio of quant funds
        /// </summary>
        string Id { get; }

        /// <summary>
        /// True if this portfolio is currently being used for backtesting
        /// </summary>
        bool IsBacktesting { get; }

        /// <summary>
        /// Check if this portfolio object is valid for use, or not
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Gets the order factory.
        /// </summary>
        OrderFactory OrderFactory { get; }

        /// <summary>
        /// Handler for order tickets
        /// </summary>
        OrderTicketHandler OrderTicketHandler { get; }

        /// <summary>
        /// Gets the order tracker.
        /// </summary>
        OrderTracker OrderTracker { get; }

        /// <summary>
        /// Updated results of this trading session, for this portfolio
        /// </summary>
        Result Results { get; }

        /// <summary>
        /// Gets the quant funds.
        /// </summary>
        IQuantFund[] QuantFunds { get; }

        /// <summary>
        /// Current portfolio status
        /// </summary>
        PortfolioStatus Status { get; set; }

        /// <summary>
        /// Gets the current data subscriptions.
        /// </summary>
        DataSubscriptionManager Subscription { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Adds the quant fund.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="message"></param>
        IQuantFund AddFund(Assembly assembly, AddFundMessage message);

        /// <summary>
        /// Send Error message
        /// </summary>
        /// <param name="severity"></param>
        /// <param name="message">The message.</param>
        /// <param name="fundid"></param>
        void Log(LogLevel severity, string message, string fundid = "");

        /// <summary>
        /// Called when data is received.
        /// </summary>
        /// <param name="updates">The updates.</param>
        void OnData(DataUpdates updates);

        /// <summary>
        /// Called when [order ticket event].
        /// </summary>
        /// <param name="pendingorder"></param>
        /// <param name="orderticketevent">The orderticketevent.</param>
        void OnOrderTicketEvent(PendingOrder pendingorder, OrderTicketEvent orderticketevent);

        /// <summary>
        /// Removes the quant fund.
        /// </summary>
        /// <param name="quantfund">The quantfund.</param>
        void RemoveFund(IQuantFund quantfund);

        /// <summary>
        /// Sends the event messages.
        /// </summary>
        void SendEventMessages();

        /// <summary>
        /// Terminates this instance.
        /// </summary>
        void Terminate();

        #endregion Public Methods
    }
}