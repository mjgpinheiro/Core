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
using Quantler.Securities;
using System;
using Quantler.Account;

namespace Quantler.Broker.SettlementModels
{
    /// <summary>
    /// Settle funds right away
    /// </summary>
    public class ImmediateSettlementModel : SettlementModel
    {
        #region Public Methods

        /// <summary>
        /// Settle funds to account right away
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="quantfund">The quantfund.</param>
        /// <param name="security">The security.</param>
        /// <param name="occureddtutc">The occureddtutc.</param>
        /// <param name="amount">The amount.</param>
        /// <exception cref="NullReferenceException">Could not find quant fund for settlement</exception>
        public void SettleFunds(BrokerAccount account, Security security, DateTime occureddtutc, decimal amount, IQuantFund quantfund = null)
        {
            //check if we found agent
            if (quantfund == null)
                throw new NullReferenceException("Could not find quant fund for settlement");

            //Add settled funds
            account.Cash.AddCash(security.BaseCurrency, amount, quantfund);
        }

        #endregion Public Methods
    }
}