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

namespace Quantler.Configuration
{
    public enum ConfigFileType
    {
        /// <summary>
        /// Interactive brokers trading api settings
        /// </summary>
        InteractiveBrokers,

        /// <summary>
        /// Global settings
        /// </summary>
        Global,

        /// <summary>
        /// Backtest related settings
        /// </summary>
        Simulation,

        /// <summary>
        /// Trading days and non trading days configuration
        /// </summary>
        MarketHours,

        /// <summary>
        /// Trading symbol details information
        /// </summary>
        SecurityConfig,

        /// <summary>
        /// The broker configuration
        /// </summary>
        Broker,

        /// <summary>
        /// Historical currency rates, for conversion
        /// </summary>
        CurrencyRates,

        /// <summary>
        /// Portfolio config for local implementations
        /// </summary>
        Portfolio
    }
}