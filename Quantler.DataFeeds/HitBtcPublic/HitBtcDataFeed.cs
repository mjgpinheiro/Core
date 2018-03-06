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
using Polly;
using Quantler.Api.HitBtc;
using Quantler.Common;
using Quantler.Data;
using Quantler.Data.Market;
using Quantler.DataFeeds.HitBtcPublic.Models.v1;
using Quantler.DataFeeds.HitBtcPublic.Models.v2;
using Quantler.Interfaces;
using Quantler.Messaging;
using Quantler.Securities;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using WebSocket4Net;

namespace Quantler.DataFeeds.HitBtcPublic
{
    /// <summary>
    /// HitBTC data feed
    /// TODO: add netmqtimer and disconnect check
    /// </summary>
    /// <seealso cref="BaseFeed" />
    /// <seealso cref="DataFeed" />
    [Export(typeof(DataFeed))]
    public class HitBtcDataFeed : BaseFeed, DataFeed
    {
        #region Private Fields

        /// <summary>
        /// The hit BTC API
        /// </summary>
        private readonly HitBtcApi _hitBtcApi = new HitBtcApi();

        /// <summary>
        /// Current logging instance
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The current order book
        /// </summary>
        private readonly Dictionary<string, OrderBook> _orderbook = new Dictionary<string, OrderBook>();

        /// <summary>
        /// The seqno memory, to keep track of seq no received
        /// </summary>
        private readonly FixedSizedQueue<string> _seqnomemory = new FixedSizedQueue<string>(5000);

        /// <summary>
        /// The known symbols
        /// </summary>
        private readonly List<string> _availableTickers = new List<string>();

        /// <summary>
        /// Known currency names for conversion
        /// </summary>
        private string[] _currencyNames;

        /// <summary>
        /// The data feed version (for backwards compatibility)
        /// </summary>
        private readonly int _datafeedversion = 1;

        /// <summary>
        /// The endpoint
        /// </summary>
        private readonly string _endpoint = "wss://api.hitbtc.com/api/2/ws";

        /// <summary>
        /// Check if we are reconnecting
        /// </summary>
        private bool _isReconnecting;

        /// <summary>
        /// The last method id
        /// </summary>
        private int _lastmethodid = 1;

        /// <summary>
        /// The last ticker update
        /// </summary>
        private DateTime _lastTickerUpdate = DateTime.MinValue;

        /// <summary>
        /// The locker object
        /// </summary>
        private readonly object _locker = new object();

        /// <summary>
        /// Current socket
        /// </summary>
        private WebSocket _socket;

        /// <summary>
        /// The subscribed tickers
        /// </summary>
        private List<string> _subscribedtickers = new List<string>();

        /// <summary>
        /// Currently active symbol requests
        /// </summary>
        private readonly List<SocketMethodModel<SymbolRequestModel>> _symbolRequests = new List<SocketMethodModel<SymbolRequestModel>>();

        /// <summary>
        /// The ticker symbols cache
        /// </summary>
        private readonly Dictionary<string, TickerSymbol> _tickerSymbols = new Dictionary<string, TickerSymbol>();

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// True if this data feed is in a connected state
        /// </summary>
        public bool IsConnected => _socket != null && _socket.State == WebSocketState.Open;

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

            //Not needed as data feed will retrieve all data already
            UpdateTickers();
            if (subscriptionRequest.Ticker != FireHoseTicker)
                SubscribeTicker(GetFeedTicker(subscriptionRequest.Ticker));
            //Check for all tickers
            else
            {
                IsFirehose = true;
                _availableTickers.ForEach(x => SubscribeTicker(x));
            }
        }

        /// <summary>
        /// True if this security can be subscribed to by this data feed
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        public bool CanSubscribe(TickerSymbol ticker)
        {
            //Get updates to ticker symbols
            UpdateTickers();

            //Return check
            return _availableTickers.Contains(GetFeedTicker(ticker));
        }

