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
using System;

namespace Quantler.Messaging.Event
{
    /// <summary>
    /// Fund information message update
    /// </summary>
    /// <seealso cref="EventMessage" />
    public class FundInfoMessage : EventMessage
    {
        #region Public Properties

        /// <summary>
        /// Current status of quant fund
        /// </summary>
        public FundState Status { get; set; }

        /// <summary>
        /// Associated quant fund id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// True if this fund is running
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// Name of the fund
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Associated roi
        /// </summary>
        public decimal ROI { get; set; }

        /// <summary>
        /// Associated portfolio id
        /// </summary>
        public string PortfolioId { get; set; }

        /// <summary>
        /// Started date and time of fund
        /// </summary>
        public DateTime StartedUtc { get; set; }

        /// <summary>
        /// Message type
        /// </summary>
        public override EventMessageType Type => EventMessageType.AccountInfo;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Generate fund information message
        /// </summary>
        /// <param name="portfolioid"></param>
        /// <param name="quantfund"></param>
        /// <returns></returns>
        public static FundInfoMessage Generate(string portfolioid, IQuantFund quantfund)
        {
            return new FundInfoMessage
            {
                Status = quantfund.State,
                Id = quantfund.FundId,
                IsRunning = quantfund.IsRunning,
                Name = quantfund.Name,
                ROI = quantfund.Results.QuantFund.ROI,
                PortfolioId = portfolioid,
                StartedUtc = quantfund.StartedDTUtc
            };
        }

        /// <summary>
        /// Checks if this message is really different from any previous messages
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override bool Equals(EventMessage message)
        {
            if (message is FundInfoMessage)
            {
                var instance = message as FundInfoMessage;
                return instance.ROI == ROI &&
                       instance.UniqueId == UniqueId &&
                       instance.IsRunning == IsRunning &&
                       instance.Status == Status;
            }
            else
                return false;
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Get unique id
        /// </summary>
        /// <returns></returns>
        protected override string GetUniqueId() => Id;

        #endregion Protected Methods
    }
}