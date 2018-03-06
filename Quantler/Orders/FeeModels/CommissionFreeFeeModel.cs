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
using System.Composition;

namespace Quantler.Orders.FeeModels
{
    /// <summary>
    /// Commission free trading, no fees apply
    /// </summary>
    [Export(typeof(FeeModel))]
    public class CommissionFreeFeeModel : FeeModel
    {
        #region Public Methods

        /// <summary>
        /// Get commission and fees
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public decimal GetCommissionAndFees(PendingOrder order)
            => 0m;

        /// <summary>
        /// Get interest to be paid or received from mainting this position
        /// </summary>
        /// <param name="date"></param>
        /// <param name="fill"></param>
        /// <returns></returns>
        public virtual decimal GetInterest(DateTime date, Fill fill)
            => 0m;

        #endregion Public Methods
    }
}