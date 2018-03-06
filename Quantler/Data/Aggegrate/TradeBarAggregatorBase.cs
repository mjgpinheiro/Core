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
    /// Aggregator that can make bars from any base data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="TradeBar" />
    public abstract class TradeBarAggregatorBase<T> : TimeSerieAggregator<T, TradeBar>
        where T : DataPoint
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeBarAggregatorBase{T}"/> class.
        /// </summary>
        /// <param name="period"></param>
        public TradeBarAggregatorBase(TimeSpan period)
            : base(period)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeBarAggregatorBase{T}"/> class.
        /// </summary>
        /// <param name="count"></param>
        public TradeBarAggregatorBase(int count)
            : base(count)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeBarAggregatorBase{T}"/> class.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="period"></param>
        public TradeBarAggregatorBase(int count, TimeSpan period)
            : base(count, period)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the current bar.
        /// </summary>
        public TradeBar CurrentBar => (TradeBar)CurrentData;

        #endregion Public Properties
    }
}