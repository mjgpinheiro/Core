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
using System.Composition;
using System.Linq;
using MoreLinq;
using NLog;
using Quantler.Api.CoinMarketCap;
using Quantler.Configuration.Model;
using Quantler.Interfaces;

namespace Quantler.Bootstrap.Currencies
{
    /// <summary>
    /// Implements crypto currency conversion
    /// </summary>
    [Export(typeof(Currency))]
    public class CryptoCurrency : BaseCurrency, Currency
    {
        #region Private Fields

        /// <summary>
        /// The coinmarketcap API
        /// </summary>
        private readonly CoinmarketcapApi _coinmarketcapApi = new CoinmarketcapApi();

        /// <summary>
        /// Logging instance
        /// </summary>
        private ILogger _log = LogManager.GetCurrentClassLogger();

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Compatible for indirect cross conversions
        /// </summary>
        /// <param name="value"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public override decimal Convert(decimal value, CurrencyType from, CurrencyType to)
        {
            //Get currency ticker symbol
            string ticker = $"{from}/{to}";
            if (CurrencySymbols.ContainsKey(ticker))
                return base.Convert(value, from, to);

            //Check for crossing indirect
            ticker = $"{to}/{from}";
            if (CurrencySymbols.ContainsKey(ticker))
            {
                //get value
                var conversion = 1 / GetLastKnownConversion(Clock.CurrentUtc, to).Rates[from.ToString()];
                return value / conversion;
            }
            throw new Exception($"No possible direct or indirect conversion possible between currency {@from} and {to}");
        }

        /// <summary>
        /// Update current currency rates from source
        /// </summary>
        public void Update()
        {
            //Check date ranges
            UpdateRange();

            //check if we need to update
            if (IsBacktest)
                return;

            //Check if we need to update
            if (History.Count(x => x.Value.Max(n => n.Key).Date < Clock.CurrentUtc.Date) == 0)
                return;

            //we need to update current rates
            _log.Info("Updating current crypto currency rates...");

            //Get from API
            //Possible: "AUD", "BRL", "CAD", "CHF", "CNY", "EUR", "GBP", "HKD", "IDR", "INR", "JPY", "KRW", "MXN", "RUB"
            CurrencyType[] convertcurrency = { CurrencyType.AUD, CurrencyType.BRL, CurrencyType.CAD, CurrencyType.CHF, CurrencyType.CNY, CurrencyType.EUR,
            CurrencyType.GBP, CurrencyType.HKD, CurrencyType.IDR, CurrencyType.INR, CurrencyType.JPY, CurrencyType.KRW, CurrencyType.MXN, CurrencyType.RUB };
            string[] cryptoids = { "bitcoin", "ethereum", "tether"};
            Dictionary<string, CurrencyType> tograb = new Dictionary<string, CurrencyType>();
            cryptoids.ForEach(crypto => convertcurrency.ForEach(basec => tograb.Add(crypto, basec)));

            //Get for each currency
            foreach (var item in tograb)
            {
                //@base
                CurrencyType @base = CurrencyType.BTC;

                //Conversions found
                Dictionary<string, decimal> conversions = new Dictionary<string, decimal>();

                //Grab all conversions
                foreach (var basecurrency in tograb.Values)
                {
                    //Check response
                    try
                    {
                        //Get the data
                        var responseObject = _coinmarketcapApi.GetTickerAsync(item.Key, basecurrency).Result;

                        //Parse data
                        @base = (CurrencyType)Enum.Parse(typeof(CurrencyType), responseObject["symbol"]);
                        conversions.Add(@base.ToString(), decimal.Parse(responseObject["price_" + basecurrency]));
                    }
                    catch (Exception exc)
                    {
                        _log.Warn(exc, $"Could not update currency from coinmarketcap, ticker = {item}, base = {basecurrency}");
                    }
                }

                //Update current config
                Update(Clock.CurrentUtc.Date, @base, new CurrencyRatesConfig
                {
                    Base = @base.ToString(),
                    Date = Clock.CurrentUtc.Date,
                    Rates = conversions
                });
            }

            //Update currently loaded range
            UpdateRange();
        }

        #endregion Public Methods
    }
}