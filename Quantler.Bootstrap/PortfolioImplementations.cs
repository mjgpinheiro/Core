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

using Quantler.Broker;
using Quantler.Common;
using Quantler.Configuration;
using Quantler.Data.Market.Filter;
using Quantler.Interfaces;
using Quantler.Messaging;
using Quantler.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using NLog;
using Quantler.Performance;

namespace Quantler.Bootstrap
{
    /// <summary>
    /// Portfolio instance implementations
    /// </summary>
    public class PortfolioImplementations
    {
        /// <summary>
        /// Current instance logging
        /// </summary>
        private static readonly ILogger _log = LogManager.GetCurrentClassLogger();

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PortfolioImplementations"/> class.
        /// </summary>
        /// <param name="actionsScheduler">The actions scheduler.</param>
        /// <param name="api">The API.</param>
        /// <param name="brokerconnection">The brokerconnection.</param>
        /// <param name="cluster">The cluster.</param>
        /// <param name="datafeed">The datafeed.</param>
        /// <param name="datafilter">The datafilter.</param>
        /// <param name="eventrunner">The eventrunner.</param>
        /// <param name="exceptionhandler">The exceptionhandler.</param>
        /// <param name="messagequeue">The messagequeue.</param>
        /// <param name="ordertickethandler">The ordertickethandler.</param>
        /// <param name="currency">The currency.</param>
        public PortfolioImplementations(ActionsScheduler actionsScheduler, Benchmark benchmark,
                    BrokerConnection brokerconnection, Cluster cluster, DataFeed datafeed,
                    DataFilter datafilter, EventRunner eventrunner, ExceptionHandler exceptionhandler,
                    MessageQueue messagequeue, OrderTicketHandler ordertickethandler, Currency currency)
        {
            ActionsScheduler = actionsScheduler;
            Benchmark = benchmark;
            BrokerConnection = brokerconnection;
            Cluster = cluster;
            DataFeed = datafeed;
            DataFilter = datafilter;
            EventRunner = eventrunner;
            ExceptionHandler = exceptionhandler;
            MessageQueue = messagequeue;
            OrderTicketHandler = ordertickethandler;
            Currency = currency;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Scheduled events implementation
        /// </summary>
        public ActionsScheduler ActionsScheduler { get; }

        /// <summary>
        /// Gets the broker connection.
        /// </summary>
        public BrokerConnection BrokerConnection { get; }

        /// <summary>
        /// Gets the benchmark implementation.
        /// </summary>
        public Benchmark Benchmark { get; }

        /// <summary>
        /// Cluster implementation in use
        /// </summary>
        public Cluster Cluster { get; set; }

        /// <summary>
        /// Gets the currency model used.
        /// </summary>
        public Currency Currency { get; }

        /// <summary>
        /// Gets the associated data feed for retrieving data
        /// </summary>
        public DataFeed DataFeed { get; }

        /// <summary>
        /// Gets the data filter.
        /// </summary>
        public DataFilter DataFilter { get; }

        /// <summary>
        /// Event runner for publishing updates to any front-end
        /// </summary>
        public EventRunner EventRunner { get; }

        /// <summary>
        /// Gets the exception handler.
        /// </summary>
        public ExceptionHandler ExceptionHandler { get; }

        /// <summary>
        /// Logic for retrieving portfolio commands (on/off/add/remove)
        /// </summary>
        public MessageQueue MessageQueue { get; }

        /// <summary>
        /// Gets the order ticket handler.
        /// </summary>
        public OrderTicketHandler OrderTicketHandler { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Returns the default implementations used from config
        /// </summary>
        /// <param name="broker">The broker.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public static Dictionary<string, string> DefaultConfig(string broker = "")
        {
            //Return object
            Dictionary<string, string> toreturn = new Dictionary<string, string>
            {
                {"DataFeed", Config.GlobalConfig.DataFeed },
                {"Currency", Config.GlobalConfig.Currency },
                {"ActionsScheduler", Config.GlobalConfig.ActionsScheduler },
                {"BrokerConnection", Config.GlobalConfig.BrokerConnection },
                {"Cluster", Config.GlobalConfig.Cluster },
                {"DataFilter", Config.GlobalConfig.DataFilter },
                {"EventRunner", Config.GlobalConfig.EventRunner },
                {"ExceptionHandler", Config.GlobalConfig.ExceptionHandler },
                {"MessageQueue", Config.GlobalConfig.MessageQueue },
                {"OrderTicketHandler", Config.GlobalConfig.OrderTicketHandler },
                {"Benchmark", Config.GlobalConfig.Benchmark }
            };

            //Check for broker type related default config
            if (!string.IsNullOrWhiteSpace(broker))
            {
                //Get broker
                if (!Enum.TryParse(broker, out BrokerType brokertype))
                    throw new ArgumentException($"Broker type {broker} is unknown, cannot initialize portfolio objects for unknown broker type");

                //Get config
                var configfound = Config.BrokerConfig.FirstOrDefault(x => x.BrokerType == brokertype.ToString());
                if (configfound == null)
                    throw new Exception($"Could not find broker configuration for broker with type {brokertype}");

                //Get broker related config
                if (string.IsNullOrWhiteSpace(toreturn["DataFeed"]))
                    toreturn["DataFeed"] = configfound.DataFeed;
                if (string.IsNullOrWhiteSpace(toreturn["Currency"]))
                    toreturn["Currency"] = configfound.CurrencyImplementation;
                if (string.IsNullOrWhiteSpace(toreturn["BrokerConnection"]))
                    toreturn["BrokerConnection"] = configfound.BrokerConnection;
                if (string.IsNullOrWhiteSpace(toreturn["OrderTicketHandler"]))
                    toreturn["OrderTicketHandler"] = configfound.OrderTicketHandler;
            }

            //Return what we have
            return toreturn;
        }

        /// <summary>
        /// Get current associated implementations from config
        /// </summary>
        /// <param name="brokertype">The broker type.</param>
        /// <returns></returns>
        public static PortfolioImplementations FromConfig(string brokertype = "") =>
            FromConfig(DefaultConfig(brokertype));

        /// <summary>
        /// Get current associated implementations based on config supplied
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        /// <exception cref="Exception">
        /// </exception>
        private static PortfolioImplementations FromConfig(Dictionary<string, string> config)
        {
            //Return object
            var loader = DynamicLoader.Instance;

            //Log implementations
            _log.Trace($"PORTFOLIO IMPLEMENTATIONS: ");
            config.ForEach(item => _log.Trace($"\t {item.Key}: {item.Value}"));

            //Error message
            string ErrorMessage(string item) => $"Could not load implementation item {item}";

            //Get items
            string configitem = "ActionsScheduler";
            if (!loader.TryGetInstance(config[configitem], out ActionsScheduler actionsScheduler))
                throw new Exception(ErrorMessage(configitem));

            configitem = "BrokerConnection";
            if (!loader.TryGetInstance(config[configitem], out BrokerConnection brokerconnection))
                throw new Exception(ErrorMessage(configitem));

            configitem = "Benchmark";
            if (!loader.TryGetInstance(config[configitem], out Benchmark benchmark))
                throw new Exception(ErrorMessage(configitem));

            configitem = "Cluster";
            if (!loader.TryGetInstance(config[configitem], out Cluster cluster))
                throw new Exception(ErrorMessage(configitem));

            configitem = "DataFeed";
            if (!loader.TryGetInstance(config[configitem], out DataFeed datafeed))
                throw new Exception(ErrorMessage(configitem));

            configitem = "DataFilter";
            if (!loader.TryGetInstance(config[configitem], out DataFilter datafilter))
                throw new Exception(ErrorMessage(configitem));

            configitem = "EventRunner";
            if (!loader.TryGetInstance(config[configitem], out EventRunner eventrunner))
                throw new Exception(ErrorMessage(configitem));

            configitem = "ExceptionHandler";
            if (!loader.TryGetInstance(config[configitem], out ExceptionHandler exceptionhandler))
                throw new Exception(ErrorMessage(configitem));

            configitem = "MessageQueue";
            if (!loader.TryGetInstance(config[configitem], out MessageQueue messagequeue))
                throw new Exception(ErrorMessage(configitem));

            configitem = "OrderTicketHandler";
            if (!loader.TryGetInstance(config[configitem], out OrderTicketHandler orderTicketHandler))
                throw new Exception(ErrorMessage(configitem));

            configitem = "Currency";
            if (!loader.TryGetInstance(config[configitem], out Currency currency))
                throw new Exception(ErrorMessage(configitem));

            //Return what we have
            return new PortfolioImplementations(actionsScheduler, benchmark, brokerconnection, cluster, datafeed,
                datafilter, eventrunner, exceptionhandler, messagequeue, orderTicketHandler, currency);
        }

        #endregion Public Methods
    }
}