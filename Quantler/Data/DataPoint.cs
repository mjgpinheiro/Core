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

using MessagePack;
using Quantler.Data.Bars;
using Quantler.Data.Corporate;
using Quantler.Data.Market;
using Quantler.Securities;
using System;

namespace Quantler.Data
{
    /// <summary>
    /// All data is derived from the data point interface
    /// </summary>
    [Union(0, typeof(Tick))]
    [Union(1, typeof(TradeBar))]
    [Union(2, typeof(QuoteBar))]
    [Union(3, typeof(Delisting))]
    [Union(4, typeof(Dividend))]
    [Union(5, typeof(Split))]
    [Union(6, typeof(TradingStatus))]
    public interface DataPoint
    {
        #region Public Properties

        /// <summary>
        /// Gets the type of the data.
        /// </summary>
        DataType DataType { get; }

        /// <summary>
        /// Gets the time and date this data point has ended (for instance the closing time of a bar)
        /// </summary>
        DateTime EndTime { get; }

        /// <summary>
        /// Gets a value indicating whether this data point is used for back filling.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is back filling; otherwise, <c>false</c>.
        /// </value>
        bool IsBackfilling { get; }

        /// <summary>
        /// Gets the time and date this data point is a reference to in the associated timezone
        /// </summary>
        DateTime Occured { get; }

        /// <summary>
        /// Gets the time and date this data point is a reference to in utc
        /// </summary>
        DateTime OccuredUtc { get; }

        /// <summary>
        /// Current main price of this data point
        /// </summary>
        decimal Price { get; }

        /// <summary>
        /// Gets the associated ticker
        /// </summary>
        TickerSymbol Ticker { get; }

        /// <summary>
        /// Gets the time zone.
        /// </summary>
        TimeZone TimeZone { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Serializes this instance
        /// </summary>
        /// <param name="compress">if set to <c>true</c> [compress].</param>
        /// <returns></returns>
        byte[] Serialize(bool compress = true);

        /// <summary>
        /// Serializes this instance.
        /// </summary>
        /// <returns></returns>
        string SerializeJson();

        #endregion Public Methods
    }
}