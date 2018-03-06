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

using MessagePack;
using MoreLinq;
using NetMQ;
using NetMQ.Sockets;
using NLog;
using Quantler.Configuration;
using Quantler.Data;
using Quantler.Interfaces;
using Quantler.Messaging;
using Quantler.Securities;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Quantler.Broker;

namespace Quantler.DataFeeds.Quantler
{
    /// <summary>
    /// Can be used to stream historical data in a currently running quant fund for debugging purposes
    /// TODO: in the flow for the bootstrap, the backfilling process should be kicked first before starting the data feed (same counts for the local feed)
    /// </summary>
    /// <seealso cref="BaseFeed" />
    [Export(typeof(DataFeed))]
    public class HistoricalRemoteFeed : BaseFeed, DataFeed
    {
        #region Private Fields

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();
        private string[] _allowedTickers;
        private DateTime _endDateTime = DateTime.MinValue;
        private string _endpoint;
        private RequestSocket _histRequestSocket;
        private NetMQPoller _poller;
        private DateTime _startDateTime = DateTime.MinValue;
        private List<DataSubscriptionRequest> _subscribed = new List<DataSubscriptionRequest>();
        private NetMQTimer _timeoutTimer;
        private TimeSpan _timeoutTimerSpan = TimeSpan.FromSeconds(15);
        private string _source;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Check if we are connected
        /// </summary>
        public bool IsConnected => _poller.IsRunning;

        /// <summary>
        /// True if this task is running
        /// </summary>
        public bool IsRunning => IsConnected;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Add subscription to socket
        /// </summary>
        /// <param name="subscriptionRequest"></param>
        public void AddSubscription(DataSubscriptionRequest subscriptionRequest)
        {
            //Check for all subscription
            if (!_subscribed.Select(x => x.GetSubscriptionName()).Contains(subscriptionRequest.GetSubscriptionName()))
                _subscribed.Add(subscriptionRequest);

            //Logging
            _log.Info("Subscribed to subscription " + subscriptionRequest.GetSubscriptionName());
        }

        /// <summary>
        /// Check if we can subscribe to this ticker
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        public bool CanSubscribe(TickerSymbol ticker) =>
            _allowedTickers.Contains(ticker.Name);

        /// <summary>
        /// Get historical ticks from data feed, if possible.
        /// TODO: implement feed and test
        /// </summary>
        /// <param name="subscriptionsRequest">Data subscriptions to request</param>
        /// <param name="start">Start date for history request</param>
        /// <param name="end">End date for history request</param>
        /// <returns></returns>
        public override void GetDataHistory(DataSubscriptionRequest[] subscriptionsRequest, DateTime start, DateTime end)
        {
            //Prepare request and process response
            using (var requestSocket = new RequestSocket(_endpoint))
            {
                //Set response
                requestSocket.ReceiveReady += OnReceive;

                //Send request
                var msg = CreateHistoryRequest(subscriptionsRequest, start, end, true);
                requestSocket.SendMultipartMessage(msg);
            }
        }

        /// <summary>
        /// Gets the name of ticker in the feed.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <returns></returns>
        public override string GetFeedTicker(TickerSymbol ticker) =>
            ticker.Name + "." + ticker.Currency + "." + _source;

        /// <summary>
        /// Gets the name of the ticker at quantler.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <returns></returns>
        public override TickerSymbol GetQuantlerTicker(string ticker)
        {
            try
            {
                var splitted = ticker.Split('.');
                return new TickerSymbol(splitted[0] + splitted[1], splitted[0], (CurrencyType)Enum.Parse(typeof(CurrencyType), splitted[2]));
            }
            catch (Exception exc)
            {
                _log.Error(exc, $"Could not get quantler ticker name for ticker: {ticker}");
            }

            //If all fails
            return TickerSymbol.NIL(ticker);
        }

