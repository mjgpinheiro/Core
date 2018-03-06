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

using NLog;
using Quantler.Account;
using Quantler.Broker.SettlementModels;
using Quantler.Broker.SpreadModels;
using Quantler.Configuration;
using Quantler.Interfaces;
using Quantler.Orders;
using Quantler.Orders.FeeModels;
using Quantler.Orders.FillModels;
using Quantler.Orders.LatencyModels;
using Quantler.Orders.SlippageModels;
using Quantler.Securities;
using Quantler.Securities.MarginModels;
using System;
using System.Linq;

namespace Quantler.Broker.Model
{
    /// <summary>
    /// Generic broker model implementation, simple and generic
    /// </summary>
    public class GenericBrokerModel : BrokerModel
    {
        #region Protected Fields

        protected readonly ILogger _log = LogManager.GetCurrentClassLogger();
        protected readonly FeeModel FeeModel;
        protected readonly FillModel FillModel;
        protected readonly LatencyModel LatencyModel;
        protected readonly MarginCallModel MarginCallModel;
        protected readonly MarginModel MarginModel;
        protected readonly SettlementModel SettlementModel;
        protected readonly SlippageModel SlippageModel;
        protected readonly SpreadModel SpreadModel;
        protected CurrencyType[] CompatibleBaseCurrencies;

        #endregion Protected Fields

        #region Public Constructors

        /// <summary>
        /// Create generic brokerage model
        /// </summary>
        /// <param name="accounttype"></param>
        /// <param name="latencymodel"></param>
        /// <param name="slippagemodel"></param>
        public GenericBrokerModel(
            AccountType accounttype,
            LatencyModel latencymodel,
            SlippageModel slippagemodel)
            : this(accounttype, new CommissionFreeFeeModel(), new ImmediateFillBehaviour(), latencymodel, new GenericMarginCallModel(),
                  new EquityMarginModel(0m, 0m), new ImmediateSettlementModel(), slippagemodel, new FloatingMarketSpreadModel())
        {
        }

        #endregion Public Constructors

        #region Protected Constructors

        /// <summary>
        /// Create brokerage model for internal usage
        /// </summary>
        /// <param name="accounttype"></param>
        /// <param name="feemodel"></param>
        /// <param name="fillmodel"></param>
        /// <param name="latencymodel"></param>
        /// <param name="margincallmodel"></param>
        /// <param name="marginmodel"></param>
        /// <param name="settlementmodel"></param>
        /// <param name="slippagemodel"></param>
        /// <param name="spreadmodel"></param>
        protected GenericBrokerModel(
            AccountType accounttype,
            FeeModel feemodel,
            FillModel fillmodel,
            LatencyModel latencymodel,
            MarginCallModel margincallmodel,
            MarginModel marginmodel,
            SettlementModel settlementmodel,
            SlippageModel slippagemodel,
            SpreadModel spreadmodel)
        {
            //Set given values
            AccountType = accounttype;
            FeeModel = feemodel;
            FillModel = fillmodel;
            LatencyModel = latencymodel;
            MarginCallModel = margincallmodel;
            SettlementModel = settlementmodel;
            SlippageModel = slippagemodel;
            MarginModel = marginmodel;
            SpreadModel = spreadmodel;
        }

        #endregion Protected Constructors

        #region Public Properties

        /// <summary>
        /// Account type used
        /// </summary>
        public AccountType AccountType { get; }

