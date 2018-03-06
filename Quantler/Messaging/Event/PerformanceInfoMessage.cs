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
using Quantler.Performance;

namespace Quantler.Messaging.Event
{
    /// <summary>
    /// Fund performance update event message
    /// </summary>
    /// <seealso cref="EventMessage" />
    public class PerformanceInfoMessage : EventMessage
    {
        #region Public Properties

        /// <summary>
        /// Currency used for calculating values
        /// </summary>
        public CurrencyType Currency { get; set; }

        /// <summary>
        /// Fund id
        /// </summary>
        public string FundId { get; set; }

        /// <summary>
        /// Associated algorithm result
        /// </summary>
        public QuantFundResult QuantFundResult { get; set; }

        /// <summary>
        /// Associated trading related results
        /// </summary>
        public TradeResult TradeResult { get; set; }

        /// <summary>
        /// Type of event
        /// </summary>
        public override EventMessageType Type => EventMessageType.PerformanceInfo;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Generate performance information
        /// </summary>
        /// <param name="fundid">Related FundId</param>
        /// <param name="result">Fund results instance</param>
        /// <param name="displaycurrency"></param>
        /// <returns></returns>
        public static PerformanceInfoMessage Create(string fundid, Result result, CurrencyType displaycurrency)
        {
            return new PerformanceInfoMessage
            {
                FundId = fundid,
                QuantFundResult = result.QuantFund.Copy(),
                TradeResult = result.Trades.Copy(),
                Currency = displaycurrency
            };
        }

        /// <summary>
        /// Check if this message is the same as a previous message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override bool Equals(EventMessage message)
        {
            if (message is PerformanceInfoMessage)
            {
                var instance = message as PerformanceInfoMessage;
                return instance.UniqueId == UniqueId &&
                        instance.QuantFundResult.LastModifiedUtc == QuantFundResult.LastModifiedUtc &&
                        instance.TradeResult.LastModifiedUtc == TradeResult.LastModifiedUtc;
            }
            else
                return false;
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Unique id for this instance
        /// </summary>
        /// <returns></returns>
        protected override string GetUniqueId() => FundId;

        #endregion Protected Methods
    }
}