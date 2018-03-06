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
using Quantler.Data.Bars;
using Quantler.Exchanges.Sessions;
using System;

namespace Quantler.Exchanges
{
    /// <summary>
    /// Base Exchange Model, derive from this exchangeModel model
    /// </summary>
    public class BaseExchangeModel : ExchangeModel
    {
        #region Public Properties

        /// <summary>
        /// True if extended market hours should be used
        /// </summary>
        public bool ExtendedMarketHours { get; set; }

        /// <summary>
        /// Returns true if the current market is open (takes into account if extended market hours is used)
        /// </summary>
        public bool IsOpen => IsOpenOnDateTime(LocalTime, ExtendedMarketHours);

        /// <summary>
        /// Get the exchangeModel based local time
        /// </summary>
        public DateTime LocalTime =>
            WorldClock.GetDateTimeInZone(TimeZone);

        /// <summary>
        /// Exchange name
        /// </summary>
        public string Name { get; protected set; } = "QUANTLER";

        /// <summary>
        /// Get the current timezone information used for this exchangeModel
        /// </summary>
        public TimeZone TimeZone { get; protected set; }

        /// <summary>
        /// Total amount of trading days in a regular year for this exchangeModel
        /// </summary>
        public int TradingDaysPerYear { get; protected set; } = 252;

        /// <summary>
        /// Get the current time in Utc
        /// </summary>
        public DateTime UtcTime =>
            WorldClock.CurrentUtc;

        #endregion Public Properties

        #region Protected Properties

        /// <summary>
        /// Holidays holder
        /// </summary>
        protected LocalHoliday LocalHoliday { get; set; }

        /// <summary>
        /// Market sessions for full days holder
        /// </summary>
        protected LocalMarketSessionDay LocalMarketSessionDay { get; set; }

        /// <summary>
        /// Current world clock for current time and time conversion
        /// </summary>
        protected WorldClock WorldClock { get; set; }

        #endregion Protected Properties

        #region Public Methods

        /// <summary>
        /// Determines whether [is open between] [the specified localstart].
        /// </summary>
        /// <param name="localstart">The localstart.</param>
        /// <param name="localend">The localend.</param>
        /// <returns>
        ///   <c>true</c> if [is open between] [the specified localstart]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsOpenBetween(DateTime localstart, DateTime localend) =>
            IsOpenBetween(localstart, localend, ExtendedMarketHours);

        /// <summary>
        /// Determines whether [is open between] [the specified localstart].
        /// </summary>
        /// <param name="localstart">The localstart.</param>
        /// <param name="localend">The localend.</param>
        /// <param name="extendedmarkethours">if set to <c>true</c> [extendedmarkethours].</param>
        /// <returns>
        ///   <c>true</c> if [is open between] [the specified localstart]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsOpenBetween(DateTime localstart, DateTime localend, bool extendedmarkethours)
        {
            //Check if it contains different dates
            if (localstart.Date == localend.Date)
                return IsOpenOnDateTime(localstart, extendedmarkethours) &&
                       IsOpenOnDateTime(localend, extendedmarkethours);

            //Easy check
            else if (!IsOpenOnDateTime(localstart, extendedmarkethours) || !IsOpenOnDateTime(localend, extendedmarkethours))
                return false;

            //Check on longer than one day
            else if (localend > NextMarketClose(localstart, extendedmarkethours))
                return false;

            //If we are here, we are open
            else return true;
        }

        /// <summary>
        /// Determines whether [is open during bar] [the specified bar].
        /// </summary>
        /// <param name="bar">The bar.</param>
        /// <returns>
        ///   <c>true</c> if [is open during bar] [the specified bar]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsOpenDuringBar(DataPointBar bar) =>
            bar.TimeZone == TimeZone
                ? IsOpenBetween(bar.Occured, bar.EndTime)
                : IsOpenBetween(bar.OccuredUtc.ConvertTo(TimeZone.Utc, TimeZone),
                    bar.EndTime.ConvertTo(bar.TimeZone, TimeZone));

