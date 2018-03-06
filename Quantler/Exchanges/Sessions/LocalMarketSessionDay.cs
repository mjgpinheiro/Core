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
    /// Local exchangeModel session information
    /// </summary>
    public class LocalMarketSessionDay
    {
        #region Private Fields

        /// <summary>
        /// Regular Friday
        /// </summary>
        private Dictionary<MarketSessionType, LocalMarketSession> _friday;

        /// <summary>
        /// Regular Monday
        /// </summary>
        private Dictionary<MarketSessionType, LocalMarketSession> _monday;

        /// <summary>
        /// Regular Saturday
        /// </summary>
        private Dictionary<MarketSessionType, LocalMarketSession> _saturday;

        /// <summary>
        /// Regular Sunday
        /// </summary>
        private Dictionary<MarketSessionType, LocalMarketSession> _sunday;

        /// <summary>
        /// Regular Thursday
        /// </summary>
        private Dictionary<MarketSessionType, LocalMarketSession> _thursday;

        /// <summary>
        /// Regular Tuesday
        /// </summary>
        private Dictionary<MarketSessionType, LocalMarketSession> _tuesday;

        /// <summary>
        /// Regular Wednesday
        /// </summary>
        private Dictionary<MarketSessionType, LocalMarketSession> _wednesday;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Create market session for exchangeModel
        /// </summary>
        /// <param name="exchangename"></param>
        public LocalMarketSessionDay(string exchangename)
        {
            //Set current exchangeModel name used
            ExchangeName = exchangename;

            //Load regular timings
            LoadRegularSessions();
        }

        #endregion Public Constructors

        #region Internal Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalMarketSessionDay"/> class.
        /// Used for deviating market session days compared to a regular session
        /// </summary>
        /// <param name="exchangename"></param>
        /// <param name="dayofweek">The dayofweek.</param>
        /// <param name="deviated">The deviated.</param>
        internal LocalMarketSessionDay(
            string exchangename,
            DayOfWeek dayofweek,
            Dictionary<MarketSessionType, LocalMarketSession> deviated)
            : this(exchangename)
        {
            //Set deviating day
            switch (dayofweek)
            {
                case DayOfWeek.Friday:
                    _friday = deviated;
                    break;

                case DayOfWeek.Monday:
                    _monday = deviated;
                    break;

                case DayOfWeek.Saturday:
                    _saturday = deviated;
                    break;

                case DayOfWeek.Sunday:
                    _sunday = deviated;
                    break;

                case DayOfWeek.Thursday:
                    _thursday = deviated;
                    break;

                case DayOfWeek.Tuesday:
                    _tuesday = deviated;
                    break;

                case DayOfWeek.Wednesday:
                    _wednesday = deviated;
                    break;
            }
        }

        #endregion Internal Constructors

        #region Public Properties

        /// <summary>
        /// Loaded exchangeModel name
        /// </summary>
        public string ExchangeName { get; set; }

        #endregion Public Properties

        #region Private Properties

        /// <summary>
        /// Check if we loaded regular sessions
        /// </summary>
        private bool IsRegularLoaded =>
            _monday != null;

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Get a closed day, optionally provide a reason why this day is a closed day (such as in the event of a holiday)
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        public Dictionary<MarketSessionType, LocalMarketSession> GetClosedDay(string reason = "") =>
            new Dictionary<MarketSessionType, LocalMarketSession>
            {
                {MarketSessionType.Closed, new LocalMarketSession(MarketSessionType.Closed, TimeSpan.Zero, TimeSpan.FromDays(1), reason) }
            };

        /// <summary>
        /// Get a trading day where open and close times deviate from the regular trading hours, optionally provide a reason why (such as in the event of a holiday)
        /// </summary>
        /// <param name="dow"></param>
        /// <param name="open"></param>
        /// <param name="close"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public Dictionary<MarketSessionType, LocalMarketSession> GetDeviatedDay(DayOfWeek dow, TimeSpan? open,
            TimeSpan? close, string reason = "")
        {
            //Check if we are normally opened on this day (else return closed day)
            var regular = GetRegularSession(dow);
            if (regular.Count == 0 || (!open.HasValue && !close.HasValue))
                return GetClosedDay(reason);

            //If opened, set deviating open and close time
            Dictionary<MarketSessionType, LocalMarketSession> session = new Dictionary<MarketSessionType, LocalMarketSession>();
            foreach (var item in regular.Values
                                    .Where(x => x.Type != MarketSessionType.Closed)
                                    .OrderBy(x => x.Type)
                                    .Select(x => x.Copy()))
            {
                //We open earlier
                if (open.HasValue && item.Contains(open.Value))
                {
                    //Change opening time
                    item.Start = open.Value;
                    session.Add(item.Type, item);
                }
                else if (open.HasValue && item.Start > open.Value)
                    session.Add(item.Type, item);

                //We close earlier
                if (close.HasValue && item.Contains(close.Value))
                {
                    //Change closing time
                    item.End = close.Value;
                    session.Add(item.Type, item);
                }
                else if (close.HasValue && item.End < close.Value)
                    session.Add(item.Type, item);
            }

            //Return what we have
            return session;
        }

        /// <summary>
        /// Returns true if the market is opened between the start and time of the given day in the week
        /// </summary>
        /// <param name="dow"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="extendedtradinghours"></param>
        /// <returns></returns>
        public bool IsOpenBetween(DayOfWeek dow, TimeSpan start, TimeSpan end, bool extendedtradinghours) =>
            start >= MarketOpenTime(dow, extendedtradinghours) &&
            end <= MarketCloseTime(dow, extendedtradinghours);

        /// <summary>
        /// Returns true if the market is opened on the requested time of day
        /// </summary>
        /// <param name="dow"></param>
        /// <param name="time"></param>
        /// <param name="extendedtradinghours"></param>
        /// <returns></returns>
        public bool IsOpenOn(DayOfWeek dow, TimeSpan time, bool extendedtradinghours) =>
            time >= MarketOpenTime(dow, extendedtradinghours) &&
            time <= MarketCloseTime(dow, extendedtradinghours);

        /// <summary>
        /// Returns the time the market closes regularly on this day of the week
        /// </summary>
        /// <param name="dow"></param>
        /// <param name="extendedtradinghours"></param>
        /// <returns></returns>
        public TimeSpan MarketCloseTime(DayOfWeek dow, bool extendedtradinghours) =>
            extendedtradinghours && GetRegularSession(dow).ContainsKey(MarketSessionType.PostMarket) ? GetRegularSession(dow)[MarketSessionType.PostMarket].End :
            extendedtradinghours && GetRegularSession(dow).ContainsKey(MarketSessionType.Market) ? GetRegularSession(dow)[MarketSessionType.Market].End :
            !extendedtradinghours && GetRegularSession(dow).ContainsKey(MarketSessionType.Market) ? GetRegularSession(dow)[MarketSessionType.Market].End :
            TimeSpan.MinValue;

        /// <summary>
        /// Returns the time the market opens regularly on this day of the week
        /// </summary>
        /// <param name="dow"></param>
        /// <param name="extendedtradinghours"></param>
        /// <returns></returns>
        public TimeSpan MarketOpenTime(DayOfWeek dow, bool extendedtradinghours) =>
            extendedtradinghours && GetRegularSession(dow).ContainsKey(MarketSessionType.PreMarket) ? GetRegularSession(dow)[MarketSessionType.PreMarket].Start :
            extendedtradinghours && GetRegularSession(dow).ContainsKey(MarketSessionType.Market) ? GetRegularSession(dow)[MarketSessionType.Market].Start :
            !extendedtradinghours && GetRegularSession(dow).ContainsKey(MarketSessionType.Market) ? GetRegularSession(dow)[MarketSessionType.Market].Start :
            TimeSpan.MaxValue;

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Get regular trading session for given day of week
        /// </summary>
        /// <param name="dow"></param>
        /// <returns></returns>
        private Dictionary<MarketSessionType, LocalMarketSession> GetRegularSession(DayOfWeek dow)
        {
            switch (dow)
            {
                case DayOfWeek.Friday:
                    return _friday;

                case DayOfWeek.Monday:
                    return _monday;

                case DayOfWeek.Saturday:
                    return _saturday;

                case DayOfWeek.Sunday:
                    return _sunday;

                case DayOfWeek.Thursday:
                    return _thursday;

                case DayOfWeek.Tuesday:
                    return _tuesday;

                case DayOfWeek.Wednesday:
                    return _wednesday;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Load regular session timings from cache for given exchangeModel
        /// </summary>
        private void LoadRegularSessions()
        {
            //Check if it is already loaded
            if (IsRegularLoaded)
                return;

            //get from config
            var config = Config.MarketHourConfig.FirstOrDefault(x => x.Exchanges.Contains(ExchangeName));

            //Check if we have configuration
            if (config == null)
                throw new Exception($"Cannot load correct exchangeModel market hours. Tried loading: {ExchangeName}");

            //Set helper function
            void Add(MarketHoursSessionConfig loaded, Dictionary<MarketSessionType, LocalMarketSession> sessions)
            {
                //Set variables
                MarketSessionType type = (MarketSessionType)Enum.Parse(typeof(MarketSessionType), loaded.Session.ToLower(), true);
                TimeSpan start = TimeSpan.Parse(loaded.Start);
                TimeSpan end = TimeSpan.Parse(loaded.End);

                //Add to input
                sessions.Add(type, new LocalMarketSession(type, start, end, "Regular"));
            }

            //Get for each day
            _monday = new Dictionary<MarketSessionType, LocalMarketSession>();
            config.Monday.ForEach(x => Add(x, _monday));

            _tuesday = new Dictionary<MarketSessionType, LocalMarketSession>();
            config.Tuesday.ForEach(x => Add(x, _tuesday));

            _wednesday = new Dictionary<MarketSessionType, LocalMarketSession>();
            config.Wednesday.ForEach(x => Add(x, _wednesday));

            _thursday = new Dictionary<MarketSessionType, LocalMarketSession>();
            config.Thursday.ForEach(x => Add(x, _thursday));

            _friday = new Dictionary<MarketSessionType, LocalMarketSession>();
            config.Friday.ForEach(x => Add(x, _friday));

            _saturday = new Dictionary<MarketSessionType, LocalMarketSession>();
            config.Saturday.ForEach(x => Add(x, _saturday));

            _sunday = new Dictionary<MarketSessionType, LocalMarketSession>();
            config.Sunday.ForEach(x => Add(x, _sunday));
        }

        #endregion Private Methods
    }
}