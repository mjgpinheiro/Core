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

using Quantler.Data;
using Quantler.Exchanges;
using Quantler.Interfaces;
using System;

namespace Quantler.Securities
{
    /// <summary>
    /// security definition
    /// </summary>
    public interface Security
    {
        #region Public Properties

        /// <summary>
        /// Get the latest ask price
        /// </summary>
        decimal AskPrice { get; }

        /// <summary>
        /// Get the latest ask size
        /// </summary>
        decimal AskSize { get; }

        /// <summary>
        /// Base currency of this security
        /// </summary>
        CurrencyType BaseCurrency { get; }

        /// <summary>
        /// Get the latest bid price
        /// </summary>
        decimal BidPrice { get; }

        /// <summary>
        /// Get the latest bid size
        /// </summary>
        decimal BidSize { get; }

        /// <summary>
        /// Gets the current close price.
        /// </summary>
        decimal Close { get; }

        /// <summary>
        /// Additional security details, if applicable to this security type
        /// </summary>
        SecurityDetails Details { get; }

        /// <summary>
        /// Get exchange model based information associated to this security
        /// </summary>
        ExchangeModel Exchange { get; }

        /// <summary>
        /// Gets the current high price.
        /// </summary>
        decimal High { get; }

        /// <summary>
        /// Last time this security received data
        /// </summary>
        DateTime LastTickEventUtc { get; }

        /// <summary>
        /// Last time this security received a trade occurrence
        /// </summary>
        DateTime LastTradeEventUtc { get; }

        /// <summary>
        /// Local time for this market
        /// </summary>
        DateTime LocalTime { get; }

        /// <summary>
        /// Gets the current low price.
        /// </summary>
        decimal Low { get; }

        /// <summary>
        /// Gets the current open price.
        /// </summary>
        decimal Open { get; }

        /// <summary>
        /// Current spread (Ask-Bid)
        /// </summary>
        decimal Spread { get; }

        /// <summary>
        /// Ticker
        /// </summary>
        TickerSymbol Ticker { get; }

        /// <summary>
        /// Last trade price
        /// </summary>
        decimal TradePrice { get; }

        /// <summary>
        /// type associated with security
        /// </summary>
        SecurityType Type { get; }

        /// <summary>
        /// Current security price, depending on which data this security has received
        /// </summary>
        decimal Price { get; }

        /// <summary>
        /// Gets a value indicating whether this security trading is halted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is halted; otherwise, <c>false</c>.
        /// </value>
        bool IsHalted { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Convert value from base currency to target currency
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        decimal ConvertValue(decimal value, CurrencyType target);

        /// <summary>
        /// Convert value from source currency to base currency
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        decimal ConvertValue(CurrencyType source, decimal value);

        /// <summary>
        /// Update current price of security
        /// </summary>
        /// <param name="datapoint"></param>
        void UpdatePrice(DataPoint datapoint);

        #endregion Public Methods
    }
}