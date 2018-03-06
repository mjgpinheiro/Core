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

using Quantler.Exchanges;
using System;
using System.Linq;

namespace Quantler.Scheduler
{
    /// <summary>
    /// Date composition
    /// </summary>
    /// <seealso cref="DateComposite" />
    public class DateFunc : DateComposite
    {
        #region Private Fields

        /// <summary>
        /// The next date function
        /// </summary>
        private readonly Func<DateTime, DateTime> _nextDateFunc;

        /// <summary>
        /// The previous date, in case we need to remember this
        /// </summary>
        private DateTime _previousDate = DateTime.MinValue;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DateFunc"/> class.
        /// </summary>
        public DateFunc()
        {
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DateFunc"/> class.
        /// </summary>
        /// <param name="nextdatefunc">The next date function.</param>
        /// <param name="name"></param>
        private DateFunc(Func<DateTime, DateTime> nextdatefunc, string name)
        {
            _nextDateFunc = nextdatefunc;
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
        /// Last trading day of each month
        /// </summary>
        /// <param name="exchangeModel">The exchangeModel.</param>
        /// <returns></returns>
        public DateComposite EndOfMonth(ExchangeModel exchangeModel) =>
            new DateFunc(x =>
            {
                //Derive the next date
                var derived = x.ConvertTo(TimeZone.Utc, exchangeModel.TimeZone).AddMonths(2).DoWhile(n => n.AddDays(-1),
                        i => !exchangeModel.IsOpenOnDate(i) && x.Month != i.Month + 1);

                //return
                return derived.ConvertTo(exchangeModel.TimeZone, TimeZone.Utc);
            }, "End-Of-Every-Month");

        /// <summary>
        /// Last trading day of specified month
        /// </summary>
        /// <param name="exchangeModel">The exchangeModel.</param>
        /// <param name="month">The month.</param>
        /// <returns></returns>
        public DateComposite EndOfMonth(ExchangeModel exchangeModel, int month) =>
            new DateFunc(x =>
            {
                //Get the correct timezone for this exchange (input is utc)
                x = x.ConvertTo(TimeZone.Utc, exchangeModel.TimeZone);

                //Derive next date
                var derived = new DateTime(
                        x.Month > month || (!exchangeModel.IsOpenOnDate(x) && x.Month == month) ? x.Year + 1 : x.Year,
                        12, 31)
                    .DoWhile(n => n.AddDays(-1), i => !exchangeModel.IsOpenOnDate(i) && i.Month != month);

                //return
                return derived.ConvertTo(exchangeModel.TimeZone, TimeZone.Utc);
            }, $"End-Of-Month-{month}");

        /// <summary>
        /// Last trading day of each week
        /// </summary>
        /// <param name="exchangeModel">The exchangeModel.</param>
        /// <returns></returns>
        public DateComposite EndOfWeek(ExchangeModel exchangeModel) =>
            new DateFunc(x =>
            {
                //Get the correct timezone for this exchange (input is utc)
                x = x.ConvertTo(TimeZone.Utc, exchangeModel.TimeZone);

                //Derive next date
                var derived = x.AddDays(7 - ((int) x.DayOfWeek + 1 > 0 ? (int) x.DayOfWeek + 2 : 0))
                        .DoWhile(n => n.AddDays(-1), n => !exchangeModel.IsOpenOnDate(n));

                //return
                return derived.ConvertTo(exchangeModel.TimeZone, TimeZone.Utc);
            }, "End-Of-Week");

        /// <summary>
        /// Every day.
        /// </summary>
        /// <returns></returns>
        public DateComposite EveryDay() =>
            new DateFunc(x =>
            {
                //Set previous date
                if (_previousDate.Date != x.Date)
                {
                    _previousDate = x.Date.AddDays(1);
                    return _previousDate;
                }
                else
                    return x;
            }, "Every-Day");

        /// <summary>
        /// Every day.
        /// </summary>
        /// <returns></returns>
        public DateComposite EveryTradingDay(ExchangeModel exchangeModel) =>
            new DateFunc(x =>
            {
                //Get the correct timezone for this exchange (input is utc)
                x = x.ConvertTo(TimeZone.Utc, exchangeModel.TimeZone);

                //Set previous date
                var derived = x;
                if (_previousDate.Date != x.Date)
                {
                    _previousDate = _previousDate.DoWhile(n => n.AddDays(1), n => !exchangeModel.IsOpenOnDate(n));
                    derived = _previousDate;
                }

                //return
                return derived.ConvertTo(exchangeModel.TimeZone, TimeZone.Utc);
            }, "Every-TradingDay");

        /// <summary>
        /// Get the next date and time to execute this event
        /// </summary>
        /// <param name="dateutc">Current date in utc.</param>
        /// <returns></returns>
        public DateTime NextDate(DateTime dateutc) =>
            _nextDateFunc(dateutc);

        /// <summary>
        /// On a specific year, month and day
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <returns></returns>
        public DateComposite On(int year, int month, int day) =>
            new DateFunc(x => new DateTime(year, month, day), $"At-Date-{year}-{month}-{day}");

        /// <summary>
        /// On specified days
        /// </summary>
        /// <param name="dates">The dates.</param>
        /// <returns></returns>
        public DateComposite On(params DateTime[] dates) =>
            new DateFunc(x => x.DoWhile(n => n.AddDays(1), n => dates.Count(i => i.Day == n.Day && i.Month == n.Month) == 0), $"On-Dates-{string.Join("-", dates)}");

        /// <summary>
        /// On the specified days of the week
        /// </summary>
        /// <param name="days">The days.</param>
        /// <returns></returns>
        public DateComposite On(params DayOfWeek[] days) =>
            new DateFunc(x => x.DoWhile(n => n.AddDays(1), n => !days.Contains(n.DayOfWeek)), $"On-Day-Of-Week-{string.Join("-", days)}");

        /// <summary>
        /// On the first trading day of the month
        /// </summary>
        /// <param name="exchangeModel">The exchangeModel.</param>
        /// <returns></returns>
        public DateComposite StartOfMonth(ExchangeModel exchangeModel) =>
            new DateFunc(x =>
            {
                //Get the correct timezone for this exchange (input is utc)
                x = x.ConvertTo(TimeZone.Utc, exchangeModel.TimeZone);

                //Derive next date
                var derived = new DateTime(x.Year, x.Month, 1).AddMonths(1)
                        .DoWhile(n => n.AddDays(1), i => !exchangeModel.IsOpenOnDate(i));

                //return
                return derived.ConvertTo(exchangeModel.TimeZone, TimeZone.Utc);
            }, "Start-Of-Every-Month");

        /// <summary>
        /// On the first trading day of the specified month
        /// </summary>
        /// <param name="exchangeModel">The exchangeModel.</param>
        /// <param name="month">The month.</param>
        /// <returns></returns>
        public DateComposite StartOfMonth(ExchangeModel exchangeModel, int month) =>
            new DateFunc(x =>
            {
                //Get the correct timezone for this exchange (input is utc)
                x = x.ConvertTo(TimeZone.Utc, exchangeModel.TimeZone);

                //Derive next date
                var derived = new DateTime(x.Month >= month ? x.Year + 1 : x.Year, 1, 1)
                    .DoWhile(n => n.AddDays(1), i => !exchangeModel.IsOpenOnDate(i) && i.Month != month);

                //return
                return derived.ConvertTo(exchangeModel.TimeZone, TimeZone.Utc);
            }, $"Start-Of-{month}-Month");

        /// <summary>
        /// On the first trading day of the week
        /// </summary>
        /// <param name="exchangeModel">The exchangeModel.</param>
        /// <returns></returns>
        public DateComposite StartOfWeek(ExchangeModel exchangeModel) =>
            new DateFunc(x =>
            {
                //Get the correct timezone for this exchange (input is utc)
                x = x.ConvertTo(TimeZone.Utc, exchangeModel.TimeZone);

                //Derive next date
                var derived = x.AddDays(7).AddDays(DayOfWeek.Monday - x.DayOfWeek)
                        .DoWhile(n => n.AddDays(1), n => !exchangeModel.IsOpenOnDate(n));

                //return
                return derived.ConvertTo(exchangeModel.TimeZone, TimeZone.Utc);
            }, "Start-Of-Every-Week");

        #endregion Public Methods
    }
}