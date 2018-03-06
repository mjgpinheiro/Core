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

using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using NLog;
using Quantler.Interfaces;

namespace Quantler.Scheduler
{
    /// <summary>
    /// Keeper of scheduled actions
    /// </summary>
    public class ScheduledActionsKeeper
    {
        #region Private Fields

        /// <summary>
        /// The log
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Known scheduled actions
        /// </summary>
        private readonly Dictionary<string, ScheduledEventAction> _eventActions = new Dictionary<string, ScheduledEventAction>();

        /// <summary>
        /// Locker object
        /// </summary>
        private readonly object _locker = new object();

        /// <summary>
        /// The world clock
        /// </summary>
        private readonly WorldClock _worldClock;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledActionsKeeper"/> class.
        /// </summary>
        /// <param name="clock">The clock.</param>
        public ScheduledActionsKeeper(WorldClock clock)
        {
            _worldClock = clock;
            DateFunc = new DateFunc();
            TimeFunc = new TimeFunc();
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the next action UTC.
        /// </summary>
        public DateTime NextActionUtc => _eventActions.Values.ToArray().Where(x => x.IsEnabled).Min(x => x.NextActionUtc);

        /// <summary>
        /// Gets the date functions.
        /// </summary>
        public DateFunc DateFunc { get; }

        /// <summary>
        /// Gets the time functions.
        /// </summary>
        public TimeFunc TimeFunc { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Add a new scheduled action
        /// </summary>
        /// <param name="scheduledaction"></param>
        public ScheduledEventAction Add(ScheduledEventAction scheduledaction)
        {
            lock (_locker)
            {
                //Check if already exists
                if (!_eventActions.ContainsKey(scheduledaction.Name))
                    _eventActions.Add(scheduledaction.Name, scheduledaction);
                else
                    throw new Exception($"Scheduled action with name {scheduledaction.Name} already exists. Name should be unique");
            }

            //Return what we have added
            return scheduledaction;
        }

        /// <summary>
        /// Executes all action due to be executed
        /// </summary>
        public void CheckAll()
        {
            lock (_locker)
            {
                foreach (var action in _eventActions.Values.Where(x => x.IsEnabled).OrderBy(x => x.NextActionUtc))
                {
                    try
                    {
                        action.Check(_worldClock.CurrentUtc);
                    }
                    catch (Exception exc)
                    {
                        _log.Error(exc, $"Could not process action due to exception, action name = {action.Name}. Scheduled action will be disabled.");
                        action.IsEnabled = false;
                    }
                }
            }
        }

        /// <summary>
        /// Add new scheduled action
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public ScheduledEventAction Event(DateComposite date, TimeComposite time, Action<string, DateTime> action) =>
            Event(date, time, action);

        /// <summary>
        /// Add new scheduled action
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public ScheduledEventAction Event(DateComposite date, TimeComposite time, Action action) =>
            Event(date, time, (n, d) => action());

        /// <summary>
        /// Add new scheduled action
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <param name="action"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ScheduledEventAction Event(DateComposite date, TimeComposite time, Action<string, DateTime> action, string name = "")
        {
            var toadd = new ScheduledAction(date, time, action, name);
            Add(toadd);
            return toadd;
        }

        /// <summary>
        /// Add new scheduled action
        /// </summary>
        /// <param name="name"></param>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public ScheduledEventAction Event(string name, DateComposite date, TimeComposite time, Action action) =>
            Event(date, time, (n, d) => action(), name);

        /// <summary>
        /// Remove a scheduled action from known events
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            lock (_locker)
            {
                _eventActions.Remove(name);
            }
        }

        /// <summary>
        /// Removes all the scheduled actions for the specified quantfund.
        /// </summary>
        /// <param name="quantfund">The quantfund.</param>
        public void Remove(IQuantFund quantfund)
        {
            lock (_locker)
            {
                var found = _eventActions.Where(x => x.Value.FundId == quantfund.FundId).ToArray();
                found.ForEach(e => _eventActions.Remove(e.Key));
            }
        }

        #endregion Public Methods
    }
}