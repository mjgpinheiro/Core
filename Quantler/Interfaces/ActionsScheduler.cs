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

using Quantler.Scheduler;
using System;

namespace Quantler.Interfaces
{
    /// <summary>
    /// Scheduler for scheduled events
    /// </summary>
    /// <seealso cref="Quantler.Interfaces.QTask" />
    public interface ActionsScheduler : QTask
    {
        #region Public Properties

        /// <summary>
        /// Gets the scheduled actions keeper.
        /// </summary>
        ScheduledActionsKeeper ScheduledActionsKeeper { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Adds the specified scheduled action.
        /// </summary>
        /// <param name="scheduledaction">The scheduled action.</param>
        void Add(ScheduledEventAction scheduledaction);

        /// <summary>
        /// Pokes the current implementation for new actions
        /// </summary>
        void Poke();

        /// <summary>
        /// Pokes for any past actions left to be performed
        /// </summary>
        /// <param name="currentutc">The currentutc.</param>
        void PokePastActions(DateTime currentutc);

        /// <summary>
        /// Initializes this instance with the worldclock implementation.
        /// </summary>
        /// <param name="clock">The clock.</param>
        void Initialize(WorldClock clock);

        /// <summary>
        /// Removes the specified event action by name.
        /// </summary>
        /// <param name="name">The name.</param>
        void Remove(string name);

        #endregion Public Methods
    }
}