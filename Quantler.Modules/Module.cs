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
using Quantler.Data.Aggegrate;
using Quantler.Indicators;
using Quantler.Interfaces;
using Quantler.Orders;
using Quantler.Performance;
using Quantler.Scheduler;
using Quantler.Securities;
using Quantler.Tracker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NLog;

namespace Quantler.Modules
{
    /// <summary>
    /// Base module implementation
    /// TODO: remove isbackfilling and isrunning from this instance and use fundstate instead (is more flexible)
    /// TODO: some parts are missing for the interface, add these missing functions
    /// </summary>
    /// <seealso cref="Quantler.Interfaces.IModule" />
    public abstract partial class Module : IModule
    {
        #region Public Properties

        /// <summary>
        /// Gets the account values.
        /// TODO: improvement - make sure this is updated on event instead of on request, otherwise we are constantly recalculating funds
        /// </summary>
        public CalculatedFunds Account =>
            QuantFund.Portfolio.CashManager.GetCalculatedFunds(QuantFund, QuantFund.Portfolio.BrokerAccount);

        /// <summary>
        /// Clock, used for retrieving time on different timezones
        /// </summary>
        public WorldClock Clock => QuantFund.Portfolio.Clock;

        /// <summary>
        /// Gets the date functions for scheduling purposes.
        /// </summary>
        protected DateFunc Date => ScheduledActionsKeeper.DateFunc;

        /// <summary>
        /// Gets the module id.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is backfilling.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is backfilling; otherwise, <c>false</c>.
        /// </value>
        public bool IsBackfilling => QuantFund.IsBackfilling;

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning => QuantFund.IsRunning;

        /// <summary>
        /// Gets the module name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the currently active order tickets.
        /// </summary>
        public OrderTicket[] OrderTickets => QuantFund.OrderTickets;

        /// <summary>
        /// Gets the current positions.
        /// </summary>
        public PositionTracker Position => QuantFund.Positions;

        /// <summary>
        /// Gets the current quant fund based results
        /// </summary>
        public Result Results => QuantFund.Results;

        /// <summary>
        /// Gets the currently active signals.
        /// </summary>
        public Dictionary<string, TradingSignal> Signals { get; } =
        new Dictionary<string, TradingSignal>();

        /// <summary>
        /// Gets the time functions for scheduling purposes.
        /// </summary>
        protected TimeFunc Time => ScheduledActionsKeeper.TimeFunc;

        /// <summary>
        /// Gets the attached universe.
        /// </summary>
        public Universe Universe => QuantFund.Universe;

        #endregion Public Properties

        #region Private Properties

        /// <summary>
        /// Gets the associated quant fund.
        /// </summary>
        private IQuantFund QuantFund { get; set; }

        /// <summary>
        /// Gets the scheduler.
        /// </summary>
        private ScheduledActionsKeeper ScheduledActionsKeeper =>
            QuantFund.Portfolio.ActionsScheduler.ScheduledActionsKeeper;

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Add a new data interval.
        /// </summary>
        /// <param name="aggregation">The aggregation.</param>
        /// <param name="security"></param>
        /// <returns></returns>
        public DataAggregator AddInterval(Security security, DataAggregator aggregation) =>
            QuantFund.Portfolio.Subscription.AddSubscription(QuantFund, security, aggregation, QuantFund.IsForceTick);

        /// <summary>
        /// Add a new data interval.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="aggregation">The aggregation.</param>
        /// <returns></returns>
        public DataAggregator AddInterval(Security security, Resolution resolution, AggregationType aggregation = AggregationType.QuoteBar)
        {
            //Get correct aggregator
            DataAggregator aggregator = null;
            switch (aggregation)
            {
                case AggregationType.QuoteBar:
                    aggregator = resolution.IsTick
                        ? new QuoteBarAggregator(Convert.ToInt32(resolution.Ticks))
                        : new QuoteBarAggregator(resolution.TimeSpan.Value);
                    break;

                case AggregationType.RenkoBar:
                    aggregator = null;
                    //aggregator = new RenkoAggregator(Convert.ToInt32(resolution.Ticks), null); //TODO: set renko type
                    break;

                case AggregationType.TradeBar:
                    aggregator = resolution.IsTick
                        ? new TradeAggregator(Convert.ToInt32(resolution.Ticks))
                        : new TradeAggregator(resolution.TimeSpan.Value);
                    break;
            }

            //Add aggregator and return
            return AddInterval(security, aggregator);
        }

