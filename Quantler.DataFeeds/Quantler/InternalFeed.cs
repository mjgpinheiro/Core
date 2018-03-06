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
using Quantler.Broker;
using Quantler.Configuration;
using Quantler.Data;
using Quantler.Interfaces;
using Quantler.Messaging;
using Quantler.Securities;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

namespace Quantler.DataFeeds.Quantler
{
    /// <summary>
    /// Internal feed for quotes
    /// TODO: implement Backfilling process
    /// </summary>
    [Export(typeof(DataFeed))]
    public class InternalFeed : BaseFeed, DataFeed
    {
        #region Private Fields

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();
        private string[] _allowedTickers;
        private string _endpoint;
        private NetMQPoller _poller;
        private string _source;
        private List<string> _subscribed = new List<string>();
        private SubscriberSocket _subscriberSocket;
        private NetMQTimer _timeoutTimer;
        private TimeSpan _timeoutTimerSpan = TimeSpan.FromSeconds(15);

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
            string subscriptionname = subscriptionRequest.GetSubscriptionName();
            if (subscriptionRequest.Ticker == FireHoseTicker)
                _subscriberSocket.Subscribe("");     //Firehose
            else if (!_subscribed.Contains(subscriptionname) && CanSubscribe(subscriptionRequest.Ticker))
            {
                //Add to known subscription
                _subscribed.Add(subscriptionname);

                //Add subscription
                _subscriberSocket.Subscribe(subscriptionname);
            }

            //Logging
            _log.Info("Subscribed to subscription " + subscriptionname);
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
        /// TODO: this request is blocking until finished, please take into account while backfilling
        /// </summary>
        /// <param name="subscriptionsRequest">Data subscriptions to request</param>
        /// <param name="start">Start date for history request</param>
        /// <param name="end">End date for history request</param>
        /// <returns></returns>
        public override void GetDataHistory(DataSubscriptionRequest[] subscriptionsRequest, DateTime start, DateTime end)
        {
            //Prepare request and process response
            string histendpoint = Config.TryGetEnvVariable("hist.endpoint", Config.GlobalConfig.HistEndpoint);
            using (var requestSocket = new RequestSocket(histendpoint))
            {
                //Set response
                requestSocket.ReceiveReady += OnReceive;

                //Send request
                var request = HistoricalRemoteFeed.CreateHistoryRequest(subscriptionsRequest, start, end, true);
                requestSocket.SendMultipartMessage(request);
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
        /// Initializes this instance.
        /// </summary>
        public void Initialize(MessageInstance initialmessage)
        {
            //Set data
            _endpoint = Config.TryGetEnvVariable("datafeedendpoint", Config.GlobalConfig.DataFeedEndpoint);

            //Set conversion method
            Conversion = data => new[] { DataPointImpl.Deserialize(data as byte[], true) };;

            //Set socket
            _subscriberSocket = new SubscriberSocket(_endpoint);
            _subscriberSocket.ReceiveReady += OnReceive;
            _subscriberSocket.Options.ReceiveHighWatermark = 1000;
            _subscriberSocket.Subscribe(Topic.HeartBeatMessage);
            _subscriberSocket.Subscribe(Topic.WelcomeMessage);

            //Set poller
            _timeoutTimer = new NetMQTimer(_timeoutTimerSpan);
            _timeoutTimer.Elapsed += OnTimeoutTimer;
            _timeoutTimer.Enable = false;
            _poller = new NetMQPoller();
            _poller.Add(_subscriberSocket);

            //Set allowed tickers
            _allowedTickers = Config.SecurityConfig.Select(x => x.Ticker).ToArray();

            //Set allowed currency conversion tickers
            //TODO: set allowed currency conversion tickers (all combinations possible)

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
            string subscriptionname = subscriptionRequest.GetSubscriptionName();
            _log.Info("Unsubscribing to subscription " + subscriptionname);
            if (_subscribed.Contains(subscriptionname))
            {
                _subscribed.Remove(subscriptionname);
                _subscriberSocket.Unsubscribe(subscriptionname);
            }
        }

        /// <summary>
        /// Start internal feed
        /// </summary>
        public void Start()
        {
            if (!IsConnected)
                _poller.RunAsync();
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

        #region Private Methods

        /// <summary>
        /// On data received handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReceive(object sender, NetMQSocketEventArgs e)
        {
            //Go trough all received items
            while (e.Socket.TryReceiveFrameString(out string topic))
            {
                if (topic == Topic.WelcomeMessage)
                {
                    //Welcome message
                    _log.Debug("Received welcome message");
                }
                else if (topic == Topic.HeartBeatMessage)
                {
                    //Heartbeat (reset timeout)
                    _timeoutTimer.EnableAndReset();
                }
                else if (topic == Topic.DataMessage)
                {
                    //Get subscription
                    string subscription = e.Socket.ReceiveFrameString();

                    //Process data
                    ProcessData(e.Socket.ReceiveFrameBytes());
                }
                else //Unknown topic
                {
                    //TODO: keep reading until we get the next topic, so we can skip this message from the feed
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