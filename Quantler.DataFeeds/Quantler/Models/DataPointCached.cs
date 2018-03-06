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

using Quantler.Data;
using System;

namespace Quantler.DataFeeds.Quantler.Models
{
    /// <summary>
    /// Model for the cache
    /// </summary>
    public class DataPointCached
    {
        #region Public Properties

        /// <summary>
        /// Data as set
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Date this datapoint relates to
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Ticker name
        /// </summary>
        public string Ticker { get; set; }

        /// <summary>
        /// Datapoint type
        /// </summary>
        public DataType Type { get; set; }

        /// <summary>
        /// Gets or sets the aggregation.
        /// </summary>
        public TimeSpan? Aggregation { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Creates the specified instance for data point caching.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <param name="type">The type.</param>
        /// <param name="aggregation"></param>
        /// <param name="date">The date.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static DataPointCached Create(string ticker, DataType type, TimeSpan? aggregation, DateTime date, byte[] data) =>
            new DataPointCached
            {
                Ticker = ticker,
                Type = type,
                Aggregation = aggregation,
                Date = date.Date,
                Data = data
            };

        /// <summary>
        /// Gets the cache key.
        /// </summary>
        /// <returns></returns>
        public string GetKey() => GetKey(Ticker, Type, Aggregation, Date);

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <param name="type">The type.</param>
        /// <param name="aggregation"></param>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static string GetKey(string ticker, DataType type, TimeSpan? aggregation, DateTime date) =>
            $"{ticker}-{type}-{aggregation}-{Util.ToQLDate(date)}";

        #endregion Public Methods
    }
}