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
using System;
using Xunit;

namespace Quantler.Tests.Indicators
{
    public class ExponentialMovingAverageTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "ExponentialMovingAverage")]
        public void ComparesAgainstExternalData()
        {
            var ema = new ExponentialMovingAverage(14);
            TestHelper.TestIndicator(ema, "spy_with_indicators.txt", "EMA14", TestHelper.AssertDeltaDecreases(2.5e-2));
        }

        [Fact]
        [Trait("Quantler.Indicators", "ExponentialMovingAverage")]
        public void EMAComputesCorrectly()
        {
            const int period = 4;
            decimal[] values = { 1m, 10m, 100m, 1000m };
            const decimal expFactor = 2m / (1m + period);

            var ema4 = new ExponentialMovingAverage(period);

            decimal current = 0m;
            for (int i = 0; i < values.Length; i++)
            {
                ema4.Update(new IndicatorDataPoint(DateTime.UtcNow.AddSeconds(i), TimeZone.Utc, values[i]));
                if (i == 0)
                {
                    current = values[i];
                }
                else
                {
                    current = values[i] * expFactor + (1 - expFactor) * current;
                }
                Assert.Equal(current, ema4.Current.Price);
            }
        }

        [Fact]
        [Trait("Quantler.Indicators", "ExponentialMovingAverage")]
        public void ResetsProperly()
        {
            // ema reset is just setting the value and samples back to 0
            var ema = new ExponentialMovingAverage(3);

            foreach (var data in TestHelper.GetDataStream(5))
            {
                ema.Update(data);
            }
            Assert.True(ema.IsReady);
            Assert.NotEqual(0m, ema.Current.Price);
            Assert.NotEqual(0, ema.Samples);

            ema.Reset();

            TestHelper.AssertIndicatorIsInDefaultState(ema);
        }

        #endregion Public Methods
    }
}