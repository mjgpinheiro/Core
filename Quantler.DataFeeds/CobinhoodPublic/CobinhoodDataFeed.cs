using Jil;
using NLog;
using Quantler.Api.Cobinhood;
using Quantler.Data;
using Quantler.DataFeeds.CobinhoodPublic.Models;
using Quantler.Interfaces;
using Quantler.Messaging;
using Quantler.Securities;
using System;
using System.Collections.Generic;
using System.Composition;
using WebSocket4Net;

namespace Quantler.DataFeeds.CobinhoodPublic
{
    /// <summary>
    /// TODO: also implement cobinhood feed here
    /// </summary>
    [Export(typeof(DataFeed))]
    public class CobinhoodDataFeed : BaseFeed, DataFeed
    {
        #region Private Fields

        /// <summary>
        /// Cobinhood datafeed endpoint
        /// </summary>
        private const string Endpoint = "wss://feed.cobinhood.com/ws";

        /// <summary>
        /// The cobinhood API
        /// </summary>
        private readonly CobinhoodApi _cobinhoodApi = new CobinhoodApi();

        /// <summary>
        /// Current logging instance
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

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
        /// If true, this instance is a firehose
        /// </summary>
        private bool _isFirehose;

        /// <summary>
        /// The last available tickers update
        /// </summary>
        private DateTime _lastTickerUpdate = DateTime.MinValue;

        /// <summary>
        /// The locker
        /// </summary>
        private object _locker = new object();

        /// <summary>
        /// Current socket
        /// </summary>
        private WebSocket _socket;

        /// <summary>
        /// The ticker symbols cache
        /// </summary>
        private Dictionary<string, TickerSymbol> _tickerSymbols = new Dictionary<string, TickerSymbol>();

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// True if this data feed is in a connected state
        /// </summary>
        public bool IsConnected => _socket.State == WebSocketState.Open;

        /// <summary>
        /// True if this task is running
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public bool IsRunning
        {
            private set;
            get;
        }

        #endregion Public Properties

        #region Public Methods

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
                _isFirehose = true;
                _availableTickers.ForEach(x =>
                {
                    SubscribeTickerSymbol(x);
                    _connectedTickers.Add(x);
                });
                return;
            }

            //Check for subscription
            if (!_connectedTickers.Contains(ticker))
            {
                _log.Info("Subscribing to ticker " + ticker);
                SubscribeTickerSymbol(ticker);
                _connectedTickers.Add(ticker);
            }
        }

        /// <summary>
        /// True if this security can be subscribed to by this data feed
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool CanSubscribe(TickerSymbol ticker)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the name of ticker in the feed.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <returns></returns>
        public override string GetFeedTicker(TickerSymbol ticker) =>
            $"{ticker.Commodity}-{ticker.Currency}";

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
                    _tickerSymbols.Add(ticker, new TickerSymbol(splitted[0] + ".BC", splitted[0], (CurrencyType)Enum.Parse(typeof(CurrencyType), splitted[1])));
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
        /// <param name="initialmessage"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Initialize(MessageInstance initialmessage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reconnect to data feed on request
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Reconnect()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stop retrieving data for the requested security
        /// </summary>
        /// <param name="subscriptionRequest"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void RemoveSubscription(DataSubscriptionRequest subscriptionRequest)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Start task
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Start()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stop task
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Stop()
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Sends a request to the cobinhood socket connection.
        /// </summary>
        /// <param name="request">The request.</param>
        private void SendRequest(SocketRequestModel request)
        {
            try
            {
                var serialized = JSON.Serialize(request);
                _log.Trace($"Sending request to cobinhood socket: {serialized}");
                _socket.Send(serialized);
            }
            catch (Exception e)
            {
                _log.Error(e, $"Could not send request to Cobinhood socket due to exception.");
            }
        }

        /// <summary>
        /// Sunscribes the ticker symbol to the current connection.
        /// </summary>
        /// <param name="tickersymbol">The tickersymbol.</param>
        private void SubscribeTickerSymbol(string tickersymbol)
        {
            //Check if we are not already subscribed
            if (_connectedTickers.Contains(tickersymbol))
                return;

            //Create request
            var socketmethod = new SocketRequestModel
            {
                Action = "subscribe",
                Type = "book",
                TradingPairId = tickersymbol
            };

            //Send for orderbook
            SendRequest(socketmethod);

            //Send for trades
            socketmethod.Type = "trade";
            SendRequest(socketmethod);

            //Send for ticker
            socketmethod.Type = "ticker";
            SendRequest(socketmethod);
        }

        /// <summary>
        /// Sunscribes the ticker symbol to the current connection.
        /// </summary>
        /// <param name="tickersymbol">The tickersymbol.</param>
        private void UnSubscribeTickerSymbol(string tickersymbol)
        {
            //Check if we are subscribed
            if (!_connectedTickers.Contains(tickersymbol))
                return;

            //Create request
            var socketmethod = new SocketRequestModel
            {
                Action = "unsubscribe",
                Type = "book",
                TradingPairId = tickersymbol
            };

            //Send for orderbook
            SendRequest(socketmethod);

            //Send for trades
            socketmethod.Type = "trade";
            SendRequest(socketmethod);

            //Send for ticker
            socketmethod.Type = "ticker";
            SendRequest(socketmethod);
        }

        /// <summary>
        /// Updates the tickers.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void UpdateTickers()
        {
            throw new NotImplementedException();
        }

        #endregion Private Methods
    }
}