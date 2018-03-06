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

namespace Quantler.Messaging.Event
{
    /// <summary>
    /// Progress message for during backtests
    /// TODO: implement progress message somewhere so we can see the progress (in the flow)
    /// </summary>
    /// <seealso cref="EventMessage" />
    public class ProgressMessage : EventMessage
    {
        #region Public Properties

        /// <summary>
        /// Current date and time in the progress
        /// </summary>
        public DateTime CurrentUtc { get; set; }

        /// <summary>
        /// End date and time for the progress
        /// </summary>
        public DateTime EndUtc { get; set; }

        /// <summary>
        /// Associated fund id
        /// </summary>
        public string FundId { get; set; }

        /// <summary>
        /// Current percentage of progress
        /// </summary>
        public decimal Percentage => (decimal)(Math.Round((CurrentUtc - StartUtc).TotalMilliseconds / (EndUtc - StartUtc).TotalMilliseconds, 3));

        /// <summary>
        /// Date and time of historical start
        /// </summary>
        public DateTime StartUtc { get; set; }

        /// <summary>
        /// Current progress state
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Type of message
        /// </summary>
        public override EventMessageType Type => EventMessageType.Progress;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Create new progress message
        /// </summary>
        /// <param name="fundid">Associated fund id for progress information</param>
        /// <param name="start">Start date and time for this fund backtest</param>
        /// <param name="end">End date and time for this fund backtest</param>
        /// <param name="current">Current date and time for this fund backtest</param>
        /// <param name="state">Current state of this backtest</param>
        /// <returns></returns>
        public static ProgressMessage Create(string fundid, DateTime start, DateTime end, DateTime current, string state)
        {
            return new ProgressMessage
            {
                CurrentUtc = current,
                EndUtc = end,
                FundId = fundid,
                StartUtc = start,
                State = state
            };
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Message unique id
        /// </summary>
        /// <returns></returns>
        protected override string GetUniqueId() => FundId;

        #endregion Protected Methods
    }
}