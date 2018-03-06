#region License Header

/*
* QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
* Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
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
*
* Modifications Copyright 2018 Quantler B.V.
*
*/

#endregion License Header

using MoreLinq;
using NLog;
using Quantler.Data;
using Quantler.Data.Corporate;
using Quantler.Data.Feed;
using Quantler.Interfaces;
using Quantler.Messaging;
using Quantler.Messaging.Event;
using Quantler.Orders;
using Quantler.Scheduler;
using Quantler.Securities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Quantler.Account;
using Quantler.Account.Cash;
using Quantler.Broker;
using Quantler.Broker.Model;
using Quantler.Configuration;
using Quantler.Configuration.Model;
using Quantler.Data.Bars;
using Quantler.Exchanges;
using Quantler.Fund;
using Quantler.Tracker;

namespace Quantler.Bootstrap
{
    /// <summary>
    /// Implementation for running a portfolio
    /// TODO: implement result handler and flow
    /// </summary>
    public class PortfolioManager
    {
        #region Private Fields

        /// <summary>
        /// Current logging instance
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Current time and date step
        /// </summary>
        private DateTime _currenttimestep;

        /// <summary>
        /// Total amount of datapoints processed
        /// </summary>
        private long _datapointsprocessed;

        /// <summary>
        /// Currently associated portfolio
        /// </summary>
        private IPortfolio _portfolio;

        /// <summary>
        /// Previous run time and date
        /// </summary>
        private DateTime _previoustime;

        /// <summary>
        /// The running tasks
        /// </summary>
        private Thread[] _runningthreads;

        /// <summary>
        /// The initial fund message
        /// </summary>
        private readonly AddFundMessage _initialFundMessage;

