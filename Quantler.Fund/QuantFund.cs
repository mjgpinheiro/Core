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
using Quantler.Common;
using Quantler.Interfaces;
using Quantler.Messaging;
using Quantler.Orders;
using Quantler.Performance;
using Quantler.Securities;
using Quantler.Tracker;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Quantler.Configuration;
using Quantler.Data.Aggegrate;
using Quantler.Modules;

namespace Quantler.Fund
{
    /// <summary>
    /// Quant Fund logic and implementations
    /// </summary>
    public partial class QuantFund : IQuantFund
    {
        #region Private Fields

        /// <summary>
        /// Current class logging instance
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Current module states
        /// </summary>
        private readonly ConcurrentDictionary<Security, SecurityStateHolder> _states = new ConcurrentDictionary<Security, SecurityStateHolder>();

        /// <summary>
        /// The associated modules
        /// </summary>
        private readonly List<IModule> _modules = new List<IModule>();

        /// <summary>
        /// Check whether or not this is a new quant fund or an already running quant fund
        /// </summary>
        private readonly bool _isnew = true;

        /// <summary>
        /// The end date and time for the backfilling process
        /// </summary>
        private DateTime _backfillingendDate = DateTime.MinValue;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuantFund"/> class.
        /// </summary>
        /// <param name="portfolio">The portfolio.</param>
        /// <param name="benchmark">The benchmark.</param>
        /// <param name="fundid"></param>
        /// <param name="allocatedcapital">The allocatedcapital.</param>
        /// <param name="isForceTick"></param>
        /// <param name="name"></param>
        public QuantFund(IPortfolio portfolio, Benchmark benchmark, string fundid, decimal allocatedcapital, bool isForceTick, string name)
        {
            //Set references
            Portfolio = portfolio;
            Benchmark = benchmark;
            FundId = fundid;
            IsForceTick = isForceTick;
            Name = name;

            //Set objects
            StartedDTUtc = DateTime.MinValue;
            Positions = new PositionTracker(portfolio.BrokerAccount, FundId, benchmark, allocatedcapital);
            Results = new Result(allocatedcapital, benchmark);

            //Initial state
            State = FundState.Stopped;
            BackFillingPeriod = TimeSpan.Zero;
            MaxOrdersPerDay = Config.GlobalConfig.MaxOrdersPerDay;

            //Initialize and allocate funds to this quant fund
            portfolio.CashManager.AddQuantFund(this, portfolio.BrokerAccount.Currency, allocatedcapital);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Returns the period in time needed for backfilling as set by any of the modules
        /// </summary>
        public TimeSpan BackFillingPeriod { get; set; }

        /// <summary>
        /// Get currently set benchmark for this quant fund
        /// </summary>
        public Benchmark Benchmark { get; }

        /// <summary>
        /// Gets the fund identifier.
        /// </summary>
        public string FundId { get; private set; }

        /// <summary>
        /// True if this agent is in the process of loading historical data
        /// </summary>
        public bool IsBackfilling => State == FundState.Backfilling;

        /// <summary>
        /// Gets a value indicating whether [force tick].
        /// </summary>
        /// <value>
        /// <c>true</c> if [force tick]; otherwise, <c>false</c>.
        /// </value>
        public bool IsForceTick { get; private set; }

        /// <summary>
        /// True if this quant fund is running and processing data
        /// </summary>
        public bool IsRunning => State == FundState.Running;

        /// <summary>
        /// Returns all modules used by this quant fund
        /// </summary>
        public IModule[] Modules => _modules.ToArray();

        /// <summary>
        /// Name of this quant fund
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the currently active order tickets.
        /// </summary>
        public OrderTicket[] OrderTickets { get; }

        /// <summary>
        /// Returns known pending orders for this quant fund
        /// </summary>
        public PendingOrder[] PendingOrders =>
            Portfolio.OrderTracker.PendingOrders.Where(x => x.FundId == FundId).ToArray();

        /// <summary>
        /// Returns the portfolio for which this quant fund is part of
        /// </summary>
        public IPortfolio Portfolio { get; }

        /// <summary>
        /// Current positions hold by this quant fund
        /// </summary>
        public PositionTracker Positions { get; }

        /// <summary>
        /// Current trading results calculated for this quant fund (isolated)
        /// </summary>
        public Result Results { get; }

        /// <summary>
        /// Date and time on which this quant fund was set to start
        /// </summary>
        public DateTime StartedDTUtc { get; }

        /// <summary>
        /// Gets the current quant fund state.
        /// </summary>
        public FundState State { get; private set; }

        /// <summary>
        /// Security universe for this quant fund
        /// </summary>
        public Universe Universe { get; private set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Gets the current fund-state for the specified security.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <returns></returns>
        public SecurityState GetState(Security security) =>
            _states.TryGetValue(security, out SecurityStateHolder holder) ? holder.Consensus : SecurityState.NoEntry;

        /// <summary>
        /// Initializes the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="fundinfo">The fundinfo.</param>
        public void Initialize(Assembly assembly, AddFundMessage fundinfo)
        {
            try
            {
                //Set state
                State = FundState.Initializing;

                //Set fund universe
                Universe = Universe.Create(fundinfo.UniverseName, Portfolio.BrokerAccount.Securities, fundinfo.Universe);

                //Set fund modules
                foreach (var modulename in fundinfo.ModuleNames)
                {
                    //Try and get the module instance
                    if (!DynamicLoader.TryGetInstance(assembly, modulename, out IModule instance))
                        throw new Exception($"Could not find module {modulename} in provided assembly. Did you add the export attribute?");

                    //Set quantfund
                    instance.SetQuantFund(this);

                    //Set parameters
                    fundinfo.Parameters.Where(x => x.ModuleName == modulename)
                        .ForEach(parm => instance.SetParameter(parm.Name, parm.Value));

                    //Add to modules
                    _modules.Add(instance);
                }

                //Set universe to position tracker
                Positions.SetUniverse(Universe.Securities.Select(x => x.Ticker).ToArray());

                //Set benchmark
                Benchmark.OnCalc(x => Universe.Sum(s => s.Price * Universe.GetWeight(s)));

                //Subscribe to all ticker symbols by default
                Universe.ForEach(x => Portfolio.Subscription.AddSubscription(this, x, new TickQuoteBarAggregator(TimeSpan.FromMinutes(1)), fundinfo.ForceTick));

                //Initialize all modules
                _modules.ForEach(m => m.Initialize());
            }
            catch (Exception exc)
            {
                _log.Error(exc, $"Could not initialize quant fund with name {Name} due to error: {exc.Message}");
                Portfolio.ExceptionHandler.HandleException(exc, FundId);
                State = FundState.DeployError;
            }
        }

        /// <summary>
        /// Liquidates this quant fund (and all its holdings).
        /// </summary>
        public void Liquidate()
        {
            //TODO: check all positions, send liquidate market order and wait for it to be filled (if currently markets are opened, else do not wait)
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the fund state for the corresponding security.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <param name="security">The security.</param>
        /// <param name="state">The state.</param>
        public void SetState(IModule module, Security security, SecurityState state)
        {
            //Check if module type is correct
            if (module is SignalModule signalmodule)
            {
                //Update state
                _states.AddOrUpdate(security, new SecurityStateHolder(security), (s, h) =>
                {
                    h.SetState(module, state);
                    return h;
                });

                //Check state for actions
                var consensus = _states[security].Consensus;
                if (consensus != SecurityState.Error && consensus != SecurityState.NoEntry)
                {
                    //Create order ticket
                    var orderTicket = signalmodule.CreateOrder(security, consensus);
                    orderTicket.Quantity = 0; //Force empty order ticket

                    //Send order to be processed
                    ProcessTicket(orderTicket);
                }
            }
        }

        /// <summary>
        /// Start the current agent and allow it to process data
        /// </summary>
        public void Start()
        {
            //Check if we are already running
            if (IsRunning)
            {
                _log.Warn($"An attempt was made to start a quant fund which is already running, the quant fund id = {FundId}");
                return;
            }

            //Check if we need to perform backfilling and other checks
            if (_isnew)
            {
                //Check if we need to check backfilling
                if (BackFillingPeriod > TimeSpan.Zero)
                {
                    //Set state for backfilling
                    State = FundState.Backfilling;
                    _backfillingendDate = Portfolio.Clock.CurrentUtc.Add(new TimeSpan(BackFillingPeriod.Ticks * -1)).Date; //TODO: while backtesting this might be different?

                    //Request data from feed for backfilling
                    //TODO: process flow of backfilling, we need to take in to account: already running instances, data which is coming in during the backfilling (to get back in line)
                }
                else
                    State = FundState.Running;
            }
            else
                State = FundState.Running;
        }

        /// <summary>
        /// Stop the current agent from running
        /// </summary>
        public void Stop() =>
            State = FundState.Stopped;

        #endregion Public Methods
    }
}