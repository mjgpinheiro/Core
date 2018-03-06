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
    /// Base implementation for aggregation logic
    /// </summary>
    /// <typeparam name="TInput">The type of the input.</typeparam>
    /// <seealso cref="Quantler.Data.Aggegrate.DataAggregator" />
    public abstract class DataAggregatorBase<TInput> : DataAggregator
        where TInput : DataPoint
    {
        #region Public Events

        /// <summary>
        /// Occurs when [data aggregated].
        /// </summary>
        public event DataAggregationHandler DataAggregated;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Gets the most recently aggregated form of the input data, if null there is no aggregation finished as of yet
        /// </summary>
        public DataPoint Aggregated
        {
            get;
            private set;
        }

        /// <summary>
        /// Last datapoint used as input for aggregation
        /// </summary>
        public abstract DataPoint CurrentData
        {
            get;
        }

        /// <summary>
        /// Gets the type of the input.
        /// </summary>
        public Type InputType => typeof(TInput);

        /// <summary>
        /// Gets the type of the output.
        /// </summary>
        public abstract Type OutputType
        {
            get;
        }

        /// <summary>
        /// Derived aggregator name
        /// </summary>
        public abstract string Name { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Checks the specified local time if it is time to notify a new aggregation has fully completed
        /// </summary>
        /// <param name="currentLocalTime">The local time.</param>
        public abstract void Check(DateTime currentLocalTime);

        /// <summary>
        /// Feeds this aggregator with new input data
        /// </summary>
        /// <param name="data">The data.</param>
        /// <exception cref="ArgumentException">data</exception>
        public void Feed(DataPoint data)
        {
            if (!(data is TInput))
                throw new ArgumentException("data", $"Received type of {data.GetType().Name} but expected {typeof(TInput).Name}");

            Feed((TInput)data);
        }

        /// <summary>
        /// Feeds the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        public abstract void Feed(TInput data);

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Called when data is aggregated.
        /// </summary>
        /// <param name="aggegrated">The aggregated data.</param>
        protected virtual void OnAggegratedData(DataPoint aggegrated)
        {
            //Send aggregated data
            DataAggregated?.Invoke(this, aggegrated);

            //Assign as previous item
            Aggregated = aggegrated;
        }

        #endregion Protected Methods
    }
}