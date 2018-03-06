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

using MoreLinq;
using Quantler.Interfaces;
using Quantler.Orders;
using Quantler.Securities;
using System.Collections.Generic;

namespace Quantler.Modules
{
    /// <summary>
    /// Signal generating module
    /// </summary>
    /// <seealso cref="Quantler.Modules.Module" />
    public abstract class SignalModule : Module
    {
        #region Private Properties

        /// <summary>
        /// Gets the associated quant fund.
        /// </summary>
        private IQuantFund QuantFund { get; set; }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Logic for creating the order, will create a market order on default
        /// </summary>
        /// <param name="security">The security.</param>
        /// <returns></returns>
        public virtual SubmitOrderTicket CreateOrder(Security security, SecurityState state) =>
            MarketOrder(security, 0);

        /// <summary>
        /// Enter long signal
        /// </summary>
        /// <param name="security">The security.</param>
        public void EnterLong(Security security) =>
            EnterLong(new[] { security });

        /// <summary>
        /// Enter long signal
        /// </summary>
        /// <param name="securities">The securities.</param>
        public void EnterLong(IEnumerable<Security> securities) =>
            securities.ForEach(security => QuantFund.SetState(this, security, SecurityState.EntryLong));

        /// <summary>
        /// Enter short signal
        /// </summary>
        /// <param name="security">The security.</param>
        public void EnterShort(Security security) =>
            EnterShort(new[] { security });

        /// <summary>
        /// Enter short signal
        /// </summary>
        /// <param name="securities">The securities.</param>
        public void EnterShort(IEnumerable<Security> securities) =>
            securities.ForEach(security => QuantFund.SetState(this, security, SecurityState.EntryShort));

        /// <summary>
        /// Exit long signal
        /// </summary>
        /// <param name="security">The security.</param>
        public void ExitLong(Security security) =>
            ExitLong(new[] { security });

        /// <summary>
        /// Exit long signal
        /// </summary>
        /// <param name="securities">The securities.</param>
        public void ExitLong(IEnumerable<Security> securities) =>
            securities.ForEach(security => QuantFund.SetState(this, security, SecurityState.ExitLong));

        /// <summary>
        /// Exit short signal
        /// </summary>
        /// <param name="security">The security.</param>
        public void ExitShort(Security security) =>
            ExitShort(new[] { security });

        /// <summary>
        /// Exit short signal
        /// </summary>
        /// <param name="securities">The securities.</param>
        public void ExitShort(IEnumerable<Security> securities) =>
            securities.ForEach(security => QuantFund.SetState(this, security, SecurityState.ExitLong));

        /// <summary>
        /// Send a liquidate signal
        /// </summary>
        /// <param name="security">The security.</param>
        public void Liquidate(Security security) =>
            Liquidate(new[] { security });

        /// <summary>
        /// Send a liquidate signal
        /// </summary>
        /// <param name="securities">The securities.</param>
        public void Liquidate(IEnumerable<Security> securities) =>
            securities.ForEach(security => QuantFund.SetState(this, security, SecurityState.Liquidate));

        /// <summary>
        /// Send no entry allowed signal
        /// </summary>
        /// <param name="security">The security.</param>
        public void NoEntry(Security security) =>
            NoEntry(new[] { security });

        /// <summary>
        /// Send no entry allowed signal
        /// </summary>
        /// <param name="securities">The securities.</param>
        public void NoEntry(IEnumerable<Security> securities) =>
            securities.ForEach(security => QuantFund.SetState(this, security, SecurityState.NoEntry));

        /// <summary>
        /// Sets the associated quantfund.
        /// </summary>
        /// <param name="quantfund">The quantfund.</param>
        public override void SetQuantFund(IQuantFund quantfund)
        {
            if (QuantFund == null && quantfund != null)
            {
                QuantFund = quantfund;
                base.SetQuantFund(quantfund);
            }
        }

        #endregion Public Methods
    }
}