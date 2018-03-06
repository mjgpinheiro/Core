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
    //TODO: this
    ///// <summary>
    ///// Aggregates data in renko bars (TODO)
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <seealso cref="Quantler.Data.Aggegrate.PriceAggregator{T}" />
    //public class RenkoAggregator : DataAggregator
    //{
    //    #region Public Fields

    //    /// <summary>
    //    /// Event handler that fires when a new piece of data is produced
    //    /// </summary>
    //    public EventHandler<RenkoBar> DataAggregated;

    //    #endregion Public Fields

    //    #region Private Fields

    //    private readonly decimal BarSize;

    //    private readonly bool EvenBars;

    //    private readonly Func<DataPoint, decimal> _selector;

    //    private readonly Func<DataPoint, long> _volumeSelector;

    //    private DateTime CloseOn;

    //    private decimal CloseRate;

    //    private RenkoBar CurrentBar;

    //    private DataAggregationHandler DataAggregationHandler;

    //    private bool _firstTick = true;

    //    private decimal _highRate;

    //    private RenkoBar _lastWicko = null;

    //    private decimal _lowRate;

    //    private DateTime _openOn;

    //    private decimal _openRate;

    //    #endregion Private Fields

    //    #region Public Constructors

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="RenkoConsolidator"/> class using the specified <paramref name="barSize"/>.
    //    /// </summary>
    //    /// <param name="barSize">The constant value size of each bar</param>
    //    /// <param name="type">The RenkoType of the bar</param>
    //    public RenkoAggregator(decimal barSize, RenkoType type)
    //    {
    //        if (type != RenkoType.Wicked)
    //            throw new ArgumentOutOfRangeException("type");

    //        BarSize = barSize;

    //        Type = type;
    //    }

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="RenkoConsolidator"/> class using the specified <paramref name="barSize"/>.
    //    /// The value selector will by default select <see cref="IBaseData.Value"/>
    //    /// The volume selector will by default select zero.
    //    /// </summary>
    //    /// <param name="barSize">The constant value size of each bar</param>
    //    /// <param name="evenBars">When true bar open/close will be a multiple of the barSize</param>
    //    public RenkoAggregator(decimal barSize, bool evenBars = true)
    //    {
    //        BarSize = barSize;
    //        _selector = x => x.Value;
    //        _volumeSelector = x => 0;
    //        EvenBars = evenBars;

    //        Type = RenkoType.Classic;
    //    }

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="RenkoConsolidator" /> class.
    //    /// </summary>
    //    /// <param name="barSize">The size of each bar in units of the value produced by <paramref name="selector"/></param>
    //    /// <param name="selector">Extracts the value from a data instance to be formed into a <see cref="RenkoBar"/>. The default
    //    /// value is (x => x.Value) the <see cref="IBaseData.Value"/> property on <see cref="IBaseData"/></param>
    //    /// <param name="volumeSelector">Extracts the volume from a data instance. The default value is null which does
    //    /// not aggregate volume per bar.</param>
    //    /// <param name="evenBars">When true bar open/close will be a multiple of the barSize</param>
    //    public RenkoAggregator(decimal barSize, Func<DataPoint, decimal> selector, Func<DataPoint, long> volumeSelector = null, bool evenBars = true)
    //    {
    //        if (barSize < Extensions.GetDecimalEpsilon())
    //        {
    //            throw new ArgumentOutOfRangeException("barSize", "RenkoConsolidator bar size must be positve and greater than 1e-28");
    //        }

    //        BarSize = barSize;
    //        EvenBars = evenBars;
    //        _selector = selector ?? (x => x.Value);
    //        _volumeSelector = volumeSelector ?? (x => 0);

    //        Type = RenkoType.Classic;
    //    }

    //    #endregion Public Constructors

    //    #region Public Events

    //    /// <summary>
    //    /// Event handler that fires when a new piece of data is produced
    //    /// </summary>
    //    event DataAggregationHandler DataAggregator.DataAggregated
    //    {
    //        add { DataAggregationHandler += value; }
    //        remove { DataAggregationHandler -= value; }
    //    }

    //    #endregion Public Events

    //    #region Public Properties

    //    /// <summary>
    //    /// Gets the most recently aggregated piece of data. This will be null if this aggregator
    //    /// has not produced any data yet.
    //    /// </summary>
    //    public DataPoint Aggregated
    //    {
    //        get; private set;
    //    }

    //    /// <summary>
    //    /// Gets the bar size used by this consolidator
    //    /// </summary>
    //    public decimal BarSize
    //    {
    //        get { return BarSize; }
    //    }

    //    /// <summary>
    //    /// Gets a clone of the data being currently consolidated
    //    /// </summary>
    //    public DataPoint CurrentData =>
    //        CurrentBar == null ? null : CurrentBar.Clone();

    //    /// <summary>
    //    /// Gets the type consumed by this consolidator
    //    /// </summary>
    //    public Type InputType =>
    //        typeof(DataPointBar);

    //    /// <summary>
    //    /// Gets the type of the output.
    //    /// </summary>
    //    public Type OutputType =>
    //        typeof(RenkoBar);

    //    /// <summary>
    //    /// Gets the kind of the bar
    //    /// </summary>
    //    public RenkoType Type { get; private set; }

    //    #endregion Public Properties

    //    #region Internal Properties

    //    // Used for unit tests
    //    internal RenkoBar OpenRenkoBar
    //    {
    //        get
    //        {
    //            return new RenkoBar(null, _openOn, CloseOn,
    //                BarSize, _openRate, _highRate, _lowRate, CloseRate);
    //        }
    //    }

    //    #endregion Internal Properties

    //    #region Public Methods

    //    /// <summary>
    //    /// Scans this consolidator to see if it should emit a bar due to time passing
    //    /// </summary>
    //    /// <param name="currentLocalTime">The current time in the local time zone (same as <see cref="Time"/>)</param>
    //    public void Scan(DateTime currentLocalTime)
    //    {
    //    }

    //    /// <summary>
    //    /// Updates this consolidator with the specified data. This method is
    //    /// responsible for raising the DataConsolidated event
    //    /// </summary>
    //    /// <param name="data">The new data for the consolidator</param>
    //    public void Update(DataPoint data)
    //    {
    //        if (Type == RenkoType.Classic)
    //            UpdateClassic(data);
    //        else
    //            UpdateWicked(data);
    //    }

    //    #endregion Public Methods

    //    #region Protected Methods

    //    /// <summary>
    //    /// Event aggregation for the DataAggregated event. This should be invoked
    //    /// by derived classes when they have consolidated a new piece of data.
    //    /// </summary>
    //    /// <param name="consolidated">The newly consolidated data</param>
    //    protected virtual void OnDataConsolidated(RenkoBar aggregated)
    //    {
    //        DataAggregated?.Invoke(this, aggregated);
    //        DataAggregationHandler?.Invoke(this, aggregated);
    //        Aggregated = aggregated;
    //    }

    //    #endregion Protected Methods

    //    #region Private Methods

    //    private void Falling(DataPoint data)
    //    {
    //        decimal limit;

    //        while (CloseRate < (limit = (_openRate - BarSize)))
    //        {
    //            var wicko = new RenkoBar(data.Symbol, _openOn, CloseOn,
    //                BarSize, _openRate, _highRate, limit, limit);

    //            _lastWicko = wicko;

    //            OnDataConsolidated(wicko);

    //            _openOn = CloseOn;
    //            _openRate = limit;
    //            _highRate = limit;
    //        }
    //    }

    //    private void Rising(DataPoint data)
    //    {
    //        decimal limit;

    //        while (CloseRate > (limit = (_openRate + BarSize)))
    //        {
    //            var wicko = new RenkoBar(data.Symbol, _openOn, CloseOn,
    //                BarSize, _openRate, limit, _lowRate, limit);

    //            _lastWicko = wicko;

    //            OnDataConsolidated(wicko);

    //            _openOn = CloseOn;
    //            _openRate = limit;
    //            _lowRate = limit;
    //        }
    //    }

    //    private void UpdateClassic(DataPoint data)
    //    {
    //        var currentValue = _selector(data);
    //        var volume = _volumeSelector(data);

    //        decimal? close = null;

    //        // if we're already in a bar then update it
    //        if (CurrentBar != null)
    //        {
    //            CurrentBar.Update(data.Time, currentValue, volume);

    //            // if the update caused this bar to close, fire the event and reset the bar
    //            if (CurrentBar.IsClosed)
    //            {
    //                close = CurrentBar.Close;
    //                OnDataConsolidated(CurrentBar);
    //                CurrentBar = null;
    //            }
    //        }

    //        if (CurrentBar == null)
    //        {
    //            var open = close ?? currentValue;
    //            if (EvenBars && !close.HasValue)
    //            {
    //                open = Math.Ceiling(open / BarSize) * BarSize;
    //            }
    //            CurrentBar = new RenkoBar(data.Symbol, data.Time, BarSize, open, volume);
    //        }
    //    }

    //    private void UpdateWicked(DataPoint data)
    //    {
    //        var rate = data.Price;

    //        if (_firstTick)
    //        {
    //            _firstTick = false;

    //            _openOn = data.Occured;
    //            CloseOn = data.Occured;
    //            _openRate = rate;
    //            _highRate = rate;
    //            _lowRate = rate;
    //            CloseRate = rate;
    //        }
    //        else
    //        {
    //            CloseOn = data.Occured;

    //            if (rate > _highRate)
    //                _highRate = rate;

    //            if (rate < _lowRate)
    //                _lowRate = rate;

    //            CloseRate = rate;

    //            if (CloseRate > _openRate)
    //            {
    //                if (_lastWicko == null ||
    //                    (_lastWicko.Direction == BarDirection.Rising))
    //                {
    //                    Rising(data);

    //                    return;
    //                }

    //                var limit = (_lastWicko.Open + BarSize);

    //                if (CloseRate > limit)
    //                {
    //                    var wicko = new RenkoBar(data.Symbol, _openOn, CloseOn,
    //                        BarSize, _lastWicko.Open, limit, _lowRate, limit);

    //                    _lastWicko = wicko;

    //                    OnDataConsolidated(wicko);

    //                    _openOn = CloseOn;
    //                    _openRate = limit;
    //                    _lowRate = limit;

    //                    Rising(data);
    //                }
    //            }
    //            else if (CloseRate < _openRate)
    //            {
    //                if (_lastWicko == null ||
    //                    (_lastWicko.Direction == BarDirection.Falling))
    //                {
    //                    Falling(data);

    //                    return;
    //                }

    //                var limit = (_lastWicko.Open - BarSize);

    //                if (CloseRate < limit)
    //                {
    //                    var wicko = new RenkoBar(data.Symbol, _openOn, CloseOn,
    //                        BarSize, _lastWicko.Open, _highRate, limit, limit);

    //                    _lastWicko = wicko;

    //                    OnDataConsolidated(wicko);

    //                    _openOn = CloseOn;
    //                    _openRate = limit;
    //                    _highRate = limit;

    //                    Falling(data);
    //                }
    //            }
    //        }
    //    }

    //    #endregion Private Methods
    //}

    ///// <summary>
    ///// Provides a type safe wrapper on the RenkoConsolidator class. This just allows us to define our selector functions with the real type they'll be receiving
    ///// </summary>
    ///// <typeparam name="TInput"></typeparam>
    //public class RenkoConsolidator<TInput> : RenkoConsolidator
    //    where TInput : DataPoint
    //{
    //    #region Public Constructors

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="RenkoConsolidator" /> class.
    //    /// </summary>
    //    /// <param name="barSize">The size of each bar in units of the value produced by <paramref name="selector"/></param>
    //    /// <param name="selector">Extracts the value from a data instance to be formed into a <see cref="RenkoBar"/>. The default
    //    /// value is (x => x.Value) the <see cref="IBaseData.Value"/> property on <see cref="IBaseData"/></param>
    //    /// <param name="volumeSelector">Extracts the volume from a data instance. The default value is null which does
    //    /// not aggregate volume per bar.</param>
    //    /// <param name="evenBars">When true bar open/close will be a multiple of the barSize</param>
    //    public RenkoConsolidator(decimal barSize, Func<TInput, decimal> selector, Func<TInput, long> volumeSelector = null, bool evenBars = true)
    //        : base(barSize, x => selector((TInput)x), volumeSelector == null ? (Func<DataPoint, long>)null : x => volumeSelector((TInput)x), evenBars)
    //    {
    //    }

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="RenkoConsolidator"/> class using the specified <paramref name="barSize"/>.
    //    /// The value selector will by default select <see cref="IBaseData.Value"/>
    //    /// The volume selector will by default select zero.
    //    /// </summary>
    //    /// <param name="barSize">The constant value size of each bar</param>
    //    /// <param name="evenBars">When true bar open/close will be a multiple of the barSize</param>
    //    public RenkoConsolidator(decimal barSize, bool evenBars = true)
    //        : base(barSize, evenBars)
    //    {
    //    }

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="RenkoConsolidator"/> class using the specified <paramref name="barSize"/>.
    //    /// The value selector will by default select <see cref="IBaseData.Value"/>
    //    /// The volume selector will by default select zero.
    //    /// </summary>
    //    /// <param name="barSize">The constant value size of each bar</param>
    //    /// <param name="type">The RenkoType of the bar</param>
    //    public RenkoConsolidator(decimal barSize, RenkoType type)
    //        : base(barSize, type)
    //    {
    //    }

    //    #endregion Public Constructors

    //    #region Public Methods

    //    /// <summary>
    //    /// Updates this consolidator with the specified data.
    //    /// </summary>
    //    /// <remarks>
    //    /// Type safe shim method.
    //    /// </remarks>
    //    /// <param name="data">The new data for the consolidator</param>
    //    public void Update(TInput data)
    //    {
    //        base.Update(data);
    //    }

    //    #endregion Public Methods
    //}
}