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
using Quantler.Interfaces;
using Quantler.Orders;
using Quantler.Securities;

namespace Quantler.Modules
{
    /// <summary>
    /// Trading related implementations
    /// </summary>
    /// <seealso cref="Quantler.Interfaces.IModule" />
    public abstract partial class Module
    {
        #region Public Methods

        /// <summary>
        /// Create a new limit order ticket
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="limitprice">The limitprice.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public SubmitOrderTicket LimitOrder(Security security, decimal quantity, decimal limitprice, string comment = "", string exchange = "") =>
            SubmitOrderTicket.LimitOrder(QuantFund.FundId, security, quantity, limitprice, comment, exchange);

        /// <summary>
        /// Create a new market on close order
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="exchange">The exchangeModel.</param>
        /// <returns></returns>
        public SubmitOrderTicket MarketOnCloseOrder(Security security, decimal quantity, string comment = "", string exchange = "") =>
            SubmitOrderTicket.MarketOnCloseOrder(QuantFund.FundId, security, quantity, comment, exchange);

        /// <summary>
        /// Create a new market on open order
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="exchange">The exchangeModel.</param>
        /// <returns></returns>
        public SubmitOrderTicket MarketOnOpenOrder(Security security, decimal quantity, string comment = "",
            string exchange = "") =>
            SubmitOrderTicket.MarketOnOpenOrder(QuantFund.FundId, security, quantity, comment, exchange);

        /// <summary>
        /// Create a new market order ticket
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public SubmitOrderTicket MarketOrder(Security security, decimal quantity, string comment = "",
            string exchange = "") =>
            SubmitOrderTicket.MarketOrder(QuantFund.FundId, security, quantity, comment, exchange);

        /// <summary>
        /// Create a new stop limit order ticket
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="stopprice">The stopprice.</param>
        /// <param name="limitprice">The limitprice.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public SubmitOrderTicket StopLimitOrder(Security security, decimal quantity, decimal stopprice, decimal limitprice, string comment = "", string exchange = "") =>
            SubmitOrderTicket.StopLimitOrder(QuantFund.FundId, security, quantity, limitprice, stopprice, comment, exchange);

        /// <summary>
        /// Create a new stop market order
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="stopprice">The stopprice.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public SubmitOrderTicket StopMarketOrder(Security security, decimal quantity, decimal stopprice, string comment = "", string exchange = "") =>
            SubmitOrderTicket.StopMarketOrder(QuantFund.FundId, security, quantity, stopprice, comment, exchange);

        /// <summary>
        /// Buys the specified security.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns></returns>
        public SubmitOrderTicket Buy(Security security, decimal quantity) =>
            MarketOrder(security, Math.Abs(quantity));

        /// <summary>
        /// Buys the specified security.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns></returns>
        public SubmitOrderTicket Buy(Security security, double quantity) =>
            MarketOrder(security, Convert.ToDecimal(quantity));

        /// <summary>
        /// Sells the specified security.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns></returns>
        public SubmitOrderTicket Sell(Security security, decimal quantity) =>
            MarketOrder(security, Math.Abs(quantity) * -1);

        /// <summary>
        /// Sells the specified security.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns></returns>
        public SubmitOrderTicket Sell(Security security, double quantity) =>
            MarketOrder(security, Convert.ToDecimal(quantity));

        #endregion Public Methods
    }
}