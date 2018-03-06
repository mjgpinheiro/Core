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
using Quantler.Account.Cash;
using Quantler.Data.Aggegrate;
using Quantler.Data.Bars;
using Quantler.Data.Market;
using Quantler.Interfaces;
using Quantler.Securities;
using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NodaTime;

namespace Quantler.Data
{
    /// <summary>
    /// Keeps track of data currently subscribed to (also adds and removes subscriptions when needed)
    /// </summary>
    public class DataSubscriptionManager
    {
        #region Private Fields

        /// <summary>
        /// Associated data feed
        /// </summary>
        private readonly DataFeed _datafeed;

        /// <summary>
        /// The current logging instance
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Base currency type for the account
        /// </summary>
        private readonly CurrencyType _basecurrency;

        /// <summary>
        /// Currently active subscription requests
        /// </summary>
        private readonly List<DataSubscriptionRequest> _requesteddatasubscriptions = new List<DataSubscriptionRequest>();

        /// <summary>
        /// Currently active subscriptions per quant fund
        /// </summary>
        private readonly Dictionary<string, List<DataSubscription>> _registeredsubscriptions = new Dictionary<string, List<DataSubscription>>();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSubscriptionManager"/> class.
        /// </summary>
        /// <param name="datafeed"></param>
        /// <param name="cashmanager"></param>
        public DataSubscriptionManager(DataFeed datafeed, CashManager cashmanager)
        {
            _datafeed = datafeed;
            _basecurrency = cashmanager.BaseCurrency;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the currently active subscriptions.
        /// </summary>
        public DataSubscriptionRequest[] ActiveSubscriptionsRequest => _requesteddatasubscriptions.ToArray();

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Gets the data subscriptions.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <param name="datatype">The datatype.</param>
        /// <returns></returns>
        public DataSubscription[] GetDataSubscriptions(TickerSymbol ticker, DataType datatype) =>
            _registeredsubscriptions.Values.SelectMany(x => x).Where(x => x.Ticker == ticker && x.DataType == datatype)
                .ToArray();

        /// <summary>
        /// Gets the currently active tickers by quant fund
        /// </summary>
        /// <param name="quantfund">The quantfund.</param>
        /// <returns></returns>
        public string[] ActiveTickers(IQuantFund quantfund) =>
            _registeredsubscriptions.ContainsKey(quantfund.FundId)
                ? _registeredsubscriptions[quantfund.FundId].Select(x => x.Ticker).Distinct().ToArray() :
                  new string[0];

        /// <summary>
        /// Returns all currently active tickers
        /// </summary>
        /// <returns></returns>
        public string[] ActiveTickers() =>
            _registeredsubscriptions
                        .SelectMany(x => x.Value)
                        .Select(x => x.Ticker)
                        .Distinct()
                        .ToArray();

        /// <summary>
        /// Adds a datasubscription for all non market price data available (delisting, splits, trading status etc)
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="quantfund"></param>
        public void AddSubscription(IQuantFund quantfund, Security security)
        {
            //Helper function
            void AddSubscriptionType(DataType datatype)
            {
                var subscriptionrequest =
                    DataSubscriptionRequest.CreateSubscriptionRequest(security.Ticker, _datafeed.DataSource, null, datatype);
                AddSubscription(subscriptionrequest);

                //Add subscription to registered subscriptions
                if (!_registeredsubscriptions.ContainsKey(quantfund.FundId))
                    _registeredsubscriptions.Add(quantfund.FundId, new List<DataSubscription>() { new DataSubscription(quantfund.FundId, subscriptionrequest, security, TimeZone.Utc, null) });
                else if (_registeredsubscriptions[quantfund.FundId].Count(x =>
                             x.Request.GetSubscriptionName() == subscriptionrequest.GetSubscriptionName()) == 0)
                {
                    _registeredsubscriptions[quantfund.FundId].Add(new DataSubscription(quantfund.FundId, subscriptionrequest, security, TimeZone.Utc, null));
                }

            }

            //Add delisting information
            AddSubscriptionType(DataType.Delisting);

            //Add dividend notifications
            AddSubscriptionType(DataType.Dividend);

            //Add earning reports
            AddSubscriptionType(DataType.Earning);

            //Add financial reports
            AddSubscriptionType(DataType.Financial);

            //Add key stats
            AddSubscriptionType(DataType.KeyStat);

            //Add stock splits
            AddSubscriptionType(DataType.Split);

            //Add trading status updates
            AddSubscriptionType(DataType.TradingStatus);

            //Check base conversion
            AddBaseCurrencyConversionFeed(security);
        }

        /// <summary>
        /// Adds a datasubscription which is derived from the requested data aggregator instance
        /// Force tick will force the data to contain the highest granularity (otherwise it might be based on 1-minute data)
        /// TODO: add unit test, if we request 1 minute data and than request tick data we should keep the tick data request and replace all 1 minute request with the tick data request? (so that we only keep the tick data request)
        /// TODO: we will only do ticks or tradebars! (where a trade bar is based on any data)
        /// </summary>
        /// <param name="quantfund"></param>
        /// <param name="security">The security.</param>
        /// <param name="aggregator">The aggregator.</param>
        /// <param name="forcetick">if set to <c>true</c> [forcetick].</param>
        /// <returns>Can be a different dataggregator due to change in data requested</returns>
        public DataAggregator AddSubscription(IQuantFund quantfund, Security security, DataAggregator aggregator, bool forcetick = false)
        {
            //Initial values
            TimeSpan? aggregationneeded = null;
            DataType datatypeneeded = DataType.Tick;
            TimeSpan preaggregated = TimeSpan.FromMinutes(1);

            if (!forcetick)
            {
                //TradeBar -> TradeBar
                if (aggregator is TimeSerieAggregator<TradeBar, TradeBar> tradetotrade && tradetotrade.IsTimeBased)
                {
                    if (tradetotrade.Period.Value.TotalSeconds % 60 == 0D)
                    {
                        aggregator = new TradeAggregator(tradetotrade.Period.Value);
                        aggregationneeded = preaggregated;
                        datatypeneeded = DataType.TradeBar;
                    }
                }

                //Tick -> TradeBar
                if (aggregator is TimeSerieAggregator<Tick, TradeBar> ticktobar && ticktobar.IsTimeBased)
                {
                    if (ticktobar.Period.Value.TotalSeconds % 60 == 0D)
                    {
                        aggregator = new TickQuoteBarAggregator(ticktobar.Period.Value);
                        aggregationneeded = TimeSpan.FromMinutes(1);
                        datatypeneeded = DataType.TradeBar;
                    }
                }
            }

            //get and add subscription
            var subscription = DataSubscriptionRequest.CreateSubscriptionRequest(security.Ticker, _datafeed.DataSource,
                aggregationneeded, datatypeneeded);
            subscription = AddSubscription(subscription);

            //Add base currency conversion
            AddBaseCurrencyConversionFeed(security);

            //Check if we already have a similar data aggregator, reuse the existing version if possible
            if (_registeredsubscriptions.ContainsKey(quantfund.FundId))
            {
                var found = _registeredsubscriptions[quantfund.FundId].FirstOrDefault(x => x.Request.GetSubscriptionName() == subscription.GetSubscriptionName());
                var existing = found?.Aggregators.FirstOrDefault(x => x.Name == aggregator.Name);
                if (existing != null)
                    return existing;
                else if (found == null)
                    _registeredsubscriptions[quantfund.FundId].Add(new DataSubscription(quantfund.FundId, subscription,
                        security, security.Exchange.TimeZone, aggregator));
                else
                    found.Aggregators.Add(aggregator);
            }
            else
            {
                //Add new
                _registeredsubscriptions.Add(quantfund.FundId, new List<DataSubscription>());
                _registeredsubscriptions[quantfund.FundId].Add(new DataSubscription(quantfund.FundId, subscription, security, security.Exchange.TimeZone, aggregator));
            }

            //Return our current aggregator
            return aggregator;
        }

        /// <summary>
        /// Removes the quant fund associated subscriptions.
        /// </summary>
        /// <param name="quantfund">The quantfund.</param>
        public void RemoveQuantFundSubscriptions(IQuantFund quantfund)
        {
            //get all tickers
            var tickers = ActiveTickers(quantfund);

            //Remove quant fund
            _registeredsubscriptions.Remove(quantfund.FundId);

            //Get all items which are no longer needed
            var active = _registeredsubscriptions
                .SelectMany(x => x.Value.Select(n => n.Ticker))
                .Distinct();

            //Remove all no longer used
            tickers.Where(x => !active.Contains(x))
                   .ForEach(RemoveSubscription);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Adds the base currency conversion feed.
        /// </summary>
        /// <param name="security">The security.</param>
        private void AddBaseCurrencyConversionFeed(Security security)
        {
            //get both ways
            string tickerbase = $"{security.BaseCurrency}/{_basecurrency}";
            string tickerreversed = $"{_basecurrency}/{security.BaseCurrency}";

            //Check both subscriptions
            var tickerbasesubscription =
                DataSubscriptionRequest.CreateSubscriptionRequest(new TickerSymbol(tickerbase, _basecurrency.ToString(), security.BaseCurrency), _datafeed.DataSource, null, DataType.Tick);
            var tickerreversesubscription =
                DataSubscriptionRequest.CreateSubscriptionRequest(new TickerSymbol(tickerreversed, security.BaseCurrency.ToString(), _basecurrency), _datafeed.DataSource, null, DataType.Tick);

            //Add to subscription if needed possible
            string warningmessage(string ticker) =>
                $"Cannot get live updates for currency {ticker} from feed {_datafeed.Name}, currency conversion will not be based on live data as the feed does not provide this conversion";

            if (_datafeed.CanSubscribe(tickerbasesubscription.Ticker))
                AddSubscription(tickerbasesubscription);
            else
                _log.Warn(warningmessage(tickerbase));

            if(_datafeed.CanSubscribe(tickerreversesubscription.Ticker))
                AddSubscription(tickerreversesubscription);
            else
                _log.Warn(warningmessage(tickerreversed));
        }

        /// <summary>
        /// Adds the subscription.
        /// </summary>
        /// <param name="subscriptionRequest">The subscription.</param>
        private DataSubscriptionRequest AddSubscription(DataSubscriptionRequest subscriptionRequest)
        {
            var found = _requesteddatasubscriptions.FirstOrDefault(x => x.GetSubscriptionName() == subscriptionRequest.GetSubscriptionName());
            if (found == null)
            {
                _requesteddatasubscriptions.Add(subscriptionRequest);
                _datafeed.AddSubscription(subscriptionRequest);
                return subscriptionRequest;
            }
            else
                return found;
        }

        /// <summary>
        /// Removes all subscriptions based on ticker name.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        private void RemoveSubscription(string ticker)
        {
            //Get all subscriptions associated to this ticker
            var found = _requesteddatasubscriptions.Where(x => x.Ticker == ticker).ToArray();

            //Remove subscription
            found.ForEach(RemoveSubscription);
        }

        /// <summary>
        /// Removes the subscription.
        /// </summary>
        /// <param name="subscriptionRequest">The subscription.</param>
        private void RemoveSubscription(DataSubscriptionRequest subscriptionRequest)
        {
            var found = _requesteddatasubscriptions.Where(x =>
                x.GetSubscriptionName() == subscriptionRequest.GetSubscriptionName()).ToArray();
            if (found.Any())
            {
                found.ForEach(x =>
                {
                    _requesteddatasubscriptions.Remove(x);
                    _datafeed.RemoveSubscription(x);
                });
            }
        }

        #endregion Private Methods
    }
}