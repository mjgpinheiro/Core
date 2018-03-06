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

using Quantler.Securities;
using System.Collections.Generic;
using System.Linq;

namespace Quantler.Data.Feed
{
    /// <summary>
    /// Defines a container type to hold data produced by a data feed subscription
    /// </summary>
    public class DataFeedPacket
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataFeedPacket"/> class
        /// </summary>
        /// <param name="security">The security whose data is held in this packet</param>
        /// <param name="subscriptions">The subscriptions that produced this data</param>
        public DataFeedPacket(Security security, DataSubscription[] subscriptions)
        {
            Security = security;
            Subscriptions = subscriptions;
            Data = new List<DataPoint>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataFeedPacket"/> class
        /// </summary>
        /// <param name="security">The security whose data is held in this packet</param>
        /// <param name="subscriptions">The subscriptions configuration that produced this data</param>
        /// <param name="data">The data to add to this packet. The list reference is reused
        /// internally and NOT copied.</param>
        public DataFeedPacket(Security security, DataSubscription[] subscriptions, params DataPoint[] data)
        {
            Security = security;
            Subscriptions = subscriptions;
            Data = data.ToList();
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the number of data points held within this packet
        /// </summary>
        public int Count => Data.Count;

        /// <summary>
        /// The data for the security
        /// </summary>
        public List<DataPoint> Data { get; }

        /// <summary>
        /// The security
        /// </summary>
        public Security Security
        {
            get;
        }

        /// <summary>
        /// The subscription configurations that produced this data
        /// </summary>
        public DataSubscription[] Subscriptions
        {
            get;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Adds the specified data to this packet
        /// </summary>
        /// <param name="data">The data to be added to this packet</param>
        public void Add(DataPoint data) => Data.Add(data);

        #endregion Public Methods
    }
}