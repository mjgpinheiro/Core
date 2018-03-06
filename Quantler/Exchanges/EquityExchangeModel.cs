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

using Quantler.Configuration;
using Quantler.Exchanges.Sessions;
using System;
using System.Composition;
using System.Linq;

namespace Quantler.Exchanges
{
    /// <summary>
    /// Equity exchangeModel model
    /// </summary>
    [Export(typeof(ExchangeModel))]
    public class EquityExchangeModel : BaseExchangeModel
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EquityExchangeModel"/> class.
        /// </summary>
        /// <param name="configexchangename">The configexchangename.</param>
        /// <param name="clock">The clock.</param>
        /// <param name="extendedmarkethours">if set to <c>true</c> [extendedmarkethours].</param>
        /// <exception cref="ArgumentException">
        /// Configuration exchangeModel name cannot be empty.
        /// </exception>
        public EquityExchangeModel(string configexchangename, WorldClock clock, bool extendedmarkethours)
        {
            //Check configuration
            if (string.IsNullOrWhiteSpace(configexchangename))
                throw new ArgumentException("Configuration exchangeModel name cannot be empty.");

            //Load config
            var config = Config.MarketHourConfig.FirstOrDefault(x => x.Exchanges.Contains(configexchangename));
            if (config == null)
                throw new ArgumentException($"Could not load configuration with name {configexchangename}");

            //Set information
            WorldClock = clock;
            TimeZone = (TimeZone) Enum.Parse(typeof(TimeZone), config.TimeZone);
            LocalMarketSessionDay = new LocalMarketSessionDay(configexchangename);
            LocalHoliday = new LocalHoliday(configexchangename, LocalMarketSessionDay);
            Name = configexchangename;
            ExtendedMarketHours = extendedmarkethours;
        }

        #endregion Public Constructors
    }
}