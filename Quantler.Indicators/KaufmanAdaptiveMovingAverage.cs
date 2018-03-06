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

namespace Quantler.Indicators
{
    /// <summary>
    /// This indicator computes the Kaufman Adaptive Moving Average (KAMA).
    /// The Kaufman Adaptive Moving Average is calculated as explained here:
    /// http://stockcharts.com/school/doku.php?id=chart_school:technical_indicators:kaufman_s_adaptive_moving_average
    /// </summary>
    public class KaufmanAdaptiveMovingAverage : WindowIndicator<IndicatorDataPoint>
    {
        #region Private Fields

        private const decimal ConstDiff = 2m / (2m + 1m) - ConstMax;
        private const decimal ConstMax = 2m / (30m + 1m);
        private decimal _periodRoc;
        private decimal _prevKama;
        private decimal _sumRoc1;
        private decimal _trailingPrice;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KaufmanAdaptiveMovingAverage"/> class using the specified name and period.
        /// </summary>
        /// <param name="name">The name of this indicator</param>
        /// <param name="period">The period of the KAMA</param>
        public KaufmanAdaptiveMovingAverage(string name, int period)
            : base(name, period + 1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KaufmanAdaptiveMovingAverage"/> class using the specified period.
        /// </summary>
        /// <param name="period">The period of the KAMA</param>
        public KaufmanAdaptiveMovingAverage(int period)
            : this("KAMA" + period, period)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets a flag indicating when this indicator is ready and fully initialized
        /// </summary>
        public override bool IsReady => Samples >= Period;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Resets this indicator to its initial state
        /// </summary>
        public override void Reset()
        {
            _sumRoc1 = 0;
            _periodRoc = 0;
            _prevKama = 0;
            _trailingPrice = 0;
            base.Reset();
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Computes the next Price of this indicator from the given state
        /// </summary>
        /// <param name="input">The input given to the indicator</param>
        /// <param name="window">The window for the input history</param>
        /// <returns>A new Price for this indicator</returns>
        protected override decimal ComputeNextValue(IReadOnlyWindow<IndicatorDataPoint> window, IndicatorDataPoint input)
        {
            if (Samples < Period)
            {
                if (Samples > 1)
                {
                    _sumRoc1 += Math.Abs(input.Price - window[1].Price);
                }

                return input.Price;
            }

            if (Samples == Period)
            {
                _sumRoc1 += Math.Abs(input.Price - window[1].Price);

                // At this point sumROC1 represents the
                // summation of the 1-day price difference
                // over the (optInTimePeriod-1)

                // Calculate the first KAMA
                // The yesterday price is used here as the previous KAMA.
                _prevKama = window[1].Price;
            }

            var newTrailingPrice = window[Period - 1];
            _periodRoc = input.Price - newTrailingPrice;

            if (Samples > Period)
            {
                // Adjust sumROC1:
                // - Remove trailing ROC1
                // - Add new ROC1
                _sumRoc1 -= Math.Abs(_trailingPrice - newTrailingPrice);
                _sumRoc1 += Math.Abs(input.Price - window[1].Price);
            }

            // Save the trailing Price.
            _trailingPrice = newTrailingPrice;

            // Calculate the efficiency ratio
            var efficiencyRatio = (_sumRoc1 <= _periodRoc) || _sumRoc1 == 0 ? 1m : Math.Abs(_periodRoc / _sumRoc1);

            // Calculate the smoothing constant
            var smoothingConstant = efficiencyRatio * ConstDiff + ConstMax;
            smoothingConstant *= smoothingConstant;

            // Calculate the KAMA like an EMA, using the
            // smoothing constant as the adaptive factor.
            _prevKama = (input.Price - _prevKama) * smoothingConstant + _prevKama;

            return _prevKama;
        }

        #endregion Protected Methods
    }
}