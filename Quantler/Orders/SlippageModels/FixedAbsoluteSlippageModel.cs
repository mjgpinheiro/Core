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
using System.Composition;

namespace Quantler.Orders.SlippageModels
{
    /// <summary>
    /// Implementation of a fixed absolute slippage amount model
    /// </summary>
    [Export(typeof(SlippageModel))]
    public class FixedAbsoluteSlippageModel : SlippageModel
    {
        #region Private Fields

        /// <summary>
        /// Slippage amount
        /// </summary>
        private decimal SlippageAmount = 0;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initialize a fixed absolute amount of slippage model
        /// </summary>
        /// <param name="slippage"></param>
        public FixedAbsoluteSlippageModel(decimal slippage) =>
            SlippageAmount = Math.Abs(slippage);

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Returns the amount of slippage to add to this order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public decimal GetSlippage(Order order) =>
            order.Direction == Direction.Long ? SlippageAmount : -SlippageAmount;

        #endregion Public Methods
    }
}