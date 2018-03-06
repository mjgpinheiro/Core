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

using Quantler.Data;
using System;
using System.Collections.Generic;
using Quantler.Data.Feed;
using Quantler.Data.Market.Filter;
using Quantler.Messaging;
using Quantler.Securities;
using Quantler.Tracker;

namespace Quantler.Interfaces
{
    /// <summary>
    /// Data feed for retrieving data from endpoint
    /// </summary>
    public interface DataFeed : QTask
    {
        #region Public Properties

        /// <summary>
        /// Gets the data source.
        /// </summary>
        DataSource DataSource { get; }

        /// <summary>
        /// True if this data feed is in a connected state
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Get time and date the last data was received
        /// </summary>
        DateTime LastDataReceivedUtc { get; }

        /// <summary>
        /// Gets or sets the name of the data feed
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the security tracker for security access.
        /// </summary>
        SecurityTracker Securities { get; }

        /// <summary>
        /// Gets the data subscription manager.
        /// </summary>
        DataSubscriptionManager DataSubscriptionManager { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Add a data retrieval subscription for the given security
        /// </summary>
        /// <param name="subscriptionRequest"></param>
        void AddSubscription(DataSubscriptionRequest subscriptionRequest);

        /// <summary>
        /// True if this security can be subscribed to by this data feed
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        bool CanSubscribe(TickerSymbol ticker);

        /// <summary>
        /// Get next available ticks
        /// </summary>
        /// <returns></returns>
        IEnumerable<DataFeedPacket> GetAvailableDataPackets();

        /// <summary>
        /// Get historical ticks from data feed, if possible
        /// </summary>
        /// <param name="subscriptionsRequest">Data subscriptions to request</param>
        /// <param name="start">Start date for history request</param>
        /// <param name="end">End date for history request</param>
        void GetDataHistory(DataSubscriptionRequest[] subscriptionsRequest, DateTime start, DateTime end);

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="initialmessage"></param>
        void Initialize(MessageInstance initialmessage);

        /// <summary>
        /// Sets the dependencies.
        /// </summary>
        /// <param name="securities">The securities.</param>
        /// <param name="datasubscriptionmanager">The data subscription manager.</param>
        /// <param name="datafilter"></param>
        void SetDependencies(SecurityTracker securities, DataSubscriptionManager datasubscriptionmanager,
            DataFilter datafilter);

        /// <summary>
        /// Reconnect to data feed on request
        /// </summary>
        void Reconnect();

        /// <summary>
        /// Stop retrieving data for the requested security
        /// </summary>
        /// <param name="subscriptionRequest"></param>
        void RemoveSubscription(DataSubscriptionRequest subscriptionRequest);

        /// <summary>
        /// Sets the data capture logic
        /// </summary>
        /// <param name="captureaction">The capture action.</param>
        void SetDataCapture(Action<string> captureaction);

        /// <summary>
        /// Gets the name of ticker in the feed.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <returns></returns>
        string GetFeedTicker(TickerSymbol ticker);

        /// <summary>
        /// Gets the name of the ticker at quantler.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <returns></returns>
        TickerSymbol GetQuantlerTicker(string ticker);

        #endregion Public Methods
    }
}