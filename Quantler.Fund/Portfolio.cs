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

using MoreLinq;
using NLog;
using Quantler.Account;
using Quantler.Account.Cash;
using Quantler.Broker.Model;
using Quantler.Data;
using Quantler.Interfaces;
using Quantler.Messaging.Event;
using Quantler.Orders;
using Quantler.Performance;
using System;
using System.Collections.Generic;
using System.Reflection;
using Quantler.Common;
using Quantler.Configuration;
using Quantler.Messaging;

namespace Quantler.Fund
{
    /// <summary>
    /// Portfolio logic implementation
    /// </summary>
    /// <seealso cref="Quantler.Interfaces.IPortfolio" />
    public class Portfolio : IPortfolio
    {
        #region Private Fields

        /// <summary>
        /// Minimum wait between messaging between updates
        /// </summary>
        private readonly TimeSpan _mintwaitmessageupdates = TimeSpan.FromSeconds(1);

        /// <summary>
        /// The associated quantfunds in this portfolio
        /// </summary>
        private readonly List<IQuantFund> _quantfunds = new List<IQuantFund>();

        /// <summary>
        /// The user log
        /// </summary>
        private readonly ILogger _userlog = LogManager.GetLogger("user");

        /// <summary>
        /// The current instance log
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The date and time we last send a message
        /// </summary>
        private DateTime _lastmessagesend = DateTime.MinValue;

