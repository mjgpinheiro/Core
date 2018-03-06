#region License Header

/*
* QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
* Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
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
*
* Modifications Copyright 2018 Quantler B.V.
*
*/

#endregion License Header

using NodaTime;
using System;
using System.Globalization;

namespace Quantler
{
    /// <summary>
    /// Time related functions
    /// </summary>
    public static class Time
    {
        #region Public Fields

        /// <summary>
        /// Timespan of one day
        /// </summary>
        public static readonly TimeSpan OneDay = TimeSpan.FromDays(1);

        /// <summary>
        /// Timespan of one hour
        /// </summary>
        public static readonly TimeSpan OneHour = TimeSpan.FromHours(1);

        /// <summary>
        /// Timespan of one millisecond
        /// </summary>
        public static readonly TimeSpan OneMilliSecond = TimeSpan.FromMilliseconds(1);

        /// <summary>
        /// Timespan of one minute
        /// </summary>
        public static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Timespan of one second
        /// </summary>
        public static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

        #endregion Public Fields

        #region Public Methods

        /// <summary>
        /// Convert unix timestamp to DateTime
        /// </summary>
        /// <param name="unixtime"></param>
        /// <param name="includemilliseconds"></param>
        /// <returns></returns>
        public static DateTime FromUnixTime(long unixtime, bool includemilliseconds = false) =>
            includemilliseconds ?
            DateTimeOffset.FromUnixTimeMilliseconds(unixtime).DateTime :
            DateTimeOffset.FromUnixTimeSeconds(unixtime).DateTime;

        /// <summary>
        /// Convert UTC time to given timezone
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="timezone"></param>
        /// <returns></returns>
        public static DateTime FromUtc(this DateTime datetime, DateTimeZone timezone) =>
             Instant.FromDateTimeUtc(datetime)
                    .InZone(timezone)
                    .ToDateTimeUnspecified();

        /// <summary>
        /// Parse a standard YY MM DD date into a DateTime. Attempt common date formats
        /// </summary>
        /// <param name="dateToParse">String date time to parse</param>
        /// <param name="date">Safely converted date</param>
        /// <returns>Date time</returns>
        public static bool TryParseDate(string dateToParse, out DateTime date)
        {
            try
            {
                //First try the exact options:
                if (DateTime.TryParseExact(dateToParse, DateFormat.SixCharacter, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out date))
                    return true;
                if (DateTime.TryParseExact(dateToParse, DateFormat.EightCharacter, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out date))
                    return true;
                if (DateTime.TryParseExact(dateToParse, DateFormat.EightCharacterLined, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out date))
                    return true;
                if (DateTime.TryParseExact(dateToParse.Substring(0, 19), DateFormat.JsonFormat,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    return true;
                if (DateTime.TryParseExact(dateToParse, DateFormat.US, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out date))
                    return true;
                if (DateTime.TryParse(dateToParse, out date))
                    return true;
            }
            catch (Exception err)
            {

            }

            date = DateTime.MinValue;
            return false;
        }

        /// <summary>
        /// Check if the given timezone is in fact in UTC (returns false if this is not the case)
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static bool IsUtc(DateTime datetime) =>
            datetime.Kind == DateTimeKind.Utc;

        /// <summary>
        /// Convert to unix timestamp, expects UTC format as input
        /// TODO: check Quantler.Logic, datetimeoffset is inaccurate
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="includemilliseconds"></param>
        /// <returns></returns>
        public static long ToUnixTime(this DateTime datetime, bool includemilliseconds = false) =>
            datetime.Kind != DateTimeKind.Utc ? throw new ArgumentException("Expected UTC input in ToUnixTime conversion") :
            includemilliseconds ?
            new DateTimeOffset(datetime, TimeSpan.Zero).ToUnixTimeMilliseconds() :
            new DateTimeOffset(datetime, TimeSpan.Zero).ToUnixTimeSeconds();

        /// <summary>
        /// Convert datetime to utc datetime
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime ToUtc(this DateTime datetime) =>
            datetime.ToUniversalTime();

        /// <summary>
        /// Rounds the specified date time in the specified time zone
        /// </summary>
        /// <param name="dateTime">Date time to be rounded</param>
        /// <param name="roundingInterval">Timespan rounding period</param>
        /// <param name="sourceTimeZone">Time zone of the date time</param>
        /// <param name="roundingTimeZone">Time zone in which the rounding is performed</param>
        /// <returns>The rounded date time in the source time zone</returns>
        public static DateTime RoundDownInTimeZone(this DateTime dateTime, TimeSpan roundingInterval, TimeZone sourceTimeZone, TimeZone roundingTimeZone)
        {
            var dateTimeInRoundingTimeZone = dateTime.ConvertTo(sourceTimeZone, roundingTimeZone);
            var roundedDateTimeInRoundingTimeZone = dateTimeInRoundingTimeZone.RoundDown(roundingInterval);
            return roundedDateTimeInRoundingTimeZone.ConvertTo(roundingTimeZone, sourceTimeZone);
        }

        /// <summary>
        /// Converts the specified time from the <paramref name="from"/> time zone to the <paramref name="to"/> time zone
        /// </summary>
        /// <param name="time">The time to be converted in terms of the <paramref name="from"/> time zone</param>
        /// <param name="from">The time zone the specified <paramref name="time"/> is in</param>
        /// <param name="to">The time zone to be converted to</param>
        /// <param name="strict">True for strict conversion, this will throw during ambiguitities, false for lenient conversion</param>
        /// <returns>The time in terms of the to time zone</returns>
        public static DateTime ConvertTo(this DateTime time, TimeZone from, TimeZone to, bool strict = false) =>
            from == to ? time : ConvertTo(time, WorldClock.GetTimezone(from), WorldClock.GetTimezone(to), strict);

        /// <summary>
        /// Converts the specified time from the <paramref name="from"/> time zone to the <paramref name="to"/> time zone
        /// </summary>
        /// <param name="time">The time to be converted in terms of the <paramref name="from"/> time zone</param>
        /// <param name="from">The time zone the specified <paramref name="time"/> is in</param>
        /// <param name="to">The time zone to be converted to</param>
        /// <param name="strict">True for strict conversion, this will throw during ambiguitities, false for lenient conversion</param>
        /// <returns>The time in terms of the to time zone</returns>
        public static DateTime ConvertTo(this DateTime time, DateTimeZone from, DateTimeZone to, bool strict = false)
        {
            if (ReferenceEquals(from, to)) return time;

            if (strict)
            {
                return from.AtStrictly(LocalDateTime.FromDateTime(time)).WithZone(to).ToDateTimeUnspecified();
            }

            return from.AtLeniently(LocalDateTime.FromDateTime(time)).WithZone(to).ToDateTimeUnspecified();
        }

        /// <summary>
        /// Get year start
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime Year(int year) =>
            new DateTime(year, 1, 1);

        #endregion Public Methods
    }
}