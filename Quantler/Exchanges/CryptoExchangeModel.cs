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

using NodaTime;
using Quantler.Configuration;
using System;
using System.Composition;
using System.Linq;
using Quantler.Data.Bars;

namespace Quantler.Exchanges
{
    /// <summary>
    /// Crypto currency exchangeModel, is always opened and based on UTC timing
    /// </summary>
    [Export(typeof(ExchangeModel))]
    public class CryptoExchangeModel : ExchangeModel
    {
        #region Private Fields

        /// <summary>
        /// World Clock, for current time
        /// </summary>
        private readonly WorldClock _worldClock;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Load crypto exchangeModel model
        /// </summary>
        /// <param name="clock"></param>
        public CryptoExchangeModel(WorldClock clock)
        {
            //Load config
            string configname = "crypto";
            var config = Config.MarketHourConfig.FirstOrDefault(x => x.Exchanges.Select(e => e.ToLower()).Contains(configname.ToLower()));
            if (config == null)
                throw new ArgumentException($"Could not load configuration with name {configname}");

            //Set information
            _worldClock = clock;
            TimeZone = (TimeZone)Enum.Parse(typeof(TimeZone), config.TimeZone);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Does not apply to crypto exchangeModel, so true
        /// </summary>
        public bool ExtendedMarketHours => true;

        /// <summary>
        /// Always true
        /// </summary>
        public bool IsOpen => true; //We are always opened

        /// <summary>
        /// Current local time (utc time)
        /// </summary>
        public DateTime LocalTime => UtcTime;

        /// <summary>
        /// Exchange name
        /// </summary>
        public string Name => "Crypto";

        /// <summary>
        /// Exchange timezone
        /// </summary>
        public TimeZone TimeZone { get; }

        /// <summary>
        /// Amount of trading days per year (crypto 365) => crypto currencies are always open for trading, 365 trading days a year
        /// </summary>
        public int TradingDaysPerYear => 365;

        /// <summary>
        /// Current utc time
        /// </summary>
        public DateTime UtcTime => _worldClock.CurrentUtc;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// We are always opened
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="extendedmarkethours"></param>
        /// <returns></returns>
        public bool IsOpenBetween(DateTime start, DateTime end, bool extendedmarkethours) => true;

        /// <summary>
        /// We are always opened
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool IsOpenBetween(DateTime start, DateTime end) => true;

        /// <summary>
        /// Determines whether [is open during bar] [the specified bar].
        /// </summary>
        /// <param name="bar">The bar.</param>
        /// <returns>
        /// <c>true</c> if [is open during bar] [the specified bar]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsOpenDuringBar(DataPointBar bar) => true;

        /// <summary>
        /// We are always opened
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool IsOpenOnDate(DateTime date) => true;

        /// <summary>
        /// We are always opened
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="extendedmarkethours"></param>
        /// <returns></returns>
        public bool IsOpenOnDateTime(DateTime datetime, bool extendedmarkethours) => true;

        /// <summary>
        /// We are always opened
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public bool IsOpenOnDateTime(DateTime datetime) => true;

        /// <summary>
        /// We are always opened
        /// </summary>
        /// <param name="localtime"></param>
        /// <param name="extendedmarkethours"></param>
        /// <returns></returns>
        public DateTime NextMarketClose(DateTime localtime, bool extendedmarkethours) => NextMarketClose(localtime);

        /// <summary>
        /// We are always opened
        /// </summary>
        /// <param name="localtime"></param>
        /// <returns></returns>
        public DateTime NextMarketClose(DateTime localtime) => new DateTime(localtime.Year, localtime.Month, localtime.Day, 23, 59, 59);

        /// <summary>
        /// We are always opened
        /// </summary>
        /// <param name="localtime"></param>
        /// <returns></returns>
        public DateTime NextMarketOpen(DateTime localtime) => localtime.Date.AddDays(1);

        /// <summary>
        /// We are always opened
        /// </summary>
        /// <param name="localtime"></param>
        /// <param name="extendedmarkethours"></param>
        /// <returns></returns>
        public DateTime NextMarketOpen(DateTime localtime, bool extendedmarkethours) => NextMarketOpen(localtime);

        #endregion Public Methods
    }
}