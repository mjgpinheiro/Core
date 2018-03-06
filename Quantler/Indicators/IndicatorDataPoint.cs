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

using Quantler.Data;
using System;
using NodaTime;
using Quantler.Securities;

namespace Quantler.Indicators
{
    /// <summary>
    /// Represents a piece of data at a specific time
    /// </summary>
    public class IndicatorDataPoint : DataPointImpl, IEquatable<IndicatorDataPoint>, IComparable<IndicatorDataPoint>, IComparable
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new default instance of IndicatorDataPoint with a time of
        /// DateTime.MinValue, a Value of 0m and a timezone of UTC.
        /// </summary>
        public IndicatorDataPoint()
        {
            Price = 0m;
            Occured = DateTime.MinValue;
            TimeZone = TimeZone.Utc;
        }

        /// <summary>
        /// Initializes a new instance of the DataPoint type using the specified time/data
        /// </summary>
        /// <param name="occured">The time this data was produced</param>
        /// <param name="timezone">The timezone this data was produced in</param>
        /// <param name="price">The data</param>
        public IndicatorDataPoint(DateTime occured, TimeZone timezone, decimal price)
        {
            Occured = occured;
            Price = price;
            TimeZone = timezone;
        }

        /// <summary>
        /// Initializes a new instance of the DataPoint type using the specified time/data
        /// </summary>
        /// <param name="ticker">The ticker symbol associated with this data</param>
        /// <param name="occured">The time this data was produced</param>
        /// <param name="timezone">The timezone this data was produced in</param>
        /// <param name="price">The data</param>
        public IndicatorDataPoint(TickerSymbol ticker, DateTime occured, TimeZone timezone, decimal price)
        {
            Ticker = ticker;
            Occured = occured;
            Price = price;
            TimeZone = timezone;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Returns the data held within the instance
        /// </summary>
        /// <param name="instance">The DataPoint instance</param>
        /// <returns>The data held within the instance</returns>
        public static implicit operator decimal(IndicatorDataPoint instance) =>
            instance.Price;

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(IndicatorDataPoint other)
        {
            if (ReferenceEquals(other, null))
            {
                // everything is greater than null via MSDN
                return 1;
            }
            return Price.CompareTo(other.Price);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj"/> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj"/>. Greater than zero This instance follows <paramref name="obj"/> in the sort order.
        /// </returns>
        /// <param name="obj">An object to compare with this instance. </param><exception cref="T:System.ArgumentException"><paramref name="obj"/> is not the same type as this instance. </exception><filterpriority>2</filterpriority>
        public int CompareTo(object obj)
        {
            var other = obj as IndicatorDataPoint;
            if (other == null)
            {
                throw new ArgumentException("Object must be of type " + GetType().GetBetterTypeName());
            }
            return CompareTo(other);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(IndicatorDataPoint other)
        {
            if (other == null)
            {
                return false;
            }
            return other.Occured == Occured && other.Price == Price;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is IndicatorDataPoint && Equals((IndicatorDataPoint)obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Price.GetHashCode() * 397) ^ Occured.GetHashCode();
            }
        }

        /// <summary>
        /// Returns a string representation of this DataPoint instance using ISO8601 formatting for the date
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> containing a fully qualified type name.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("{0} - {1}", Occured.ToString("s"), Price);
        }

        #endregion Public Methods
    }
}