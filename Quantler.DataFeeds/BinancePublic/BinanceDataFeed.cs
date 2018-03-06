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

using Flurl.Http;
using MoreLinq;
using NetMQ;
using NLog;
using Polly;
using Quantler.Data;
using Quantler.DataFeeds.BinancePublic.Models;
using Quantler.Interfaces;
using Quantler.Messaging;
using Quantler.Securities;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jil;
using Quantler.Api.Binance;
using Quantler.Api.Binance.Models;
using WebSocket4Net;
using Quantler.Data.Market;

namespace Quantler.DataFeeds.BinancePublic
{
    /// <summary>
    /// Binance data feeed implementation
    /// </summary>
    /// <seealso cref="BaseFeed" />
    /// <seealso cref="DataFeed" />
    [Export(typeof(DataFeed))]
    public class BinanceDataFeed : BaseFeed, DataFeed
    {
        #region Private Fields

        /// <summary>
        /// Locker object
        /// </summary>
        private static readonly object Locker = new object();

        /// <summary>
        /// The ticker symbol changes
        /// TODO: implement this, so we can support Bitcoin Cash
        /// </summary>
        private static readonly Dictionary<string, string> _tickerSymbolChanges = new Dictionary<string, string>
        {
            { "BCC", "BCH" } //Bitcoin cash, Binance = BCC, Quantler = BCH
        };

        /// <summary>
        /// The update locker
        /// </summary>
        private static readonly object UpdateLocker = new object();

        /// <summary>
        /// The ticker symbols cache
        /// </summary>
        private readonly Dictionary<string, TickerSymbol> _tickerSymbols = new Dictionary<string, TickerSymbol>();

        /// <summary>
        /// The binance API
        /// </summary>
        private readonly BinanceApi _binanceApi = new BinanceApi();

        /// <summary>
        /// The connected depth sockets
        /// </summary>
        private readonly Dictionary<string, WebSocket> _connecteddepthsockets = new Dictionary<string, WebSocket>();

        /// <summary>
        /// The connected trade sockets
        /// </summary>
        private readonly Dictionary<string, WebSocket> _connectedtradesockets = new Dictionary<string, WebSocket>();

        /// <summary>
        /// Current logging instance
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Endpoint
        /// </summary>
        private readonly string _endpoint = "wss://stream.binance.com:9443/ws/";

        /// <summary>
        /// The current order book
        /// </summary>
        private readonly Dictionary<string, OrderBook> _orderbook = new Dictionary<string, OrderBook>();

        /// <summary>
        /// If true, we are currently reconnecting
        /// </summary>
        private bool _isReconnecting;

        /// <summary>
        /// The known tickers
        /// </summary>
        private readonly List<string> _knowntickers = new List<string>();

        /// <summary>
        /// The symbolupdate time span
        /// </summary>
        private readonly TimeSpan _symbolupdateTimeSpan = TimeSpan.FromMinutes(15);

        /// <summary>
        /// The last symbol update
        /// </summary>
        private DateTime _lasttickerupdate = DateTime.MinValue;

        /// <summary>
        /// The currently subscribed tickers
        /// </summary>
        private readonly List<string> _subscribedtickers = new List<string>();

        /// <summary>
        /// The poller instance
        /// </summary>
        private NetMQPoller _poller;

        /// <summary>
        /// Timer for lost connection checks
        /// </summary>
        private NetMQTimer _timeoutTimer;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// True if this data feed is in a connected state
        /// </summary>
        public bool IsConnected => _connecteddepthsockets.Count > 0 && _connecteddepthsockets.All(x => x.Value.State == WebSocketState.Open);

        /// <summary>
        /// True if this task is running
        /// </summary>
        public bool IsRunning { get; private set; }

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
        /// <exception cref="Exception"></exception>
        public void AddSubscription(DataSubscriptionRequest subscriptionRequest)
        {
            //Get latest list of tickers
            UpdateTickers();

            //Check for current subscription capabilities
            if (subscriptionRequest.DataSource != DataSource)
                throw new Exception($"Cannot subscribe to data source {subscriptionRequest.DataSource} when using {DataSource}");

            //Normalize ticker
            string ticker = GetFeedTicker(subscriptionRequest.Ticker);

            //Helper function
            void AddTickerSocket(string tickersymbol)
            {
                _connecteddepthsockets.Add(tickersymbol,
                    GetAndConnectSocket($"{_endpoint}{tickersymbol.ToLower()}@depth"));
                _connectedtradesockets.Add(tickersymbol,
                    GetAndConnectSocket($"{_endpoint}{tickersymbol.ToLower()}@aggTrade"));
            }

            //Check for all tickers
            if (subscriptionRequest.Ticker == FireHoseTicker)
            {
                IsFirehose = true;
                _knowntickers.ForEach(AddTickerSocket);
                return;
            }

            //Check if this is a regular request and we are not subscribed already
            if (!_connecteddepthsockets.ContainsKey(ticker))
                AddTickerSocket(ticker);
        }

