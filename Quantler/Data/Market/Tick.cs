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
using Quantler.Securities;
using System;

namespace Quantler.Data.Market
{
    /// <summary>
    /// Base implementation for a tick (implements general functions)
    /// </summary>
    [MessagePackObject]
    public class Tick : DataPointImpl
    {
        #region Public Constructors

        /// <summary>
        /// Create a new tick
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="datasource"></param>
        public Tick(TickerSymbol ticker, DataSource datasource)
        {
            Ticker = ticker;
            DataType = DataType.Tick;
            BidSource = datasource;
            AskSource = datasource;
            Source = datasource;
        }

        /// <summary>
        /// Create a new tick
        /// </summary>
        public Tick() { }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Current market ask price
        /// </summary>
        [Key(6)]
        public decimal AskPrice { get; set; }

        /// <summary>
        /// Current market ask size
        /// </summary>
        [Key(7)]
        public decimal AskSize { get; set; }

        /// <summary>
        /// Current market ask source
        /// </summary>
        [Key(8)]
        public DataSource AskSource { get; set; } = DataSource.NA;

        /// <summary>
        /// Current market bid price
        /// </summary>
        [Key(9)]
        public decimal BidPrice { get; set; }

        /// <summary>
        /// Current market bid size
        /// </summary>
        [Key(10)]
        public decimal BidSize { get; set; }

        /// <summary>
        /// Current market bid source
        /// </summary>
        [Key(11)]
        public DataSource BidSource { get; set; } = DataSource.NA;

        /// <summary>
        /// Market depth information (usually 0)
        /// </summary>
        [Key(12)]
        public int Depth { get; set; }

        /// <summary>
        /// True if this tick has an ask price and size
        /// </summary>
        [IgnoreMember]
        public bool HasAsk => (AskPrice != 0) && (AskSize != 0);

        /// <summary>
        /// True if this tick has a bid price and size
        /// </summary>
        [IgnoreMember]
        public bool HasBid => (BidPrice != 0) && (BidSize != 0);

        /// <summary>
        /// True if tick data is present
        /// </summary>
        [IgnoreMember]
        public bool HasTick => IsTrade || HasBid || HasAsk;

        /// <summary>
        /// True if this is a full quote (both tick and ask data)
        /// </summary>
        [IgnoreMember]
        public bool IsFullQuote => HasBid && HasAsk;

        /// <summary>
        /// If size < 0 this is an index
        /// </summary>
        [IgnoreMember]
        public bool IsIndex => Size < 0;

        /// <summary>
        /// True if this tick is quote related
        /// </summary>
        [IgnoreMember]
        public bool IsQuote => !IsTrade && (HasBid || HasAsk);

        /// <summary>
        /// True if this tick is trade related
        /// </summary>
        [IgnoreMember]
        public bool IsTrade => (TradePrice != 0) && (Size > 0);

        /// <summary>
        /// Check if this is a valid tick (symbol and data)
        /// </summary>
        [IgnoreMember]
        public bool IsValid => (Ticker != "") && (IsIndex || HasTick);

        /// <summary>
        /// Current main price of this data point
        /// </summary>
        [IgnoreMember]
        public override decimal Price =>
            IsFullQuote ? (AskPrice + BidPrice) / 2 : IsTrade ? TradePrice : Math.Max(BidPrice, AskPrice);

        /// <summary>
        /// Current trade size
        /// </summary>
        [Key(13)]
        public decimal Size { get; set; }

        /// <summary>
        /// Current trade source
        /// </summary>
        [Key(14)]
        public DataSource Source { get; set; } = DataSource.NA;

        /// <summary>
        /// Trade price, if tick is trade
        /// </summary>
        [Key(15)]
        public decimal TradePrice { get; set; }

        #endregion Public Properties
    }
}