        /// <summary>
        /// Add a new data interval.
        /// </summary>
        /// <param name="universe">The universe.</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="aggregation">The aggregation.</param>
        /// <returns></returns>
        public Dictionary<Security, DataAggregator> AddInterval(Universe universe, Resolution resolution,
            AggregationType aggregation = AggregationType.QuoteBar) =>
            universe.Select(security => new { key = security, value = AddInterval(security, resolution, aggregation) })
                .ToDictionary(k => k.key, v => v.value);

        /// <summary>
        /// Adds the signal.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public TradingSignal AddSignal(string name, Func<Security, bool> action)
        {
            TradingSignal nevent = new TradingSignal(Universe, name, action);
            Signals[nevent.Name] = nevent;
            return nevent;
        }

        /// <summary>
        /// Adds the signal.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        /// <param name="interval">The interval.</param>
        /// <returns></returns>
        public TradingSignal AddSignal(string name, Func<Security, bool> action, TimeSpan interval)
        {
            TradingSignal nevent = new TradingSignal(Universe, name, action, true);
            ScheduledActionsKeeper.Event(QuantFund.FundId, Date.EveryDay(), Time.Every(interval), () =>
            {
                if(IsRunning)
                    nevent.Execute();
            });
            Signals[nevent.Name] = nevent;
            return nevent;
        }

        /// <summary>
        /// Adds the signal.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        /// <param name="trigger">The trigger.</param>
        /// <returns></returns>
        public TradingSignal AddSignal(string name, Func<Security, bool> action, DataAggregator trigger)
        {
            TradingSignal nevent = new TradingSignal(Universe, name, action);
            trigger.DataAggregated += (sender, aggregate) =>
            {
                if(IsRunning)
                    nevent.Execute();
            };
            nevent.SignalFired += OnSignal;
            Signals[nevent.Name] = nevent;
            return nevent;
        }

        /// <summary>
        /// Adds the signal.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        /// <param name="trigger">The trigger.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Universe trigger is empty for adding a trading event</exception>
        public TradingSignal AddSignal(string name, Func<Security, bool> action, Dictionary<Security, DataAggregator> trigger)
        {
            if (trigger.Count == 0)
                throw new ArgumentNullException("Universe trigger is empty for adding a trading event");

            //Get first item
            var foundtrigger = trigger.Values.First();

            //Create trading event
            TradingSignal nevent = new TradingSignal(Universe, name, action);
            foundtrigger.DataAggregated += (sender, aggregate) =>
            {
                if(IsRunning)
                    nevent.Execute();
            };
            nevent.SignalFired += OnSignal;
            Signals[nevent.Name] = nevent;
            return nevent;
        }

        /// <summary>
        /// Adds the signal.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        /// <param name="trigger">The trigger.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Universe trigger is empty for adding a trading event</exception>
        public TradingSignal AddSignal<T>(string name, Func<Security, bool> action, Dictionary<Security, IndicatorBase<T>> trigger)
            where T : DataPoint
        {
            if (trigger.Count == 0)
                throw new ArgumentNullException("Universe trigger is empty for adding a trading event");

            //Get first item
            var foundtrigger = trigger.Values.First();

            //Create trading event
            TradingSignal nevent = new TradingSignal(Universe, name, action);
            foundtrigger.Updated += (sender, updated) =>
            {
                if(IsRunning)
                    nevent.Execute();
            };
            nevent.SignalFired += OnSignal;
            Signals[nevent.Name] = nevent;
            return nevent;
        }

        /// <summary>
        /// Adds the signal.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        /// <param name="trigger">The trigger.</param>
        /// <returns></returns>
        public TradingSignal AddSignal<T>(string name, Func<Security, bool> action, IndicatorBase<T> trigger)
            where T : DataPoint
        {
            TradingSignal nevent = new TradingSignal(Universe, name, action);
            trigger.Updated += (sender, updated) =>
            {
                if(IsRunning)
                    nevent.Execute();
            };
            nevent.SignalFired += OnSignal;
            Signals[nevent.Name] = nevent;
            return nevent;
        }

