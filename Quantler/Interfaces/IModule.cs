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
using Quantler.Data.Aggegrate;
using Quantler.Orders;
using Quantler.Performance;
using Quantler.Scheduler;
using Quantler.Securities;
using Quantler.Tracker;
using System;
using System.Collections.Generic;
using Quantler.Account.Cash;

namespace Quantler.Interfaces
{
    /// <summary>
    /// Base implementation of a module
    /// </summary>
    public interface IModule
    {
        #region Public Properties

        /// <summary>
        /// Clock, used for retrieving time on different timezones
        /// </summary>
        WorldClock Clock { get; }

        /// <summary>
        /// Gets the module id.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is backfilling.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is backfilling; otherwise, <c>false</c>.
        /// </value>
        bool IsBackfilling { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        bool IsRunning { get; }

        /// <summary>
        /// Gets the module name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the currently active order tickets.
        /// </summary>
        OrderTicket[] OrderTickets { get; }

        /// <summary>
        /// Gets the current positions.
        /// </summary>
        PositionTracker Position { get; }

        /// <summary>
        /// Gets the current quant fund based results
        /// </summary>
        Result Results { get; }

        /// <summary>
        /// Gets the currently active signals.
        /// </summary>
        Dictionary<string, TradingSignal> Signals { get; }

        /// <summary>
        /// Gets the attached universe.
        /// </summary>
        Universe Universe { get; }

        /// <summary>
        /// Gets the account values.
        /// </summary>
        CalculatedFunds Account { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Add a new data interval.
        /// </summary>
        /// <param name="aggregation">The aggregation.</param>
        /// <param name="security"></param>
        /// <returns></returns>
        DataAggregator AddInterval(Security security, DataAggregator aggregation);

        /// <summary>
        /// Add a new data interval.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="aggregation">The aggregation.</param>
        /// <returns></returns>
        DataAggregator AddInterval(Security security, Resolution resolution, AggregationType aggregation = AggregationType.QuoteBar);

        /// <summary>
        /// Add a new data interval.
        /// </summary>
        /// <param name="universe">The universe.</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="aggregation">The aggregation.</param>
        /// <returns></returns>
        Dictionary<Security, DataAggregator> AddInterval(Universe universe, Resolution resolution, AggregationType aggregation = AggregationType.QuoteBar);

        /// <summary>
        /// Send debug message
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="param">The parameter.</param>
        void Debug(string message, params object[] param);

        /// <summary>
        /// Send debug message
        /// </summary>
        /// <param name="exc">The exc.</param>
        /// <param name="message">The message.</param>
        /// <param name="param">The parameter.</param>
        void Debug(Exception exc, string message, params object[] param);

        /// <summary>
        /// Disables the specified trading signal.
        /// </summary>
        /// <param name="name">The name.</param>
        void DisableSignal(string name);

        /// <summary>
        /// Enables the specified trading signal.
        /// </summary>
        /// <param name="name">The name.</param>
        void EnableSignal(string name);

        /// <summary>
        /// Send error message
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="param">The parameter.</param>
        void Error(string message, params object[] param);

        /// <summary>
        /// Send error message
        /// </summary>
        /// <param name="exc">The exc.</param>
        /// <param name="message">The message.</param>
        /// <param name="param">The parameter.</param>
        void Error(Exception exc, string message, params object[] param);

        /// <summary>
        /// Gets the current fund-state for the corresponding security.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <returns></returns>
        SecurityState GetState(Security security);

        /// <summary>
        /// Gets the currently active order tickets.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <returns></returns>
        OrderTicket[] GetOrderTickets(Security security);

        /// <summary>
        /// Search the currently active order tickets.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <returns></returns>
        OrderTicket[] GetOrderTickets(Func<OrderTicket, bool> search);

        /// <summary>
        /// Send info message
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="param">The parameter.</param>
        void Info(string message, params object[] param);

        /// <summary>
        /// Send info message
        /// </summary>
        /// <param name="exc">The exc.</param>
        /// <param name="message">The message.</param>
        /// <param name="param">The parameter.</param>
        void Info(Exception exc, string message, params object[] param);

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Initialize();

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
        /// Called when a trading signal is activated.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <param name="securities">The securities.</param>
        void OnSignal(TradingSignal signal, Security[] securities);

        /// <summary>
        /// Called when a quant fund is terminated.
        /// </summary>
        void OnTermination(out bool liquidate);

        /// <summary>
        /// Schedules the specified action.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="time">The time.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        ScheduledEventAction Schedule(DateComposite date, TimeComposite time, Action<string, DateTime> action);

        /// <summary>
        /// Schedules the specified action.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="time">The time.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        ScheduledEventAction Schedule(DateComposite date, TimeComposite time, Action action);

        /// <summary>
        /// Sets the back filling period.
        /// </summary>
        /// <param name="timespan">The timespan.</param>
        void SetBackFilling(TimeSpan timespan);

        /// <summary>
        /// Sets the parameter with the assigned value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void SetParameter(string name, string value);

        /// <summary>
        /// Sets the associated quant fund.
        /// </summary>
        /// <param name="quantfund">The quant fund.</param>
        void SetQuantFund(IQuantFund quantfund);

        /// <summary>
        /// Terminates this quant fund, can include a message
        /// </summary>
        /// <param name="message">The message.</param>
        void Terminate(string message = "");

        /// <summary>
        /// Send warning message
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="param">The parameter.</param>
        void Warning(string message, params object[] param);

        /// <summary>
        /// Send warning message
        /// </summary>
        /// <param name="exc">The exc.</param>
        /// <param name="message">The message.</param>
        /// <param name="param">The parameter.</param>
        void Warning(Exception exc, string message, params object[] param);

        #endregion Public Methods
    }
}