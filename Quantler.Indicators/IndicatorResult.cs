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

namespace Quantler.Indicators
{
    /// <summary>
    /// Represents the result of an indicator's calculations
    /// </summary>
    public class IndicatorResult
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IndicatorResult"/> class
        /// </summary>
        /// <param name="value">The value output by the indicator</param>
        /// <param name="status">The status returned by the indicator</param>
        public IndicatorResult(decimal value, IndicatorStatus status = IndicatorStatus.Success)
        {
            Value = value;
            Status = status;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// The indicator status
        /// </summary>
        public IndicatorStatus Status
        {
            get;
        }

        /// <summary>
        /// The indicator output value
        /// </summary>
        public decimal Value
        {
            get;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Converts the specified decimal value into a successful indicator result
        /// </summary>
        /// <remarks>
        /// This method is provided for backwards compatibility
        /// </remarks>
        /// <param name="value">The decimal value to be converted into an <see cref="IndicatorResult"/></param>
        public static implicit operator IndicatorResult(decimal value) =>
            new IndicatorResult(value);

        #endregion Public Methods
    }
}