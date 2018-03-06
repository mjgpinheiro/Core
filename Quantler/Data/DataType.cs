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

namespace Quantler.Data
{
    /// <summary>
    /// Keeps track of different data types supported in Quantler
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// An unknown data type
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The trade bar (OHLC)
        /// </summary>
        TradeBar = 1,

        /// <summary>
        /// The quote bar OHLC: Bid, Ask, MidPrice
        /// </summary>
        QuoteBar = 2,

        /// <summary>
        /// Renkobar
        /// </summary>
        RenkoBar = 3,

        /// <summary>
        /// The tick (quotes)
        /// </summary>
        Tick = 4,

        /// <summary>
        /// Delisting data type
        /// </summary>
        Delisting = 5,

        /// <summary>
        /// Dividend payout
        /// </summary>
        Dividend = 6,

        /// <summary>
        /// Public earning report
        /// </summary>
        Earning = 7,

        /// <summary>
        /// Financial information
        /// </summary>
        Financial = 8,

        /// <summary>
        /// Key statistics
        /// </summary>
        KeyStat = 9,

        /// <summary>
        /// Stock split
        /// </summary>
        Split = 10,

        /// <summary>
        /// Current market trading status update
        /// </summary>
        TradingStatus = 11
    }
}