        /// <summary>
        /// The porfolio benchmark
        /// </summary>
        private readonly Benchmark _porfolioBenchmark;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Portfolio"/> class.
        /// </summary>
        /// <param name="clock">The clock.</param>
        /// <param name="actionscheduler">The actionscheduler.</param>
        /// <param name="brokerconnection">The brokerconnection.</param>
        /// <param name="brokermodel">The brokermodel.</param>
        /// <param name="currency">The currency.</param>
        /// <param name="eventrunner">The eventrunner.</param>
        /// <param name="exceptionhandler">The exceptionhandler.</param>
        /// <param name="orderTicketHandler">The order ticket handler.</param>
        /// <param name="brokeraccount">The brokeraccount.</param>
        /// <param name="cashmanager">The cashmanager.</param>
        /// <param name="runmode">The runmode.</param>
        /// <param name="datafeed">The datafeed.</param>
        /// <param name="benchmark">The benchmark.</param>
        /// <param name="id">The identifier.</param>
        public Portfolio(WorldClock clock, ActionsScheduler actionscheduler, BrokerConnection brokerconnection, BrokerModel brokermodel, 
            Currency currency, EventRunner eventrunner, ExceptionHandler exceptionhandler, OrderTicketHandler orderTicketHandler, 
            BrokerAccount brokeraccount, CashManager cashmanager, RunMode runmode, DataFeed datafeed, Benchmark benchmark, string id = "")
        {
            //Set references
            ActionsScheduler = actionscheduler;
            BrokerAccount = brokeraccount;
            BrokerConnection = brokerconnection;
            BrokerModel = brokermodel;
            Clock = clock;
            Currency = currency;
            EventRunner = eventrunner;
            ExceptionHandler = exceptionhandler;
            CashManager = cashmanager;
            OrderTicketHandler = orderTicketHandler;
            _porfolioBenchmark = benchmark;

            //Set initial items
            Id = id;
            IsBacktesting = runmode == RunMode.Backtester;
            OrderFactory = new OrderFactory(this, BrokerModel);
            OrderTracker = new OrderTracker(this);
            Subscription = new DataSubscriptionManager(datafeed, CashManager);
            Results = new Result(0, _porfolioBenchmark);

            //Portfolio benchmark is not used
            benchmark.OnCalc(x => 0);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the associated actions scheduler.
        /// </summary>
        public ActionsScheduler ActionsScheduler { get; }

        /// <summary>
        /// Gets the associated account.
        /// </summary>
        public BrokerAccount BrokerAccount { get; }

        /// <summary>
        /// Gets the broker connection.
        /// </summary>
        public BrokerConnection BrokerConnection { get; }

        /// <summary>
        /// Associated broker model
        /// </summary>
        public BrokerModel BrokerModel { get; }

        /// <summary>
        /// Gets the clock.
        /// </summary>
        public WorldClock Clock { get; }

        /// <summary>
        /// Gets the currency conversion model used.
        /// </summary>
        public Currency Currency { get; }

        /// <summary>
        /// Gets the event runner.
        /// </summary>
        public EventRunner EventRunner { get; }

        /// <summary>
        /// Handler for exceptions created by either the framework or client's modules
        /// </summary>
        public ExceptionHandler ExceptionHandler { get; }

        /// <summary>
        /// Fund manager for this portfolio
        /// </summary>
        public CashManager CashManager { get; }

        /// <summary>
        /// Unique identifier for this portfolio of quant funds
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// True if this portfolio is currently being used for backtesting
        /// </summary>
        public bool IsBacktesting { get; }

        /// <summary>
        /// Returns true if this portfolio is valid.
        /// </summary>
        public bool IsValid
        {
            private set;
            get;
        }

        /// <summary>
        /// Gets the order factory.
        /// </summary>
        public OrderFactory OrderFactory { get; }

        /// <summary>
        /// Handler for order tickets
        /// </summary>
        public OrderTicketHandler OrderTicketHandler { get; }

        /// <summary>
        /// Gets the order tracker.
        /// </summary>
        public OrderTracker OrderTracker { get; }

        /// <summary>
        /// Updated results of this trading session, for this portfolio
        /// </summary>
        public Result Results { get; private set; }

        /// <summary>
        /// Gets the quant funds.
        /// </summary>
        public IQuantFund[] QuantFunds =>
            _quantfunds.ToArray();

        /// <summary>
        /// Current portfolio status
        /// </summary>
        public PortfolioStatus Status { get; set; }

        /// <summary>
        /// Gets the current data subscriptions.
        /// </summary>
        public DataSubscriptionManager Subscription { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Adds the quant fund.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="message"></param>
        public IQuantFund AddFund(Assembly assembly, AddFundMessage message)
        {
            //Check if we have initialized the result tracker of this fund
            if(Results.QuantFund.InitialCapital == 0)
                Results = new Result(BrokerAccount.Balance, _porfolioBenchmark);

            //Get the benchmark instance
            if (!DynamicLoader.Instance.TryGetInstance(Config.GlobalConfig.Benchmark, out Benchmark benchmark))
            {
                _log.Error($"Could not load instance of benchmark, {Config.GlobalConfig.Benchmark}. Cannot initialize quant fund {message.FundName}!");
                return null;
            }

            //Get quant fund
            var quantfund = new QuantFund(this, benchmark, message.FundId, message.AllocatedFunds, message.ForceTick, message.FundName);

            //Initialize the quant fund
            try
            {
                quantfund.Initialize(assembly, message);
            }
            catch (Exception exc)
            {
                _userlog.Error(exc, $"Could not initialize quant fund with id {quantfund.FundId}, please check the error");
                throw exc;
            }

            //Add the quant fund
            _quantfunds.Add(quantfund);
            return quantfund;
        }

        /// <summary>
        /// Send Error message
        /// </summary>
        /// <param name="severity"></param>
        /// <param name="message">The message.</param>
        /// <param name="fundid"></param>
        public void Log(LogLevel severity, string message, string fundid = "")
        {
            //Send to user log
            _userlog.Log(severity, message);

            //Send as message
            EventRunner.Enqueue(LoggingMessage.Create(fundid, message, severity.Name));
        }

        /// <summary>
        /// Called when data is received.
        /// </summary>
        /// <param name="updates">The datapoints.</param>
        public void OnData(DataUpdates updates)
        {
            //Process for each quant fund attached and running
            QuantFunds.ForEach(quantfund => quantfund.OnData(updates));

            //Send updates
            if(!IsBacktesting)
                SendEventMessages();
        }

        /// <summary>
        /// Called when [order ticket event].
        /// TODO: send to ordertickethandler for processing fills and updating
        /// TODO: send to quant fund for notification
        /// </summary>
        /// <param name="pendingorder"></param>
        /// <param name="orderticketevent">The orderticketevent.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnOrderTicketEvent(PendingOrder pendingorder, OrderTicketEvent orderticketevent)
        {
            throw new NotImplementedException();

            //Send updates (so if the order is completed, the receiving party will know this)
            //EventRunner.Enqueue(PendingOrderInfoMessage.Generate(pendingorder), true);
            //EventRunner.Enqueue(OrderInfoMessage.Create(pendingorder), true);
        }

        /// <summary>
        /// Removes the quant fund from this portfolio.
        /// </summary>
        /// <param name="quantfund">The quantfund.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void RemoveFund(IQuantFund quantfund)
        {
            throw new NotImplementedException();

            //Send updates (so we know this quant fund was remvoved)
            EventRunner.Enqueue(FundInfoMessage.Generate(Id, quantfund), true);

            //Remove funds
            CashManager.RemoveQuantFund(quantfund);
        }

        /// <summary>
        /// Sends the event messages.
        /// </summary>
        public void SendEventMessages()
        {
            //Send all message updates
            if (_lastmessagesend < (Clock.CurrentUtc + _mintwaitmessageupdates))
            {
                EventRunner.SendAccountInfoMessages(this);
                EventRunner.SendFundInfoMessages(Id, QuantFunds);
                EventRunner.SendInstanceInfoMessage(BrokerConnection);
                EventRunner.SendPendingOrderInfoMessages(QuantFunds, OrderTracker, Id);
                EventRunner.SendPerformanceInfoMessages(BrokerAccount, QuantFunds);
                EventRunner.SendPositionInformationMessages(QuantFunds, CashManager, BrokerAccount);
                _lastmessagesend = Clock.CurrentUtc;
            }
        }

        /// <summary>
        /// Terminates this instance.
        /// </summary>
        public void Terminate()
        {
            //Go trough each quant fund to terminate
            foreach (var quantfund in _quantfunds)
            {
                try
                {
                    quantfund.OnTermination();
                }
                catch (Exception exc)
                {
                    _log.Error(exc, $"Exception while terminating current quant fund {quantfund.FundId}");
                }
            }
        }

        #endregion Public Methods
    }
}