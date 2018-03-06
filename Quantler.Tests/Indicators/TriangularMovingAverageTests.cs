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

using Quantler.Indicators;
using Xunit;

namespace Quantler.Tests.Indicators
{
    public class TriangularMovingAverageTests : CommonIndicatorTests<IndicatorDataPoint>
    {
        #region Protected Properties

        protected override string TestColumnName => "TRIMA";

        protected override string TestFileName => "spy_trima.txt";

        #endregion Protected Properties

        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "TriangularMovingAverage")]
        public override void ComparesAgainstExternalData()
        {
            foreach (var period in new[] { 5, 6 })
            {
                RunTestIndicator(new TriangularMovingAverage(period), period);
            }
        }

        [Fact]
        [Trait("Quantler.Indicators", "TriangularMovingAverage")]
        public override void ComparesAgainstExternalDataAfterReset()
        {
            foreach (var period in new[] { 5, 6 })
            {
                var indicator = new TriangularMovingAverage(period);
                RunTestIndicator(indicator, period);
                indicator.Reset();
                RunTestIndicator(indicator, period);
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected override IndicatorBase<IndicatorDataPoint> CreateIndicator()
        {
            return new TriangularMovingAverage(5);
        }

        #endregion Protected Methods

        #region Private Methods

        private void RunTestIndicator(TriangularMovingAverage trima, int period)
        {
            TestHelper.TestIndicator(trima, TestFileName, TestColumnName + "_" + period, Assertion);
        }

        #endregion Private Methods
    }
}