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
using Quantler.Interfaces;
using System;

namespace Quantler.Scheduler
{
    /// <summary>
    /// Scheduled action
    /// </summary>
    public class ScheduledAction : ScheduledEventAction
    {
        #region Private Fields

        /// <summary>
        /// Action to perform
        /// </summary>
        private readonly Action<string, DateTime> _action;

        /// <summary>
        /// Date composite rule
        /// </summary>
        private readonly DateComposite _date;

        /// <summary>
        /// Logging instance
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Time composite rule
        /// </summary>
        private readonly TimeComposite _time;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initialize new scheduled action instance
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <param name="action"></param>
        /// <param name="name"></param>
        public ScheduledAction(DateComposite date, TimeComposite time, Action<string, DateTime> action, string name = "")
        {
            //Check for name input (and make sure it is unique)
            if (string.IsNullOrWhiteSpace(name))
                name = string.Join("-", date.Name, time.Name, Guid.NewGuid());
            else
                name += "-" + Guid.NewGuid();

            //Add fund id, if applicable
            if (!string.IsNullOrWhiteSpace(FundId))
                name = $"{FundId}-{name}";

            //Set name and other variables
            Name = name;
            _action = action;
            _date = date;
            _time = time;
        }

        /// <summary>
        /// Initialize new scheduled action instance
        /// </summary>
        /// <param name="quantfund"></param>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <param name="action"></param>
        /// <param name="name"></param>
        public ScheduledAction(IQuantFund quantfund, DateComposite date, TimeComposite time, Action<string, DateTime> action, string name = "")
            : this(date, time, action, name) => FundId = quantfund.FundId;

        #endregion Public Constructors

        #region Public Properties

        public string FundId { get; }

        /// <summary>
        /// If true, this action is enabled
        /// </summary>
        public bool IsEnabled
        {
            get;
            set;
        } = true;

        /// <summary>
        /// If true, this actions execution is logged
        /// </summary>
        public bool IsLogged => Config.GlobalConfig.SchedulerLoggingEnabled;

        /// <summary>
        /// Name of the action
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Next moment in time to execute the attached action
        /// </summary>
        public DateTime NextActionUtc
        {
            private set;
            get;
        } = DateTime.MinValue;

        /// <summary>
        /// Gets the event exception, if thrown.
        /// </summary>
        public ScheduledEventException EventException
        {
            private set;
            get;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Check current date and time for next event
        /// </summary>
        /// <param name="utctime"></param>
        public void Check(DateTime utctime)
        {
            //Get next moment
            if (NextActionUtc == DateTime.MinValue)
            {
                SetNextActionDateTime(utctime);
                _log.Trace($"Skipping scheduled action with name {Name} until {utctime}, next execution will be scheduled @ {NextActionUtc}");
            }
            else if (NextActionUtc <= utctime)
            {
                //Execute
                if (IsEnabled)
                {
                    if (IsLogged)
                        _log.Trace($"Executing scheduled action {Name} @ {utctime}");

                    try
                    {
                        _action?.Invoke(Name, utctime);
                    }
                    catch (Exception exc)
                    {
                        IsEnabled = false;
                        var exception = new ScheduledEventException(exc.Message);
                        EventException = exception;
                        throw exception;
                    }
                }
                else if (IsLogged)
                    _log.Trace($"Skipping scheduled action with name {Name} as it is disabled");

                //Get next moment in time
                SetNextActionDateTime(utctime);
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Set the next derived date and time to execute this action
        /// </summary>
        /// <param name="currentutc"></param>
        private void SetNextActionDateTime(DateTime currentutc)
        {
            //Get date
            DateTime next = _date.NextDate(currentutc);

            //Get Time
            next = _time.NextTimeOfDay(next);

            //Set next
            NextActionUtc = next;
        }

        #endregion Private Methods
    }
}