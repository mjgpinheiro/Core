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

namespace Quantler.Scheduler
{
    /// <summary>
    /// Scheduled event
    /// </summary>
    public interface ScheduledEventAction
    {
        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is logged.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is logged; otherwise, <c>false</c>.
        /// </value>
        bool IsLogged { get; }

        /// <summary>
        /// Gets the name of this event
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the next action in UTC.
        /// </summary>
        /// <value>
        /// The next action in UTC.
        /// </value>
        DateTime NextActionUtc { get; }

        /// <summary>
        /// Gets the fund identifier, if applicable.
        /// </summary>
        string FundId { get; }

        /// <summary>
        /// Gets the event exception, if thrown.
        /// </summary>
        ScheduledEventException EventException { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Checks the specified utc time.
        /// </summary>
        /// <param name="utctime">The utc time.</param>
        void Check(DateTime utctime);

        #endregion Public Methods
    }
}