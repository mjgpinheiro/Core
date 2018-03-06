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

namespace Quantler.Orders.SlippageModels
{
    /// <summary>
    /// Keeps track of how much slippage to apply when processing an order
    /// </summary>
    public interface SlippageModel
    {
        #region Public Methods

        /// <summary>
        /// Get additional slippage to be used for this order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        decimal GetSlippage(Order order);

        #endregion Public Methods
    }
}