        /// <summary>
        /// Adds the signal.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        /// <param name="trigger">The trigger.</param>
        /// <returns></returns>
        public TradingSignal AddSignal<T>(string name, Func<Security, bool> action, T trigger)
            where T : IndicatorBase<IndicatorDataPoint>
        {
            TradingSignal nevent = new TradingSignal(Universe, name, action);
            trigger.Updated += (sender, updated) =>
            {
                if(IsRunning)
                    nevent.Execute();
            };
            nevent.SignalFired += OnSignal;
            Signals[nevent.Name] = nevent;
            return nevent;
        }

        /// <summary>
        /// Adds the signal.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        /// <param name="trigger">The trigger.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Universe trigger is empty for adding a trading event</exception>
        public TradingSignal AddSignal<T>(string name, Func<Security, bool> action, Dictionary<Security, T> trigger)
            where T : IndicatorBase<IndicatorDataPoint>
        {
            if (trigger.Count == 0)
                throw new ArgumentNullException("Universe trigger is empty for adding a trading event");

            //Get first item
            var foundtrigger = trigger.Values.First();

            //Create trading event
            TradingSignal nevent = new TradingSignal(Universe, name, action);
            foundtrigger.Updated += (sender, updated) =>
            {
                if(IsRunning)
                    nevent.Execute();
            };
            nevent.SignalFired += OnSignal;
            Signals[nevent.Name] = nevent;
            return nevent;
        }

        /// <summary>
        /// Send debug message
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="param">The parameter.</param>
        public void Debug(string message, params object[] param) =>
            QuantFund.Portfolio.Log(LogLevel.Debug, $"{Name}: {string.Format(message, param)}");

        /// <summary>
        /// Send debug message
        /// </summary>
        /// <param name="exc">The exc.</param>
        /// <param name="message">The message.</param>
        /// <param name="param">The parameter.</param>
        public void Debug(Exception exc, string message, params object[] param) =>
            QuantFund.Portfolio.Log(LogLevel.Debug, $"{Name}: {string.Format(message, param)}. Error: {exc.Message}");

        /// <summary>
        /// Disables the specified trading signal.
        /// </summary>
        /// <param name="name">The name.</param>
        public void DisableSignal(string name)
        {
            if (Signals.ContainsKey(name))
                Signals[name].IsEnabled = false;
        }

        /// <summary>
        /// Enables the specified trading signal.
        /// </summary>
        /// <param name="name">The name.</param>
        public void EnableSignal(string name)
        {
            if (Signals.ContainsKey(name))
                Signals[name].IsEnabled = true;
        }

        /// <summary>
        /// Send error message
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="param">The parameter.</param>
        public void Error(string message, params object[] param) =>
            QuantFund.Portfolio.Log(LogLevel.Error, $"{Name}: {string.Format(message, param)}");

        /// <summary>
        /// Send error message
        /// </summary>
        /// <param name="exc">The exc.</param>
        /// <param name="message">The message.</param>
        /// <param name="param">The parameter.</param>
        public void Error(Exception exc, string message, params object[] param) =>
            QuantFund.Portfolio.Log(LogLevel.Error, $"{Name}: {string.Format(message, param)}. Error: {exc.Message}");

        //TODO: START -> ADD TO MODULE INTERFACE

        public Order[] GetOpenOrders() =>
            GetOpenOrders(x => x.FundId == QuantFund.FundId);

        public Order[] GetOpenOrders(Func<Order, bool> search) =>
            QuantFund.PendingOrders.Where(x => x.FundId == QuantFund.FundId && !x.Order.State.IsDone())
                .Select(x => x.Order)
                .Where(search)
                .ToArray();

        public Order[] GetOpenOrders(Security security, OrderType ordertype) =>
            GetOpenOrders(x => x.Security == security && x.Type == ordertype);

        public Order[] GetOpenOrders(Security security) =>
            GetOpenOrders(x => x.Security == security);

        public decimal MarketWeight(Security security) =>
            Position[security].TotalValue / Account.Equity;

        //TODO: END -> ADD TO MODULE INTERFACE

        /// <summary>
        /// Gets the currently active order tickets.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <returns></returns>
        public OrderTicket[] GetOrderTickets(Security security) => OrderTickets.Where(x => x.Security.Ticker == security.Ticker).ToArray();

        /// <summary>
        /// Search the currently active order tickets.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <returns></returns>
        public OrderTicket[] GetOrderTickets(Func<OrderTicket, bool> search) =>
            OrderTickets.Where(search).ToArray();

        /// <summary>
        /// Gets the current fund-state for the corresponding security.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <returns></returns>
        public SecurityState GetState(Security security) =>
            QuantFund.GetState(security);

