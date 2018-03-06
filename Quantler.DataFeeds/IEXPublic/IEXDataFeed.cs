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

using Jil;
using MoreLinq;
using NLog;
using Quantler.Api.IEX;
using Quantler.Data;
using Quantler.Data.Market;
using Quantler.DataFeeds.IEXPublic.Models;
using Quantler.Interfaces;
using Quantler.Messaging;
using Quantler.Securities;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections.Generic;
using System.Composition;

namespace Quantler.DataFeeds.IEXPublic
{
    /// <summary>
    /// IEXchange data feed implementation
    /// TODO: implement netmq poller and monitor
    /// </summary>
    [Export(typeof(DataFeed))]
    public class IEXDataFeed : BaseFeed, DataFeed
    {
        #region Private Fields

        /// <summary>
        /// The iex API
        /// </summary>
        private readonly IEXApi _iexApi = new IEXApi();

        /// <summary>
        /// Current logging instance
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The known symbols
        /// </summary>
        private readonly List<string> _availableTickers = new List<string>();

        /// <summary>
        /// The endpoint
        /// </summary>
        private string _endpoint = "https://ws-api.iextrading.com/1.0/tops";

        /// <summary>
        /// The socket IO connection manager
        /// </summary>
        private Manager _manager;

        /// <summary>
        /// The socket
        /// </summary>
        private Socket _socket;

        /// <summary>
        /// The subscribed symbols
        /// </summary>
        private List<string> _subscribedSymbols = new List<string>();

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// True if this data feed is in a connected state
        /// </summary>
        public bool IsConnected => _manager?.ReadyState == Manager.ReadyStateEnum.OPEN;

        /// <summary>
        /// True if this task is running
        /// </summary>
        public bool IsRunning
        {
            private set;
            get;
        }

        #endregion Public Properties

        #region Private Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is fire hose.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is fire hose; otherwise, <c>false</c>.
        /// </value>
        private bool IsFirehose { get; set; }

        /// <summary>
        /// Gets or sets the last known ticker update.
        /// </summary>
        private DateTime LastTickerUpdate { get; set; }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Add a data retrieval subscription for the given security
        /// </summary>
        /// <param name="subscriptionRequest"></param>
        public void AddSubscription(DataSubscriptionRequest subscriptionRequest)
        {
            //Check for current subscription capabilities
            if (subscriptionRequest.DataSource != DataSource)
                throw new Exception($"Cannot subscribe to data source {subscriptionRequest.DataSource} when using {DataSource}");

            //Check ticker type and subscribe
            if (subscriptionRequest.Ticker == FireHoseTicker)
            {
                _socket.Emit("subscribe", "firehose");
                IsFirehose = true;
            }
            else if (CanSubscribe(subscriptionRequest.Ticker) && !IsFirehose && !_subscribedSymbols.Contains(subscriptionRequest.Ticker.Name))
            {
                _socket.Emit("subscribe", subscriptionRequest.Ticker);
                _subscribedSymbols.Add(subscriptionRequest.Ticker.Name);
            }
        }

        /// <summary>
        /// True if this security can be subscribed to by this data feed
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        public bool CanSubscribe(TickerSymbol ticker)
        {
            //Check for updates in ticker symbols
            UpdateTickers();

            //Check if we know this symbol
            return _availableTickers.Contains(GetFeedTicker(ticker));
        }

        /// <summary>
        /// Gets the name of ticker in the feed.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <returns></returns>
        public override string GetFeedTicker(TickerSymbol ticker) =>
            ticker.Name.Replace(".US", "");

        /// <summary>
        /// Gets the name of the ticker at quantler.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <returns></returns>
        public override TickerSymbol GetQuantlerTicker(string ticker) =>
            new TickerSymbol(ticker + ".US", ticker, CurrencyType.USD);

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize(MessageInstance initialmessage)
        {
            //Set conversion method
            Conversion = (data) => new[] { Parse(data as string) };

            //Get symbols
            UpdateTickers();

            //Set Defaults
            Name = "IEX";
            DataSource = DataSource.IEX;
        }

