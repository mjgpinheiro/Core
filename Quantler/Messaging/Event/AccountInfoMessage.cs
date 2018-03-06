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

using Quantler.Account.Cash;
using Quantler.Interfaces;

namespace Quantler.Messaging.Event
{
    /// <summary>
    /// Update send outbound for account related information
    /// </summary>
    /// <seealso cref="EventMessage" />
    public class AccountInfoMessage : EventMessage
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the account identifier.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Gets or sets the fund identifier.
        /// </summary>
        public string FundId { get; set; }

        /// <summary>
        /// Gets or sets the porfoltio identifier.
        /// </summary>
        public string PorfoltioId { get; set; }

        /// <summary>
        /// Message type
        /// </summary>
        public override EventMessageType Type => EventMessageType.AccountInfo;

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        public CalculatedFunds Values { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Creates the account information message for a portfolio
        /// </summary>
        /// <param name="portfolioid">The portfolio id.</param>
        /// <param name="accountid">The account id.</param>
        /// <param name="values">The values.</param>
        /// <param name="currency">The currency.</param>
        /// <param name="displaycurrency">The display currency.</param>
        /// <param name="fundid"></param>
        /// <returns></returns>
        public static AccountInfoMessage Create(string portfolioid, string accountid, CalculatedFunds values, Currency currency, CurrencyType displaycurrency, string fundid = "")
        {
            if(displaycurrency == values.BaseCurrency)
                //Return generated object
                return new AccountInfoMessage
                {
                    PorfoltioId = portfolioid,
                    Values = values,
                    AccountId = accountid,
                    FundId = fundid
                };
            else
            {
                //Return currency adjusted values
                return new AccountInfoMessage
                {
                    PorfoltioId = portfolioid,
                    Values = values.ConvertCurrency(currency, displaycurrency),
                    AccountId = accountid,
                    FundId = fundid
                };
            }
        }

        /// <summary>
        /// Check if this message is the same as a previous message
        /// (prevents us from sending messages which have the same information)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override bool Equals(EventMessage message)
        {
            if (message is AccountInfoMessage)
            {
                var instance = message as AccountInfoMessage;
                return instance.Values.Equity == Values.Equity &&
                        instance.Values.FreeMargin == Values.FreeMargin &&
                        instance.UniqueId == UniqueId &&
                        instance.Values.UnsettledCash == Values.UnsettledCash &&
                        instance.Values.BuyingPower == Values.BuyingPower &&
                        instance.Values.FloatingPnl == Values.FloatingPnl;
            }
            else
                return false;
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Get unique id for this message type
        /// </summary>
        /// <returns></returns>
        protected override string GetUniqueId() => PorfoltioId + FundId + AccountId;

        #endregion Protected Methods
    }
}