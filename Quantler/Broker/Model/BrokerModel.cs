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

using Quantler.Account;
using Quantler.Broker.SettlementModels;
using Quantler.Broker.SpreadModels;
using Quantler.Interfaces;
using Quantler.Orders;
using Quantler.Orders.FeeModels;
using Quantler.Orders.FillModels;
using Quantler.Orders.LatencyModels;
using Quantler.Orders.SlippageModels;
using Quantler.Securities;
using Quantler.Securities.MarginModels;

namespace Quantler.Broker.Model
{
    /// <summary>
    /// Defines the settings of a broker (Commissions, Typical Spreads etc..)
    /// </summary>
    public interface BrokerModel
    {
        #region Public Properties

        /// <summary>
        /// Account type to take into account
        /// </summary>
        AccountType AccountType { get; }

        /// <summary>
        /// Type of broker
        /// </summary>
        BrokerType BrokerType { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Determines whether this instance [can submit order] the specified order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can submit order] the specified order; otherwise, <c>false</c>.
        /// </returns>
        bool CanSubmitOrder(Order order, out string message);

        /// <summary>
        /// Determines whether this instance [can update order] the specified order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can update order] the specified order; otherwise, <c>false</c>.
        /// </returns>
        bool CanUpdateOrder(Order order, out string message);

        /// <summary>
        /// Returns the amount of day trading orders left before day trading flag has been triggered
        /// </summary>
        /// <param name="account">The brokerage account</param>
        /// <param name="historicalfills">The historical fills.</param>
        /// <returns></returns>
        void DayTradingOrdersLeft(BrokerAccount account, Fill[] historicalfills);

        /// <summary>
        /// Gets the correct initial capital, in case we need to convert to a different currency.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <param name="basecurrency">The basecurrency.</param>
        /// <param name="amount">The amount.</param>
        void GetCompatibleInitialCapital(Currency currency, ref CurrencyType basecurrency, ref decimal amount);

        /// <summary>
        /// Get the fee model associated to this security
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        FeeModel GetFeeModel(Security security);

        /// <summary>
        /// Get the fill model associated to this security
        /// </summary>
        /// <returns></returns>
        FillModel GetFillModel(Order order);

        /// <summary>
        /// Get the latency model associated to this security
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        LatencyModel GetLatencyModel(Security security);

        /// <summary>
        /// Returns the margin call model used by this broker, for processing margin calls
        /// </summary>
        /// <returns></returns>
        MarginCallModel GetMarginCallModel();

        /// <summary>
        /// Get the margin model associated to the provided security
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        MarginModel GetMarginModel(Security security);

        /// <summary>
        /// Gets the minimum price increment.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <returns></returns>
        decimal GetMinimumPriceIncrement(Security security);

        /// <summary>
        /// Get security details for security
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        SecurityDetails GetSecurityDetails(Security security);

        /// <summary>
        /// Get the settlement model associated to this security
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        SettlementModel GetSettlementModel(Security security);

        /// <summary>
        /// Get the slippage model associated to this security
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        SlippageModel GetSlippageModel(Security security);

        /// <summary>
        /// Get the current spread used for this order (if market spread is not used)
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        SpreadModel GetSpreadModel(Security security);

        /// <summary>
        /// Check if this ordertype is supported by this broker
        /// </summary>
        /// <param name="ordertype"></param>
        /// <returns></returns>
        bool IsOrderTypeSupported(OrderType ordertype);

        /// <summary>
        /// Checks if this security is supported by this broker
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        bool IsSecuritySupported(Security security);

        #endregion Public Methods
    }
}