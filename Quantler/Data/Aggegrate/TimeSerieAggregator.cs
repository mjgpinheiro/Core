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
    /// Time-serie based aggregation logic, takes a data point as input and aggregates this to another data point type
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TAggregated"></typeparam>
    public abstract class TimeSerieAggregator<TInput, TAggregated> : DataAggregatorBase<TInput>
        where TInput : DataPoint
        where TAggregated : DataPointImpl
    {
        #region Private Fields

        /// <summary>
        /// If it is not aggregated based on time, we aggregate based on count/occurance
        /// </summary>
        private readonly int? _count;

        /// <summary>
        /// Keeps track of the current count for aggregation
        /// </summary>
        private int _currentcount;

        /// <summary>
        /// The current datapoint aggregation is stored in to
        /// </summary>
        private TAggregated _currentdata;

        /// <summary>
        /// The last time we updated, if this is time related
        /// </summary>
        private DateTime? _lastupdate;

        #endregion Private Fields

        #region Protected Constructors

        /// <summary>
        /// Creates an aggregator to produce new  <typeparamref name="TAggregated"/> instance based on the supplied period
        /// </summary>
        /// <param name="period"></param>
        protected TimeSerieAggregator(TimeSpan period) =>
            Period = period;

        /// <summary>
        /// Creates an aggregator to produce new  <typeparamref name="TAggregated"/> instance based on the supplied count of items
        /// </summary>
        /// <param name="count"></param>
        protected TimeSerieAggregator(int count) =>
            _count = count;

        /// <summary>
        /// Creates an aggregator to produce new  <typeparamref name="TAggregated"/> instance based on the supplied period and count of items
        /// </summary>
        /// <param name="count"></param>
        /// <param name="period"></param>
        protected TimeSerieAggregator(int count, TimeSpan period)
        {
            Period = period;
            _count = count;
        }

        #endregion Protected Constructors

        #region Public Events

        /// <summary>
        /// Event handler that fires when a new piece of data is produced. We define this as a 'new'
        /// event so we can expose it as a <typeparamref name="TAggregated"/> instead of a <see cref="DataPointImpl"/> instance
        /// </summary>
        public new event EventHandler<TAggregated> DataAggregated;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Gets a clone of the data being aggregated
        /// </summary>
        public override DataPoint CurrentData => _currentdata?.Clone();

        /// <summary>
        /// If true, this is instance is time related
        /// </summary>
        public bool IsTimeBased => !_count.HasValue && Period.HasValue;

        /// <summary>
        /// Derived aggregator name
        /// </summary>
        public override string Name => Period + InputType.FullName + OutputType.FullName + GetType().FullName;

        /// <summary>
        /// Gets the type produced by this aggregation
        /// </summary>
        public override Type OutputType => typeof(TAggregated);

        /// <summary>
        /// Gets the period.
        /// </summary>
        public TimeSpan? Period { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Checks this aggregator to see if there are any past bars to update
        /// </summary>
        /// <param name="currentLocalTime"></param>
        public override void Check(DateTime currentLocalTime)
        {
            //Check if we are time related
            if (Period.HasValue && _currentdata != null)
            {
                currentLocalTime = GetRoundedBarTime(currentLocalTime);
                if (Period.Value != TimeSpan.Zero && currentLocalTime - _currentdata.Occured >= Period.Value && currentLocalTime > _lastupdate)
                {
                    OnAggegratedData(_currentdata);
                    _lastupdate = currentLocalTime;
                    _currentdata = null;
                }
            }
        }

        /// <summary>
        /// Updates this aggregator with the specified data.
        /// </summary>
        /// <param name="data"></param>
        public override void Feed(TInput data)
        {
            //Check if we are allowed to process this data
            if (!ShouldProcess(data))
                return;

            //Initials
            bool fireevent = false;
            bool preaggregate = _count.HasValue || (Period.HasValue && Period.Value == TimeSpan.Zero);
            DateTime occured = data.Occured;

            //Check for counts
            if (_count.HasValue)
            {
                //Add a count
                _currentcount++;
                if (_currentcount >= _count)
                {
                    _currentcount = 0;
                    fireevent = true;
                }
            }

            //Set initial time
            if (!_lastupdate.HasValue)
                _lastupdate = IsTimeBased ? DateTime.MinValue : occured;

            //Check if we are time based
            if (IsTimeBased)
                //Check event for current bar
                fireevent = _currentdata != null && occured - _currentdata.Occured >= Period.Value && GetRoundedBarTime(occured) > _lastupdate;

            //Check if we need to perform aggregation logic on data, before sending an event
            if (preaggregate && occured >= _lastupdate)
                AggregateBar(ref _currentdata, data);

            //Check if we need to fire the event
            if (fireevent)
            {
                //Get current bar
                if (_currentdata is TradeBar currentbar)
                {
                    //Set correct period of bar
                    if (Period.HasValue)
                        currentbar.Period = Period.Value;
                    else if (!(data is TradeBar))
                        currentbar.Period = occured - _lastupdate.Value;
                }

                //Perform actions
                OnAggegratedData(_currentdata);
                _lastupdate = IsTimeBased && _currentdata != null ? _currentdata.Occured.Add(Period ?? TimeSpan.Zero) : occured;
                _currentdata = null;
            }

            //Check for ex-ante aggregation
            if (!preaggregate && occured >= _lastupdate)
                AggregateBar(ref _currentdata, data);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Aggregates the new 'data' into the 'currentbar'. The 'currentbar' will be null following the event firing
        /// </summary>
        /// <param name="currentbar"></param>
        /// <param name="data"></param>
        protected abstract void AggregateBar(ref TAggregated currentbar, TInput data);

        /// <summary>
        /// Get the rounded down bar time, rounded to the specified timespan interval
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        protected DateTime GetRoundedBarTime(DateTime time) =>
            Period.HasValue && !_count.HasValue ? time.RoundDown((TimeSpan)Period) : time;

        /// <summary>
        /// Determines if we are allowed to process this data point
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual bool ShouldProcess(TInput data) => data.DataType != DataType.Tick || (data is Tick tick && tick.IsValid);

        /// <summary>
        /// Called when data is aggregated.
        /// </summary>
        /// <param name="aggegrated">The aggregated data.</param>
        protected virtual void OnAggegratedData(TAggregated aggegrated)
        {
            base.OnAggegratedData(aggegrated);
            DataAggregated?.Invoke(this, aggegrated);
        }

        #endregion Protected Methods
    }
}