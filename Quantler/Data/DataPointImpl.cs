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
using NodaTime;
using Quantler.Securities;

namespace Quantler.Data
{
    /// <summary>
    /// Base class for implementing data points
    /// </summary>
    /// <seealso cref="DataPoint" />
    [MessagePackObject]
    public abstract class DataPointImpl : DataPoint
    {
        #region Public Properties

        /// <summary>
        /// Gets the type of the data.
        /// </summary>
        [Key(0)]
        public DataType DataType { get; set; } = DataType.Unknown;

        /// <summary>
        /// Gets the time and date this datapoint has ended (for instance the closing time of a bar)
        /// </summary>
        [IgnoreMember]
        public virtual DateTime EndTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this data point is used for backfilling.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is backfilling; otherwise, <c>false</c>.
        /// </value>
        [Key(1)]
        public bool IsBackfilling
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the time and date this datapoint is a reference to in the associated timezone
        /// </summary>
        [Key(2)]
        public DateTime Occured
        {
            get;
            set;
        }

        /// <summary>
        /// Current main price of this data point
        /// </summary>
        [Key(3)]
        public virtual decimal Price { get; set; }

        /// <summary>
        /// Gets the associated ticker
        /// </summary>
        [Key(4)]
        public TickerSymbol Ticker
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the time zone.
        /// </summary>
        [Key(5)]
        public TimeZone TimeZone
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the time and date this datapoint is a reference to in utc
        /// </summary>
        [IgnoreMember]
        public DateTime OccuredUtc => Occured.ConvertTo(WorldClock.GetTimezone(TimeZone), DateTimeZone.Utc);

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Deserializes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="iscompressed">if set to <c>true</c> [iscompressed].</param>
        /// <returns></returns>
        public static DataPoint Deserialize(byte[] data, bool iscompressed = false)
        {
            //Check input
            if (data == null || data.Length == 0)
                return null;
            try
            {
                return iscompressed ? 
                    LZ4MessagePackSerializer.Deserialize<DataPoint>(data) : 
                    MessagePackSerializer.Deserialize<DataPoint>(data);
            }
            catch(Exception exc)
            {
                //This should not happen!
                throw new Exception($"Could not deserialize datapoint due to exception", exc);
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public virtual DataPoint Clone() => 
            MessagePackSerializer.Deserialize<DataPoint>(MessagePackSerializer.Serialize(this));

        /// <summary>
        /// Serializes this instance.
        /// </summary>
        /// <returns></returns>
        public string SerializeJson() =>
            MessagePackSerializer.ToJson<DataPoint>(this);

        /// <summary>
        /// Serializes this instance
        /// </summary>
        /// <param name="compress">if set to <c>true</c> [compress].</param>
        /// <returns></returns>
        public byte[] Serialize(bool compress = true) =>
            compress
                ? LZ4MessagePackSerializer.Serialize<DataPoint>(this)
                : MessagePackSerializer.Serialize<DataPoint>(this);

        /// <summary>
        /// Updates the specified instance.
        /// </summary>
        /// <param name="tradeprice">The tradeprice.</param>
        /// <param name="bidprice">The bidprice.</param>
        /// <param name="askprice">The askprice.</param>
        /// <param name="tradesize">The tradesize.</param>
        /// <param name="bidsize">The bidsize.</param>
        /// <param name="asksize">The asksize.</param>
        public virtual void Update(decimal tradeprice, decimal bidprice, decimal askprice, decimal tradesize, decimal bidsize, decimal asksize) =>
            Price = tradeprice;

        #endregion Public Methods
    }
}