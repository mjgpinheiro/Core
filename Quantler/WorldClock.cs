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

using NLog;
using NodaTime;
using System;
using System.Collections.Generic;

namespace Quantler
{
    /// <summary>
    /// Keeps track of time, converts it to a different timezone on request
    /// </summary>
    public class WorldClock
    {
        #region Private Fields

        /// <summary>
        /// Cached time
        /// </summary>
        private readonly Dictionary<string, CachedTime> _cachedUpdates = new Dictionary<string, CachedTime>();

        /// <summary>
        /// Get current time based on set function
        /// </summary>
        private readonly Func<DateTime> _currentTime;

        /// <summary>
        /// Local timezone
        /// </summary>
        private readonly TimeZone _localBaseTimeZone;

        /// <summary>
        /// Local log
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initialize worldclock based on time function
        /// Backtesting: currenttime => DataPoint.Occured
        /// Livetrading: currenttime => DateTime.UtcNow
        /// </summary>
        /// <param name="currenttime">Function to retrieve what time it is</param>
        /// <param name="basetimezone">Entire framework expects this time to be Utc!</param>
        public WorldClock(Func<DateTime> currenttime, TimeZone basetimezone = TimeZone.Utc)
        {
            _currentTime = currenttime;
            _localBaseTimeZone = basetimezone;
            _log.Info($"Setting up world clock instance with timezone {basetimezone}, current time = {currenttime()}");
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Get the current time based on the supplied timezone
        /// </summary>
        public DateTime Current => _currentTime();

        /// <summary>
        /// Timezone this world clock is based on, usually Etc/UTC
        /// </summary>
        public TimeZone BaseTimeZone => _localBaseTimeZone;

        /// <summary>
        /// Gets the current time in utc.
        /// </summary>
        public DateTime CurrentUtc => BaseTimeZone == TimeZone.Utc ? Current : GetDateTimeInZone(TimeZone.Utc);

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Get current datetime in another timezone
        /// </summary>
        /// <param name="timezone"></param>
        /// <returns></returns>
        public DateTime GetDateTimeInZone(string timezone) =>
            GetDateTimeZone(timezone).ToDateTimeUnspecified();

        /// <summary>
        /// Get current datetime in another timezone
        /// </summary>
        /// <param name="timezone"></param>
        /// <returns></returns>
        public DateTime GetDateTimeInZone(DateTimeZone timezone) =>
            GetDateTimeInZone(timezone.Id);

        /// <summary>
        /// Get current datetime in another timezone
        /// </summary>
        /// <param name="timezone"></param>
        /// <returns></returns>
        public DateTime GetDateTimeInZone(TimeZone timezone) =>
            GetDateTimeInZone(Util.GetEnumDescription(timezone));

        /// <summary>
        /// Get current datetime in another timezone
        /// </summary>
        /// <param name="timezone"></param>
        /// <returns></returns>
        public ZonedDateTime GetDateTimeZone(string timezone)
        {
            //Get current time
            var time = Current;

            //Check cache
            if (!_cachedUpdates.TryGetValue(timezone, out CachedTime cached))
            {
                //Set new item
                cached = new CachedTime
                {
                    DateTime = ToTimeZoneSpecified(time, timezone),
                    DateTimeZone = GetTimezone(timezone),
                    LastUpdate = time.Ticks,
                    TimeZone = timezone
                };

                //Add to cached
                _cachedUpdates.Add(timezone, cached);
            }
            else if (cached.LastUpdate != time.Ticks)
            {
                cached.DateTime = ToTimeZoneSpecified(time, timezone);
                cached.LastUpdate = time.Ticks;
            }

            //Return what we have
            return cached.DateTime;
        }

        /// <summary>
        /// Get timezone
        /// </summary>
        /// <param name="zoneid"></param>
        /// <returns></returns>
        public static DateTimeZone GetTimezone(string zoneid) =>
            DateTimeZoneProviders.Tzdb[zoneid];

        /// <summary>
        /// Get timezone
        /// </summary>
        /// <param name="timezone">The requested timezone</param>
        /// <returns></returns>
        public static DateTimeZone GetTimezone(TimeZone timezone) =>
            DateTimeZoneProviders.Tzdb[Util.GetEnumDescription(timezone)];

        /// <summary>
        /// Convert to another timezone and return an unspecified datetime
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="timezone"></param>
        /// <returns></returns>
        public DateTime ToTimeZone(DateTime datetime, string timezone) =>
            ToTimeZoneSpecified(datetime, timezone).ToDateTimeUnspecified();

        /// <summary>
        /// Convert to another timezone and return an unspecified datetime
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="timezone"></param>
        /// <returns></returns>
        public DateTime ToTimeZone(DateTime datetime, DateTimeZone timezone) =>
            ToTimeZoneSpecified(datetime, timezone.Id).ToDateTimeUnspecified();

        /// <summary>
        /// Convert to another timezone and return a specified datetime
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="timezone"></param>
        /// <returns></returns>
        public ZonedDateTime ToTimeZoneSpecified(DateTime datetime, string timezone) =>
            ConvertDateTimeToDifferentTimeZone(datetime, Util.GetEnumDescription(_localBaseTimeZone), timezone);

        /// <summary>
        /// Convert a given datetime from a timezone to another timezone
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static ZonedDateTime ConvertDateTimeToDifferentTimeZone(DateTime datetime, DateTimeZone from, DateTimeZone to)
        {
            var fromLocal = LocalDateTime.FromDateTime(datetime);
            ZonedDateTime fromDatetime = from.AtStrictly(fromLocal);
            return fromDatetime.WithZone(to);
        }

        /// <summary>
        /// Convert a given datetime from a timezone to another timezone
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static ZonedDateTime ConvertDateTimeToDifferentTimeZone(DateTime datetime, TimeZone from, TimeZone to) =>
            ConvertDateTimeToDifferentTimeZone(datetime, GetTimezone(from), GetTimezone(to));

        /// <summary>
        /// Convert a given datetime from a timezone to another timezone
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="fromzoneid"></param>
        /// <param name="tozoneid"></param>
        /// <returns></returns>
        public static ZonedDateTime ConvertDateTimeToDifferentTimeZone(DateTime datetime, string fromzoneid, string tozoneid) =>
            ConvertDateTimeToDifferentTimeZone(datetime, DateTimeZoneProviders.Tzdb[fromzoneid], DateTimeZoneProviders.Tzdb[tozoneid]);

        #endregion Public Methods

        #region Private Classes

        /// <summary>
        /// Holds time references and lowers the need for time conversions
        /// </summary>
        private class CachedTime
        {
            #region Public Properties

            /// <summary>
            /// Date and time for the given cached moment in the given timezone
            /// </summary>
            public ZonedDateTime DateTime { get; set; }

            /// <summary>
            /// Associated timezone
            /// </summary>
            public DateTimeZone DateTimeZone { get; set; }

            /// <summary>
            /// Last time this value was updated
            /// </summary>
            public long LastUpdate { get; set; }

            /// <summary>
            /// Associated timezone
            /// </summary>
            public string TimeZone { get; set; }

            #endregion Public Properties
        }

        #endregion Private Classes
    }
}