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

using Quantler.Interfaces;
using Quantler.Scheduler;
using System;

namespace Quantler.Bootstrap.Schedulers
{
    /// <summary>
    /// Base implementation for scheduled actions
    /// </summary>
    /// <seealso cref="Quantler.Interfaces.ActionsScheduler" />
    public abstract class BaseSchedulerImpl : ActionsScheduler
    {
        #region Public Properties

        /// <summary>
        /// True if this task is running
        /// </summary>
        public bool IsRunning
        {
            protected set;
            get;
        }

        /// <summary>
        /// Gets the scheduled actions keeper.
        /// </summary>
        public ScheduledActionsKeeper ScheduledActionsKeeper
        {
            protected set;
            get;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Adds the specified scheduled action.
        /// </summary>
        /// <param name="scheduledaction">The scheduled action.</param>
        public void Add(ScheduledEventAction scheduledaction) =>
            ScheduledActionsKeeper.Add(scheduledaction);

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public virtual void Initialize(WorldClock clock) =>
            ScheduledActionsKeeper = new ScheduledActionsKeeper(clock);

        /// <summary>
        /// Pokes the using the specified utc date and time.
        /// </summary>
        public virtual void Poke() =>
            ScheduledActionsKeeper.CheckAll();

        /// <summary>
        /// Pokes the past actions.
        /// </summary>
        /// <param name="currentutc">The currentutc.</param>
        public virtual void PokePastActions(DateTime currentutc) =>
            ScheduledActionsKeeper.NextActionUtc.DoWhile(x =>
            {
                Poke();
                return x;
            }, x => ScheduledActionsKeeper.NextActionUtc > currentutc);

        /// <summary>
        /// Removes the specified event action by name.
        /// </summary>
        /// <param name="name">The name.</param>
        public void Remove(string name) =>
            ScheduledActionsKeeper.Remove(name);

        /// <summary>
        /// Start task
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Stop task
        /// </summary>
        public abstract void Stop();

        #endregion Public Methods
    }
}