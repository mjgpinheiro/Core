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

using Quantler.Data.Bars;
using System;

namespace Quantler.Indicators
{
    /// <summary>
    /// This indicator computes the Average Directional Movement Index Rating (ADXR).
    /// The Average Directional Movement Index Rating is calculated with the following formula:
    /// ADXR[i] = (ADX[i] + ADX[i - period + 1]) / 2
    /// </summary>
    public class AverageDirectionalMovementIndexRating : BarIndicator
    {
        #region Private Fields

        private readonly AverageDirectionalIndex _adx;
        private readonly RollingWindow<decimal> _adxHistory;
        private readonly int _period;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AverageDirectionalMovementIndexRating"/> class using the specified name and period.
        /// </summary>
        /// <param name="name">The name of this indicator</param>
        /// <param name="period">The period of the ADXR</param>
        public AverageDirectionalMovementIndexRating(string name, int period)
            : base(name)
        {
            _period = period;
            _adx = new AverageDirectionalIndex(name + "_ADX", period);
            _adxHistory = new RollingWindow<decimal>(period);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AverageDirectionalMovementIndexRating"/> class using the specified period.
        /// </summary>
        /// <param name="period">The period of the ADXR</param>
        public AverageDirectionalMovementIndexRating(int period)
            : this("ADXR" + period, period)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets a flag indicating when this indicator is ready and fully initialized
        /// </summary>
        public override bool IsReady => Samples >= _period;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Resets this indicator to its initial state
        /// </summary>
        public override void Reset()
        {
            _adx.Reset();
            _adxHistory.Reset();
            base.Reset();
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Computes the next value of this indicator from the given state
        /// </summary>
        /// <param name="input">The input given to the indicator</param>
        /// <returns>A new value for this indicator</returns>
        protected override decimal ComputeNextValue(DataPointBar input)
        {
            _adx.Update(input);
            _adxHistory.Add(_adx);

            return (_adx + _adxHistory[Math.Min(_adxHistory.Count - 1, _period - 1)]) / 2;
        }

        #endregion Protected Methods
    }
}