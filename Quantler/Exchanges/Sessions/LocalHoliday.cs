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
using Quantler.Configuration.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quantler.Exchanges.Sessions
{
    /// <summary>
    /// Local holiday instance information
    /// </summary>
    public class LocalHoliday
    {
        #region Private Fields

        /// <summary>
        /// The known holidays
        /// </summary>
        private readonly Dictionary<DateTime, LocalMarketSessionDay> _knownHolidays;

        /// <summary>
        /// The loaded years
        /// </summary>
        private readonly List<int> _loadedYears = new List<int>();

        /// <summary>
        /// The market hours configuration
        /// </summary>
        private readonly MarketHoursConfig _marketHoursConfig;

        /// <summary>
        /// The regular trading week (without deviations)
        /// </summary>
        private readonly LocalMarketSessionDay _regular;

        /// <summary>
        /// The exchangeModel name
        /// </summary>
        private readonly string _exchangename;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Local holiday keeper and loader
        /// </summary>
        /// <param name="exchangename"></param>
        /// <param name="regular"></param>
        public LocalHoliday(string exchangename, LocalMarketSessionDay regular)
        {
            _exchangename = exchangename;
            _knownHolidays = new Dictionary<DateTime, LocalMarketSessionDay>();
            _marketHoursConfig = Config.MarketHourConfig.FirstOrDefault(x => x.Exchanges.Contains(exchangename));
            _regular = regular;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Get holiday session if applicable
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public LocalMarketSessionDay GetHolidaySession(DateTime date)
        {
            //Check if date is loaded, if not pre-fill to future as well
            if (!IsLoaded(date))
                LoadHolidays(date.Year);

            //Check if there is a holiday
            if (_knownHolidays.ContainsKey(date.Date))
                return _knownHolidays[date.Date];
            else
                return null;
        }

        /// <summary>
        /// True if this day is a holiday, therefore it might have different opening and closing times
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool IsHoliday(DateTime date) => GetHolidaySession(date) != null;

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// True if this date is loaded
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private bool IsLoaded(DateTime date) =>
            _loadedYears.Contains(date.Year);

        /// <summary>
        /// Load holidays between dates
        /// </summary>
        /// <param name="year"></param>
        private void LoadHolidays(int year)
        {
            //Check input
            if (_marketHoursConfig == null)
                throw new Exception("Market hour configuration is not loaded, cannot derive holidays");

            //Check on known holidays
            var holidays = HolidayFunctions.GetHolidaySessions(year, _marketHoursConfig.Holidays, _regular);

            //Check on result
            foreach (var holiday in holidays)
            {
                //Check if we are fully closed
                if (string.IsNullOrWhiteSpace(holiday.Key.Open) &&
                    string.IsNullOrWhiteSpace(holiday.Key.Closed))
                    _knownHolidays.Add(holiday.Value, new LocalMarketSessionDay(_exchangename, holiday.Value.DayOfWeek, _regular.GetClosedDay("Holiday: " + holiday.Key.Name)));
                else
                {
                    //Check for open time
                    TimeSpan? open = null;
                    if (!string.IsNullOrWhiteSpace(holiday.Key.Open))
                        open = TimeSpan.Parse(holiday.Key.Open);

                    //Check for closed time
                    TimeSpan? close = null;
                    if (!string.IsNullOrWhiteSpace(holiday.Key.Closed))
                        close = TimeSpan.Parse(holiday.Key.Closed);

                    //Set returned deviated day
                    _knownHolidays.Add(holiday.Value, new LocalMarketSessionDay(_exchangename, holiday.Value.DayOfWeek, _regular.GetDeviatedDay(holiday.Value.DayOfWeek, open, close, "Holiday: " + holiday.Key.Name)));
                }
            }

            //Set new known year
            _loadedYears.Add(year);
        }

        #endregion Private Methods
    }
}