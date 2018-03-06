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
using System.Composition;
using System.Linq;
using NLog;
using Quantler.Api.Fixer;
using Quantler.Interfaces;

namespace Quantler.Bootstrap.Currencies
{
    /// <summary>
    /// Implements fiat currency conversion
    /// During live trading, the following API is used: http://api.fixer.io
    /// During backtesting, we backfill based on config file
    /// </summary>
    [Export(typeof(Currency))]
    public class FiatCurrency : BaseCurrency, Currency
    {
        #region Private Fields

        /// <summary>
        /// Logging instance
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The compatible currency types
        /// </summary>
        private readonly CurrencyType[] _compatibleCurrencyTypes = 
        {
            CurrencyType.AUD,
            CurrencyType.BRL,
            CurrencyType.CAD,
            CurrencyType.CHF,
            CurrencyType.CNY,
            CurrencyType.EUR,
            CurrencyType.GBP,
            CurrencyType.HKD,
            CurrencyType.IDR,
            CurrencyType.JPY,
            CurrencyType.KRW,
            CurrencyType.USD,
            CurrencyType.RUB,
            CurrencyType.MXN,
            CurrencyType.INR
        };

        #endregion Private Fields

        #region Public Methods

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

            //Check if we have updates
            if (Clock.CurrentUtc.DayOfWeek == DayOfWeek.Saturday || Clock.CurrentUtc.DayOfWeek == DayOfWeek.Sunday)
                return; //No weekend updates

            DateTime currenTime = Clock.CurrentUtc.ConvertTo(TimeZone.Utc, TimeZone.CET);

            if (currenTime.Hour < 17)
                return; //No updates before 16:00 CET
            if (History.Count(x => x.Value.Max(n => n.Key).Date < currenTime.Date) == 0)
                return; //Check if we need to update

            //we need to update current rates
            _log.Info("Updating current fiat currency rates...");

            //get from api
            var client = new FixerApi();
            foreach (var basecurrency in _compatibleCurrencyTypes)
            {
                try
                {
                    var found = client.GetLatestConversionRatesAsync(basecurrency).Result;
                    if (found != null)
                        Update(found.Date, basecurrency, found);
                }
                catch (Exception exc)
                {
                    _log.Warn(exc, $"Could not process fiat currency request for retrieving new currency updates for base currency {basecurrency}");
                }
            }

            //Update currently loaded range
            UpdateRange();
        }

        #endregion Public Methods
    }
}