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
using Microsoft.AspNet.SignalR.Client;
using MoreLinq;
using NLog;
using Polly;
using Quantler.Api;
using Quantler.Api.Bittrex;
using Quantler.Common;
using Quantler.Data;
using Quantler.Data.Market;
using Quantler.DataFeeds.BittrexPublic.Models;
using Quantler.Interfaces;
using Quantler.Messaging;
using Quantler.Securities;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;

namespace Quantler.DataFeeds.BittrexPublic
{
    /// <summary>
    /// Bittrex public data feed for crypto currencies
    /// NOTICE: AS LONG AS BITTREX WILL NOT PROVIDE AN OFFICAL DATAFEED, WE (QUANTLER) WILL NOT OFFICIALLY SUPPORT ANY CONNECTIONS WITH BITTREX
    ///         USAGE OF ALL AND ANY BITTREX RELATED IMPLEMENTATIONS IS AT YOUR OWN RISK!!!!
    /// </summary>
    /// <seealso cref="BaseFeed" />
    /// <seealso cref="DataFeed" />
    [Export(typeof(DataFeed))]
    public class BittrexDataFeed : BaseFeed, DataFeed
    {
        #region Private Fields

        /// <summary>
        /// Bittrex data feed endpoint
        /// </summary>
        private const string Endpoint = "https://socket.bittrex.com/signalr";

        /// <summary>
        /// The bittrex API
        /// </summary>
        private readonly BittrexApi _bittrexApi = new BittrexApi();

        /// <summary>
        /// Current logging instance
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The nounce memory, to keep track of nounce received
        /// </summary>
        private readonly FixedSizedQueue<string> _nouncememory = new FixedSizedQueue<string>(5000);

        /// <summary>
        /// The current order book
        /// </summary>
        private readonly Dictionary<string, OrderBook> _orderbook = new Dictionary<string, OrderBook>();

        /// <summary>
        /// The available tickers
        /// </summary>
        private List<string> _availableTickers = new List<string>();

        /// <summary>
        /// The connected tickers
        /// </summary>
        private List<string> _connectedTickers = new List<string>();

        /// <summary>
        /// The hub connection
        /// </summary>
        private HubConnection _hubConnection = new HubConnection(Endpoint);

        /// <summary>
        /// The hub proxy
        /// </summary>
        private IHubProxy _hubProxy;

        /// <summary>
        /// Check if we are reconnecting
        /// </summary>
        private bool _isReconnecting;

        /// <summary>
        /// The last available tickers update
        /// </summary>
        private DateTime _lastTickerUpdate = DateTime.MinValue;

        /// <summary>
        /// The locker
        /// </summary>
        private object _locker = new object();

        /// <summary>
        /// The ticker symbols cache
        /// </summary>
        private Dictionary<string, TickerSymbol> _tickerSymbols = new Dictionary<string, TickerSymbol>();

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// True if this datafeed is in a connected state
        /// </summary>
        public bool IsConnected => _hubConnection.State == ConnectionState.Connected;

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
            //Check for updates
            UpdateTickers();

            //Check for current subscription capabilities
            if (subscriptionRequest.DataSource != DataSource)
                throw new Exception($"Cannot subscribe to data source {subscriptionRequest.DataSource} when using {DataSource}");

            //Normalize ticker
            string ticker = GetFeedTicker(subscriptionRequest.Ticker);

            //Check for all tickers
            if (subscriptionRequest.Ticker == FireHoseTicker)
            {
                IsFirehose = true;
                _availableTickers.ForEach(x =>
                {
                    _hubProxy.Invoke("SubscribeToExchangeDeltas", x);
                    _connectedTickers.Add(x);
                });
                return;
            }

            //Check for subscription
            if (!_connectedTickers.Contains(ticker))
            {
                _log.Info("Subscribing to ticker " + ticker);
                _hubProxy.Invoke("SubscribeToExchangeDeltas", ticker);
                _connectedTickers.Add(ticker);
            }
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
            return _availableTickers.Contains(GetFeedTicker(ticker));
        }

