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
using Quantler.Orders;
using System;
using System.Collections.Generic;
using Quantler.Securities;

namespace Quantler.Modules
{
    /// <summary>
    /// Risk management based module implementation
    /// </summary>
    /// <seealso cref="Quantler.Modules.TradingModule" />
    public abstract class RiskManagementModule : TradingModule
    {
        #region Public Methods

        /// <summary>
        /// Check if we are allowed to perform trades
        /// </summary>
        /// <returns></returns>
        public virtual bool IsTradingAllowed(Security security) => true;

        /// <summary>
        /// Perform risk management checks and retrieve new orders to offset risks
        /// </summary>
        /// <param name="orderticket"></param>
        /// <param name="state"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public virtual IEnumerable<OrderTicket> RiskManagement(SubmitOrderTicket orderticket, SecurityState state, decimal weight) =>
            throw new NotImplementedException("RiskManagement method should be implemented for a risk management module to function.");

        #endregion Public Methods

        #region Protected Properties

        /// <summary>
        /// No order helper object
        /// </summary>
        protected IEnumerable<OrderTicket> Nothing => new OrderTicket[0];

        #endregion Protected Properties
    }
}