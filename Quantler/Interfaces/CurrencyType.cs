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

namespace Quantler.Interfaces
{
    /// <summary>
    /// Known base currency types
    /// </summary>
    public enum CurrencyType
    {
        //FIAT
        /// <summary>
        /// Australia Dollar
        /// </summary>
        AUD,

        /// <summary>
        /// United Kingdom Pound
        /// </summary>
        GBP,

        /// <summary>
        /// Brazil Real
        /// </summary>
        BRL,

        /// <summary>
        /// Canada Dollar
        /// </summary>
        CAD,

        /// <summary>
        /// Switzerland Franc
        /// </summary>
        CHF,

        /// <summary>
        /// China Yuan Renminbi
        /// </summary>
        CNY,

        /// <summary>
        /// Hong Kong Dollar
        /// </summary>
        HKD,

        /// <summary>
        /// Indonesia Rupiah
        /// </summary>
        IDR,

        /// <summary>
        /// India Rupee
        /// </summary>
        INR,

        /// <summary>
        /// Japan Yen
        /// </summary>
        JPY,

        /// <summary>
        /// Korea (South) Won
        /// </summary>
        KRW,

        /// <summary>
        /// Mexico Peso
        /// </summary>
        MXN,

        /// <summary>
        /// Russia Ruble
        /// </summary>
        RUB,

        /// <summary>
        /// United States Dollar
        /// </summary>
        USD,

        /// <summary>
        /// Euro Member Countries
        /// </summary>
        EUR,

        //Crypto
        /// <summary>
        /// Bitcoin
        /// </summary>
        BTC,

        /// <summary>
        /// The eth
        /// </summary>
        ETH,

        /// <summary>
        /// The usdt
        /// </summary>
        USDT
    }
}