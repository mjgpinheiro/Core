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

namespace Quantler.Brokers.BrokerTools
{
    /// <summary>
    /// Helper class for throttling api calls, in case we can only request data for x times per timespan
    /// </summary>
    public class ApiThrottle
    {
        #region Private Fields

        /// <summary>
        /// Minimum time to wait for a new call
        /// </summary>
        private readonly TimeSpan _minwait;

        /// <summary>
        /// Rate limiter
        /// </summary>
        private readonly RateGate _RateGate;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initiliaze ApiThrottle to only allow for apicalls after timespan wait
        /// </summary>
        /// <param name="minwait"></param>
        public ApiThrottle(TimeSpan minwait)
        {
            _minwait = minwait;
            _RateGate = new RateGate(1, minwait);
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Execute an api call and have it wait for the next moment it is allowed to do a call
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="call"></param>
        /// <returns></returns>
        public T ExecuteCall<T>(Func<T> call)
        {
            _RateGate.WaitToProceed();
            return call();
        }

        #endregion Public Methods
    }
}