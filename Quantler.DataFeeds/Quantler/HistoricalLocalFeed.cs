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

using CacheManager.Core;
using MoreLinq;
using NLog;
using Quantler.Broker;
using Quantler.Common;
using Quantler.Compression;
using Quantler.Configuration;
using Quantler.Data;
using Quantler.Data.DataFile;
using Quantler.DataFeeds.Quantler.Models;
using Quantler.Interfaces;
using Quantler.Messaging;
using Quantler.Securities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Quantler.DataFeeds.Quantler
{
    /// <summary>
    /// Uses a cache as source, file storage as secondary source
    /// TODO: caching and rest of functionalities do not take in to account aggregation
    /// </summary>
    [Export(typeof(DataFeed))]
    public class HistoricalLocalFeed : BaseFeed, DataFeed
    {
        #region Private Fields

        /// <summary>
        /// Caching interface
        /// </summary>
        private static ICacheManager<DataPointCached> _cachedFiles;

        /// <summary>
        /// The available tickers
        /// </summary>
        private string[] _availableTickers;

        /// <summary>
        /// The base datafeed we are repeating
        /// </summary>
        private DataFeed _baseDataFeed;

        /// <summary>
        /// The file folder
        /// </summary>
        private string _fileFolder;

        /// <summary>
        /// Logging reference
        /// </summary>
        private ILogger _log;

        /// <summary>
        /// Source name
        /// </summary>
        private string _source;

        /// <summary>
        /// Known data subscriptions
        /// </summary>
        private List<DataSubscriptionRequest> _subscriptions = new List<DataSubscriptionRequest>();

        /// <summary>
        /// Check if we need to make use of caching
        /// </summary>
        private bool _useCache;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// End date and time to load
        /// </summary>
        public DateTime EndDateTime { get; private set; }

        /// <summary>
        /// True if the expected folder exists
        /// </summary>
        public bool IsConnected => Directory.Exists(_fileFolder);

        /// <summary>
        /// True if it is currently still collecting data
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Date and time this historical period started
        /// </summary>
        public DateTime StartDateTime { get; private set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Add subscription to security
        /// </summary>
        /// <param name="subscriptionRequest"></param>
        public void AddSubscription(DataSubscriptionRequest subscriptionRequest)
        {
            if (!_subscriptions.Select(x => x.GetSubscriptionName()).Contains(subscriptionRequest.GetSubscriptionName()))
                _subscriptions.Add(subscriptionRequest);
        }

        /// <summary>
        /// Check if we can subscribe to this ticker
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        public bool CanSubscribe(TickerSymbol ticker) =>
            _availableTickers.Contains(ticker.Name);

        /// <summary>
        /// Get historical data from data feed, if possible
        /// TODO implement
        /// </summary>
        /// <param name="subscriptionsRequest">Data subscriptions to request</param>
        /// <param name="start">Start date for history request</param>
        /// <param name="end">End date for history request</param>
        public override void GetDataHistory(DataSubscriptionRequest[] subscriptionsRequest, DateTime start, DateTime end)
        {
            var found = new ConcurrentBag<DataPoint>();
            var tasks = new List<Task>();

            foreach (var sub in subscriptionsRequest)
            {
                DateTime currentdate = start.Date;
                while (currentdate < end)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        var data = LoadFromCacheAsync(sub, currentdate).Result;
                        if(data != null)
                            GetDataFromStream(sub.Ticker.Name, data).ForEach(x => found.Add(x));
                    }));

                    currentdate = currentdate.AddDays(1);
                }
            }

            //Wait for all threads
            Task.WaitAll(tasks.ToArray());

            //Set to backfilling
            //found.ForEach(x => x.IsBackfilling = true); //TODO: we need to be able to set to backfilling?

            //Return what we have
            found.OrderBy(x => x.OccuredUtc)
                .ThenBy(x => x.Ticker)
                .ForEach(x => CurrenDatapoints.Enqueue(x));
        }

        /// <summary>
        /// Gets the name of ticker in the feed.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <returns></returns>
        public override string GetFeedTicker(TickerSymbol ticker) =>
            _baseDataFeed.GetFeedTicker(ticker);

        /// <summary>
        /// Gets the name of the ticker at quantler.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <returns></returns>
        public override TickerSymbol GetQuantlerTicker(string ticker) =>
            _baseDataFeed.GetQuantlerTicker(ticker);

        /// <summary>
        /// Construct historical datafeed
        /// </summary>
        public void Initialize(MessageInstance initialmessage)
        {
            //Get tickers
            _availableTickers = Config.SecurityConfig.Select(x => x.Ticker).ToArray();

            //Set caching
            if (_cachedFiles == null)
            {
                _cachedFiles = CacheFactory.Build<DataPointCached>("histdata", settings =>
                {
                    settings.WithDictionaryHandle("inProc")
                            .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromMinutes(30));
                    //TODO: outproce, memcache?
                    //      see: https://github.com/MichaCo/CacheManager/issues/189
                });
            }

            //Check if this is a backtest
            if (initialmessage is SimulationMessage backtestmessage)
            {
                //Set period
                StartDateTime = backtestmessage.StartDateTime;
                EndDateTime = backtestmessage.EndDateTime;

                //Set environment
                _fileFolder = Config.TryGetEnvVariable("MarketDataFolder", Config.SimulationConfig.MarketDataFolder);
                _useCache = bool.Parse(Config.TryGetEnvVariable("UseCache", Config.SimulationConfig.UseCache.ToString()));

                //Check input
                if (!Directory.Exists(_fileFolder))
                    throw new DirectoryNotFoundException($"Cannot find backtest file directory {_fileFolder}");

                //Set source
                switch ((BrokerType)Enum.Parse(typeof(BrokerType), backtestmessage.BrokerType))
                {
                    case BrokerType.Robinhood:
                        _source = "IEX";
                        break;

                    case BrokerType.Bittrex:
                        _source = "Bittrex";
                        break;

                    case BrokerType.HitBtc:
                        _source = "HitBtc";
                        break;

                    case BrokerType.Cobinhood:
                        _source = "CobinHood";
                        break;

                    case BrokerType.FreeTrade:
                        _source = "IEX";
                        break;

                    case BrokerType.Unknown:
                        throw new Exception($"Data source is unknown, cannot retrieve data from an unknown data source");
                    default:
                        throw new ArgumentOutOfRangeException($"{backtestmessage.BrokerType} is unknown, cannot derive data source from an unknown broker type");
                }

                //Get broker connection associated to source
                var datafeedname =
                    Config.BrokerConfig.FirstOrDefault(x => string.Compare(x.BrokerType, backtestmessage.BrokerType, StringComparison.CurrentCultureIgnoreCase) == 0)?.DataFeed;
                if (!DynamicLoader.Instance.TryGetInstance(datafeedname, out _baseDataFeed))
                    throw new Exception($"Cannot derive broker datafeed ({datafeedname}) for broker type ({backtestmessage.BrokerType})");
            }
            else
                throw new Exception($"Cannot initialize datafeed {Name} if it is not a backtest");
        }

        /// <summary>
        /// Reconnect feed (not supported for historical data)
        /// </summary>
        public void Reconnect()
        {
        }

        /// <summary>
        /// Remove a subscription (not supported for historical data)
        /// </summary>
        /// <param name="ticker"></param>
        public void RemoveSubscription(DataSubscriptionRequest ticker)
        {
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            //Set to no longer running
            IsRunning = false;

            //Release last data
            while (CurrenDatapoints.TryDequeue(out DataPoint lastpoint))
            {
            }
        }

        /// <summary>
        /// Start historical feed
        /// </summary>
        public void Start()
        {
            //Start running
            IsRunning = true;
            DateTime currentdate = StartDateTime;
            while (IsRunning && EndDateTime > currentdate)
            {
                //Get data for today
                List<Task> tasks = new List<Task>();
                Dictionary<string, MemoryStream> streams = new Dictionary<string, MemoryStream>();
                _subscriptions.ForEach(sub =>
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        var data = await LoadFromCacheAsync(sub, currentdate);
                        if(data != null)
                            streams.Add(sub.Ticker.Name, data);
                        _log.Trace($"Loaded data: {currentdate.ToString("yyyyMMdd")} - {sub.Ticker} - {sub.DataType}");
                    }));
                });

                Task.WaitAll(tasks.ToArray());
                tasks.Clear();

                //Set background tasks
                List<DataPoint> datapoints = new List<DataPoint>();
                streams.Where(x => x.Value != null)
                       .ForEach(x => tasks.Add(Task.Run(() => datapoints.AddRange(GetDataFromStream(x.Key, x.Value)))));
                Task.WaitAll(tasks.ToArray());

                //Check order
                if (datapoints.DistinctBy(x => x.Ticker).Count() > 1)
                    datapoints = datapoints.OrderBy(x => x.OccuredUtc)
                                            .ThenBy(x => x.Ticker)
                                            .ToList();

                //Add to current data points
                datapoints.ForEach(x => CurrenDatapoints.Enqueue(x));

                //Add date
                currentdate = currentdate.AddDays(1);

                //Check if we need to wait (max 1M in cache)
                while (CurrenDatapoints.Count > 1000000 && IsRunning)
                    Thread.Sleep(10);
            }

            //Check end of simulation
            while (CurrenDatapoints.Count > 0)
                Thread.Sleep(10);

            //Stop
            IsRunning = false;
        }

        /// <summary>
        /// Stop running historical (cut off any new data)
        /// </summary>
        public void Stop()
        {
            //Stop instance
            IsRunning = false;

            //Release everything we have
            Reset();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Get ticks from stream, returns all ticks that are present in the stream
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        private IEnumerable<DataPoint> GetDataFromStream(string ticker, MemoryStream stream)
        {
            //Load data reader
            using (DataReader reader = new DataReader(ticker, stream))
            {
                //Set return object
                List<DataPoint> toreturn = new List<DataPoint>();

                //Get return data
                reader.GotData += (sender, data) => toreturn.Add(data);
                while (reader.NextDataPoint())
                    continue;

                //Return what we have
                return toreturn;
            }
        }

        /// <summary>
        /// Try and load data for date from cache
        /// http://www.alphadevx.com/a/387-Changing-the-maximum-item-size-allowed-by-Memcache
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        private async Task<MemoryStream> LoadFromCacheAsync(DataSubscriptionRequest sub, DateTime date)
        {
            string key = DataPointCached.GetKey(sub.Ticker.Name, sub.DataType, sub.Aggregation, date);
            byte[] found = null;

            //Check if we are allowed to use cache
            if (_useCache)
            {
                //Try and load from cache
                var cacheitem = _cachedFiles.Get(key);
                if (cacheitem != null)
                    return new MemoryStream(Compress.UncompressBytes(cacheitem.Data, key));
            }

            //Fall back to disk if not present
            if (found == null)
            {
                var disk = await LoadFromDiskAsync(sub, date);
                if (disk != null && _useCache)
                {
                    //Create cache item, we store the result compressed in cache
                    var cacheitem = DataPointCached.Create(sub.Ticker.Name, sub.DataType, sub.Aggregation, date, Compress.CompressBytes(disk.ToArray(), key));
                    _cachedFiles.Put(key, cacheitem);
                }

                //Return disk data
                return disk;
            }

            //This should not happen
            return null;
        }

        /// <summary>
        /// Try to load entry from archive
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        private async Task<MemoryStream> LoadFromDiskAsync(DataSubscriptionRequest sub, DateTime date)
        {
            //Get correct date
            int qldate = Util.ToQLDate(date);

            //Get archive
            string archive = DataUtil.GetArchiveFileName(_fileFolder, _source, sub.DataType, sub.Aggregation.HasValue ? $"{sub.Aggregation.Value.TotalMinutes}m" : $"1t", qldate);

            //Get Filename
            string file = DataUtil.SafeFilename(sub.Ticker.Name, Util.ToQLDate(date));

            //Load from archive
            return await Archive.ReadAsync(archive, file);
        }

        #endregion Private Methods
    }
}