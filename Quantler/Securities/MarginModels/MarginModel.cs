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

namespace Quantler.Securities.MarginModels
{
    public interface MarginModel
    {
        #region Public Methods

        /// <summary>
        /// Percentage of value needed to be held in free cash to send an order for this security
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        decimal GetInitialMarginRequired(Security security);

        /// <summary>
        /// Returns the current leverage of a security
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        decimal GetLeverage(Security security);

        /// <summary>
        /// Gets the total amount of margin in use for this security
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        decimal GetMaintenanceMargin(Security security);

        /// <summary>
        /// Percentage of value neeeded to be allocated to the margin in use for mainting the position of this security
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        decimal GetMaintenanceMarginRequired(Security security);

        /// <summary>
        /// Gets the current amount of margin ready for use
        /// </summary>
        /// <returns></returns>
        decimal GetRemainingMargin(IQuantFund quantfund = null);

        #endregion Public Methods
    }
}