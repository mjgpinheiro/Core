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

namespace Quantler.Account
{
    /// <summary>
    /// Account action (in the event of a deposit or withdrawal)
    /// </summary>
    public class AccountAction
    {
        /// <summary>
        /// Gets the current balance.
        /// </summary>
        public decimal Balance { get; }

        /// <summary>
        /// Gets the type of the currency.
        /// </summary>
        public CurrencyType CurrencyType { get; }

        /// <summary>
        /// Gets the type of the account action.
        /// </summary>
        public AccountActionType AccountActionType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountAction"/> class.
        /// </summary>
        /// <param name="accountactiontype">The accountactiontype.</param>
        /// <param name="currencytype">The currencytype.</param>
        /// <param name="balance">The balance.</param>
        public AccountAction(AccountActionType accountactiontype, CurrencyType currencytype, decimal balance)
        {
            AccountActionType = accountactiontype;
            CurrencyType = currencytype;
            Balance = balance;
        }
    }
}