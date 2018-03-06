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
using Quantler.Data.Bars;
using Quantler.Data.Market;
using Quantler.Exchanges;
using Quantler.Interfaces;
using System;

namespace Quantler.Securities
{
    /// <summary>
    /// Base security implementation
    /// </summary>
    /// <seealso cref="Quantler.Securities.Security" />
    public abstract class SecurityBase : Security
    {
        #region Private Fields

        /// <summary>
        /// Currency conversion tool
        /// </summary>
        private readonly Currency _currencyConversion;

        #endregion Private Fields

        #region Protected Constructors

        /// <summary>
        /// Base implementation for a security object
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="exchangeModel"></param>
        /// <param name="details"></param>
        /// <param name="type"></param>
        /// <param name="conversion"></param>
        protected SecurityBase(TickerSymbol ticker, ExchangeModel exchangeModel, SecurityDetails details, SecurityType type, Currency conversion)
        {
            //Set references
            _currencyConversion = conversion;
            Exchange = exchangeModel;
            BaseCurrency = ticker.Currency;
            Details = details;
            Ticker = ticker;
            Type = type;

            //Defaults
            LastTickEventUtc = DateTime.MinValue;
            LastTradeEventUtc = DateTime.MinValue;
        }

        #endregion Protected Constructors

        #region Public Properties

        /// <summary>
        /// Last received ask price
        /// </summary>
        public decimal AskPrice
        {
            get;
            protected set;
        }

        /// <summary>
        /// Last received ask size
        /// </summary>
        public decimal AskSize
        {
            get;
            protected set;
        }

        /// <summary>
        /// Security base currency denomination
        /// </summary>
        public CurrencyType BaseCurrency
        {
            get;
            protected set;
        }

        /// <summary>
        /// Last received bid price
        /// </summary>
        public decimal BidPrice
        {
            get;
            protected set;
        }

        /// <summary>
        /// Last received bid size
        /// </summary>
        public decimal BidSize
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the current close price.
        /// </summary>
        public decimal Close
        {
            get;
            private set;
        }

        /// <summary>
        /// Additional security specified applied details
        /// </summary>
        public SecurityDetails Details
        {
            get;
            internal set;
        }

        /// <summary>
        /// Traded exchangeModel for this security
        /// </summary>
        public ExchangeModel Exchange
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the current high price.
        /// </summary>
        public decimal High
        {
            get;
            private set;
        }

        /// <summary>
        /// Last date and time tick was received
        /// </summary>
        public DateTime LastTickEventUtc
        {
            get;
            private set;
        }

        /// <summary>
        /// Last date and time trade was received
        /// </summary>
        public DateTime LastTradeEventUtc
        {
            get;
            protected set;
        }

        /// <summary>
        /// Current local time for this security
        /// </summary>
        public DateTime LocalTime =>
            Exchange.LocalTime;

        /// <summary>
        /// Gets the current low price.
        /// </summary>
        public decimal Low
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current open price.
        /// </summary>
        public decimal Open
        {
            get;
            private set;
        }

        /// <summary>
        /// Current spread (live)
        /// </summary>
        public decimal Spread =>
            AskPrice - BidPrice;

        /// <summary>
        /// Ticker symbol used
        /// </summary>
        public TickerSymbol Ticker
        {
            get;
            protected set;
        }

        /// <summary>
        /// Last trade prace received
        /// </summary>
        public decimal TradePrice
        {
            get;
            protected set;
        }

        /// <summary>
        /// Type of this security
        /// </summary>
        public virtual SecurityType Type
        {
            get;
            protected set;
        } = SecurityType.NIL;

        /// <summary>
        /// Current security price, depending on which data this security has received
        /// </summary>
        public decimal Price
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets a value indicating whether this security trading is halted.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is halted; otherwise, <c>false</c>.
        /// </value>
        public bool IsHalted
        {
            get;
            internal set;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Convert value from base currency to target currency
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public decimal ConvertValue(decimal value, CurrencyType target) =>
            _currencyConversion.Convert(value, BaseCurrency, target);

        /// <summary>
        /// Convert value from source currency to base currency
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public decimal ConvertValue(CurrencyType source, decimal value) =>
            _currencyConversion.Convert(value, source, BaseCurrency);

        /// <summary>
        /// Update security information based on data point received
        /// </summary>
        /// <param name="datapoint"></param>
        public virtual void UpdatePrice(DataPoint datapoint)
        {
            //Check for tick
            if (datapoint is Tick tick)
            {
                if (tick.IsQuote)
                {
                    if (tick.HasAsk)
                    {
                        AskPrice = tick.AskPrice;
                        AskSize = tick.AskSize;
                    }
                    if (tick.HasBid)
                    {
                        BidPrice = tick.BidPrice;
                        BidSize = tick.BidSize;
                    }

                    LastTickEventUtc = tick.OccuredUtc;
                    Price = BidPrice;
                }
                if (tick.IsTrade)
                {
                    TradePrice = tick.TradePrice;
                    LastTradeEventUtc = tick.OccuredUtc;
                    Price = TradePrice;
                }
            }

            //Check for bar
            if (datapoint is Bar bar)
            {
                Open = bar.Open;
                High = bar.High;
                Low = bar.Low;
                Close = bar.Low;
                Price = bar.Close;
            }
        }

        #endregion Public Methods
    }
}