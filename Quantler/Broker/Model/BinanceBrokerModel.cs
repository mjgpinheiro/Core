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
    /// Binance broker/exchangeModel (https://www.binance.com)
    /// </summary>
    /// <seealso cref="Quantler.Broker.Model.GenericBrokerModel" />
    public class BinanceBrokerModel : GenericBrokerModel
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BinanceBrokerModel"/> class.
        /// </summary>
        /// <param name="accounttype">The accounttype.</param>
        /// <param name="latencymodel">The latencymodel.</param>
        /// <param name="slippagemodel">The slippagemodel.</param>
        public BinanceBrokerModel(
                    AccountType accounttype,
                    LatencyModel latencymodel,
                    SlippageModel slippagemodel)
            : base(accounttype, new BinanceFeeModel(), new ImmediateFillBehaviour(), latencymodel, new GenericMarginCallModel(),
                  new EquityMarginModel(0m, 0m), new ImmediateSettlementModel(), slippagemodel,
                  new FloatingMarketSpreadModel())
        {
            CompatibleBaseCurrencies = new[] { CurrencyType.BTC, CurrencyType.USDT, CurrencyType.ETH };
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Type of broker
        /// </summary>
        public override BrokerType BrokerType { get; protected set; } = BrokerType.Binance;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Get associated security details
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        public override SecurityDetails GetSecurityDetails(Security security)
        {
            if (security.Type == SecurityType.Crypto)
                return base.GetSecurityDetails(security);
            else
            {
                _log.Error($"Security type {security.Type} is unsupported for broker of type {BrokerType}");
                return SecurityDetails.NIL();
            }
        }

        /// <summary>
        /// Check if order type as requested is supported, default yes
        /// Binance supports:
        ///     - Market Orders
        ///     - Limit Orders
        ///     - Stop Limit Orders
        /// </summary>
        /// <param name="ordertype"></param>
        /// <returns></returns>
        public override bool IsOrderTypeSupported(OrderType ordertype) =>
            ordertype == OrderType.Market ||
            ordertype == OrderType.Limit ||
            ordertype == OrderType.StopLimit;

        #endregion Public Methods
    }
}