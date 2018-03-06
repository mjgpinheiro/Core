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

using Quantler.Configuration.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quantler.Exchanges.Sessions
{
    /// <summary>
    /// Helper functions for loading holidays from a config file
    /// </summary>
    public static class HolidayFunctions
    {
        #region Public Methods

        /// <summary>
        /// Load all market session days for holidays derived from configurations
        /// </summary>
        /// <param name="year"></param>
        /// <param name="configfiles"></param>
        /// <returns></returns>
        public static Dictionary<MarketHoursHolidayConfig, DateTime> GetHolidaySessions(int year, IEnumerable<MarketHoursHolidayConfig> configfiles, LocalMarketSessionDay regular)
        {
            //Save found holidays for depended holidays
            Dictionary<MarketHoursHolidayConfig, DateTime> knownholidays = new Dictionary<MarketHoursHolidayConfig, DateTime>();

            //Check on regular holidays
            foreach (var config in configfiles.Where(x => !x.Date.DaysAfterHoliday.HasValue && !x.Date.DaysBeforeHoliday.HasValue))
            {
                //Set current date
                DateTime day = Time.Year(year);

                //Get day of week, if applicable
                DayOfWeek? dow = null;
                if (!string.IsNullOrWhiteSpace(config.Date.DayOfWeek))
                    dow = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), config.Date.DayOfWeek);

                //Check config
                if (config.Name.ToLower() == "easter sunday")
                    day = EasterSunday(year);

                //Fixed Month
                if (config.Date.Month.HasValue)
                    day = day.AddMonths(config.Date.Month.Value - 1);
                //Fixed day in month
                if (config.Date.Day.HasValue)
                    day = day.AddDays(config.Date.Day.Value - 1);
                //Check for week of month
                if (config.Date.WeekOfMonth.HasValue)
                    day = day.AddDays(7 * (config.Date.WeekOfMonth.Value - 1));
                //Fixed day of week that occurred
                if (config.Date.DayOccurance.HasValue && dow != null)
                {
                    //Find the moment this day of week occurs
                    int occurance = day.DayOfWeek == dow ? 1 : 0;

                    while (occurance != config.Date.DayOccurance.Value)
                    {
                        day = day.AddDays(1);
                        if (day.DayOfWeek == dow)
                            occurance++;
                    }
                }
                //Check last day occurrence
                if (config.Date.IsLastDayOccurance.HasValue && config.Date.IsLastDayOccurance.Value)
                {
                    //Find the moment this day of week occurs
                    DateTime lasttimefound = day;
                    DateTime nextmonthoccurance = day.AddDays(60);

                    while (lasttimefound < nextmonthoccurance)
                    {
                        day = day.Add(Time.OneDay);
                        if (day.DayOfWeek == dow && day.Month == lasttimefound.Month)
                            lasttimefound = day;
                    }

                    //Set last time we found this day in that month
                    day = lasttimefound;
                }
                //Check if this holiday is only every x amount of years
                if (config.Date.StartYear.HasValue && config.Date.EveryXYear.HasValue)
                {
                    int startyear = config.Date.StartYear.Value;
                    int everyxyear = config.Date.EveryXYear.Value;

                    //Skip it if it is not in our year
                    if ((year - startyear) % everyxyear > 0)
                        continue;
                }

                //Add to known holidays
                knownholidays.Add(config, day);
            }

            //Check on derived holidays
            foreach (var config in configfiles.Where(x => x.Date.DaysAfterHoliday.HasValue || x.Date.DaysBeforeHoliday.HasValue))
            {
                //Set current date
                DateTime day = Time.Year(year);

                //Get holiday if known
                DateTime referencedholiday = knownholidays.First(x => String.Equals(x.Key.Name, config.Date.BeforeHoliday, StringComparison.CurrentCultureIgnoreCase) ||
                                                                      String.Equals(x.Key.Name, config.Date.AfterHoliday, StringComparison.CurrentCultureIgnoreCase))
                                                          .Value;

                //Before holiday?
                if (config.Date.DaysBeforeHoliday.HasValue)
                    day = referencedholiday.AddDays(-config.Date.DaysBeforeHoliday.Value);
                //After holiday?
                else if (config.Date.DaysAfterHoliday.HasValue)
                    day = referencedholiday.AddDays(config.Date.DaysAfterHoliday.Value);

                //Add to known holidays
                knownholidays.Add(config, day);
            }

            //Return what we know
            return knownholidays;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Helper function for retrieving the easter sunday
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        private static void EasterSunday(int year, ref int month, ref int day)
        {
            int g = year % 19;
            int c = year / 100;
            int h = h = (c - (int)(c / 4) - (int)((8 * c + 13) / 25)
                                                + 19 * g + 15) % 30;
            int i = h - (int)(h / 28) * (1 - (int)(h / 28) *
                        (int)(29 / (h + 1)) * (int)((21 - g) / 11));

            day = i - ((year + (int)(year / 4) +
                          i + 2 - c + (int)(c / 4)) % 7) + 28;
            month = 3;

            if (day > 31)
            {
                month++;
                day -= 31;
            }
        }

        /// <summary>
        /// Get easter sunday for given year
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        private static DateTime EasterSunday(int year)
        {
            int month = 0;
            int day = 0;
            EasterSunday(year, ref month, ref day);

            return new DateTime(year, month, day);
        }

        #endregion Private Methods
    }
}