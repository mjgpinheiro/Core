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
using NLog;
using Quantler.Data;
using Quantler.Data.Feed;
using Quantler.Tracker;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Quantler.Configuration;
using Quantler.Data.Market.Filter;
using Quantler.Securities;

namespace Quantler.DataFeeds
{
    /// <summary>
    /// Base implementation for data feeds
    /// </summary>
    public abstract class BaseFeed
    {
        #region Protected Fields

        /// <summary>
        /// The fire hose ticker
        /// </summary>
        protected const string FireHoseTicker = "*";

        /// <summary>
        /// Currently received and queued ticks
        /// </summary>
        protected readonly ConcurrentQueue<DataPoint> CurrenDatapoints = new ConcurrentQueue<DataPoint>();

        /// <summary>
        /// The conversion implementation
        /// </summary>
        protected Func<object, DataPoint[]> Conversion = null;

        /// <summary>
        /// The data capture implementation
        /// </summary>
        protected Action<string> DataCapture;

        #endregion Protected Fields

        #region Private Fields

        /// <summary>
        /// Logging instance
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The minimum wait time in milliseconds
        /// </summary>
        private readonly int _waittimeinms = Config.GlobalConfig.DataFeedWaitTimeInMS;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        public DataSource DataSource { get; protected set; }

        /// <summary>
        /// Gets or sets the data subscription manager.
        /// </summary>
        public DataSubscriptionManager DataSubscriptionManager { get; protected set; }

        /// <summary>
        /// Last UTC time tick was received
        /// </summary>
        public virtual DateTime LastDataReceivedUtc
        {
            protected set;
            get;
        } = DateTime.MinValue;

        /// <summary>
        /// Gets or sets the name of this data feed
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the security tracker for creating and accessing securities
        /// </summary>
        public SecurityTracker Securities { get; protected set; }

        /// <summary>
        /// Gets the associated data filter
        /// </summary>
        public DataFilter DataFilter { get; protected set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Gets the name of ticker in the feed.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <returns></returns>
        public abstract string GetFeedTicker(TickerSymbol ticker);

        /// <summary>
        /// Gets the name of the ticker at quantler.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <returns></returns>
        public abstract TickerSymbol GetQuantlerTicker(string ticker);

        /// <summary>
        /// Get available ticks, base implementation
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<DataFeedPacket> GetAvailableDataPackets()
        {
            //Default values
            DateTime time = DateTime.MinValue;
            TimeSpan timespan = TimeSpan.FromMilliseconds(10); //Combine all data from the last 10 milliseconds
            Dictionary<TickerSymbol, DataFeedPacket> datapackets = new Dictionary<TickerSymbol, DataFeedPacket>();

            //Check for new data
            while (CurrenDatapoints.Count > 0)
            {
                if (CurrenDatapoints.TryPeek(out DataPoint item))
                {
                    //Get current moment in time
                    if (time == DateTime.MinValue)
                        time = item.OccuredUtc + timespan;
                    else if (time < item.OccuredUtc)
                        break;                  //Not the same timespan

                    //Get item
                    if (CurrenDatapoints.TryDequeue(out item))
                    {
                        //Get security
                        Security security;
                        if (Securities != null)
                        {
                            security = Securities[item.Ticker];
                            if (security is UnknownSecurity)
                            {
                                _log.Warn($"Received unknown security with ticker {security.Ticker}");
                                continue;
                            }
                        }
                        else
                            security = new UnknownSecurity(item.Ticker.Name);

                        //Check if we are allowed to accept this data point
                        if (!DataFilter.Accept(security, item))
                            continue;

                        //Get data
                        LastDataReceivedUtc = item.OccuredUtc;
                        if (datapackets.TryGetValue(item.Ticker, out DataFeedPacket packet))
                            packet.Add(item);
                        else
                            datapackets.Add(item.Ticker, new DataFeedPacket(security, DataSubscriptionManager?.GetDataSubscriptions(item.Ticker, item.DataType), item));
                    }
                }
                else
                    break;                      //No items
            }

            //Wait for new data if we do not have any
            if (datapackets.Count == 0)
                Thread.Sleep(_waittimeinms);

            //Return what we have
            return datapackets.Values;
        }

        /// <summary>
        /// Get historical ticks from data feed, if possible
        /// </summary>
        /// <param name="subscriptionsRequest">Data subscriptions to request</param>
        /// <param name="start">Start date for history request</param>
        /// <param name="end">End date for history request</param>
        /// <returns></returns>
        public virtual void GetDataHistory(DataSubscriptionRequest[] subscriptionsRequest, DateTime start, DateTime end) =>
            //Default implementation does not support historical data
            _log.Error($"Data feed with name {Name} does not support loading historical data for backfilling purposes!");

        /// <summary>
        /// Sets the data capture handler logic.
        /// </summary>
        /// <param name="captureaction">The capture action.</param>
        public void SetDataCapture(Action<string> captureaction)
        {
            _log.Info($"Setting data capture action for data feed with name: {Name}");
            DataCapture = captureaction;
        }

        /// <summary>
        /// Sets the dependencies.
        /// </summary>
        /// <param name="securities">The securities.</param>
        /// <param name="datasubscriptionmanager">The data subscription manager.</param>
        /// <param name="datafilter"></param>
        public void SetDependencies(SecurityTracker securities, DataSubscriptionManager datasubscriptionmanager, DataFilter datafilter)
        {
            Securities = securities;
            DataSubscriptionManager = datasubscriptionmanager;
            DataFilter = datafilter;
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Try convert currently received data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="datapoints"></param>
        /// <returns></returns>
        protected virtual bool TryConvert(object data, out DataPoint[] datapoints)
        {
            //initialize
            datapoints = null;

            //try
            try
            {
                //Deserialize
                datapoints = Conversion?.Invoke(data);

                //Success
                return datapoints != null;
            }
            catch
            {
                _log.Warn($"Unable to properly convert data received from feed. Data received will be omitted");
                return false;
            }
        }

        #endregion Protected Methods
    }
}