        /// <summary>
        /// True if this security can be subscribed to by this data feed
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        public bool CanSubscribe(TickerSymbol ticker)
        {
            //Check for updates available tickers
            UpdateTickers();

            //Check for available ticker
            return _knowntickers.Contains(GetFeedTicker(ticker));
        }
            
        /// <summary>
        /// Gets the name of ticker in the feed.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <returns></returns>
        public override string GetFeedTicker(TickerSymbol ticker) =>
            ticker.Name + ticker.Commodity;

        /// <summary>
        /// Gets the name of the ticker at quantler.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <returns></returns>
        public override TickerSymbol GetQuantlerTicker(string ticker)
        {
            try
            {
                //Check if item is ready in cache
                if (!_tickerSymbols.ContainsKey(ticker))
                {
                    //Initial objects
                    CurrencyType currency = CurrencyType.USD;
                    string commodity = string.Empty;

                    //Check combination
                    if (ticker.EndsWith("BTC"))
                    {
                        currency = CurrencyType.BTC;
                        commodity = ticker.Replace("BTC", "");
                    }
                    else if (ticker.EndsWith("ETH"))
                    {
                        currency = CurrencyType.ETH;
                        commodity = ticker.Replace("ETH", "");
                    }
                    else if (ticker.EndsWith("USDT"))
                    {
                        currency = CurrencyType.USDT;
                        commodity = ticker.Replace("USDT", "");
                    }
                    else if (ticker.EndsWith("BNB"))
                        return TickerSymbol.NIL(ticker); //We currently do not support BNB denominated currencies

                    //Check for symbol changes on binance

                    //Create tickersymbol object based on information supplied
                    _tickerSymbols.Add(ticker, new TickerSymbol(commodity + ".BC", commodity, currency));
                }

                //Return what we have
                return _tickerSymbols[ticker];
            }
            catch (Exception exc)
            {
                _log.Error(exc, $"Cannot convert feed name ticker to quantler ticker: {ticker}");
            }

            //Unknown?
            return TickerSymbol.NIL(ticker);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="initialmessage"></param>
        public void Initialize(MessageInstance initialmessage)
        {
            //Defaults
            IsFirehose = false;
            Conversion += o => ParseData(o.ToString());

            //get tickers
            UpdateTickers();

            //Set name
            Name = "Binance";
            DataSource = DataSource.Binance;

            //Set timer check
            _timeoutTimer = new NetMQTimer(TimeSpan.FromMinutes(1));
            _timeoutTimer.Elapsed += (sender, args) =>
            {
                _log.Debug($"TimeOut timer activated!");
                Reconnect();
            };
            _timeoutTimer.Enable = false;
            _poller = new NetMQPoller { _timeoutTimer };
            _poller.RunAsync();
        }

        /// <summary>
        /// Parses the data from source to datapoints.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="ticker">Associated ticker</param>
        /// <returns></returns>
        public DataPointImpl[] ParseData(string data, string ticker = "")
        {
            //Initial items
            DepthModel depthupdate = null;
            TradeModel treadeupdate = null;
            OrderBookModel orderbookupdate = null;
            var toreturn = new List<DataPointImpl>();
            data = data.Replace(",[]", "");

            //Convert models
            if (data.Contains("depthUpdate"))
                depthupdate = JSON.Deserialize<DepthModel>(data);
            else if (data.Contains("aggTrade"))
                treadeupdate = JSON.Deserialize<TradeModel>(data);
            else if (data.Contains("lastUpdateId"))
            {
                //Check if this contains additional data
                if (data.Contains("|"))
                {
                    var splitted = data.Split('|');
                    ticker = splitted[0];
                    data = splitted[1];
                }

                //Parse data
                orderbookupdate = JSON.Deserialize<OrderBookModel>(data);
            }

            //Full update order book
            if (orderbookupdate != null && !string.IsNullOrWhiteSpace(ticker))
            {
                //Check data
                if (!_orderbook.TryGetValue(ticker, out var orderbook))
                {
                    //Set order book instance
                    orderbook = new OrderBook(ticker);
                    _orderbook[ticker] = orderbook;
                }

                //Helper function
                void UpdateBook(bool isBid, PriceModel x) =>
                    orderbook.AddQuote(isBid, x.DoubleValue, x.DoubleSize);

                //Clear and refresh
                orderbook.Clear();
                orderbookupdate.Asks.ForEach(u => UpdateBook(false, u));
                orderbookupdate.Bids.ForEach(u => UpdateBook(true, u));
            }
            //Update order book
            else if (depthupdate != null)
            {
                //Check data
                if (!_orderbook.TryGetValue(ticker, out var orderbook))
                {
                    //Set order book instance
                    orderbook = new OrderBook(ticker);
                    _orderbook[ticker] = orderbook;
                }

                //DateTime occured
                DateTime occuredutc = Time.FromUnixTime(depthupdate.EventTime, true);

                //Check for quote updates in the order book (size == 0 = remove from order book)
                bool update = depthupdate.AskPricesModel.Count(x => orderbook.AddQuote(false, x.DoubleValue, x.DoubleSize)) > 0;
                update = depthupdate.BidPricesModel.Count(x => orderbook.AddQuote(true, x.DoubleValue, x.DoubleSize)) > 0 | update;

                //Check for full quote update
                if(update)
                    toreturn.Add(new Tick(GetQuantlerTicker(depthupdate.Symbol), DataSource)
                    {
                        AskSize = Convert.ToDecimal(orderbook.AskSize),
                        AskPrice = Convert.ToDecimal(orderbook.BestAsk),
                        BidPrice = Convert.ToDecimal(orderbook.BestBid),
                        BidSize = Convert.ToDecimal(orderbook.BidSize),
                        Depth = 0,
                        Occured = occuredutc,
                        TimeZone = TimeZone.Utc
                    });
            }
            //Send trade
            else if (treadeupdate != null)
            {
                //DateTime occured
                DateTime occuredutc = Time.FromUnixTime(treadeupdate.EventTime, true);

                toreturn.Add(new Tick(GetQuantlerTicker(treadeupdate.Symbol), DataSource)
                {
                    TradePrice = decimal.Parse(treadeupdate.Price, NumberStyles.Any, new CultureInfo("en-US")),
                    Size = decimal.Parse(treadeupdate.Quantity, NumberStyles.Any, new CultureInfo("en-US")),
                    Occured = occuredutc,
                    TimeZone = TimeZone.Utc
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
            lock (Locker)
            {
                if (!_isReconnecting && IsRunning)
                    _isReconnecting = true;
                else
                    return;
            }

            //Stop connection and retry connecting indefinitely
            Exception lastException = null;
            Policy.Handle<Exception>()
                    .WaitAndRetryForever((retryAttempt, context) =>
                    {
                        //Set reset
                        TimeSpan ts = TimeSpan.FromSeconds(5);

                        //Logging
                        _log.Debug($"Could not reconnect binance feed due to exception. Reconnecting in {ts.TotalSeconds} seconds (attempt {retryAttempt})");
                        _log.Debug($"Exception Message: {lastException?.Message}");

                        //Get timespan for next retry
                        return ts;
                    })
                    .Execute(() =>
                    {
                        try
                        {
                            //Open connections
                            UpdateTickers(true, true);

                            //Wait
                            Thread.Sleep(TimeSpan.FromSeconds(20));
                            if (!IsConnected)
                            {
                                _lasttickerupdate = DateTime.MinValue;
                                throw new Exception("Not connected...");
                            }
                            else
                                _log.Debug("Feed is fully connected!");
                        }
                        catch (Exception e)
                        {
                            lastException = e;
                            throw;
                        }
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
            try
            {
                var ticker = GetFeedTicker(subscriptionRequest.Ticker);
                _log.Info("Unsubscribing to ticker " + ticker);
                _subscribedtickers.Remove(ticker);
                var socket = _connecteddepthsockets.FirstOrDefault(x => x.Key == ticker);
                socket.Value.Close();
                socket.Value.Dispose();
                _connecteddepthsockets.Remove(ticker);
                socket = _connectedtradesockets.FirstOrDefault(x => x.Key == ticker);
                socket.Value.Close();
                socket.Value.Dispose();
                _connectedtradesockets.Remove(ticker);
            }
            catch (Exception e)
            {
                _log.Error(e, $"Could not remove subscription {subscriptionRequest.GetSubscriptionName()} from binance feed!");
            }

        }

        /// <summary>
        /// Start task
        /// </summary>
        public void Start()
        {
            //Set state
            IsRunning = true;

            //Start
            Reconnect();
        }

        /// <summary>
        /// Stop task
        /// </summary>
        public void Stop()
        {
            //Stop connection
            lock (Locker)
            {
                _connecteddepthsockets.ForEach(s =>
                {
                    s.Value.Close();
                    s.Value.Dispose();
                });
                _connectedtradesockets.ForEach(s =>
                {
                    s.Value.Close();
                    s.Value.Dispose();
                });
            }

            //Set state
            IsRunning = false;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Gets the and connect socket.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <returns></returns>
        private WebSocket GetAndConnectSocket(string endpoint)
        {
            //Reset connection
            var socket = new WebSocket(endpoint);

            //Set events
            socket.MessageReceived += (sender, e) => OnData(e.Message);
            socket.Opened += (sender, e) => _log.Info($"Connection opened {endpoint}");
            socket.Error += (sender, e) => _log.Error($"Error: {e.Exception.Message}");
            socket.Closed += (sender, e) =>
            {
                _log.Error($"Connection with data feed was closed: {endpoint}");
            };

            //Open connection
            socket.Open();

            //Return socket
            return socket;
        }

        /// <summary>
        /// Called when data was received
        /// </summary>
        /// <param name="data">The data.</param>
        private void OnData(string data)
        {
            try
            {
                //Reset timer
                _timeoutTimer.EnableAndReset();

                //Data capture
                DataCapture?.Invoke(data);

                //Convert and use
                if (TryConvert(data, out DataPoint[] datapoint))
                    datapoint.ForEach(x => CurrenDatapoints.Enqueue(x));

                //Check for next conversion
                UpdateTickers();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Could not process data");
            }
        }

        /// <summary>
        /// Updates the known tickers and checks for reconnect if needed.
        /// </summary>
        /// <param name="force">if set to <c>true</c> [force].</param>
        /// <param name="sync">Synchronized</param>
        private void UpdateTickers(bool force = false, bool sync = false)
        {
            lock (Locker)
            {
                //Check next check
                if (DateTime.UtcNow < _lasttickerupdate.Add(_symbolupdateTimeSpan) && !force)
                    return;

                //Set last updae
                _lasttickerupdate = DateTime.UtcNow;
            }

            var bgtask = Task.Run(() =>
            {
                //Check lock
                if (!Monitor.TryEnter(UpdateLocker))
                    return;

                //Get symbols
                try
                {

                    //Get data and add to missing symbols
                    TickerModel[] items = _binanceApi.GetTickerSymbolsAsync().Result;

                    //Check if we need to filter
                    if (!IsFirehose)
                        items = items.Where(x => _subscribedtickers.Contains(x.symbol)).ToArray();

                    //Check for all items
                    foreach (var item in items)
                    {
                        //Check if this item is stuck in a connecting state
                        if (_connecteddepthsockets.ContainsKey(item.symbol) && _connecteddepthsockets[item.symbol].State == WebSocketState.Connecting)
                        {
                            try
                            {
                                //Disconnect
                                _connecteddepthsockets[item.symbol].Close();
                                _connectedtradesockets[item.symbol].Close();
                            }
                            catch (Exception e)
                            {
                                _log.Warn(e, $"Could not close connection ({item.symbol}): {e.Message}");
                            }

                            //Dispose
                            try
                            {
                                _connecteddepthsockets[item.symbol].Dispose();
                                _connectedtradesockets[item.symbol].Dispose();
                            }
                            catch (Exception e)
                            {
                                _log.Warn(e, $"Could not dispose connection ({item.symbol}): {e.Message}");
                            }

                            //Remove
                            try
                            {
                                _connecteddepthsockets.Remove(item.symbol);
                                _connectedtradesockets.Remove(item.symbol);
                            }
                            catch (Exception e)
                            {
                                _log.Warn(e, $"Could not remove connection ({item.symbol}): {e.Message}");
                            }
                        }

                        //Add or reconnect
                        if (!_connecteddepthsockets.ContainsKey(item.symbol))
                        {
                            try
                            {
                                _connecteddepthsockets.Add(item.symbol,
                                    GetAndConnectSocket($"{_endpoint}{item.symbol.ToLower()}@depth"));
                                _connectedtradesockets.Add(item.symbol,
                                    GetAndConnectSocket($"{_endpoint}{item.symbol.ToLower()}@aggTrade"));
                            }
                            catch (Exception e)
                            {
                                _log.Error(e, $"Could not initiate connection ({item.symbol}): {e.Message}");
                            }
                        }
                        else if (_connecteddepthsockets[item.symbol].State != WebSocketState.Open)
                        {
                            try
                            {
                                _connecteddepthsockets[item.symbol].Open();
                                _connectedtradesockets[item.symbol].Open();
                            }
                            catch (Exception e)
                            {
                                _log.Warn(e, $"Could not re-open connection ({item.symbol}): {e.Message}");
                            }
                        }
                    }

                    try
                    {
                        //Update orderbook so we are in sync
                        foreach (var ticker in items)
                        {
                            var data = $"https://api.binance.com/api/v1/depth?symbol={ticker.symbol}".GetStringAsync().Result;
                            ParseData(data, ticker.symbol);
                        }
                    }
                    catch (Exception e)
                    {
                        _log.Error(e, $"Could not update orderbook data due to exception: {e.Message}");
                    }
                }
                catch (Exception e)
                {
                    _log.Error(e, $"Could not get current tickers {e.Message}");
                }
                finally
                {
                    Monitor.Exit(UpdateLocker);
                }
            });

            //Check if we need to run this sync
            if (sync)
                bgtask.Wait();
        }

        #endregion Private Methods
    }
}