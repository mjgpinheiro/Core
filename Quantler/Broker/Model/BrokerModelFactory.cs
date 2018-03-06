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
using Quantler.Orders.LatencyModels;
using Quantler.Orders.SlippageModels;
using System;

namespace Quantler.Broker.Model
{
    /// <summary>
    /// Used as a helper for creating broker models
    /// </summary>
    public class BrokerModelFactory
    {
        #region Public Methods

        /// <summary>
        /// Get broker model based on input
        /// </summary>
        /// <param name="accounttype"></param>
        /// <param name="brokertype"></param>
        /// <returns></returns>
        public static BrokerModel GetBroker(AccountType accounttype, BrokerType brokertype)
        {
            //defaults
            LatencyModel latencymodel = new FixedLatencyModel(0);
            SlippageModel slippagemodel = new FixedAbsoluteSlippageModel(0);

            //Build return object
            switch (brokertype)
            {
                case BrokerType.Binance:
                    return new BinanceBrokerModel(accounttype, latencymodel, slippagemodel);

                case BrokerType.Bittrex:
                    return new BittrexBrokerModel(accounttype, latencymodel, slippagemodel);

                case BrokerType.Robinhood:
                    return new RobinHoodBrokerModel(accounttype, latencymodel, slippagemodel);

                case BrokerType.HitBtc:
                    return new HitBtcBrokerModel(accounttype, latencymodel, slippagemodel);

                case BrokerType.Cobinhood:
                    return new CobinHoodBrokerModel(accounttype, latencymodel, slippagemodel);

                case BrokerType.FreeTrade:
                    return new CobinHoodBrokerModel(accounttype, latencymodel, slippagemodel);

                default:
                    throw new Exception($"Unknown broker type {brokertype}");
            }
        }

        #endregion Public Methods
    }
}