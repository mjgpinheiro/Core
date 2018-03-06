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
    /// Aggregator for creating larger bars based on smaller trade bars
    /// </summary>
    /// <seealso cref="TradeBar" />
    public class TradeAggregator : TradeBarAggregatorBase<TradeBar>
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeAggregator"/> class.
        /// </summary>
        /// <param name="period"></param>
        public TradeAggregator(TimeSpan period)
            : base(period)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeAggregator"/> class.
        /// </summary>
        /// <param name="count"></param>
        public TradeAggregator(int count)
            : base(count)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeAggregator"/> class.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="period"></param>
        public TradeAggregator(int count, TimeSpan period)
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
        protected override void AggregateBar(ref TradeBar currentbar, TradeBar data)
        {
            //Check current bar
            if (currentbar == null)
            {
                currentbar = new TradeBar
                {
                    Occured = GetRoundedBarTime(data.Occured),
                    TimeZone = data.TimeZone,
                    Ticker = data.Ticker,
                    Open = data.Open,
                    High = data.High,
                    Low = data.Low,
                    Close = data.Close,
                    Volume = data.Volume,
                    DataType = DataType.TradeBar,
                    Period = IsTimeBased && Period.HasValue ? (TimeSpan)Period : data.Period
                };
            }
            else
            {
                //Aggregate the working bar
                currentbar.Close = data.Close;
                currentbar.Volume += data.Volume;
                if (!IsTimeBased) currentbar.Period += data.Period;
                if (data.Low < currentbar.Low) currentbar.Low = data.Low;
                if (data.High > currentbar.High) currentbar.High = data.High;
            }
        }

        #endregion Protected Methods
    }
}