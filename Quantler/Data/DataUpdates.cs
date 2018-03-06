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

using MoreLinq;
using Quantler.Data.Bars;
using Quantler.Data.Corporate;
using Quantler.Data.Market;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Quantler.Securities;

namespace Quantler.Data
{
    /// <summary>
    /// Holder for different data updates
    /// TODO: what if we have multiple quotebars for the same ticker?
    /// </summary>
    public class DataUpdates : IEnumerable<KeyValuePair<TickerSymbol, DataPoint>>
    {
        #region Public Indexers

        /// <summary>
        /// Gets the <see cref="DataUpdates"/> with the specified ticker.
        /// </summary>
        public DataUpdates this[string ticker] =>
            new DataUpdates(OccuredUtc, _dataPoints.Where(x => x.Ticker == ticker).ToArray());

        /// <summary>
        /// Gets the <see cref="DataUpdates"/> with the specified security.
        /// </summary>
        public DataUpdates this[Security security] =>
            this[security.Ticker.Name];

        /// <summary>
        /// Gets the <see cref="DataUpdates"/> with the specified universe.
        /// </summary>
        public DataUpdates this[Universe universe]
        {
            get
            {
                var tickers = universe.Securities.Select(x => x.Ticker).ToArray();
                return new DataUpdates(OccuredUtc, _dataPoints.Where(x => tickers.Contains(x.Ticker)).ToArray());
            }
        }

        #endregion Public Indexers

        #region Private Fields

        /// <summary>
        /// The retrieved data points
        /// </summary>
        private readonly DataPoint[] _dataPoints;

        #endregion Private Fields

        private DataUpdates(DateTime occuredutc, DataPoint[] dataPoints, Delistings delistings, Dividends dividends,
            Earnings earnings, Financials financials, KeyStats keystats, QuoteBars quotebars, Splits splits,
            TradeBars tradebars, TradingStatusUpdates tradingStatusUpdates,
            Ticks ticks)
        {
            //TODO: for cloning this object (so it becomes immutable)
            throw new NotImplementedException();
        }

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataUpdates"/> class.
        /// </summary>
        /// <param name="occuredUtc">The occured.</param>
        /// <param name="datapoints">The datapoints.</param>
        public DataUpdates(DateTime occuredUtc, params DataPoint[] datapoints)
        {
            //Set datetime
            OccuredUtc = occuredUtc;

            //Set datapoints
            _dataPoints = datapoints;

            //Create holders
            Delistings = CreateHolder<Delistings, Delisting>(Delistings);
            Dividends = CreateHolder<Dividends, Dividend>(Dividends);
            Earnings = CreateHolder<Earnings, Earning>(Earnings);
            Financials = CreateHolder<Financials, Financial>(Financials);
            KeyStats = CreateHolder<KeyStats, KeyStat>(KeyStats);
            QuoteBars = CreateHolder<QuoteBars, QuoteBar>(QuoteBars);
            Splits = CreateHolder<Splits, Split>(Splits);
            TradeBars = CreateHolder<TradeBars, TradeBar>(TradeBars);
            TradingStatusUpdates = CreateHolder<TradingStatusUpdates, TradingStatus>(TradingStatusUpdates);

            //For ticks we need to do differently as we can have multiple ticks on the same update
            Ticks = CreateTicksHolder(Ticks);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Number of updates available
        /// </summary>
        public int Count => _dataPoints.Length;

        /// <summary>
        /// Gets all the datapoints in this data update instance
        /// </summary>
        public DataPoint[] Data => _dataPoints;

        /// <summary>
        /// Current delistigns
        /// </summary>
        public Delistings Delistings { get; }

        /// <summary>
        /// Current dividend payouts
        /// </summary>
        public Dividends Dividends { get; }

        /// <summary>
        /// Current earning reports
        /// </summary>
        public Earnings Earnings { get; }

        /// <summary>
        /// Current financial statements
        /// </summary>
        public Financials Financials { get; }

        /// <summary>
        /// If true, this instance has updates
        /// </summary>
        public bool HasUpdates => Count > 0;

        /// <summary>
        /// Key statistics updates
        /// </summary>
        public KeyStats KeyStats { get; }

        /// <summary>
        /// Date and time for this update
        /// </summary>
        public DateTime OccuredUtc { get; }

        /// <summary>
        /// Updates in quote bars
        /// </summary>
        public QuoteBars QuoteBars { get; }

        /// <summary>
        /// Stock splits in current update
        /// </summary>
        public Splits Splits { get; }

        /// <summary>
        /// Gets all symbol tickers in this data update instance
        /// </summary>
        public TickerSymbol[] Tickers => _dataPoints.Select(x => x.Ticker).Distinct().ToArray();

        /// <summary>
        /// Current ticks received
        /// </summary>
        public Ticks Ticks { get; }

        /// <summary>
        /// Trade bars available in this update
        /// </summary>
        public TradeBars TradeBars { get; }

        /// <summary>
        /// Trading status updates from exchangeModel
        /// </summary>
        public TradingStatusUpdates TradingStatusUpdates { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Determines whether this instance contains the specified ticker
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <returns>
        ///   <c>true</c> if this instance contains ticker; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsTicker(string ticker) =>
            _dataPoints.Count(x => x.Ticker == ticker) > 0;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerator<KeyValuePair<TickerSymbol, DataPoint>> GetEnumerator() =>
            _dataPoints.Select(x => new KeyValuePair<TickerSymbol, DataPoint>(x.Ticker, x)).GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Tries the get a data point by type, if ticks are requested, only the first instance is returned
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ticker">The ticker.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public bool TryGetDataPoint<T>(string ticker, out T data)
            where T : DataPoint
        {
            //Get datapoint and check if we have data
            data = _dataPoints.OfType<T>().FirstOrDefault(x => x.Ticker == ticker);
            return data != null;
        }

        /// <summary>
        /// Creates a deep copy
        /// </summary>
        /// <returns></returns>
        public DataUpdates Clone() =>
            new DataUpdates(OccuredUtc, _dataPoints, Delistings, Dividends, Earnings, Financials, KeyStats, 
                QuoteBars, Splits, TradeBars, TradingStatusUpdates, Ticks);

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Creates the holder for storing the retrieved values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="holder">The holder.</param>
        /// <returns></returns>
        private T CreateHolder<T, TItem>(T holder)
            where T : Dictionary<TickerSymbol, TItem>, new()
            where TItem : DataPoint
        {
            if (holder != null) return holder;
            holder = new T();
            _dataPoints.OfType<TItem>().ForEach(item => holder[item.Ticker] = item);
            return holder;
        }

        /// <summary>
        /// Creates the ticks holder.
        /// </summary>
        /// <param name="ticks">The ticks.</param>
        /// <returns></returns>
        private Ticks CreateTicksHolder(Ticks ticks)
        {
            if (ticks != null) return ticks;
            ticks = new Ticks();
            _dataPoints.OfType<Tick>().GroupBy(x => x.Ticker).ForEach(t => ticks[t.Key] = t.ToArray());
            return ticks;
        }

        #endregion Private Methods
    }
}