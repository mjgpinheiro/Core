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
using Quantler.Indicators;
using System;
using Xunit;

namespace Quantler.Tests.Indicators
{
    public class FractalAdaptiveMovingAverageTests
    {
        #region Public Methods

        [Fact(Skip = "Fix Me")]
        [Trait("Quantler.Indicators", "FractalAdaptiveMovingAverage")]
        public void ComparesAgainstExternalData()
        {
            var indicator = new FractalAdaptiveMovingAverage("", 16, 198);
            RunTestIndicator(indicator);
        }

        [Fact]
        [Trait("Quantler.Indicators", "FractalAdaptiveMovingAverage")]
        public void ResetsProperly()
        {
            FractalAdaptiveMovingAverage frama = new FractalAdaptiveMovingAverage("", 6, 198);

            foreach (var data in TestHelper.GetDataStream(7))
            {
                frama.Update(new TradeBar { High = data, Low = data, TimeZone = TimeZone.Utc });
            }
            Assert.True(frama.IsReady);
            Assert.NotEqual(0m, frama.Current.Price);
            Assert.NotEqual(0, frama.Samples);

            frama.Reset();

            TestHelper.AssertIndicatorIsInDefaultState(frama);
        }

        #endregion Public Methods

        #region Private Methods

        private static void AssertResult(double expected, decimal actual)
        {
            Assert.True(Math.Abs((decimal)expected - actual) < 0.006m);
        }

        private static void RunTestIndicator(BarIndicator indicator)
        {
            TestHelper.TestIndicator(indicator, "frama.txt", "Filt", (actual, expected) => { AssertResult(expected, actual.Current.Price); });
        }

        #endregion Private Methods
    }
}