        /// <summary>
        /// Gets the name of ticker in the feed.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <returns></returns>
        public override string GetFeedTicker(TickerSymbol ticker) =>
            ticker.Currency + "-" + ticker.Commodity;

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
                    string[] splitted = ticker.Split('-');
                    _tickerSymbols.Add(ticker, new TickerSymbol(splitted[1] + ".BC", splitted[1], (CurrencyType)Enum.Parse(typeof(CurrencyType), splitted[0])));
                }

                //Return what we have
                return _tickerSymbols[ticker];
            }
            catch (Exception exc)
            {
                _log.Error(exc, $"Cannot convert feed name ticker to quantler ticker: {ticker}");
            }

            return TickerSymbol.NIL(ticker);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize(MessageInstance initialmessage)
        {
            //Defaults
            IsFirehose = false;
            Conversion = data => ParseData(data, DateTime.UtcNow);

            //get tickers
            UpdateTickers();

            //Set name
            Name = "Bittrex";
            DataSource = DataSource.Bittrex;
        }

        /// <summary>
        /// Conversions for the retrieved data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="currentutc"></param>
        /// <returns></returns>
        public DataPointImpl[] ParseData(object data, DateTime currentutc)
        {
            //Check data update type
            var result = JSON.DeserializeDynamic(data.ToString());
            var toreturn = new List<DataPointImpl>();

            //Only do exchangeModel updates
            if (result.M == "updateExchangeState")
            {
                //Deserialize
                HubResponseModel<ExchangeStateModel> exchangeupdate = JSON.Deserialize<HubResponseModel<ExchangeStateModel>>(data.ToString(), Options.ISO8601);

                //Go trough all items
                foreach (var item in exchangeupdate.Content)
                {
                    //Check NOUNCE
                    string nounce = $"{item.MarketName}.{item.Nounce}";
                    if (_nouncememory.Contains(nounce))
                        continue;
                    else
                        _nouncememory.Enqueue(nounce);

                    //Check data
                    if (!_orderbook.TryGetValue(item.MarketName, out var orderbook))
                    {
                        //Set order book instance
                        orderbook = new OrderBook(item.MarketName);
                        _orderbook[item.MarketName] = orderbook;
                    }

                    //Update function
                    bool ProcessUpdate(int type, bool isbid, double price, double quantity) =>
                        (type == 0 && orderbook.AddQuote(isbid, price, quantity))
                        || (type == 1 && orderbook.RemoveQuote(isbid, price, quantity))
                        || (type == 2 && orderbook.UpdateQuote(isbid, price, quantity));

                    //Check for quote updates in the order book (size == 0 = remove from order book)
                    bool update = item.Sells.Count(x => ProcessUpdate(x.Type, false, x.Rate, x.Quantity)) > 0;
                    update = item.Buys.Count(x => ProcessUpdate(x.Type, true, x.Rate, x.Quantity)) > 0 | update;

                    //If update, send
                    if(update)
                        toreturn.Add(new Tick(GetQuantlerTicker(item.MarketName), DataSource)
                        {
                            AskPrice = Convert.ToDecimal(orderbook.BestAsk),
                            AskSize = Convert.ToDecimal(orderbook.AskSize),
                            BidPrice = Convert.ToDecimal(orderbook.BestBid),
                            BidSize = Convert.ToDecimal(orderbook.BidSize),
                            TimeZone = TimeZone.Utc,
                            Occured = currentutc,
                            Depth = 0
                        });

                    //Trades
                    foreach (var x in item.Fills)
                    {
                        toreturn.Add(new Tick(GetQuantlerTicker(item.MarketName), DataSource)
                        {
                            TradePrice = Convert.ToDecimal(x.Rate),
                            Size = Convert.ToDecimal(x.Quantity),
                            Occured = x.TimeStamp,
                            TimeZone = TimeZone.Utc,
                            Price = Convert.ToDecimal(x.Rate)
                        });
                    }
                }
            }
            else if (result.M == "updateSummaryState")
            {
                //Deserialize
                HubResponseModel<SummaryStateModel> updatesummary = JSON.Deserialize<HubResponseModel<SummaryStateModel>>(data.ToString(), Options.ISO8601);

                //Set all data
                foreach (var item in updatesummary.Content)
                {
                    //Check NOUNCE
                    string nounce = $"Summary.{item.Nounce}";
                    if (_nouncememory.Contains(nounce))
                        continue;
                    else
                        _nouncememory.Enqueue(nounce);

                    foreach (var x in item.Deltas)
                    {
                        //Check data
                        if (!_orderbook.TryGetValue(x.MarketName, out var orderbook))
                        {
                            //Set order book instance
                            orderbook = new OrderBook(x.MarketName);
                            _orderbook[x.MarketName] = orderbook;

                            //Set initial best bid and ask
                            orderbook.SetBestBook(true, x.Bid, x.BaseVolume);
                            orderbook.SetBestBook(false, x.Ask, x.BaseVolume);
                        }
                        else
                        {
                            //Update order book (best bid and ask)
                            orderbook.SetBestBook(true, x.Bid, orderbook.BidSize);
                            orderbook.SetBestBook(false, x.Ask, orderbook.AskSize);
                        }

                        //Summary data
                        toreturn.Add(new Tick(GetQuantlerTicker(x.MarketName), DataSource)
                        {
                            AskPrice = Convert.ToDecimal(x.Ask),
                            AskSize = Convert.ToDecimal(orderbook.AskSize),
                            BidPrice = Convert.ToDecimal(x.Bid),
                            BidSize = Convert.ToDecimal(orderbook.BidSize),
                            Depth = 0,
                            Occured = x.TimeStamp,
                            Price = Convert.ToDecimal(x.Last),
                            Size = Convert.ToDecimal(x.BaseVolume),
                            TradePrice = Convert.ToDecimal(x.Last),
                            TimeZone = TimeZone.Utc
                        });
                    }
                }
            }

            //Return result
            return toreturn.ToArray();
        }

        /// <summary>
        /// Reconnect to data feed on request
        /// </summary>
        public void Reconnect()
        {
            //Check for multiple calls
            lock (_locker)
            {
                if (!_isReconnecting)
                    _isReconnecting = true;
                else
                    return;
            }

            //Check if we need to reconnect
            if (IsConnected || !IsRunning)
                return;

            _log.Warn("Bittrex data feed reconnecting");

            //Stop connection and retry connecting indefinitely
            Policy.Handle<Exception>()
                    .WaitAndRetryForever((retryAttempt, context) =>
                    {
                        //Get timespan for next retry
                        var ts = TimeSpan.FromSeconds(5);

                        //Logging
                        _log.Error($"Could not reconnect bittrex feed due to exception. Reconnecting in {ts.TotalSeconds} seconds (attempt {retryAttempt})");

                        //return timer
                        return ts;
                    })
                    .Execute(() =>
                    {
                        try
                        {
                            //Check if we already connected
                            if (IsConnected)
                                return;

                            //Bypass cloudflare protection
                            var cookies = CloudflareBypass.GetCookies("https://www.bittrex.com/", "SignalR.Client.NetStandard/2.2.2.0 (Unknown OS)");

                            //Destroy current connection (since we need to set a new header based on CF protection)
                            _hubConnection?.Stop();

                            //Set hub connection
                            _hubConnection = new HubConnection(Endpoint);
                            _hubConnection.CookieContainer = cookies;
                            _hubConnection.Received += OnData;
                            _hubConnection.Closed += Reconnect;
                            _hubProxy = _hubConnection.CreateHubProxy("CoreHub");

                            //Start new connection
                            _hubConnection.Start().Wait();

                            //Wait
                            Thread.Sleep(2000);

                            //Check again
                            if (!IsConnected)
                                throw new Exception("Connection check failed...");
                        }
                        catch (Exception exc)
                        {
                            _log.Error(exc, "Could not connect to bittrex feed");
                            throw;
                        }
                    });

            //Resubscribe
            _connectedTickers.ForEach(x => _hubProxy.Invoke("SubscribeToExchangeDeltas", x));
            _isReconnecting = false;
        }

        /// <summary>
        /// Stop retrieving data for the requested security
        /// </summary>
        /// <param name="subscriptionRequest"></param>
        public void RemoveSubscription(DataSubscriptionRequest subscriptionRequest)
        {
            var ticker = GetFeedTicker(subscriptionRequest.Ticker);
            _log.Info("Unsubscribing to ticker " + ticker);
            _hubProxy.Invoke("UmsubscribeToExchangeDeltas", ticker);
            _connectedTickers.Remove(ticker);
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
            _hubConnection.Stop();

            //Set state
            IsRunning = false;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Called when data was received
        /// </summary>
        /// <param name="data">The data.</param>
        private void OnData(string data)
        {
            try
            {
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
        /// Updates the tickers currently available
        /// </summary>
        private void UpdateTickers()
        {
            //Check for next update
            if ((DateTime.UtcNow - _lastTickerUpdate) < TimeSpan.FromHours(1))
                return;
            else
                _lastTickerUpdate = DateTime.UtcNow;

            //Get from public api
            //get data
            var result = _bittrexApi.GetMarketsAsync().Result;
            if (result != null && result.Success)
                result.Result.ForEach(x =>
                {
                    string tickername = x.MarketName;
                    if (!_availableTickers.Contains(tickername))
                    {
                        //Add to known tickers
                        _availableTickers.Add(tickername);

                        //Check for firehose
                        if (IsFirehose)
                        {
                            _connectedTickers.Add(tickername);
                            _hubProxy.Invoke("SubscribeToExchangeDeltas", tickername);
                        }
                    }
                });
            else
                _log.Error("Could not retrieve bittrex market information");
        }

        #endregion Private Methods
    }
}