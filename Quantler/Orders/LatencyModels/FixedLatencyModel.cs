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
using System.Composition;

namespace Quantler.Orders.LatencyModels
{
    /// <summary>
    /// Implementation of a fixed latency model for an order
    /// </summary>
    [Export(typeof(LatencyModel))]
    public class FixedLatencyModel : LatencyModel
    {
        #region Private Fields

        /// <summary>
        /// Current latency set in milliseconds
        /// </summary>
        private readonly TimeSpan _latency;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initialize a fixed latency model which adds latency in milliseconds
        /// </summary>
        /// <param name="latencyinmilliseconds"></param>
        public FixedLatencyModel(int latencyinmilliseconds) => _latency = TimeSpan.FromMilliseconds(latencyinmilliseconds);

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Get current order latency
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public TimeSpan GetLatency(Order order) => _latency;

        #endregion Public Methods
    }
}