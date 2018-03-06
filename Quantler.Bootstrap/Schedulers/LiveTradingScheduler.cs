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
using System;
using System.Composition;
using System.Threading;
using Quantler.Interfaces;

namespace Quantler.Bootstrap.Schedulers
{
    /// <summary>
    /// Live trading implementation for scheduled actions
    /// </summary>
    [Export(typeof(ActionsScheduler))]
    public class LiveTradingScheduler : BaseSchedulerImpl
    {
        #region Private Fields

        /// <summary>
        /// Current logging instance
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Currently running thread
        /// </summary>
        private Timer _processItems;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Start new instance
        /// </summary>
        public override void Start()
        {
            if (!IsRunning)
            {
                _log.Info("Starting scheduler");
                _processItems = new Timer(OnProcess, "", 1000 - DateTime.UtcNow.Millisecond, 1000); //Check every (full) second for new actions to be performed
                IsRunning = true;
            }
        }

        /// <summary>
        /// Stop current instance
        /// </summary>
        public override void Stop()
        {
            if (IsRunning)
            {
                //Turn off timer
                _processItems.Change(0, 0);

                //Release last items
                _log.Info("Stopping action scheduler");

                //Set to stopped state
                _log.Info("Action scheduler has been stopped");
                IsRunning = false;
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Process actions
        /// </summary>
        /// <param name="state"></param>
        private void OnProcess(object state) =>
            Poke();

        #endregion Private Methods
    }
}