        /// <summary>
        /// Determines whether [is open on date] [the specified localtime].
        /// </summary>
        /// <param name="localtime">The localtime.</param>
        /// <returns>
        ///   <c>true</c> if [is open on date] [the specified localtime]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsOpenOnDate(DateTime localtime) =>
            GetDaysSession(localtime).MarketOpenTime(localtime.DayOfWeek, true) < TimeSpan.MaxValue;

        /// <summary>
        /// Determines whether [is open on date time] [the specified localtime].
        /// </summary>
        /// <param name="localtime">The localtime.</param>
        /// <returns>
        ///   <c>true</c> if [is open on date time] [the specified localtime]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsOpenOnDateTime(DateTime localtime) =>
            IsOpenOnDateTime(localtime, ExtendedMarketHours);

        /// <summary>
        /// Check if this exchangeModel is open on the provided date and time
        /// </summary>
        /// <param name="localtime"></param>
        /// <param name="extendedmarkethours"></param>
        /// <returns></returns>
        public bool IsOpenOnDateTime(DateTime localtime, bool extendedmarkethours) =>
            GetDaysSession(localtime).IsOpenOn(localtime.DayOfWeek, localtime.TimeOfDay, extendedmarkethours);

        /// <summary>
        /// Get date and time the next closing of the market is
        /// </summary>
        /// <param name="localtime"></param>
        /// <returns></returns>
        public DateTime NextMarketClose(DateTime localtime) =>
            NextMarketClose(localtime, ExtendedMarketHours);

        /// <summary>
        /// Get date and time the next closing of the market is
        /// </summary>
        /// <param name="localtime"></param>
        /// <param name="extendedmarkethours"></param>
        /// <returns></returns>
        public DateTime NextMarketClose(DateTime localtime, bool extendedmarkethours)
        {
            //Check for a period of time, we have to be closed some day
            DateTime maxcheck = localtime.AddDays(14);
            DateTime current = localtime;

            //Check on dates
            do
            {
                //Get day close
                var close = GetDaysSession(current).MarketCloseTime(current.DayOfWeek, extendedmarkethours);

                //Check if it closes today (or is it open 24 until midnight?)
                if (close != Time.OneDay)
                    return current.Add(close);

                //Add another day we have checked
                current = current.Add(Time.OneDay);

                //Check if we are not opening during midnight (like 01:00)
                var open = GetDaysSession(current).MarketOpenTime(current.DayOfWeek, extendedmarkethours);
                if (open != TimeSpan.MinValue)
                    return current;
            } while (current < maxcheck);

            //Should not happen, right?
            return DateTime.MaxValue;
        }

        /// <summary>
        /// Next moment in time the market will open.
        /// </summary>
        /// <param name="localtime">The local time.</param>
        /// <returns></returns>
        public DateTime NextMarketOpen(DateTime localtime) =>
            NextMarketOpen(localtime, ExtendedMarketHours);

        /// <summary>
        /// Next moment in time the market will open.
        /// </summary>
        /// <param name="localtime">The local time.</param>
        /// <param name="extendedmarkethours">if set to <c>true</c> [extendedmarkethours].</param>
        /// <returns></returns>
        public DateTime NextMarketOpen(DateTime localtime, bool extendedmarkethours)
        {
            //Check for a period of time, we have to be open some day
            DateTime maxcheck = localtime.AddDays(14);
            DateTime current = localtime.AddDays(1).Date;

            //Check on dates
            do
            {
                //Get day open
                var open = GetDaysSession(current).MarketOpenTime(current.DayOfWeek, extendedmarkethours);

                //Check if it closes today (or is it open 24 until midnight?)
                if (open != TimeSpan.MaxValue)
                    return current.Add(open);

                //Add another day we have checked
                current = current.Add(Time.OneDay);
            } while (current < maxcheck);

            //Should not happen, right?
            return DateTime.MaxValue;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Returns the local market session day instance and checks for holidays
        /// </summary>
        /// <param name="localtime"></param>
        /// <returns></returns>
        private LocalMarketSessionDay GetDaysSession(DateTime localtime) =>
            LocalHoliday.IsHoliday(localtime.Date) ?
            LocalHoliday.GetHolidaySession(localtime.Date) :
            LocalMarketSessionDay;

        #endregion Private Methods
    }
}