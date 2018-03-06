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
using Quantler.Common;
using Quantler.Configuration;
using Quantler.Interfaces;
using Quantler.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Quantler.Bootstrap
{
    /// <summary>
    /// Main Thread
    /// </summary>
    public class Bootstrap
    {
        #region Private Fields

        /// <summary>
        /// Current class logger
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Current isolator instance
        /// </summary>
        private Isolator _isolator;

        /// <summary>
        /// The current run mode
        /// </summary>
        private RunMode _runmode;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initialize bootstrap
        /// </summary>
        /// <param name="instanceimpl"></param>
        public Bootstrap(InstanceImplementations instanceimpl) =>
            InstanceImplementations = instanceimpl;

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the cancellation token.
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; private set; }

        /// <summary>
        /// Gets the current portfolio.
        /// </summary>
        public PortfolioManager CurrentPortfolio { get; private set; }

        /// <summary>
        /// Associated instance implementations
        /// </summary>
        public InstanceImplementations InstanceImplementations { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Exits this instance.
        /// </summary>
        public void Exit(bool force = false)
        {
            if (!force)
            {
                //Stop receiving new jobs
                InstanceImplementations.JobQueue.Stop();

                //Terminate current portfolio
                CurrentPortfolio.Terminate();
            }
            else
                CancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Run the current instance (main thread)
        /// </summary>
        public void Run(RunMode mode)
        {
            try
            {
                //Set instance details
                _runmode = mode;

                //Start incoming message queue
                try
                {
                    InstanceImplementations.JobQueue.Initialize();
                    InstanceImplementations.JobQueue.Start();
                }
                catch (Exception exc)
                {
                    _log.Error(exc, $"Could not start message queue due to error. Exiting application");
                    return;
                }

                //Check for run mode
                if (mode == RunMode.Backtester)
                {
                    //Simply keep running the main thread
                    while (InstanceImplementations.JobQueue.IsRunning)
                    {
                        //Check for new messages
                        if (!InstanceImplementations.JobQueue.TryGetNextMessage(out MessageInstance message))
                        {
                            _log.Trace("No messages found for backtesting, waiting 5 seconds for the next message check...");
                            Thread.Sleep(5000); //Wait for next message for 5 seconds
                        }
                        else if (message.MessageType != MessageType.Backtest)
                        {
                            _log.Error($"Retrieved message that was not of expected type {MessageType.Backtest} but was of type {message.MessageType}. Discarding message.");
                            InstanceImplementations.JobQueue.Complete(message, MessageResult.Resend);
                        }
                        else if (Framework.IsNotEqualVersion(message.FrameworkVersion))
                        {
                            //We cannot handle a message with is made for another framework version, resend
                            _log.Warn($"Received message with inconsistent framework version, retrieved {message.FrameworkVersion}, while current version is {Framework.CurrentVersion}");
                            InstanceImplementations.JobQueue.Complete(message, MessageResult.Resend);
                        }
                        else
                        {
                            //Ack message
                            InstanceImplementations.JobQueue.Acknowledge(message);

                            //Run backtest
                            try
                            {
                                if (message is SimulationMessage backtestmessage)
                                {
                                    //Start a live trading instance
                                    CurrentPortfolio = new PortfolioManager(PortfolioImplementations.FromConfig(backtestmessage.BrokerType), backtestmessage);

                                    //Run
                                    RunInControlledEnvironment(TimeSpan.FromSeconds(Config.GlobalConfig.BacktestTimeMaxSeconds), 1064, () => CurrentPortfolio.Run());
                                    CurrentPortfolio.Terminate();

                                    //Process backtest
                                    InstanceImplementations.JobQueue.Complete(message, MessageResult.Success);
                                }
                                else
                                {
                                    //Something strange happened
                                    _log.Error($"Could not deserialize message for backtest.");
                                    InstanceImplementations.JobQueue.Complete(message, MessageResult.DeadLettered);
                                }
                            }
                            catch (Exception exc)
                            {
                                //Notify the front-end
                                CurrentPortfolio.PortfolioImplementations.ExceptionHandler.HandleException(new Exception($"Failed to initialize the request."));

                                //Deadletter this item because it failed
                                _log.Error(exc, "Deadlettered message, because something terrible happened");
                                InstanceImplementations.JobQueue.Complete(message, MessageResult.DeadLettered);
                            }
                        }
                    }
                }
                else if (mode == RunMode.LiveTrading)
                {
                    while (InstanceImplementations.JobQueue.IsRunning)
                    {
                        //get live trading instane information
                        if (!InstanceImplementations.JobQueue.TryGetNextMessage(out MessageInstance message))
                        {
                            _log.Trace("No messages found for live trading, waiting 5 seconds for next message...");
                            Thread.Sleep(5000); //Wait for next message
                        }
                        else if (message.MessageType != MessageType.LiveTrading)
                            throw new Exception($"Epected a livetrading message, but recieved {message.MessageType} from messagequeue instead");
                        else if (Framework.IsNotEqualVersion(message.FrameworkVersion))
                        {
                            //We cannot handle a message with is made for another framework version, resend
                            _log.Warn(
                                $"Received message with inconsistent framework version, retrieved {message.FrameworkVersion}, while current version is {Framework.CurrentVersion}");
                            InstanceImplementations.JobQueue.Complete(message, MessageResult.Resend);
                        }
                        else
                        {
                            //Start a live trading instance
                            var livetradingmessage = message as LiveTradingMessage;
                            CurrentPortfolio = new PortfolioManager(PortfolioImplementations.FromConfig(livetradingmessage.BrokerType), livetradingmessage);

                            //Run
                            RunInControlledEnvironment(TimeSpan.MaxValue, 1024, () =>
                            {
                                //No need to request a new item
                                InstanceImplementations.JobQueue.Stop();

                                //Notify messagequeu
                                InstanceImplementations.JobQueue.Complete(message, MessageResult.Success);

                                //Run instance
                                CurrentPortfolio.Run();
                            });

                            //If we are done with live trading, exit this instance
                            Exit();
                        }
                    }
                }
                else
                    throw new Exception($"Runmode {mode} is not implemented");
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Fatal error, exiting application...");
                Environment.Exit(1);
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Cooldowns this instance, stops all assisting threads and releases all leftovers for a new run or exit.
        /// </summary>
        private void Cooldown()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Runs this instance in a controlled environment.
        /// </summary>
        /// <param name="maxruntime">The maxruntime.</param>
        /// <param name="maxram">The maxram.</param>
        /// <param name="action">The action.</param>
        private void RunInControlledEnvironment(TimeSpan maxruntime, long maxram, Action action)
        {
            try
            {
                //Create new isolator
                _isolator = new Isolator();
                CancellationTokenSource = _isolator.CancellationTokenSource;

                //Execute the requested instance in a controlled environment
                var complete = _isolator.ExecuteWithTimeLimit(maxruntime, null, action, maxram);

                //Check results
                if (!complete)
                {
                    _log.Error($"Failed to complete within time limits: {maxruntime}");

                    //Send to portfolio for the user to know
                    CurrentPortfolio.PortfolioImplementations.ExceptionHandler.HandleException(new Exception($"Failed to complete instance request within the time limit of {maxruntime}, please make your quant fund run faster."));
                }
            }
            catch (Exception exc)
            {
                CurrentPortfolio.PortfolioImplementations.ExceptionHandler.HandleException(new Exception("Unhandled exception occurred during execution, something went wrong."));
                throw exc;
            }
        }

        #endregion Private Methods

    }
}