        /// <summary>
        /// The initial message instance
        /// </summary>
        private readonly MessageInstance _initialMessageInstance;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PortfolioManager"/> class for running a backtest.
        /// </summary>
        /// <param name="portfolioimplementations">The portfolioimplementations.</param>
        /// <param name="simulation">The backtest.</param>
        public PortfolioManager(PortfolioImplementations portfolioimplementations, SimulationMessage simulation)
            : this(portfolioimplementations)
        {
            //Set initial message
            _initialMessageInstance = simulation;

            //Since this is a backtest request
            RunMode = RunMode.Backtester;

            //World clock is depended on data received
            var clock = new WorldClock(() => PortfolioImplementations.DataFeed.LastDataReceivedUtc == DateTime.MinValue ?
                                                    simulation.StartDateTime :
                                                    portfolioimplementations.DataFeed.LastDataReceivedUtc);

            //Get additional information
            if (!Enum.TryParse(simulation.AccountType, out AccountType accounttype))
                throw new Exception($"Cannot initialize backtest account type {simulation.AccountType}");
            if (!Enum.TryParse(simulation.BrokerType, out BrokerType brokertype))
                throw new Exception($"Cannot initialize backtest broker type {simulation.BrokerType}");
            if (!Enum.TryParse(simulation.BaseCurrency, out CurrencyType basecurrency))
                throw new Exception($"Cannot initialize backtest base currency type {simulation.BaseCurrency}");

            //Get latest currency rates, so we are up to date (trough forced reload)
            _log.Debug($"Initializing currency implementation: {PortfolioImplementations.Currency.GetType().FullName}");
            Config.LoadConfigFile<CurrencyRatesConfig[]>(Config.GlobalConfig.CurrencyRatesConfigFile, true);
            PortfolioImplementations.Currency.Initialize(clock, true);

            //Get broker model
            var brokermodel = BrokerModelFactory.GetBroker(accounttype, brokertype);

            //Check if the currency selected matches the currency of this broker (for instance when using crypto currencies)
            decimal allocatedfunds = simulation.QuantFund.AllocatedFunds;
            brokermodel.GetCompatibleInitialCapital(portfolioimplementations.Currency, ref basecurrency, ref allocatedfunds);
            simulation.QuantFund.AllocatedFunds = allocatedfunds;

            //Create portfolio
            _portfolio = CreatePortfolio(simulation.PortfolioId, Guid.NewGuid().ToString(), brokermodel,
                simulation.Leverage, basecurrency, basecurrency, clock, simulation.ExtendedMarketHours);

            //Set initial funds
            _portfolio.CashManager.AddCash(basecurrency, allocatedfunds);

            //Set initial fund message
            _initialFundMessage = simulation.QuantFund;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PortfolioManager"/> class for running a live trading instance.
        /// </summary>
        /// <param name="portfolioImplementations">The portfolio implementations.</param>
        /// <param name="livetrading">The livetrading.</param>
        public PortfolioManager(PortfolioImplementations portfolioImplementations, LiveTradingMessage livetrading)
            : this(portfolioImplementations)
        {
            //Set initial message
            _initialMessageInstance = livetrading;

            //Since this is a live trading request
            RunMode = RunMode.LiveTrading;

            //World clock is current time
            var clock = new WorldClock(() => DateTime.UtcNow);

            //Get additional information
            if (!Enum.TryParse(livetrading.AccountType, out AccountType accounttype))
                throw new Exception($"Cannot initialize backtest account type {livetrading.AccountType}");
            if (!Enum.TryParse(livetrading.BrokerType, out BrokerType brokertype))
                throw new Exception($"Cannot initialize backtest broker type {livetrading.BrokerType}");
            if (!Enum.TryParse(livetrading.BaseCurrency, out CurrencyType basecurrency))
                throw new Exception($"Cannot initialize backtest base currency type {livetrading.BaseCurrency}");
            if (!Enum.TryParse(livetrading.DisplayCurrency, out CurrencyType displaycurrency))
                throw new Exception($"Cannot initialize backtest base currency type {livetrading.DisplayCurrency}");

            //Get latest currency rates, so we are up to date (trough forced reload)
            _log.Debug($"Initializing currency implementation: {PortfolioImplementations.Currency.GetType().FullName}");
            Config.LoadConfigFile<CurrencyRatesConfig[]>(Config.GlobalConfig.CurrencyRatesConfigFile, true);
            PortfolioImplementations.Currency.Initialize(clock, true);

            //Get broker model
            var brokermodel = BrokerModelFactory.GetBroker(accounttype, brokertype);

            //Create portfolio
            _portfolio = CreatePortfolio(livetrading.PortfolioId, livetrading.AccountId, brokermodel,
                livetrading.Leverage, basecurrency, displaycurrency, clock,
                livetrading.ExtendedMarketHours);

            //Set initial fund message
            _initialFundMessage = livetrading.QuantFund;
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PortfolioManager"/> class.
        /// </summary>
        /// <param name="portfolioImplementations">The portfolio implementations.</param>
        private PortfolioManager(PortfolioImplementations portfolioImplementations) =>
            PortfolioImplementations = portfolioImplementations;

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets the current time step.
        /// </summary>
        public TimeSpan CurrentTimeStep =>
            _currenttimestep == DateTime.MinValue ? TimeSpan.Zero : DateTime.UtcNow - _currenttimestep;

        /// <summary>
        /// Gets a value indicating whether this instance is done.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is done; otherwise, <c>false</c>.
        /// </value>
        public bool IsDone => State == PortfolioStatus.Stopped || State == PortfolioStatus.RuntimeError;

        /// <summary>
        /// Gets the portfolio implementations.
        /// </summary>
        public PortfolioImplementations PortfolioImplementations { get; }

        /// <summary>
        /// Gets the run mode. (Backtest, LiveTrading)
        /// </summary>
        public RunMode RunMode { get; }

        /// <summary>
        /// Gets the current state.
        /// </summary>
        public PortfolioStatus State => _portfolio?.Status ?? PortfolioStatus.Initializing;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Runs this instance.
        /// </summary>
        public void Run()
        {
            //Set data
            _datapointsprocessed = 0;
            var margincallfrequency = TimeSpan.FromMinutes(5);
            var nextmargincalltime = DateTime.MinValue;
            var settlementscanfrequency = TimeSpan.FromMinutes(30);
            var nextsettlementscan = DateTime.MinValue;

            //Keep track of delistings
            var delisting = new List<Delisting>();

            //Initialize
            _portfolio.Status = PortfolioStatus.Running;
            InitializeDependecies();

            //Initialize initial quant fund
            AddQuantFund(_initialFundMessage);

            //Set and start threads
            Thread datafeedthread = new Thread(PortfolioImplementations.DataFeed.Start) { IsBackground = true, Name = "DataFeed Thread" };
            datafeedthread.Start();
            Thread actionschedulertrhead = new Thread(PortfolioImplementations.ActionsScheduler.Start) { IsBackground = true, Name = "Scheduler Thread" };
            actionschedulertrhead.Start();
            Thread eventrunnerthread = new Thread(PortfolioImplementations.EventRunner.Start) { IsBackground = true, Name = "EventRunner Thread" };
            eventrunnerthread.Start();
            Thread ordertickethandlerthread = new Thread(PortfolioImplementations.OrderTicketHandler.Start) { IsBackground = true, Name = "OrderTicketHandler Thread" };
            ordertickethandlerthread.Start();
            _runningthreads = new[] { datafeedthread, actionschedulertrhead, eventrunnerthread, ordertickethandlerthread };

            //Set scheduled maintenance actions
            ScheduledPortfolioActions();

            //Set benchmark


            //Start, by looping trough the data received by the datafeed, utill we are done receiving data
            //TODO: on exit of instance => gracefully exit
            while (PortfolioImplementations.DataFeed.IsRunning)
            {
                //Get the current data points
                var data = NextDatapoints();

                //Reset current timer among others
                _currenttimestep = data.OccuredUtc;
                _datapointsprocessed += data.DataPointCount;

                //Check if we are still running
                if (IsDone)
                {
                    _log.Error($"Current state of portfolio is done, {_portfolio.Status} at {_portfolio.Clock.CurrentUtc}");
                    break;
                }

                //Check if we have terminated current execution
                if (State == PortfolioStatus.Terminating)
                {
                    _log.Warn($"Terminating current execution of portfolio");
                    _portfolio.Terminate();
                    break;
                }

                //Check if there are errors
                if (_portfolio.ExceptionHandler.StopExecution)
                {
                    _log.Error(_portfolio.ExceptionHandler.RuntimeError, $"Execution was stopped due to a fatal error");
                    _portfolio.Status = PortfolioStatus.RuntimeError;
                    break;
                }

                //Check if we need to process any messages
                while (PortfolioImplementations.MessageQueue.TryGetNextMessage(out MessageInstance message))
                {
                    if (message == null) continue;
                    else PortfolioImplementations.MessageQueue.Acknowledge(message);
                    try
                    {
                        if (message is CommandMessage command)
                        {
                            //process command message
                            ProcessCommand(command);

                            //End of message
                            PortfolioImplementations.MessageQueue.Complete(message, MessageResult.Success);
                        }
                        else if (message is AddFundMessage addfund)
                        {
                            //process add fund message
                            AddQuantFund(addfund);

                            //End of message
                            PortfolioImplementations.MessageQueue.Complete(message, MessageResult.Success);
                        }
                        else
                        {
                            PortfolioImplementations.MessageQueue.Complete(message, MessageResult.DeadLettered);
                            _log.Error($"Received message of type {message.MessageType}, which was unexpected for a running portfolio");
                        }
                    }
                    catch (Exception exc)
                    {
                        PortfolioImplementations.MessageQueue.Complete(message, MessageResult.Failure);
                        _log.Error(exc, $"Error while processing message of type {message.MessageType}");
                    }
                }

                //Check if we need to check on orders for simulated brokers
                if (PortfolioImplementations.BrokerConnection is SimBrokerConnection simbroker)
                    simbroker.ProcessMarketData(data.Updates);

                //Check event handler if backtesting (we need to fire it manually)
                if (RunMode == RunMode.Backtester)
                    PortfolioImplementations.ActionsScheduler.PokePastActions(_portfolio.Clock.CurrentUtc);

                //Update current currencies if applicable
                PortfolioImplementations.Currency.Update(data.Updates);

                //Check for delistings
                if (data.Updates.Delistings.Count > 0)
                {
                    ProcessDelistedSymbols(data.Updates.Delistings);
                    delisting.AddRange(data.Updates.Delistings.Values);
                }

                //Check for trading status updates
                if (data.Updates.TradingStatusUpdates.Keys.Count > 0)
                {
                    foreach (var item in data.Updates.TradingStatusUpdates)
                    {
                        var security = _portfolio.BrokerAccount.Securities[item.Key];
                        if (security is UnknownSecurity)
                            return;
                        //TODO: this
                        //else if (security is SecurityBase baseSecurity)
                        //    baseSecurity.IsHalted = item.Value.Status != "T";
                    }
                }

                //Process simulated orders
                PortfolioImplementations.OrderTicketHandler.OnData(data.Updates);

                //Check for margin calls
                if (_portfolio.Clock.CurrentUtc >= nextmargincalltime)
                {
                    //Check for new margin call orders
                    OrderTicket[] margincallorders = new OrderTicket[0];
                    bool issuemargincallwarning = false;
                    try
                    {
                        margincallorders = _portfolio.BrokerModel.GetMarginCallModel()
                            .CheckMarginCall(_portfolio.QuantFunds, _portfolio.BrokerAccount, out issuemargincallwarning);
                    }
                    catch (Exception exc)
                    {
                        _log.Error(exc, $"Could not process margin call model implementation due to error.");
                    }

                    try
                    {
                        if (margincallorders.Length > 0)
                        {
                            //TODO: set to array
                            //TODO: quant fund needs to check if this margin call is related to this fund
                            //Let the Quant Funds know we are about to send a margin call
                            //_portfolio.QuantFunds.ForEach(sf => sf.OnMarginCall(margincallorders));

                            ////Execute margin call
                            //var executedtickets = _portfolio.BrokerModel.GetMarginCallModel()
                            //    .ProcessMarginCall(margincallorders);
                            //executedtickets.ForEach(x => _portfolio.Log(LogSeverity.Error,
                            //    $"Executed margin call order at {x.CreatedUtc} for security {x.Ticker} - Quantity {x.Quantity} @ {x.FillPrice}"));
                        }
                        else if (issuemargincallwarning)
                        {
                            //_portfolio.QuantFunds.ForEach(x => x.OnMarginCallWarning());
                        }
                    }
                    catch (Exception exc)
                    {
                        _portfolio.ExceptionHandler.HandleException(exc);
                        _portfolio.Status = PortfolioStatus.RuntimeError;
                        _log.Error(exc, $"Exception while processing margin call orders, stopping current portfolio");
                        return;
                    }
                }

                //Check for settled and unsettled funds
                if (_portfolio.Clock.CurrentUtc > nextsettlementscan)
                {
                    _portfolio.CashManager.Update(_portfolio.Clock.CurrentUtc);
                    nextsettlementscan = _portfolio.Clock.CurrentUtc + settlementscanfrequency;
                }

                //Apply dividends
                //TODO: apply dividends

                //Apply splits
                //TODO: apply splits

                //Update aggregators with new data
                try
                {
                    foreach (var update in data.AggregatorUpdates)
                    {
                        //Get data
                        var aggregators = update.Target.Aggregators;

                        //Check each aggregator
                        foreach (var aggregator in aggregators)
                        {
                            foreach (var datapoint in update.Data)
                            {
                                //Check if it is a datapoint bar
                                if (!(datapoint is DataPointBar dpbar))
                                    continue;

                                //Only push data into aggregator on the native, subscribed resolution
                                if (EndTimeIsInNativeResolution(update.Target, dpbar.EndTime))
                                    aggregator.Feed(datapoint);
                            }

                            // scan for time after we've pumped all the data through for this aggregator
                            var localtime = _portfolio.Clock.GetDateTimeInZone(update.Target.ExchangeTimeZone);
                            aggregator.Check(localtime);
                        }
                    }
                }
                catch (Exception exc)
                {
                }

                try
                {
                    //Process portfolio
                    _portfolio.OnData(data.Updates);
                }
                catch (Exception exc)
                {
                    _portfolio.ExceptionHandler.HandleException(exc);
                    _portfolio.Status = PortfolioStatus.RuntimeError;
                    _log.Error(exc, $"Runtime error on datapoint processing");
                    return;
                }

                //Check if this is a backtest, in this case send progress update message
                //if (RunMode == RunMode.Backtester)
                //    _portfolio.EventRunner.Enqueue(ProgressMessage.Create("", )); //TODO: check if we need to send a message and create this message

                //Save previous time
                _previoustime = _portfolio.Clock.CurrentUtc;
            }

            //We are done, send final signal
            _log.Trace($"Firing on end of execution");
            try
            {
                _portfolio.QuantFunds.ForEach(x => x.OnTermination());
            }
            catch (Exception exc)
            {
                _portfolio.ExceptionHandler.HandleException(exc);
                _portfolio.Status = PortfolioStatus.RuntimeError;
                _log.Error(exc, $"Exception while processing termination of quant funds");
            }

            //Check reason for stopping this portfolio
            if (_portfolio.Status == PortfolioStatus.Liquidated)
            {
                void Liquidate(IQuantFund quantfund)
                {
                    quantfund.Liquidate();
                    PortfolioImplementations.EventRunner.Enqueue(FundInfoMessage.Generate(_portfolio.Id, quantfund));
                }

                _log.Trace($"Liquidating all portfolio holdings");
                _portfolio.QuantFunds.ForEach(Liquidate);
            }
            else if (_portfolio.Status == PortfolioStatus.Stopped)
            {
                _log.Trace($"Stopped portfolio");
            }
        }

        /// <summary>
        /// Terminates this instance.
        /// </summary>
        public void Terminate()
        {
            //Set to termination
            _portfolio.Status = PortfolioStatus.Terminating;

            //Stop all threads
            PortfolioImplementations.ActionsScheduler.Stop();
            PortfolioImplementations.DataFeed.Stop();
            PortfolioImplementations.OrderTicketHandler.Stop();
            PortfolioImplementations.EventRunner.Stop();

            //Just to be sure
            _runningthreads
                .Where(x => x.IsAlive)
                .ForEach(t => t.Abort());

            //Disconnect from brokerage
            _log.Trace($"Disconnecting broker connection");
            if (PortfolioImplementations.BrokerConnection != null &&
                PortfolioImplementations.BrokerConnection.IsConnected)
                PortfolioImplementations.BrokerConnection.Disconnect();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Initializes the dependecies.
        /// </summary>
        private void InitializeDependecies()
        {
            try
            {
                //Initialize broker connection
                _log.Debug($"Initializing broker connection implementation: {PortfolioImplementations.BrokerConnection.GetType().FullName}");
                if (PortfolioImplementations.BrokerConnection is SimBrokerConnection simBrokerConnection)
                    simBrokerConnection.SetPortfolio(_portfolio);

                //Initialize scheduler
                _log.Debug($"Initializing actionscheduler implementation: {PortfolioImplementations.ActionsScheduler.GetType().FullName}");
                PortfolioImplementations.ActionsScheduler.Initialize(_portfolio.Clock);

                //Set datafeed dependencies
                _log.Debug($"Initializing DataFeed: {PortfolioImplementations.DataFeed.GetType().FullName}");
                PortfolioImplementations.DataFeed.SetDependencies(_portfolio.BrokerAccount.Securities, _portfolio.Subscription, PortfolioImplementations.DataFilter);
                PortfolioImplementations.DataFeed.Initialize(_initialMessageInstance);

                //Initialize clustering
                _log.Debug($"Initializing Cluster: {PortfolioImplementations.Cluster.GetType().FullName}");
                PortfolioImplementations.Cluster.Initialize();

                //Initialize eventrunner
                _log.Debug($"Initializing event runner: {PortfolioImplementations.EventRunner.GetType().FullName}");
                PortfolioImplementations.EventRunner.Initialize(_initialMessageInstance);

                //Initialize MessagQueye
                _log.Debug($"Initializing MessageQueue: {PortfolioImplementations.MessageQueue.GetType().FullName}");
                PortfolioImplementations.MessageQueue.Initialize();

                //Initialize OrderTicketHandler
                _log.Debug(
                    $"Initializing Order Ticket Handler: {PortfolioImplementations.OrderTicketHandler.GetType().FullName}");
                PortfolioImplementations.OrderTicketHandler.Initialize(_portfolio);
            }
            catch (Exception e)
            {
                _log.Error(e, $"Exception while initializing dependecies: {e.Message} Exiting this request.");
                throw;
            }

        }

        /// <summary>
        /// Adds the quant fund to a current portfolio.
        /// </summary>
        /// <param name="addfund">The addfund.</param>
        private void AddQuantFund(AddFundMessage addfund)
        {
            //Get assembly
            byte[] input = Convert.FromBase64String(addfund.Base64Assembly);
            var assembly = Assembly.Load(input);

            //Add to portfolio
            var quantfund = _portfolio.AddFund(assembly, addfund);

            //Start the quant fund
            if(addfund.DirecStart)
                quantfund?.Start();
        }

        /// <summary>
        /// Creates the portfolio.
        /// </summary>
        /// <param name="portfolioid">The portfolioid.</param>
        /// <param name="accountid">The accountid.</param>
        /// <param name="brokermodel">The brokermodel.</param>
        /// <param name="leverage">The leverage.</param>
        /// <param name="basecurrency">The basecurrency.</param>
        /// <param name="displaycurrency">The displaycurrency.</param>
        /// <param name="clock">The clock.</param>
        /// <param name="extendedmarkethours">if set to <c>true</c> [extendedmarkethours].</param>
        /// <returns></returns>
        private IPortfolio CreatePortfolio(string portfolioid, string accountid, BrokerModel brokermodel,
            int leverage, CurrencyType basecurrency, CurrencyType displaycurrency, WorldClock clock, bool extendedmarkethours)
        {
            //Get exchange models
            var exchangemodels = Config.BrokerConfig.FirstOrDefault(x => x.BrokerType == brokermodel.BrokerType.ToString())
                .Exchanges.Select(x => ExchangeModelFactory.GetExchange(clock, x, extendedmarkethours))
                .ToArray();

            //Get cash manager
            var cashmanager = new CashManager(PortfolioImplementations.Currency, leverage);

            //Get security factory
            var securityfactory = new SecurityFactory(brokermodel,
                PortfolioImplementations.Currency, basecurrency, exchangemodels);

            //Get security tracker
            var securitytracker = new SecurityTracker(securityfactory);

            //Get broker account
            var brokeraccount =
                new BrokerAccount(accountid, cashmanager, securitytracker, basecurrency, displaycurrency, leverage);
            cashmanager.SetBrokerAccount(brokeraccount);

            //Return what we have
            return new Portfolio(clock, PortfolioImplementations.ActionsScheduler,
                PortfolioImplementations.BrokerConnection, brokermodel,
                PortfolioImplementations.Currency, PortfolioImplementations.EventRunner,
                PortfolioImplementations.ExceptionHandler, PortfolioImplementations.OrderTicketHandler,
                brokeraccount, cashmanager, RunMode, PortfolioImplementations.DataFeed,
                PortfolioImplementations.Benchmark, portfolioid);
        }

        /// <summary>
        /// Next iteration of datapoints from the datafeed
        /// </summary>
        /// <returns></returns>
        private DataUpdateHolder NextDatapoints()
        {
            //TODO: backfilling process, what to do here, not much I think? => since backfilling packets are combined with the feed, we do need to be able to get back in line with reality though

            //Get data from feed
            var datafeedpoints = PortfolioImplementations.DataFeed.GetAvailableDataPackets().ToArray();

            //Set initials
            var subscriptionupdates = new List<UpdateData<DataSubscription>>();
            var dataupdates = new DataUpdates(_portfolio.Clock.CurrentUtc, datafeedpoints.SelectMany(x => x.Data).ToArray());
            var datapoints = new List<DataPoint>();

            //Go trough packets
            foreach (var datapacket in datafeedpoints)
            {
                //Get data points
                datapoints.AddRange(datapacket.Data);

                //Get data subscription updates
                foreach (var subscription in datapacket.Subscriptions)
                    subscriptionupdates.Add(new UpdateData<DataSubscription>(subscription, subscription.Type, datapacket.Data));
            }

            //Return what we have
            return new DataUpdateHolder(dataupdates, datapoints, subscriptionupdates);
        }

        /// <summary>
        /// Processes the quant fund command.
        /// </summary>
        /// <param name="command">The command.</param>
        private void ProcessCommand(CommandMessage command)
        {
            //Try and get the related quant fund
            var quantfund = _portfolio.QuantFunds.FirstOrDefault(x => x.FundId == command.FundId);
            if (quantfund == null)
                return;

            //Check command type
            switch (command.Type)
            {
                case CommandType.Start:
                    if (quantfund.State != FundState.Running)
                        quantfund.Start();
                    break;

                case CommandType.Stop:
                    quantfund.Stop();
                    break;

                case CommandType.Terminate:
                    _portfolio.RemoveFund(quantfund);
                    break;
            }
        }

        /// <summary>
        /// Processes the delisted ticker symbol.
        /// </summary>
        /// <param name="delistings">The delistings.</param>
        private void ProcessDelistedSymbols(Delistings delistings)
        {
            foreach (var delisted in delistings.Values)
            {
                //Get security
                var security = _portfolio.BrokerAccount.Securities[delisted.Ticker] as SecurityBase;
                if (security == null)
                    continue;

                //Set trading to halted
                //TODO: this
                //security.IsHalted = true;

                //Check if this portfolio has a position
                if (_portfolio.BrokerAccount.Positions[security].IsFlat)
                    continue; //No position, nothing to do

                //Check if delisting will happen now
                var delistingtime = delisted.OccuredUtc;
                var nextmarketopen = security.Exchange.NextMarketOpen(delistingtime, false);
                var nexmarketclose = security.Exchange.NextMarketClose(nextmarketopen, false);

                if (security.LocalTime < nexmarketclose) continue;

                //Liquidate current position
                var orderTicket = SubmitOrderTicket.MarketOrder(null, security,
                    _portfolio.BrokerAccount.Positions[security].FlatQuantity, "Delisting order");
                _portfolio.OrderTicketHandler.Process(orderTicket);
            }
        }

        /// <summary>
        /// Schedules the portfolio actions.
        /// </summary>
        private void ScheduledPortfolioActions()
        {
            //Always send latest message updates (live trading each 5 seconds, backtest each day)
            TimeSpan messageupdates = RunMode == RunMode.LiveTrading ? TimeSpan.FromSeconds(5) : TimeSpan.FromDays(1);
            _portfolio.ActionsScheduler.Add(new ScheduledAction(new DateFunc().EveryDay(), new TimeFunc().Every(messageupdates), (s, d) => _portfolio.SendEventMessages(), "SendEventMessages()"));

            //Update currencies from currency implementation every 90 minutes
            _portfolio.ActionsScheduler.Add(new ScheduledAction(new DateFunc().EveryDay(), new TimeFunc().Every(TimeSpan.FromMinutes(90)), (s, d) => PortfolioImplementations.Currency.Update()));
        }

        /// <summary>
        /// Determines if a data point is in it's native, configured resolution
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        /// <param name="datapointendtime">The datapointendtime.</param>
        /// <returns></returns>
        private bool EndTimeIsInNativeResolution(DataSubscription subscription, DateTime datapointendtime)
        {
            //Check if it is tick data
            if (subscription.Resolution == Resolution.Tick)
                return true;

            //Check resolution
            var roundeddatapointendtime = datapointendtime.RoundDownInTimeZone(subscription.Resolution.TimeSpan.Value,
                subscription.ExchangeTimeZone, subscription.DateTimeZone);
            return datapointendtime == roundeddatapointendtime;
        }

        #endregion Private Methods
    }
}