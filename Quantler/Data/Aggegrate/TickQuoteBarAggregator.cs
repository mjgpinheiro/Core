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
using Quantler.Data.Market;
using System;

namespace Quantler.Data.Aggegrate
{
    /// <summary>
    /// Aggregates ticks into quote bars, ignoring trades
    /// </summary>
    /// <seealso cref="TimeSerieAggregator{Tick, QuoteBar}" />
    public class TickQuoteBarAggregator : TimeSerieAggregator<Tick, QuoteBar>
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TickQuoteBarAggregator"/> class.
        /// </summary>
        /// <param name="period"></param>
        public TickQuoteBarAggregator(TimeSpan period)
            : base(period)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TickQuoteBarAggregator"/> class.
        /// </summary>
        /// <param name="count"></param>
        public TickQuoteBarAggregator(int count)
            : base(count)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TickQuoteBarAggregator"/> class.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="period"></param>
        public TickQuoteBarAggregator(int count, TimeSpan period)
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
        protected override void AggregateBar(ref QuoteBar currentbar, Tick data)
        {
            //Check current bar
            if (currentbar == null)
            {
                currentbar = new QuoteBar
                {
                    Ticker = data.Ticker,
                    Occured = GetRoundedBarTime(data.Occured),
                    TimeZone = data.TimeZone,
                    Bid = null,
                    Ask = null
                };

                if (Period.HasValue) currentbar.Period = Period.Value;
            }

            //Update the bid and ask
            currentbar.Update(0, data.BidPrice, data.AskPrice, 0, data.BidSize, data.AskSize);
        }

        /// <summary>
        /// Determines if we are allowed to process this data point
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool ShouldProcess(Tick data) =>
            (data.IsQuote || data.IsFullQuote) && data.IsValid;

        #endregion Protected Methods
    }
}