        /// <summary>
        /// Type of broker
        /// </summary>
        public virtual BrokerType BrokerType { get; protected set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Determines whether this instance [can submit order] the specified order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        /// <c>true</c> if this instance [can submit order] the specified order; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanSubmitOrder(Order order, out string message)
        {
            message = string.Empty;
            return true;
        }

        /// <summary>
        /// Determines whether this instance [can update order] the specified order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        /// <c>true</c> if this instance [can update order] the specified order; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanUpdateOrder(Order order, out string message)
        {
            message = string.Empty;
            return true;
        }

        /// <summary>
        /// Returns the amount of day trading orders left before day trading flag has been triggered
        /// http://www.finra.org/investors/day-trading-margin-requirements-know-rules
        /// </summary>
        /// <param name="account">The brokerage account</param>
        /// <param name="historicalfills">The historical fills.</param>
        /// <returns></returns>
        public virtual void DayTradingOrdersLeft(BrokerAccount account, Fill[] historicalfills)
        {
            account.DayTradingOrdersLeft = int.MaxValue;
            account.PatternDayTradingHit = false;
        }

        /// <summary>
        /// Gets the correct initial capital, in case we need to convert to a different currency.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <param name="basecurrency">The basecurrency.</param>
        /// <param name="amount">The amount.</param>
        /// <exception cref="Exception"></exception>
        public virtual void GetCompatibleInitialCapital(Currency currency, ref CurrencyType basecurrency,
            ref decimal amount)
        {
            //Check if this base currency is already compatible
            if (CompatibleBaseCurrencies.Contains(basecurrency))
                return;

            //Check if we have compatible currencies
            if (CompatibleBaseCurrencies.Length == 0)
                throw new Exception($"Cannot derive correct base currency for broker model {BrokerType}, no compatible base currencies has been defined!");

            //Get another currency in this case
            CurrencyType converted = CompatibleBaseCurrencies.First();
            decimal oldamount = amount;
            amount = currency.Convert(amount, basecurrency, converted);

            //Some logging
            _log.Info($"The base currency of {basecurrency} is not compatible with broker of type {BrokerType} so this value was converted to " +
                      $"represent the value provided in a compatible currency {converted}. Old = {oldamount} {basecurrency}, New = {amount} {converted}");

            //Set correct base currency
            basecurrency = converted;
        }

        /// <summary>
        /// Get the broker associated fee model
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        public virtual FeeModel GetFeeModel(Security security) => FeeModel;

        /// <summary>
        /// Get the broker associated fill model
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual FillModel GetFillModel(Order order) => FillModel;

        /// <summary>
        /// Get the broker associated latency model
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        public virtual LatencyModel GetLatencyModel(Security security) => LatencyModel;

        /// <summary>
        /// Get the broker associated margin call model
        /// </summary>
        /// <returns></returns>
        public virtual MarginCallModel GetMarginCallModel() => MarginCallModel;

        /// <summary>
        /// Get the broker associated margin model
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        public virtual MarginModel GetMarginModel(Security security) => MarginModel;

        /// <summary>
        /// Gets the minimum price increment.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <returns></returns>
        public virtual decimal GetMinimumPriceIncrement(Security security)
        {
            //Any NMS stock priced in an increment smaller than $0.0001 if that bid or offer, order, or indication of interest is priced less than $1.00 per share.
            //Rule: 17 CFR 242.612 - Minimum pricing increment.
            //Source: https://www.law.cornell.edu/cfr/text/17/242.612
            if (security.Type == SecurityType.Equity && security.Ticker.Name.EndsWith(".US") && security.Price < 1m)
                return 0.0001m;
            else
                return security.Details.MinimumPriceIncrement;
        }

        /// <summary>
        /// Retrieve the security details, which are broker specific
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        public virtual SecurityDetails GetSecurityDetails(Security security)
        {
            //Get from config
            var config = Config.SecurityConfig.FirstOrDefault(x => x.Ticker == security.Ticker);
            if (config == null)
                throw new Exception("Cannot find config for ticker name " + security.Ticker);

            //Check based on security type
            //TODO: check for correct implementation of step quantity and min quantity for brokerages
            switch (security.Type)
            {
                case SecurityType.Equity:
                    return new SecurityDetails(1, decimal.MaxValue, config.Step, config.Step, config.Digits, config.ExpenseRatio);

                case SecurityType.Crypto:
                    return new SecurityDetails(1, decimal.MaxValue, config.Step, config.Step, config.Digits, config.ExpenseRatio);

                case SecurityType.NIL:
                    return SecurityDetails.NIL();
            }

            //If nothing works
            throw new Exception($"Unknown security type {security.Type}");
        }

        /// <summary>
        /// Get the broker associated settlement model
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        public virtual SettlementModel GetSettlementModel(Security security) => SettlementModel;

        /// <summary>
        /// Get the broker associated slippage model
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        public virtual SlippageModel GetSlippageModel(Security security) => SlippageModel;

        /// <summary>
        /// Get the broker associated spread model
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        public virtual SpreadModel GetSpreadModel(Security security) => SpreadModel;

        /// <summary>
        /// Check if order type as requested is supported, default yes
        /// </summary>
        /// <param name="ordertype"></param>
        /// <returns></returns>
        public virtual bool IsOrderTypeSupported(OrderType ordertype) => true;

        /// <summary>
        /// Check if security is supported by checking the config file
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        public virtual bool IsSecuritySupported(Security security) =>
            Config.SecurityConfig.FirstOrDefault(x => String.Equals(x.Ticker, security.Ticker.Name, StringComparison.CurrentCultureIgnoreCase) &&
                x.Brokers.Select(b => b.ToLower()).Contains(BrokerType.ToString().ToLower())) != null;

        /// <summary>
        /// Get the stop out level for this broker
        /// </summary>
        /// <returns></returns>
        public virtual decimal StopOutLevel() => 0.20m;

        #endregion Public Methods
    }
}