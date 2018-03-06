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

using System;

namespace Quantler.Data.Aggegrate
{
    /// <summary>
    /// Takes a data point and aggregates it into another data point format
    /// </summary>
    public interface DataAggregator
    {
        #region Public Events

        /// <summary>
        /// Occurs when data is aggregated.
        /// </summary>
        event DataAggregationHandler DataAggregated;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Gets the most recently aggregated form of the input data, if null there is no aggregated finished as of yet
        /// </summary>
        DataPoint Aggregated { get; }

        /// <summary>
        /// Last data point used as input for aggregation
        /// </summary>
        DataPoint CurrentData { get; }

        /// <summary>
        /// Gets the type of the input.
        /// </summary>
        Type InputType { get; }

        /// <summary>
        /// Gets the type of the output.
        /// </summary>
        Type OutputType { get; }

        /// <summary>
        /// Derived aggregator name
        /// </summary>
        string Name { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Checks the specified local time if it is time to notify a new aggregation has fully completed
        /// </summary>
        /// <param name="currentLocalTime">The local time.</param>
        void Check(DateTime currentLocalTime);

        /// <summary>
        /// Feeds this aggregator with new input data
        /// </summary>
        /// <param name="data">The data.</param>
        void Feed(DataPoint data);

        #endregion Public Methods
    }
}