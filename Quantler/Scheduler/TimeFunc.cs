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
using Quantler.Exchanges;
using System;

namespace Quantler.Scheduler
{
    /// <summary>
    /// Time composite
    /// </summary>
    /// <seealso cref="TimeComposite" />
    public class TimeFunc : TimeComposite
    {
        #region Private Fields

        /// <summary>
        /// The next time function
        /// </summary>
        private readonly Func<DateTime, DateTime> _nextTimeFunc;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeFunc"/> class.
        /// </summary>
        public TimeFunc()
        {
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeFunc"/> class.
        /// </summary>
        /// <param name="nexttimefunc">The next time derive function.</param>
        /// <param name="name"></param>
        private TimeFunc(Func<DateTime, DateTime> nexttimefunc, string name)
        {
            _nextTimeFunc = nexttimefunc;
            Name = name;
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Name of the composite, for easier logging
        /// </summary>
        public string Name { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// First moment after the markets are opened
        /// </summary>
        /// <param name="exchangeModel">The exchangeModel.</param>
        /// <param name="afteropen">Time delay after market open.</param>
        /// <returns></returns>
        public TimeComposite AfterMarketOpen(ExchangeModel exchangeModel, TimeSpan afteropen) =>
            new TimeFunc(x =>
            {
                //Get the correct timezone for this exchange (input is utc)
                x = x.ConvertTo(TimeZone.Utc, exchangeModel.TimeZone);

                //Derive next date
                var derived = exchangeModel.NextMarketOpen(x).Add(afteropen);

                //return
                return derived.ConvertTo(exchangeModel.TimeZone, TimeZone.Utc);
            }, $"After-Market-Open-{exchangeModel.Name}-{afteropen}");

        /// <summary>
        /// At the specified time of the day
        /// </summary>
        /// <param name="timeofday">The time of the day.</param>
        /// <returns></returns>
        public TimeComposite At(TimeSpan timeofday) =>
            At(timeofday, TimeZone.Utc);

        /// <summary>
        /// At the specified time of the day
        /// </summary>
        /// <param name="timeofday">Time of the day</param>
        /// <param name="timezone">Related timezone</param>
        /// <returns></returns>
        public TimeComposite At(TimeSpan timeofday, TimeZone timezone) =>
            new TimeFunc(x =>
            {
                //Get the correct timezone for this exchange (input is utc)
                x = x.ConvertTo(TimeZone.Utc, timezone);

                //Derive next date
                var derived = x.TimeOfDay > timeofday ? x.Date.AddDays(1).Add(timeofday) : x.Date.Add(timeofday);

                //return
                return derived.ConvertTo(timezone, TimeZone.Utc);
            }, $"At-{timeofday}-{timezone}");

        /// <summary>
        /// At the specified time of the day
        /// </summary>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="timezone">The timezone.</param>
        /// <returns></returns>
        public TimeComposite At(int hour, int minute = 0, int second = 0, TimeZone timezone = TimeZone.Utc) =>
            At(new TimeSpan(hour, minute, second), TimeZone.Utc);

        /// <summary>
        /// At the specified time of the day
        /// </summary>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="timezone">The timezone.</param>
        /// <returns></returns>
        public TimeComposite At(int hour, int minute, TimeZone timezone) =>
            At(new TimeSpan(hour, minute, 0), timezone);

        /// <summary>
        /// At the specified time of the day
        /// </summary>
        /// <param name="hour">The hour.</param>
        /// <param name="timezone">The timezone.</param>
        /// <returns></returns>
        public TimeComposite At(int hour, TimeZone timezone) =>
            At(new TimeSpan(hour, 0, 0), timezone);

        /// <summary>
        /// Before the next market close
        /// </summary>
        /// <param name="exchangeModel">The exchangeModel.</param>
        /// <param name="beforeclose">Time before the market closes.</param>
        /// <returns></returns>
        public TimeComposite BeforeMarketClose(ExchangeModel exchangeModel, TimeSpan beforeclose) =>
            new TimeFunc(x =>
            {                
                //Get the correct timezone for this exchange (input is utc)
                x = x.ConvertTo(TimeZone.Utc, exchangeModel.TimeZone);

                //Derive next date
                var derived = exchangeModel.NextMarketClose(x.ConvertTo(TimeZone.Utc, exchangeModel.TimeZone))
                        .Add(-beforeclose);

                //return
                return derived.ConvertTo(exchangeModel.TimeZone, TimeZone.Utc);
            }, $"Before-Market-Close-{exchangeModel.Name}-{beforeclose}");

        /// <summary>
        /// Every specified interval
        /// </summary>
        /// <param name="interval">The interval.</param>
        /// <returns></returns>
        public TimeComposite Every(TimeSpan interval) =>
            new TimeFunc(x => x.AddSeconds(x.TimeOfDay.TotalSeconds % interval.TotalSeconds), $"Every-{interval}");

        /// <summary>
        /// Get the next time of the day to execute the event
        /// </summary>
        /// <param name="timeutc">Time in Utc</param>
        /// <returns></returns>
        public DateTime NextTimeOfDay(DateTime timeutc) =>
            _nextTimeFunc(timeutc);

        #endregion Public Methods
    }
}