        /// <summary>
        /// Initialize new internal live data feed
        /// </summary>
        public void Initialize(MessageInstance initialmessage)
        {
            //Set data
            _endpoint = Config.TryGetEnvVariable("hist.endpoint", Config.GlobalConfig.HistEndpoint);
            _startDateTime = DateTime.Parse(Config.TryGetEnvVariable("hist.start", Config.GlobalConfig.HistStart));
            _endDateTime = DateTime.Parse(Config.TryGetEnvVariable("hist.end", Config.GlobalConfig.HistEnd));

            //Set conversion method
            Conversion = (data) => new[] { DataPointImpl.Deserialize(data as byte[], true) };

            //Set socket
            _histRequestSocket = new RequestSocket(_endpoint);
            _histRequestSocket.ReceiveReady += OnReceive;
            _histRequestSocket.Options.ReceiveHighWatermark = 1000;

            //Set poller
            _timeoutTimer = new NetMQTimer(_timeoutTimerSpan);
            _timeoutTimer.Elapsed += OnTimeoutTimer;
            _timeoutTimer.Enable = false;
            _poller = new NetMQPoller();
            _poller.Add(_histRequestSocket);

            //Set allowed tickers
            _allowedTickers = Config.SecurityConfig.Select(x => x.Ticker).ToArray();

            //Set source
            string brokertype = (initialmessage is LiveTradingMessage liveTradingMessage)
                ? liveTradingMessage.BrokerType
                : (initialmessage is SimulationMessage backtestmessage)
                    ? backtestmessage.BrokerType
                    : "";
            switch ((BrokerType)Enum.Parse(typeof(BrokerType), brokertype))
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
                    throw new ArgumentOutOfRangeException($"{brokertype} is unknown, cannot derive data source from an unknown broker type");
            }
        }

        /// <summary>
        /// Reconnect to feed
        /// </summary>
        public void Reconnect() => Start();

        /// <summary>
        /// Remove a subscription for a data feed
        /// </summary>
        /// <param name="subscriptionRequest"></param>
        public void RemoveSubscription(DataSubscriptionRequest subscriptionRequest)
        {
        }

        /// <summary>
        /// Start internal feed
        /// </summary>
        public void Start()
        {
            if (!IsConnected)
                _poller.RunAsync();

            //Send request for data
            var histrequest = CreateHistoryRequest(_subscribed.ToArray(), _startDateTime, _endDateTime, false);
            _histRequestSocket.SendMultipartMessage(histrequest);
        }

        /// <summary>
        /// Stop internal feed
        /// </summary>
        public void Stop()
        {
            if (IsConnected)
                _poller.Stop();
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Creates the history request.
        /// </summary>
        /// <param name="subscriptionsRequest">The subscriptions.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="isbackfilling">if set to <c>true</c> [is backfilling].</param>
        /// <returns></returns>
        internal static NetMQMessage CreateHistoryRequest(DataSubscriptionRequest[] subscriptionsRequest, DateTime start, DateTime end, bool isbackfilling)
        {
            var toreturn = new NetMQMessage();
            toreturn.Append(isbackfilling ? Topic.BackfillingMessage : Topic.HistoryMessage);
            toreturn.Append(start.ToUnixTime().ToString());
            toreturn.Append(end.ToUnixTime().ToString());
            subscriptionsRequest.ForEach(x => toreturn.Append(x.Serialize()));
            return toreturn;
        }

        #endregion Internal Methods

        #region Private Methods

        /// <summary>
        /// On date received handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReceive(object sender, NetMQSocketEventArgs e)
        {
            //Go trough all received items
            while (e.Socket.TryReceiveFrameString(out string topic))
            {
                if (topic == Topic.HistoryMessage || topic == Topic.BackfillingMessage)
                {
                    //Get subscription
                    string subscription = e.Socket.ReceiveFrameString();

                    //Check if we know this data type
                    ProcessData(e.Socket.ReceiveFrameBytes());

                    //Timer reset (if we are expecting data)
                    if (LastDataReceivedUtc.Date < _endDateTime.AddDays(-1))
                        _timeoutTimer.EnableAndReset();
                    else
                        _timeoutTimer.Enable = false;
                }
                else //Unknown topic
                {
                }
            }
        }

        /// <summary>
        /// Timeout timer for no data received notification
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimeoutTimer(object sender, NetMQTimerEventArgs e) =>
            _log.Error($"No data received due to HB timeout of {(int)_timeoutTimerSpan.TotalSeconds} seconds occurred, this means we are no longer connected to the data feed. Waiting for a reconnect...");

        /// <summary>
        /// Processes the data.
        /// </summary>
        /// <param name="data">The data.</param>
        private void ProcessData(byte[] data)
        {
            //Add to queue
            if (TryConvert(data, out DataPoint[] datapoint))
                datapoint.ForEach(x => CurrenDatapoints.Enqueue(x));

            //For data capture
            if (DataCapture != null)
                datapoint.ForEach(x => DataCapture?.Invoke(MessagePackSerializer.ToJson(x)));
        }

        #endregion Private Methods
    }
}