#region License Header

/*
* QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
* Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
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
*
* Modifications Copyright 2018 Quantler B.V.
*
*/

#endregion License Header

using MessagePack;
using System;
using NodaTime;
using Quantler.Securities;

namespace Quantler.Data.Bars
{
    /// <summary>
    /// Trade bar for
    /// </summary>
    /// <seealso cref="Quantler.Data.DataPointImpl" />
    [MessagePackObject]
    public class TradeBar : DataPointImpl, DataPointBar
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeBar"/> class.
        /// </summary>
        public TradeBar()
        {
            Ticker = TickerSymbol.NIL("");
            DataType = DataType.TradeBar;
            Period = TimeSpan.FromMinutes(1);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeBar"/> class.
        /// </summary>
        /// <param name="original">The original.</param>
        public TradeBar(TradeBar original)
        {
            DataType = DataType.TradeBar;
            Occured = new DateTime(original.Occured.Ticks);
            Ticker = original.Ticker;
            Price = original.Close;
            Open = original.Open;
            High = original.High;
            Low = original.Low;
            Close = original.Close;
            Volume = original.Volume;
            Period = original.Period;
            TimeZone = original.TimeZone;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeBar"/> class.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="timezone"></param>
        /// <param name="ticker">The ticker.</param>
        /// <param name="open">The open.</param>
        /// <param name="high">The high.</param>
        /// <param name="low">The low.</param>
        /// <param name="close">The close.</param>
        /// <param name="volume">The volume.</param>
        /// <param name="period">The period.</param>
        public TradeBar(DateTime time, TimeZone timezone, TickerSymbol ticker, decimal open, decimal high, decimal low, decimal close, long volume, TimeSpan? period = null)
        {
            Occured = time;
            Ticker = ticker;
            Price = close;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
            Period = period ?? TimeSpan.FromMinutes(1);
            DataType = DataType.TradeBar;
            TimeZone = timezone;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the close price
        /// </summary>
        [Key(6)]
        public decimal Close
        {
            get;
            set;
        }

        /// <summary>
        /// The closing time of this bar, computed via the Occured and Period
        /// </summary>
        [Key(7)]
        public override DateTime EndTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the high price
        /// </summary>
        [Key(8)]
        public decimal High
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the low price
        /// </summary>
        [Key(9)]
        public decimal Low
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the open price
        /// </summary>
        [Key(10)]
        public decimal Open
        {
            get;
            set;
        }

        /// <summary>
        /// The period of this trade bar, (second, minute, daily, ect...)
        /// </summary>
        [Key(11)]
        public TimeSpan Period { get; set; }

        /// <summary>
        /// Gets or sets the volume
        /// </summary>
        [Key(12)]
        public decimal Volume
        {
            get;
            set;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Updates the specified instance.
        /// </summary>
        /// <param name="tradeprice">The tradeprice.</param>
        /// <param name="bidprice">The bidprice.</param>
        /// <param name="askprice">The askprice.</param>
        /// <param name="tradesize">The tradesize.</param>
        /// <param name="bidsize">The bidsize.</param>
        /// <param name="asksize">The asksize.</param>
        public override void Update(decimal tradeprice, decimal bidprice, decimal askprice, decimal tradesize, decimal bidsize, decimal asksize)
        {
            if (tradeprice > High) High = tradeprice;
            if (tradeprice < Low) Low = tradeprice;
            Volume += tradesize;
            Close = tradeprice;
        }

        #endregion Public Methods
    }
}