        /// <summary>
        /// Gets the name of ticker in the feed.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <returns></returns>
        public override string GetFeedTicker(TickerSymbol ticker) =>
            ticker.Currency + ticker.Commodity;

        /// <summary>
        /// Gets the name of the ticker at quantler.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <returns></returns>
        public override TickerSymbol GetQuantlerTicker(string ticker)
        {
            try
            {
                //Check if we already have this ticker
                if (_tickerSymbols.ContainsKey(ticker))
                    return _tickerSymbols[ticker];

                //Get currency names
                string commodity = "";
                string basecurrency = "";
                TickerSymbol created = TickerSymbol.NIL(ticker);

                var found = _currencyNames.FirstOrDefault(ticker.EndsWith);
                if(found != null)
                {
                    commodity = $"{ticker.Replace(found, "")}";
                    basecurrency = found;
                    created = new TickerSymbol(commodity + ".BC", commodity,
                        (CurrencyType) Enum.Parse(typeof(CurrencyType), basecurrency));
                }

                //Return what we know
                _tickerSymbols.Add(ticker, created);
                return _tickerSymbols[ticker];
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
            //Set conversion method
            Conversion = data => ParseData(data as string, DateTime.UtcNow);

            //Set Defaults
            Name = "HitBtc";
            DataSource = DataSource.HitBtc;

            //Set currency names
            _currencyNames = Enum.GetNames(typeof(CurrencyType));
        }

        /// <summary>
        /// Conversion method for received data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public DataPointImpl[] ParseData(string data, DateTime occured)
        {
            //Check if this is legacy data
            if (_datafeedversion == 1)
                return ParseDataV1(data);

            //Parse data
            SocketMethodModel<OrderBookParamsModel> orderbooksnapshot = null;
            SocketMethodModel<OrderBookParamsModel> orderbookupdate = null;
            SocketMethodModel<TradesParamsModel> tradeupdate = null;
            ChannelUpdateModel channelupdate = null;
            if (data.Contains("updateOrderbook"))
                orderbookupdate = JSON.Deserialize<SocketMethodModel<OrderBookParamsModel>>(data);
            else if (data.Contains("snapshotOrderbook"))
                orderbooksnapshot = JSON.Deserialize<SocketMethodModel<OrderBookParamsModel>>(data);
            else if (data.Contains("updateTrades"))
                tradeupdate = JSON.Deserialize<SocketMethodModel<TradesParamsModel>>(data);
            else if (data.Contains("channel"))
                return new DataPointImpl[0];
            else if (data.Contains("result"))
            {
                //Process method request
                var request = JSON.Deserialize<SocketMethodModel<SymbolRequestModel>>(data);
                if (request.Result)
                    _symbolRequests.RemoveAll(x => x.Id == request.Id);
                else
                {
                    var found = _symbolRequests.FirstOrDefault(x => x.Id == request.Id);
                    if (found != null)
                    {
                        _log.Debug($"Request for HitBTC failed, retrying. Request body: {JSON.Serialize(request)}");
                        SendRequest(found);
                    }
                }

                //Return nothing
                return new DataPointImpl[0];
            }
            else
                return new DataPointImpl[0];

            //Set return object
            var toreturn = new List<DataPointImpl>();

            //Check orderbook snapshot
            if (orderbooksnapshot != null)
            {
                //Reset orderbook and recreate
                var snapshot = orderbooksnapshot.Params;

                //Check sequence number
                var fullseqno = snapshot.Sequence + snapshot.Symbol + "snapshot";
                if (_seqnomemory.Contains(fullseqno))
                    return toreturn.ToArray();
                else
                    _seqnomemory.Enqueue(fullseqno);

                //Check order book and data
                if (!_orderbook.TryGetValue(snapshot.Symbol, out var fullOrderBook))
                {
                    fullOrderBook = new OrderBook(snapshot.Symbol);
                    _orderbook[snapshot.Symbol] = fullOrderBook;
                }

                //Reset order book
                fullOrderBook.Clear();
                snapshot.Bid.ForEach(b => fullOrderBook.AddQuote(true, b.DoublePrice, b.DoubleSize));
                snapshot.Ask.ForEach(a => fullOrderBook.AddQuote(false, a.DoublePrice, a.DoubleSize));
            }

            //Check orderbook update
            if (orderbookupdate != null)
            {
                var seqno = orderbookupdate.Params.Symbol + orderbookupdate.Params.Sequence;
                if (_seqnomemory.Contains(seqno))
                    return null;
                else
                    _seqnomemory.Enqueue(seqno);

                //Check order book and data
                if (!_orderbook.TryGetValue(orderbookupdate.Params.Symbol, out var orderbook))
                {
                    orderbook = new OrderBook(orderbookupdate.Params.Symbol);
                    _orderbook[orderbookupdate.Params.Symbol] = orderbook;
                }

                //Check for quote updates in the order book (size == 0 = remove from order book)
                bool update = orderbookupdate.Params.Ask.Count(x => orderbook.AddQuote(false, x.DoublePrice, x.DoubleSize)) > 0;
                update = orderbookupdate.Params.Bid.Count(x => orderbook.AddQuote(true, x.DoublePrice, x.DoubleSize)) > 0 | update;

                //Check if we need to update level 0 quote
                if (update)
                    //Deserialize tick
                    toreturn.Add(new Tick(GetQuantlerTicker(orderbookupdate.Params.Symbol), DataSource)
                    {
                        AskPrice = Convert.ToDecimal(orderbook.BestAsk),
                        BidPrice = Convert.ToDecimal(orderbook.BestBid),
                        AskSize = Convert.ToDecimal(orderbook.AskSize),
                        BidSize = Convert.ToDecimal(orderbook.BidSize),
                        Depth = 0,
                        Occured = occured,
                        TimeZone = TimeZone.Utc
                    });
            }

            //Check for trade updates
            if (tradeupdate != null)
            {
                tradeupdate.Params.Data.ForEach(trade =>
                {
                    toreturn.Add(new Tick(GetQuantlerTicker(tradeupdate.Params.Symbol), DataSource)
                    {
                        Occured = occured,
                        TimeZone = TimeZone.Utc,
                        TradePrice = Convert.ToDecimal(trade.Price),
                        Size = Convert.ToDecimal(trade.Quantity) * (trade.Side == "sell" ? -1 : 1)
                    });
                });
            }

            //Return what we have
            return toreturn.ToArray();
        }

        /// <summary>
        /// Reconnect to data feed on request
        /// </summary>
        public void Reconnect()
        {
            //Check if we are already trying to reconnect
            lock (_locker)
            {
                if (!_isReconnecting)
                    _isReconnecting = true;
                else
                    return;
            }

            //Stop connection and retry connecting indefinitely
            Policy.Handle<Exception>()
                    .WaitAndRetryForever((retryAttempt, context) =>
                    {
                        //Get timespan for next retry
                        var ts = TimeSpan.FromSeconds(5);

                        //Logging
                        _log.Warn($"Could not reconnect hitbtc feed due to exception. Reconnecting in {ts.TotalSeconds} seconds (attempt {retryAttempt})");

                        //return timer
                        return ts;
                    })
                    .Execute(() =>
                    {
                        //Check for connection
                        if (IsConnected)
                            return;

                        //Set socket
                        if (_socket != null)
                        {
                            _socket.Close();
                            _socket.Dispose();
                        }
                        _socket = new WebSocket(_endpoint);

                        //Set events
                        _socket.MessageReceived += (sender, e) => OnData(e.Message);
                        _socket.Opened += (sender, e) => _log.Info("Connection opened");
                        _socket.Error += (sender, e) => _log.Error(e.Exception, $"Error: {e.Exception.Message}");
                        _socket.Closed += (sender, e) =>
                        {
                            _log.Info("Connection closed, reconnecting");
                            _subscribedtickers = new List<string>();
                            _lastTickerUpdate = DateTime.MinValue;
                            Reconnect();
                        };

                        //Open connection
                        _socket.Open();

                        //Wait
                        Thread.Sleep(5000);
                        if (!IsConnected)
                            throw new Exception("Not connected...");
                        else
                            _subscribedtickers.ForEach(x => SubscribeTicker(x,true)); //Add subscribed tickers
                    });

            //No longer reconnecting
            _isReconnecting = false;
        }

        /// <summary>
        /// Stop retrieving data for the requested security
        /// </summary>
        /// <param name="subscriptionRequest"></param>
        public void RemoveSubscription(DataSubscriptionRequest subscriptionRequest)
        {
            //Check if we are fire hose, than skip
            if (IsFirehose)
                return;

            //Check and request for removal
            string feedname = GetFeedTicker(subscriptionRequest.Ticker);
            if (_subscribedtickers.Contains(feedname))
                UnsubscribeTicker(feedname);
        }

        /// <summary>
        /// Start task
        /// </summary>
        public void Start()
        {
            //Check if we are connected
            if (!IsConnected)
                Reconnect();

            //Set current state
            IsRunning = true;
        }

        /// <summary>
        /// Stop task
        /// </summary>
        public void Stop()
        {
            //Stop socket
            _socket.Close();
            _socket.Dispose();
            _socket = null;

            //Set current state
            IsRunning = false;
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
        /// Parses the data using the v1 API implementation.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private DataPointImpl[] ParseDataV1(string data)
        {
            //Get json
            MarketDataModel message = JSON.Deserialize<MarketDataModel>(data, Options.ISO8601);

            //Set return object
            var toreturn = new List<DataPointImpl>();

            //Check if this is a snapshot
            if (message.MarketDataSnapshotFullRefresh != null)
            {
                //Get data
                var refresh = message.MarketDataSnapshotFullRefresh;

                //Check sequence number
                var fullseqno = refresh.snapshotSeqNo + refresh.symbol + "snapshot";
                if (_seqnomemory.Contains(fullseqno))
                    return toreturn.ToArray();
                else
                    _seqnomemory.Enqueue(fullseqno);

                //Check order book and data
                if (!_orderbook.TryGetValue(refresh.symbol, out var fullOrderBook))
                {
                    fullOrderBook = new OrderBook(refresh.symbol);
                    _orderbook[refresh.symbol] = fullOrderBook;
                }

                //Reset order book
                fullOrderBook.Clear();
                refresh.bid.ForEach(b => fullOrderBook.AddQuote(true, b.DoublePrice, b.size));
                refresh.ask.ForEach(a => fullOrderBook.AddQuote(false, a.DoublePrice, a.size));

                //No updates
                return toreturn.ToArray();
            }

            //Parse data
            var parsed = message.MarketDataIncrementalRefresh;
            DateTime occured = Time.FromUnixTime(parsed.timestamp, true);

            //Check seqno
            var seqno = parsed.seqNo + parsed.symbol;
            if (_seqnomemory.Contains(seqno))
                return toreturn.ToArray();
            else
                _seqnomemory.Enqueue(seqno);

            //Check order book and data
            if (!_orderbook.TryGetValue(parsed.symbol, out var orderbook))
            {
                orderbook = new OrderBook(parsed.symbol);
                _orderbook[parsed.symbol] = orderbook;
            }

            //Check for quote updates in the order book (size == 0 = remove from order book)
            bool update = parsed.ask.Count(x => orderbook.AddQuote(false, x.DoublePrice, Convert.ToDouble(x.size))) > 0;
            update = parsed.bid.Count(x => orderbook.AddQuote(true, x.DoublePrice, Convert.ToDouble(x.size))) > 0 | update;

            //Check if we need to update level 0 quote
            if (update)
                //Deserialize tick
                toreturn.Add(new Tick(GetQuantlerTicker(parsed.symbol), DataSource)
                {
                    AskPrice = Convert.ToDecimal(orderbook.BestAsk),
                    BidPrice = Convert.ToDecimal(orderbook.BestBid),
                    AskSize = Convert.ToDecimal(orderbook.AskSize),
                    BidSize = Convert.ToDecimal(orderbook.BidSize),
                    Depth = 0,
                    Occured = occured,
                    TimeZone = TimeZone.Utc
                });

            //Check if we have trade data
            message.MarketDataIncrementalRefresh.trade.ForEach(trade =>
            {
                toreturn.Add(new Tick(GetQuantlerTicker(parsed.symbol), DataSource)
                {
                    Occured = occured,
                    TimeZone = TimeZone.Utc,
                    TradePrice = Convert.ToDecimal(trade.price),
                    Size = Convert.ToDecimal(trade.size)
                });
            });

            //Return what we have
            return toreturn.ToArray();
        }

        /// <summary>
        /// Sends the request.
        /// </summary>
        /// <param name="request">The request.</param>
        private void SendRequest(SocketMethodModel<SymbolRequestModel> request)
        {
            //Register request
            if (_symbolRequests.Count(x => x.Id == request.Id) == 0)
                _symbolRequests.Add(request);

            //Send request
            _socket.Send(JSON.Serialize(request));
            Thread.Sleep(25); //We cannot perform too many request after each other
        }

        /// <summary>
        /// Subscribes the ticker with the feed.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <param name="force"></param>
        private void SubscribeTicker(string ticker, bool force = false)
        {
            //Check if we already subscribe to this ticker
            if (_subscribedtickers.Contains(ticker) && !force)
                return;
            if (force)
                _subscribedtickers.Remove(ticker);

            //Add to subscribed tickers
            _subscribedtickers.Add(ticker);
            Console.WriteLine($"Subscribing to: {ticker}");

            //Create request
            var socketmethod = new SocketMethodModel<SymbolRequestModel>
            {
                Id = _lastmethodid++,
                Method = "subscribeTicker",
                Params = new SymbolRequestModel
                {
                    symbol = ticker
                }
            };

            //Send subcribe to ticker
            SendRequest(socketmethod);

            //Send subscribe to orderbook
            socketmethod.Method = "subscribeOrderbook";
            socketmethod.Id = _lastmethodid++;
            SendRequest(socketmethod);

            //Send subscribe to trade information
            socketmethod.Method = "subscribeTrades";
            socketmethod.Id = _lastmethodid++;
            SendRequest(socketmethod);
        }

        /// <summary>
        /// Subscribes the ticker with the feed.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        private void UnsubscribeTicker(string ticker)
        {
            //Check if we already subscribe to this ticker
            if (!_subscribedtickers.Contains(ticker))
                return;

            //Remove subscribed ticker
            _subscribedtickers.Remove(ticker);
            Console.WriteLine($"Unsubscribing to: {ticker}");

            //Create request
            var socketmethod = new SocketMethodModel<SymbolRequestModel>
            {
                Id = _lastmethodid++,
                Method = "unsubscribeTicker",
                Params = new SymbolRequestModel
                {
                    symbol = ticker
                }
            };

            //Send subcribe to ticker
            SendRequest(socketmethod);

            //Send subscribe to orderbook
            socketmethod.Method = "unsubscribeOrderbook";
            socketmethod.Id = _lastmethodid++;
            SendRequest(socketmethod);

            //Send subscribe to trade information
            socketmethod.Method = "unsubscribeTrades";
            socketmethod.Id = _lastmethodid++;
            SendRequest(socketmethod);
        }

        /// <summary>
        /// Updates the tickers currently available
        /// </summary>
        private void UpdateTickers()
        {
            //Check for next update
            if ((DateTime.UtcNow - _lastTickerUpdate) < TimeSpan.FromHours(1))
                return;
            else
                _lastTickerUpdate = DateTime.UtcNow;

            //Get from public API
            var result = _hitBtcApi.GetSymbolsAsync().Result;
            result.ForEach(x =>
            {
                if (!_availableTickers.Contains(x.Id))
                    //Add to known tickers
                    _availableTickers.Add(x.Id);
            });
        }

        #endregion Private Methods
    }
}