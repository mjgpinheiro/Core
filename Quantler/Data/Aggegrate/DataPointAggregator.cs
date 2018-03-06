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
    /// Aggregate any data point type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="TradeBarAggregatorBase{T}" />
    public class DataPointAggregator<T> : TradeBarAggregatorBase<T>
        where T : DataPoint
    {
        #region Private Fields

        /// <summary>
        /// The price function
        /// </summary>
        private readonly Func<T, decimal> _priceFunction;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPointAggregator{T}"/> class.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="pricefunction">The price function.</param>
        public DataPointAggregator(int count, Func<T, decimal> pricefunction = null)
            : base(count) =>
            _priceFunction = pricefunction ?? (x => x.Price);

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPointAggregator{T}"/> class.
        /// </summary>
        /// <param name="period">The period.</param>
        /// <param name="pricefunction">The price function.</param>
        public DataPointAggregator(TimeSpan period, Func<T, decimal> pricefunction = null)
            : base(period) =>
            _priceFunction = pricefunction ?? (x => x.Price);

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPointAggregator{T}"/> class.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="period">The period.</param>
        /// <param name="pricefunction">The price function.</param>
        public DataPointAggregator(int count, TimeSpan period, Func<T, decimal> pricefunction = null)
            : base(count, period) =>
            _priceFunction = pricefunction ?? (x => x.Price);

        #endregion Public Constructors

        #region Protected Methods

        /// <summary>
        /// Aggregates the new 'data' into the 'currentbar'. The 'currentbar' will be null following the event firing
        /// </summary>
        /// <param name="currentbar"></param>
        /// <param name="data"></param>
        protected override void AggregateBar(ref TradeBar currentbar, T data)
        {
            //Derive price
            decimal price = _priceFunction(data);

            //Aggregate data
            if (currentbar == null)
            {
                currentbar = new TradeBar
                {
                    Ticker = data.Ticker,
                    Occured = GetRoundedBarTime(data.Occured),
                    TimeZone = data.TimeZone,
                    Close = price,
                    High = price,
                    Low = price,
                    Open = price,
                    DataType = data.DataType,
                    Price = price
                };
            }
            else
            {
                //Aggregate the working bar
                CurrentBar.Close = price;
                if (price < currentbar.Low) currentbar.Low = price;
                if (price > currentbar.High) currentbar.High = price;
            }
        }

        #endregion Protected Methods
    }
}