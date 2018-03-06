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

using MessagePack;
using System;
using Quantler.Securities;

namespace Quantler.Data
{
    /// <summary>
    /// Data subscription request, which can be either send to a data feed or used internally to identify current subscriptions
    /// </summary>
    [MessagePackObject]
    public class DataSubscriptionRequest
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the aggregation, as requested
        /// </summary>
        [Key(0)]
        public TimeSpan? Aggregation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        [Key(1)]
        public DataSource DataSource
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the type of the data point.
        /// </summary>
        [Key(2)]
        public DataType DataType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tickers.
        /// </summary>
        [Key(3)]
        public TickerSymbol Ticker
        {
            get;
            set;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Creates a new subscription object.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <param name="datasource">The datasource.</param>
        /// <param name="aggregation">The aggregation.</param>
        /// <param name="datatype">The datatype.</param>
        /// <returns></returns>
        public static DataSubscriptionRequest CreateSubscriptionRequest(TickerSymbol ticker, DataSource datasource, TimeSpan? aggregation,
            DataType datatype) =>
            new DataSubscriptionRequest
            {
                Aggregation = aggregation,
                DataSource = datasource,
                DataType = datatype,
                Ticker = ticker
            };

        /// <summary>
        /// Gets the fire hose subscription.
        /// </summary>
        /// <param name="datasource">The data source.</param>
        /// <returns></returns>
        public static DataSubscriptionRequest GetFireHoseSubscriptionRequest(DataSource datasource) =>
            new DataSubscriptionRequest
            {
                Aggregation = null,
                DataSource = datasource,
                DataType = DataType.Tick,
                Ticker = TickerSymbol.All()
            };

        /// <summary>
        /// De-serializes the data in a data subscription object.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public DataSubscriptionRequest Deserialize(byte[] data) =>
            LZ4MessagePackSerializer.Deserialize<DataSubscriptionRequest>(data);

        /// <summary>
        /// Gets the name of the subscription.
        /// </summary>
        /// <returns></returns>
        public string GetSubscriptionName() =>
            $"{Ticker}:{DataSource}:{DataType}:" + (Aggregation.HasValue ? ((int)Aggregation.Value.TotalSeconds).ToString() : string.Empty);

        /// <summary>
        /// Serializes this instance.
        /// </summary>
        /// <returns></returns>
        public byte[] Serialize() =>
            LZ4MessagePackSerializer.Serialize(this);

        #endregion Public Methods
    }
}