        /// <summary>
        /// Send info message
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="param">The parameter.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Info(string message, params object[] param) =>
            QuantFund.Portfolio.Log(LogLevel.Info, $"{Name}: {string.Format(message, param)}");

        /// <summary>
        /// Send info message
        /// </summary>
        /// <param name="exc">The exc.</param>
        /// <param name="message">The message.</param>
        /// <param name="param">The parameter.</param>
        public void Info(Exception exc, string message, params object[] param) =>
            QuantFund.Portfolio.Log(LogLevel.Info, $"{Name}: {string.Format(message, param)}. Error: {exc.Message}");

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Called when data is received.
        /// </summary>
        /// <param name="data">The data.</param>
        public virtual void OnData(DataUpdates data)
        {
        }

        /// <summary>
        /// Called at the end of each trading day.
        /// </summary>
        public virtual void OnEndOfDay()
        {
        }

        /// <summary>
        /// Called when a margin call has occured.
        /// </summary>
        /// <param name="tickets">The tickets.</param>
        public virtual void OnMarginCall(List<SubmitOrderTicket> tickets)
        {
        }

        /// <summary>
        /// Called when an order ticket event has occured.
        /// </summary>
        /// <param name="orderticketevent">The orderticketevent.</param>
        public virtual void OnOrderTicketEvent(OrderTicketEvent orderticketevent)
        {
        }

        /// <summary>
        /// Called when a trading signal is activated.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <param name="securities">The securities.</param>
        public virtual void OnSignal(TradingSignal signal, Security[] securities)
        {
        }

        /// <summary>
        /// Called when a quant fund is terminated. Default behaviour is that this Quant Fund is liquidated.
        /// </summary>
        public virtual void OnTermination(out bool liquidate) => liquidate = true;

        /// <summary>
        /// Schedules the specified action.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="time">The time.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public ScheduledEventAction Schedule(DateComposite date, TimeComposite time, Action action) =>
            QuantFund.Portfolio.ActionsScheduler.ScheduledActionsKeeper.Add(new ScheduledAction(QuantFund, date, time,
                (n, d) => action()));

        /// <summary>
        /// Schedules the specified action.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="time">The time.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public ScheduledEventAction Schedule(DateComposite date, TimeComposite time, Action<string, DateTime> action) =>
            QuantFund.Portfolio.ActionsScheduler.ScheduledActionsKeeper.Add(new ScheduledAction(QuantFund, date, time, action));

        /// <summary>
        /// Sets the back filling period.
        /// </summary>
        /// <param name="timespan">The timespan.</param>
        public void SetBackFilling(TimeSpan timespan)
        {
            if (QuantFund.BackFillingPeriod < timespan)
                QuantFund.BackFillingPeriod = timespan;
        }

        /// <summary>
        /// Sets the parameter with the assigned value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void SetParameter(string name, string value)
        {
            //Find property
            PropertyInfo propertyInfo = GetType().GetProperty(name);
            if (propertyInfo == null)
                throw new Exception($"Could not derive property with name {name} to set parameter value {value} for module with name {Name}");

            //Set property with desired value
            propertyInfo.SetValue(this, Convert.ChangeType(value, propertyInfo.PropertyType));
        }

        /// <summary>
        /// Sets the associated quantfund.
        /// </summary>
        /// <param name="quantfund">The quantfund.</param>
        public virtual void SetQuantFund(IQuantFund quantfund)
        {
            if (QuantFund == null && quantfund != null)
                QuantFund = quantfund;
        }

        /// <summary>
        /// Terminates this quant fund, can include a message
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void Terminate(string message = "")
        {
            //Set quant fund on stop or deleting

            //Quant fund will notify each module on termination
            throw new NotImplementedException();
        }

        /// <summary>
        /// Send warning message
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="param">The parameter.</param>
        public void Warning(string message, params object[] param) =>
            QuantFund.Portfolio.Log(LogLevel.Warn, $"{Name}: {string.Format(message, param)}");

        /// <summary>
        /// Send warning message
        /// </summary>
        /// <param name="exc">The exc.</param>
        /// <param name="message">The message.</param>
        /// <param name="param">The parameter.</param>
        public void Warning(Exception exc, string message, params object[] param) =>
            QuantFund.Portfolio.Log(LogLevel.Warn, $"{Name}: {string.Format(message, param)}. Error: {exc.Message}");

        #endregion Public Methods
    }
}