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

namespace Quantler.Broker
{
    /// <summary>
    /// Broker Types
    /// </summary>
    public enum BrokerType
    {
        /// <summary>
        /// https://www.robinhood.com/
        /// </summary>
        Robinhood = 1,

        /// <summary>
        /// https://bittrex.com
        /// </summary>
        Bittrex = 2,

        /// <summary>
        /// https://hitbtc.com
        /// </summary>
        HitBtc = 3,

        /// <summary>
        /// https://cobinhood.com/
        /// </summary>
        Cobinhood = 4,

        /// <summary>
        /// http://freetrade.io/
        /// </summary>
        FreeTrade = 5,

        /// <summary>
        /// https://www.binance.com/
        /// </summary>
        Binance = 6,

        /// <summary>
        /// Default, unknown
        /// </summary>
        Unknown = 0
    }
}