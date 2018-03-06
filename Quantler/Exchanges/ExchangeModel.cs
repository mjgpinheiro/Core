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
using System;

namespace Quantler.Exchanges
{
    /// <summary>
    /// Contains exchangeModel based information and logic
    /// </summary>
    public interface ExchangeModel
    {
        #region Public Properties

        /// <summary>
        /// Use extended market hours
        /// </summary>
        bool ExtendedMarketHours { get; }

        /// <summary>
        /// Returns true if this exchangeModel is open for trading
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Returns the local time of this exchangeModel
        /// </summary>
        DateTime LocalTime { get; }

        /// <summary>
        /// Exchange name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the time zone for this exchangeModel
        /// </summary>
        TimeZone TimeZone { get; }

        /// <summary>
        /// Returns the amount trading days per year for this exchangeModel
        /// </summary>
        int TradingDaysPerYear { get; }

        /// <summary>
        /// Returns the current time in utc
        /// </summary>
        DateTime UtcTime { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// If true, this exchangeModel is opened between these date and time moments
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="extendedmarkethours"></param>
        /// <returns></returns>
        bool IsOpenBetween(DateTime start, DateTime end, bool extendedmarkethours);

        /// <summary>
        /// If true, this exchangeModel is opened between these date and time moments
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        bool IsOpenBetween(DateTime start, DateTime end);

        /// <summary>
        /// Determines whether [is open during bar] [the specified bar].
        /// </summary>
        /// <param name="bar">The bar.</param>
        /// <returns>
        ///   <c>true</c> if [is open during bar] [the specified bar]; otherwise, <c>false</c>.
        /// </returns>
        bool IsOpenDuringBar(DataPointBar bar);

        /// <summary>
        /// If true, this exchangeModel is opened at the specified date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        bool IsOpenOnDate(DateTime date);

        /// <summary>
        /// If true, this exchangeModel is opened at the specified date and time
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="extendedmarkethours"></param>
        /// <returns></returns>
        bool IsOpenOnDateTime(DateTime datetime, bool extendedmarkethours);

        /// <summary>
        /// If true, this exchangeModel is opened at the specified date and time
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        bool IsOpenOnDateTime(DateTime datetime);

        /// <summary>
        /// Returns the next moment in time the market will close
        /// </summary>
        /// <param name="localtime"></param>
        /// <param name="extendedmarkethours"></param>
        /// <returns></returns>
        DateTime NextMarketClose(DateTime localtime, bool extendedmarkethours);

        /// <summary>
        /// Returns the next moment in time the market will close
        /// </summary>
        /// <param name="localtime"></param>
        /// <returns></returns>
        DateTime NextMarketClose(DateTime localtime);

        /// <summary>
        /// Next moment in time the market will open.
        /// </summary>
        /// <param name="localtime">The local time.</param>
        /// <returns></returns>
        DateTime NextMarketOpen(DateTime localtime);

        /// <summary>
        /// Next moment in time the market will open.
        /// </summary>
        /// <param name="localtime">The local time.</param>
        /// <param name="extendedmarkethours">if set to <c>true</c> [extendedmarkethours].</param>
        /// <returns></returns>
        DateTime NextMarketOpen(DateTime localtime, bool extendedmarkethours);

        #endregion Public Methods
    }
}