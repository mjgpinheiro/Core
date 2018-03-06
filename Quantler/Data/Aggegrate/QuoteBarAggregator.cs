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

using Quantler.Data.Bars;
using System;

namespace Quantler.Data.Aggegrate
{
    /// <summary>
    /// Aggregates quote bars in larger quote bars
    /// </summary>
    /// <seealso cref="QuoteBar" />
    public class QuoteBarAggregator : TimeSerieAggregator<QuoteBar, QuoteBar>
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuoteBarAggregator"/> class.
        /// </summary>
        /// <param name="period"></param>
        public QuoteBarAggregator(TimeSpan period)
            : base(period)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuoteBarAggregator"/> class.
        /// </summary>
        /// <param name="count"></param>
        public QuoteBarAggregator(int count)
            : base(count)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuoteBarAggregator"/> class.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="period"></param>
        public QuoteBarAggregator(int count, TimeSpan period)
            : base(count, period)
        {
        }

        #endregion Public Constructors

        #region Protected Methods

        /// <summary>
        /// Aggregates the new 'data' into the 'currentbar'. The 'currentbar' will be null following the event firing
        /// </summary>
        /// <param name="currentbar"></param>
        /// <param name="data"></param>
        protected override void AggregateBar(ref QuoteBar currentbar, QuoteBar data)
        {
            //Get current pricing
            var bid = data.Bid;
            var ask = data.Ask;

            //Check if we have a bar
            if (currentbar == null)
            {
                currentbar = new QuoteBar
                {
                    Ticker = data.Ticker,
                    Occured = GetRoundedBarTime(data.Occured),
                    Bid = bid?.Clone(),
                    Ask = ask?.Clone(),
                    TimeZone = data.TimeZone
                };
            }

            //Update the bid and ask
            if (bid != null)
            {
                currentbar.LastBidSize = data.LastBidSize;
                if (currentbar.Bid == null)
                    currentbar.Bid = new BarImpl(bid.Open, bid.High, bid.Low, bid.Close);
                else
                {
                    currentbar.Bid.Close = bid.Close;
                    if (currentbar.Bid.High < bid.High) currentbar.Bid.High = bid.High;
                    if (currentbar.Bid.Low > bid.Low) currentbar.Bid.Low = bid.Low;
                }
            }
            if (ask != null)
            {
                currentbar.LastAskSize = data.LastAskSize;
                if (currentbar.Ask == null)
                    currentbar.Ask = new BarImpl(ask.Open, ask.High, ask.Low, ask.Close);
                else
                {
                    currentbar.Ask.Close = ask.Close;
                    if (currentbar.Ask.High < ask.High) currentbar.Ask.High = ask.High;
                    if (currentbar.Ask.Low > ask.Low) currentbar.Ask.Low = ask.Low;
                }
            }

            //Set price
            currentbar.Price = data.Price;
        }

        #endregion Protected Methods
    }
}