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
using Quantler.Securities;

namespace Quantler.Data.Bars
{
    /// <summary>
    /// Bar based on quote data
    /// </summary>
    /// <seealso cref="DataPointImpl" />
    /// <seealso cref="DataPointBar" />
    [MessagePackObject]
    public class QuoteBar : DataPointImpl, DataPointBar
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuoteBar"/> class.
        /// </summary>
        public QuoteBar()
        {
            Ticker = TickerSymbol.NIL("");
            Occured = new DateTime();
            Bid = new BarImpl();
            Ask = new BarImpl();
            Price = 0;
            Period = TimeSpan.FromMinutes(1);
            DataType = DataType.QuoteBar;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuoteBar"/> class.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="ticker">The ticker.</param>
        /// <param name="bid">The bid.</param>
        /// <param name="bidsize">The bidsize.</param>
        /// <param name="ask">The ask.</param>
        /// <param name="asksize">The asksize.</param>
        /// <param name="period">The period.</param>
        public QuoteBar(DateTime time, TickerSymbol ticker, Bar bid, decimal bidsize, Bar ask, decimal asksize, TimeSpan? period = null)
        {
            Ticker = ticker;
            Occured = time;
            Bid = bid == null ? null : new BarImpl(bid.Open, bid.High, bid.Low, bid.Close);
            Ask = ask == null ? null : new BarImpl(ask.Open, ask.High, ask.Low, ask.Close);
            if (Bid != null) LastBidSize = bidsize;
            if (Ask != null) LastAskSize = asksize;
            Price = Close;
            Period = period ?? TimeSpan.FromMinutes(1);
            DataType = DataType.QuoteBar;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Ask OHLC
        /// </summary>
        [Key(6)]
        public BarImpl Ask { get; set; }

        /// <summary>
        /// Bid OHLC
        /// </summary>
        [Key(7)]
        public BarImpl Bid { get; set; }

        /// <summary>
        /// Closing price of the QuoteBar. Defined as the price at Start Time + TimeSpan.
        /// </summary>
        [IgnoreMember]
        public decimal Close
        {
            get
            {
                if (Bid != null && Ask != null)
                {
                    if (Bid.Close != 0m && Ask.Close != 0m)
                        return (Bid.Close + Ask.Close) / 2m;

                    if (Bid.Close != 0)
                        return Bid.Close;

                    if (Ask.Close != 0)
                        return Ask.Close;

                    return 0m;
                }
                if (Bid != null)
                {
                    return Bid.Close;
                }
                if (Ask != null)
                {
                    return Ask.Close;
                }
                return Price;
            }
        }

        /// <summary>
        /// The closing time of this bar, computed via the Time and Period
        /// </summary>
        [Key(8)]
        public override DateTime EndTime
        {
            get => Occured + Period;
            set => Period = value - Occured;
        }

        /// <summary>
        /// High price of the QuoteBar during the time period.
        /// </summary>
        [IgnoreMember]
        public decimal High
        {
            get
            {
                if (Bid != null && Ask != null)
                {
                    if (Bid.High != 0m && Ask.High != 0m)
                        return (Bid.High + Ask.High) / 2m;

                    if (Bid.High != 0)
                        return Bid.High;

                    if (Ask.High != 0)
                        return Ask.High;

                    return 0m;
                }
                if (Bid != null)
                {
                    return Bid.High;
                }
                if (Ask != null)
                {
                    return Ask.High;
                }
                return 0m;
            }
        }

        /// <summary>
        /// Average ask size
        /// </summary>
        [Key(9)]
        public decimal LastAskSize { get; set; }

        /// <summary>
        /// Average bid size
        /// </summary>
        [Key(10)]
        public decimal LastBidSize { get; set; }

        /// <summary>
        /// Low price of the QuoteBar during the time period.
        /// </summary>
        [IgnoreMember]
        public decimal Low
        {
            get
            {
                if (Bid != null && Ask != null)
                {
                    if (Bid.Low != 0m && Ask.Low != 0m)
                        return (Bid.Low + Ask.Low) / 2m;

                    if (Bid.Low != 0)
                        return Bid.Low;

                    if (Ask.Low != 0)
                        return Ask.Low;

                    return 0m;
                }
                if (Bid != null)
                {
                    return Bid.Low;
                }
                if (Ask != null)
                {
                    return Ask.Low;
                }
                return 0m;
            }
        }

        /// <summary>
        /// Opening price of the bar: Defined as the price at the start of the time period.
        /// </summary>
        [IgnoreMember]
        public decimal Open
        {
            get
            {
                if (Bid != null && Ask != null)
                {
                    if (Bid.Open != 0m && Ask.Open != 0m)
                        return (Bid.Open + Ask.Open) / 2m;

                    if (Bid.Open != 0)
                        return Bid.Open;

                    if (Ask.Open != 0)
                        return Ask.Open;

                    return 0m;
                }
                if (Bid != null)
                {
                    return Bid.Open;
                }
                if (Ask != null)
                {
                    return Ask.Open;
                }
                return 0m;
            }
        }

        /// <summary>
        /// The period of this quote bar, (second, minute, daily, ect...)
        /// </summary>
        [Key(11)]
        public TimeSpan Period { get; set; }

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
            //Check pricing for quotes
            if (Bid == null && bidprice != 0) Bid = new BarImpl();
            Bid?.Update(bidprice);

            if (Ask == null && askprice != 0) Ask = new BarImpl();
            Ask?.Update(askprice);

            if (bidsize > 0)
                LastBidSize = bidsize;
            if (asksize > 0)
                LastAskSize = asksize;

            //Check if we do not have a trade price
            if (tradeprice != 0) Price = tradeprice;
            else if (askprice != 0) Price = askprice;
            else if (bidprice != 0) Price = bidprice;
        }

        #endregion Public Methods
    }
}