        /// <summary>
        /// Conversion method for received data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public DataPointImpl Parse(string data)
        {
            //Get json
            TickModel message = JSON.Deserialize<TickModel>(data);
            if (message.lastUpdated < 0)
                return null;

            DateTime occuredutc = Time.FromUnixTime(message.lastUpdated, true);
            DateTime occured = occuredutc.ConvertTo(TimeZone.Utc, TimeZone.NewYork);

            //Deserialize tick
            Tick tickdata = new Tick(GetQuantlerTicker(message.symbol), DataSource)
            {
                AskPrice = message.askPrice,
                BidPrice = message.bidPrice,
                AskSize = message.askSize,
                BidSize = message.bidSize,
                Depth = 0,
                Occured = occured,
                TimeZone = TimeZone.NewYork,
                Size = message.lastSaleSize,
                TradePrice = message.lastSalePrice
            };

            return tickdata;
        }

        /// <summary>
        /// Reconnect to data feed on request
        /// </summary>
        public void Reconnect()
        {
            try
            {
                //Reset connection
                _socket = IO.Socket(_endpoint,
                    new IO.Options
                    {
                        AutoConnect = true,
                        Reconnection = true,
                        ReconnectionAttempts = 5000,
                        ReconnectionDelay = 20,
                        ForceNew = true,
                        Timeout = 20000
                    });

                //Set connection event
                _socket.On(Socket.EVENT_CONNECT, () =>
                {
                    _log.Trace($"Connected to IEX live data using endpoint {_endpoint}");

                    //Check for updates in ticker symbols
                    UpdateTickers();

                    //Check current symbols
                    if (IsFirehose)
                        _socket.Emit("subscribe", "firehose");
                    else if (_subscribedSymbols.Count > 0)
                        _socket.Emit("subscribe", string.Join(",", _subscribedSymbols));
                });
                _socket.On(Socket.EVENT_DISCONNECT, (data) =>
                {
                    _log.Info($"Connection to {_endpoint} was closed (disconnect), see message {data}");
                });
                _socket.On(Socket.EVENT_ERROR, (data) =>
                {
                    _log.Info($"Connection to {_endpoint} contained error, see message {data}");
                });
                _socket.On(Socket.EVENT_RECONNECT_FAILED, (data) =>
                {
                    _log.Info($"Connection to {_endpoint} reconnection failed (EVENT_RECONNECT_FAILED), see message {data}");
                });
                _socket.On(Socket.EVENT_RECONNECT_ERROR, (data) =>
                {
                    _log.Info($"Reconnect to {_endpoint} error (EVENT_RECONNECT_ERROR), see message {data}");
                });

                //Set as retrieve data
                _socket.On("message", message => OnData(message as string));
                _manager = _socket.Io();
            }
            catch (Exception exc)
            {
                _log.Error(exc, exc.Message);
            }
        }

        /// <summary>
        /// Stop retrieving data for the requested security
        /// </summary>
        /// <param name="subscriptionRequest"></param>
        public void RemoveSubscription(DataSubscriptionRequest subscriptionRequest)
        {
            var ticker = subscriptionRequest.Ticker;
            if (CanSubscribe(ticker))
            {
                _socket.Emit("unsubscribe", GetFeedTicker(ticker));
                _subscribedSymbols.Remove(ticker.Name);
            }
        }

        /// <summary>
        /// Start task
        /// </summary>
        public void Start()
        {
            if (!IsConnected)
                Reconnect();
        }

        /// <summary>
        /// Stop task
        /// </summary>
        public void Stop()
        {
            _socket.Disconnect();
            _socket.Close();
            _socket = null;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// On data event from socket
        /// </summary>
        /// <param name="data"></param>
        private void OnData(string data)
        {
            //Data capture
            DataCapture?.Invoke(data);

            //Convert and use
            if (TryConvert(data, out DataPoint[] datapoint))
                datapoint.ForEach(x => CurrenDatapoints.Enqueue(x));
        }

        /// <summary>
        /// Updates the tickers currently available
        /// </summary>
        private void UpdateTickers()
        {
            //Check for next update
            if ((DateTime.UtcNow - LastTickerUpdate) < TimeSpan.FromHours(1))
                return;
            else
                LastTickerUpdate = DateTime.UtcNow;

            //Get from public api
            var result = _iexApi.GetSymbolsAsync().Result;
            result.ForEach(x =>
            {
                if (!_availableTickers.Contains(x.Symbol))
                    //Add to known tickers
                    _availableTickers.Add(x.Symbol);
            });
        }

        #endregion Private Methods
    }
}