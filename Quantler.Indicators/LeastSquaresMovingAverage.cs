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

using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Linq;

namespace Quantler.Indicators
{
    /// <summary>
    /// The Least Squares Moving Average (LSMA) first calculates a least squares regression line
    /// over the preceding time periods, and then projects it forward to the current period. In
    /// essence, it calculates what the value would be if the regression line continued.
    /// Source: https://rtmath.net/helpFinAnalysis/html/b3fab79c-f4b2-40fb-8709-fdba43cdb363.htm
    /// </summary>
    public class LeastSquaresMovingAverage : WindowIndicator<IndicatorDataPoint>
    {
        #region Private Fields

        /// <summary>
        /// Array representing the time.
        /// </summary>
        private readonly double[] t;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LeastSquaresMovingAverage"/> class.
        /// </summary>
        /// <param name="name">The name of this indicator</param>
        /// <param name="period">The number of data points to hold in the window</param>
        public LeastSquaresMovingAverage(string name, int period)
            : base(name, period)
        {
            t = Vector<double>.Build.Dense(period, i => i + 1).ToArray();
            Intercept = new Identity(name + "_Intercept");
            Slope = new Identity(name + "_Slope");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LeastSquaresMovingAverage"/> class.
        /// </summary>
        /// <param name="period">The number of data points to hold in the window.</param>
        public LeastSquaresMovingAverage(int period)
            : this("LSMA" + period, period)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// The point where the regression line crosses the y-axis (price-axis)
        /// </summary>
        public IndicatorBase<IndicatorDataPoint> Intercept { get; }

        /// <summary>
        /// The regression line slope
        /// </summary>
        public IndicatorBase<IndicatorDataPoint> Slope { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Resets this indicator and all sub-indicators (Intercept, Slope)
        /// </summary>
        public override void Reset()
        {
            Intercept.Reset();
            Slope.Reset();
            base.Reset();
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Computes the next value of this indicator from the given state
        /// </summary>
        /// <param name="window"></param>
        /// <param name="input">The input given to the indicator</param>
        /// <returns>
        /// A new value for this indicator
        /// </returns>
        protected override decimal ComputeNextValue(IReadOnlyWindow<IndicatorDataPoint> window, IndicatorDataPoint input)
        {
            // Until the window is ready, the indicator returns the input value.
            if (window.Samples <= window.Size) return input;

            // Sort the window by time, convert the observations to double and transform it to an array
            var series = window
                .OrderBy(i => i.Occured)
                .Select(i => Convert.ToDouble(i.Price))
                .ToArray();
            // Fit OLS
            var ols = Fit.Line(x: t, y: series);
            Intercept.Update(input.Occured, input.TimeZone, (decimal)ols.Item1);
            Slope.Update(input.Occured, input.TimeZone, (decimal)ols.Item2);

            // Calculate the fitted value corresponding to the input
            return Intercept + Slope * Period;
        }

        #endregion Protected Methods
    }
}