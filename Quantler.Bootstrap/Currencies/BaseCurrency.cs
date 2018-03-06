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

using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using Quantler.Configuration;
using Quantler.Configuration.Model;
using Quantler.Data;
using Quantler.Interfaces;

namespace Quantler.Bootstrap.Currencies
{
    /// <summary>
    /// Base implementation for currency models
    /// </summary>
    public abstract class BaseCurrency
    {
        #region Protected Fields

        /// <summary>
        /// World clock, so we know what time it is
        /// </summary>
        protected WorldClock Clock;

        /// <summary>
        /// The known currency symbols
        /// </summary>
        protected Dictionary<string, CurrencyType[]> CurrencySymbols = new Dictionary<string, CurrencyType[]>();

        /// <summary>
        /// History of conversion rates
        /// </summary>
        protected Dictionary<string, Dictionary<DateTime, CurrencyRatesConfig>> History;

        /// <summary>
        /// If true, we are currently in backtesting mode
        /// </summary>
        protected bool IsBacktest;

        #endregion Protected Fields

        #region Public Properties

        /// <summary>
        /// Minimum date loaded
        /// </summary>
        public DateTime LoadedFromUtc
        {
            private set;
            get;
        }

        /// <summary>
        /// Maximum date loaded
        /// </summary>
        public DateTime LoadedToUtc
        {
            private set;
            get;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        public virtual decimal Convert(decimal value, CurrencyType from, CurrencyType to)
        {
            //Check if conversion is needed
            if (from == to)
                return value;

            //Check conversion rate
            var container = GetLastKnownConversion(Clock.CurrentUtc, from);
            if (container == null || !container.Rates.TryGetValue(to.ToString(), out decimal conversionrate))
                throw new Exception($"No conversion rate for combination {from}/{to} is known, did we load any conversion data?");
            else
                return value * conversionrate;
        }

        /// <summary>
        /// Initializes the current instance
        /// </summary>
        /// <param name="clock">The clock.</param>
        /// <param name="isbacktest">if set to <c>true</c> [isbacktest].</param>
        public virtual void Initialize(WorldClock clock, bool isbacktest)
        {
            //Set items
            Clock = clock;
            IsBacktest = isbacktest;

            //Set all possible currency symbol combinations
            var found = Enum.GetNames(typeof(CurrencyType));
            found.ForEach(basecurrency =>
            {
                var commodities = Enum.GetNames(typeof(CurrencyType));
                commodities.ForEach(com =>
                {
                    CurrencySymbols.Add(basecurrency + "/" + com, new[]
                    {
                        (CurrencyType)Enum.Parse(typeof(CurrencyType), basecurrency),
                        (CurrencyType)Enum.Parse(typeof(CurrencyType), com)
                    });
                });
            });

            //Get from config
            History = GetFromConfig();
            UpdateRange();
        }

        /// <summary>
        /// Updates the currency conversion based on tick received
        /// </summary>
        /// <param name="updates">The current data updates.</param>
        public void Update(DataUpdates updates)
        {
            foreach (var tick in updates.Ticks.Values.SelectMany(x => x))
            {
                //Check symbol
                if (!CurrencySymbols.ContainsKey(tick.Ticker.Name))
                    return;

                //get base rate
                var baserate = CurrencySymbols[tick.Ticker.Name][0].ToString();
                var torate = CurrencySymbols[tick.Ticker.Name][1].ToString();
                var date = tick.OccuredUtc.Date;

                //Set pricing
                decimal price = tick.IsFullQuote ? Math.Abs(tick.AskPrice + tick.BidPrice) / 2 : tick.Price;
                if (!History.TryGetValue(baserate, out Dictionary<DateTime, CurrencyRatesConfig> his))
                    if (!his.TryGetValue(date, out CurrencyRatesConfig config))
                        config.Rates[torate] = price;
                    else
                        his.Add(date, new CurrencyRatesConfig
                        {
                            Base = baserate,
                            Date = date,
                            Rates = new Dictionary<string, decimal> { { torate, price } }
                        });
                else
                    History.Add(baserate, new Dictionary<DateTime, CurrencyRatesConfig>
                    {
                        {
                            date, new CurrencyRatesConfig
                            {
                                Base = baserate,
                                Date = date,
                                Rates = new Dictionary<string, decimal> { { torate, price } }
                            }
                        }
                    });
            }
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Get last known currency conversion rate, returns null if we cannot find any
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        protected CurrencyRatesConfig GetLastKnownConversion(DateTime date, CurrencyType @base)
        {
            //Check values
            if (History[@base.ToString()].TryGetValue(date.Date, out CurrencyRatesConfig found))
                return found;               //We found a conversion rate
            else if (date.Year <= 1988)
                return null;                //We have gone to far back, 1988 no need to search any longer, else we end up with dinosaurs
            else
                return GetLastKnownConversion(date.AddDays(-1), @base); //Lets try one day earlier
        }

        /// <summary>
        /// Updates the specified date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="base">The base.</param>
        /// <param name="rate">The rate.</param>
        protected void Update(DateTime date, CurrencyType @base, CurrencyRatesConfig rate)
        {
            //Update locally
            string basecurrency = @base.ToString();
            if (!History.TryGetValue(basecurrency, out Dictionary<DateTime, CurrencyRatesConfig> rates))
                rates[date.Date] = rate;
            else
                History.Add(basecurrency, new Dictionary<DateTime, CurrencyRatesConfig>
                {
                    {date.Date, rate }
                });

            //Update config file
            UpdateConfig(date, @base, rate);
            UpdateRange();
        }

        /// <summary>
        /// Update currently loaded range
        /// </summary>
        protected void UpdateRange()
        {
            LoadedFromUtc = History.Max(x => x.Value.Min(n => n.Key)).Date;
            LoadedToUtc = History.Min(x => x.Value.Max(n => n.Key)).Date;
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Parses config file for usage
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, Dictionary<DateTime, CurrencyRatesConfig>> GetFromConfig() =>
                            Config.CurrencyConfig.GroupBy(x => x.Base)
                                                 .ToDictionary(x => x.Key, x => x.ToDictionary(n => n.Date));

        /// <summary>
        /// Update current config file with new information
        /// </summary>
        /// <param name="date"></param>
        /// <param name="base"></param>
        /// <param name="rate"></param>
        private void UpdateConfig(DateTime date, CurrencyType @base, CurrencyRatesConfig rate)
        {
            //Set proper date
            date = date.Date;

            //Get current config
            var currentconfig = GetFromConfig();

            //Check if we need to add
            var current = currentconfig[@base.ToString()];
            if (!current.ContainsKey(date))
                Config.AppendConfigFile(currentconfig, rate);
        }

        #endregion Private Methods
    }
}