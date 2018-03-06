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

namespace Quantler.Account.Cash
{
    /// <summary>
    /// Represents unsettled cash
    /// </summary>
    public class UnsettledCash : SettledCash
    {
        #region Public Constructors

        /// <summary>
        /// Instantiate new unsettled cash
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="settlement"></param>
        public UnsettledCash(decimal amount, DateTime settlement)
            : base(amount) => SettlementUtc = settlement;

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Date and time this cash will be settled for usage
        /// </summary>
        public DateTime SettlementUtc { get; }

        #endregion Public Properties
    }
}