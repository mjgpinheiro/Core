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
    /// Message for an exception occurance
    /// </summary>
    public class ExceptionMessage : EventMessage
    {
        #region Public Properties

        /// <summary>
        /// Fund for which this exception was raised
        /// </summary>
        public string FundId { get; set; }

        /// <summary>
        /// Associated exception message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Associated portfolio id
        /// </summary>
        public string PorfolioId { get; set; }

        /// <summary>
        /// Associated stacktrace
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// Gets or sets the module identifier.
        /// </summary>
        public string ModuleId { get; set; }

        /// <summary>
        /// Event type
        /// </summary>
        public override EventMessageType Type => EventMessageType.Exception;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Generate exception message for event runner
        /// </summary>
        /// <param name="fundid"></param>
        /// <param name="portfolioid"></param>
        /// <param name="moduleid"></param>
        /// <param name="exc"></param>
        /// <returns></returns>
        public static ExceptionMessage Create(string fundid, string portfolioid, string moduleid, Exception exc)
        {
            return new ExceptionMessage
            {
                FundId = fundid,
                Message = exc.Message,
                PorfolioId = portfolioid,
                StackTrace = exc.StackTrace,
                ModuleId = moduleid
            };
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Get this messages unique id
        /// </summary>
        /// <returns></returns>
        protected override string GetUniqueId() => (FundId + Message + OccuredUtc.ToUnixTime(true)).GetHashCode().ToString();

        #endregion Protected Methods
    }
}