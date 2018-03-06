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

using System;
using System.Collections.Generic;

namespace Quantler.Data.Feed
{
    /// <summary>
    /// Represents a grouping of datapoints emitted from a datafeed
    /// </summary>
    public class DataUpdateHolder
    {
        #region Public Properties

        /// <summary>
        /// Gets the aggregator updates.
        /// </summary>
        public List<UpdateData<DataSubscription>> AggregatorUpdates { get; }

        /// <summary>
        /// Gets the number of datapoints contained in this holder
        /// </summary>
        public int DataPointCount => DataPoints.Count;

        /// <summary>
        /// Contained datapoints
        /// </summary>
        public List<DataPoint> DataPoints { get; }

        /// <summary>
        /// Gets the date and time this update has occured
        /// </summary>
        public DateTime OccuredUtc => Updates.OccuredUtc;

        /// <summary>
        /// Gets the contained updates.
        /// </summary>
        public DataUpdates Updates { get; }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataUpdateHolder"/> class.
        /// </summary>
        /// <param name="updates">The updates.</param>
        /// <param name="datapoints">The datapoints.</param>
        /// <param name="aggregatorupdates">The aggregatorupdates.</param>
        public DataUpdateHolder(DataUpdates updates, List<DataPoint> datapoints, List<UpdateData<DataSubscription>> aggregatorupdates)
        {
            Updates = updates;
            DataPoints = datapoints;
            AggregatorUpdates = aggregatorupdates;
        }

        #endregion Public Constructors
    }
}