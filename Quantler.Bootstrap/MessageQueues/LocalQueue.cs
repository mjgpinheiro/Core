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
using Quantler.Configuration;
using Quantler.Configuration.Model;
using Quantler.Messaging;
using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Quantler.Bootstrap.MessageQueues
{
    /// <summary>
    /// Local queue derives messages from file input
    /// </summary>
    [Export(typeof(MessageQueue))]
    public class LocalQueue : MessageQueue
    {
        #region Public Properties

        /// <summary>
        /// True if this task is running
        /// </summary>
        public bool IsRunning => true; //Always running

        #endregion Public Properties

        #region Private Fields

        /// <summary>
        /// Currently known messages
        /// </summary>
        private readonly List<MessageInstance> messages = new List<MessageInstance>();

        /// <summary>
        /// Current instance logging
        /// </summary>
        private ILogger _log = LogManager.GetCurrentClassLogger();

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Acknowledge current message
        /// </summary>
        /// <param name="message"></param>
        public void Acknowledge(MessageInstance message)
        {
            //Notify
            _log.Info($"Local message instance {message.UniqueId} was acknowledged");

            //Remove
            messages.Remove(message);
        }

        /// <summary>
        /// Set result of processing this message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="result"></param>
        public void Complete(MessageInstance message, MessageResult result)
        {
            //Notify
            _log.Info($"Local message instance {message.UniqueId} was completed with result {result}");
        }

        /// <summary>
        /// Initialize message queue instance
        /// </summary>
        public void Initialize()
        {
            //Check if assembly exists
            if (!TryConvertAssemblyToBase64(Config.PortfolioConfig.Assembly, out string assembly))
            {
                _log.Error($"Cannot derive quant fund from base64 encoded variant, please check input");
                return;
            }

            //Get message instance based on config
            if (Config.GlobalConfig.Mode == "Backtester")
            {
                //Check if quant funds are defined in config
                if (Config.PortfolioConfig.QuantFunds.Length == 0)
                {
                    _log.Error($"No quant funds defined in portfolio config, cannot initiate backtest");
                    return;
                }

                //Set initiating message
                SimulationMessage backtestmessage = new SimulationMessage
                {
                    UniqueId = Guid.NewGuid().ToString(),
                    AccountType = Config.PortfolioConfig.AccountType,
                    BaseCurrency = Config.PortfolioConfig.BaseCurrency,
                    BrokerType = Config.PortfolioConfig.BrokerType,
                    Leverage = Config.PortfolioConfig.Leverage,
                    MessageType = MessageType.Backtest,
                    IsResult = false,
                    PortfolioId = Guid.NewGuid().ToString(),
                    SendUtc = DateTime.UtcNow,
                    StartDateTime = Time.FromUnixTime(Config.PortfolioConfig.StartDateUtc),
                    EndDateTime = Time.FromUnixTime(Config.PortfolioConfig.EndDateUtc),
                    FrameworkVersion = Framework.CurrentVersion,
                    ExtendedMarketHours = Config.PortfolioConfig.ExtendedMarketHours
                };

                //Set fund message
                backtestmessage.QuantFund = CreateFund(Config.PortfolioConfig.QuantFunds[0], assembly);

                //Add to known messages
                messages.Add(backtestmessage);
            }
            else if (Config.GlobalConfig.Mode == "LiveTrading")
            {
                //Create message
                var livetradingmessage = new LiveTradingMessage
                {
                    AccountId = "",
                    AccountType = Config.PortfolioConfig.AccountType,
                    QuantFund = CreateFund(Config.PortfolioConfig.QuantFunds[0], assembly),
                    UniqueId = Guid.NewGuid().ToString(),
                    MessageType = MessageType.LiveTrading,
                    SendUtc = DateTime.UtcNow,
                    FrameworkVersion = Framework.CurrentVersion,
                    IsResult = false,
                    BaseCurrency = Config.PortfolioConfig.BaseCurrency,
                    BrokerType = Config.PortfolioConfig.BrokerType,
                    ExtendedMarketHours = Config.PortfolioConfig.ExtendedMarketHours,
                    Leverage = Config.PortfolioConfig.Leverage,
                    PortfolioId = Guid.NewGuid().ToString(),
                    DisplayCurrency = Config.PortfolioConfig.DisplayCurrency,
                    ResultOk = false,
                    ResultMessage = string.Empty
                };

                //Add to known messages
                messages.Add(livetradingmessage);
            }
            else
                _log.Error($"Could not initialize local message queue, no input found in global config");
        }

        /// <summary>
        /// Start task
        /// </summary>
        public void Start()
        {
            //N/A
        }

        /// <summary>
        /// Stop task
        /// </summary>
        public void Stop()
        {
            //N/A
        }

        /// <summary>
        /// Try to get the next message in the queue
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryGetNextMessage(out MessageInstance item)
        {
            //Default item
            item = null;

            //Get item
            if (messages.Count > 0)
            {
                item = messages.First();
                return true;
            }
            else
                return false;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Creates the fund message instance.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private AddFundMessage CreateFund(QuantFundConfig config, string assembly)
        {
            var toreturn = new AddFundMessage
            {
                Universe = config.StaticUniverse.ToDictionary(x => x.Ticker, x => x.Weight),
                AllocatedFunds = config.AllocatedFunds,
                Base64Assembly = assembly,
                ForceTick = config.ForceTick,
                FundId = string.IsNullOrWhiteSpace(config.Id) ? Guid.NewGuid().ToString() : config.Id,
                UniqueId = Guid.NewGuid().ToString(),
                MessageType = MessageType.AddFund,
                SendUtc = DateTime.UtcNow,
                FrameworkVersion = Framework.CurrentVersion,
                IsResult = false,
                FundName = config.Name,
                Parameters = config.Parameters.Select(x => new ModuleParameter
                {
                    Name = x.Name,
                    ModuleName = x.ModuleName,
                    Value = x.Value
                }).ToList(),
                UniverseName = config.UniverseName,
                ModuleNames = config.Modules
            };

            //Check universe weights
            if (toreturn.Universe.Sum(x => x.Value) != 1)
            {
                _log.Error(
                    $"Sum of universe attached to quant fund with name {config.Name} and universe name {config.UniverseName} does not sum to 1, cannot use a universe that does not sum to 1. Exiting...");
                throw new Exception($"Universe weights of universe {config.UniverseName} does not sum to 1");
            }

            //Return what we have
            return toreturn;
        }

        /// <summary>
        /// Try and convert an assembly to its base64 representation
        /// </summary>
        /// <param name="assemblypath"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool TryConvertAssemblyToBase64(string assemblypath, out string result)
        {
            //Set initial value
            result = string.Empty;

            //try and retrieve assembly
            try
            {
                var assembly = Assembly.LoadFile(assemblypath);
                byte[] bytearray = File.ReadAllBytes(assemblypath);
                result = Convert.ToBase64String(bytearray);
                return true;
            }
            catch (Exception exc)
            {
                _log.Error(exc, $"Could not retrieve assembly where modules are present due to error: {exc.Message}");
                return false;
            }
        }

        #endregion Private Methods
    }
}