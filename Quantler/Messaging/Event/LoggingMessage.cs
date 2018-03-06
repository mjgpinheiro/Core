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

namespace Quantler.Messaging.Event
{
    /// <summary>
    /// Loggig event message
    /// </summary>
    /// <seealso cref="EventMessage" />
    public class LoggingMessage : EventMessage
    {
        #region Public Properties

        /// <summary>
        /// Associated fund id
        /// </summary>
        public string FundId { get; set; }

        /// <summary>
        /// Associated message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Severity of the message
        /// </summary>
        public string Severity { get; set; }

        /// <summary>
        /// Logging message type
        /// </summary>
        public override EventMessageType Type => EventMessageType.Logging;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Create new instance for a logging event message
        /// </summary>
        /// <param name="fundid"></param>
        /// <param name="message"></param>
        /// <param name="severity"></param>
        /// <returns></returns>
        public static LoggingMessage Create(string fundid, string message, string severity) =>
            new LoggingMessage
            {
                FundId = fundid,
                Message = message,
                Severity = severity
            };

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Set unique id logic
        /// </summary>
        /// <returns></returns>
        protected override string GetUniqueId() => (FundId + Message + OccuredUtc.ToUnixTime(true)).GetHashCode().ToString();

        #endregion Protected Methods
    }
}