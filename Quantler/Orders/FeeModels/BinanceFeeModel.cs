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
    /// Binance exchangeModel fees (https://www.binance.com/)
    /// Source: https://binance.zendesk.com/hc/en-us/articles/115000429332
    /// </summary>
    /// <seealso cref="FeeModel" />
    [Export(typeof(FeeModel))]
    public class BinanceFeeModel : FeeModel
    {
        #region Public Methods

        /// <summary>
        /// Returns the applied commissions and fees for processing this pending order
        /// </summary>
        /// <param name="pendingorder"></param>
        /// <returns></returns>
        public decimal GetCommissionAndFees(PendingOrder pendingorder) => pendingorder.Value * 0.001m;

        /// <summary>
        /// Get amount of interest to be paid or received from holding this fill for the specific date in base currency
        /// </summary>
        /// <param name="date"></param>
        /// <param name="fill"></param>
        /// <returns></returns>
        public decimal GetInterest(DateTime date, Fill fill) => 0m;

        #endregion Public Methods
    }
}