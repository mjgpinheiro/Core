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
    public class LinearWeightedMovingAverageTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "LinearWeightedMovingAverage")]
        public void Lwma1ComputesCorrectly()
        {
            const int period = 1;
            decimal[] values = { 1m };

            var lwma = new LinearWeightedMovingAverage(period);

            decimal current = 0m;
            for (int i = 0; i < values.Length; i++)
            {
                lwma.Update(new IndicatorDataPoint(DateTime.UtcNow.AddSeconds(i), TimeZone.Utc, values[i]));
            }
            current = 1m;
            Assert.Equal(current, lwma.Current.Price);
        }

        [Fact]
        [Trait("Quantler.Indicators", "LinearWeightedMovingAverage")]
        public void Lwma2ComputesCorrectly()
        {
            const int period = 2;
            decimal[] values = { 1m, 2m };

            var lwma = new LinearWeightedMovingAverage(period);

            decimal current = 0m;
            for (int i = 0; i < values.Length; i++)
            {
                lwma.Update(new IndicatorDataPoint(DateTime.UtcNow.AddSeconds(i), TimeZone.Utc, values[i]));
            }
            current = ((2 * 2m) + (1 * 1m)) / 3;
            Assert.Equal(current, lwma.Current.Price);
        }

        [Fact]
        [Trait("Quantler.Indicators", "LinearWeightedMovingAverage")]
        public void Lwma4ComputesCorrectly()
        {
            const int period = 4;
            decimal[] values = { 1m, 2m, 3m, 4m };

            var lwma = new LinearWeightedMovingAverage(period);

            decimal current = 0m;
            for (int i = 0; i < values.Length; i++)
            {
                lwma.Update(new IndicatorDataPoint(DateTime.UtcNow.AddSeconds(i), TimeZone.Utc, values[i]));
            }
            current = ((4 * .4m) + (3 * .3m) + (2 * .2m) + (1 * .1m));
            Assert.Equal(current, lwma.Current.Price);
        }

        [Fact]
        [Trait("Quantler.Indicators", "LinearWeightedMovingAverage")]
        // See http://en.wikipedia.org/wiki/Moving_average
        // for the formula and the numbers in this test.
        public void Lwma5ComputesCorrectly()
        {
            const int period = 5;
            decimal[] values = { 77m, 79m, 79m, 81m, 83m };

            var lwma = new LinearWeightedMovingAverage(period);

            decimal current = 0m;
            for (int i = 0; i < values.Length; i++)
            {
                lwma.Update(new IndicatorDataPoint(DateTime.UtcNow.AddSeconds(i), TimeZone.Utc, values[i]));
            }
            current = 83 * (5m / 15) + 81 * (4m / 15) + 79 * (3m / 15) + 79 * (2m / 15) + 77 * (1m / 15);
            Assert.Equal(current, lwma.Current.Price);
        }

        [Fact]
        [Trait("Quantler.Indicators", "LinearWeightedMovingAverage")]
        public void ResetsProperly()
        {
            const int period = 4;
            decimal[] values = { 1m, 2m, 3m, 4m, 5m };

            var lwma = new LinearWeightedMovingAverage(period);

            for (int i = 0; i < values.Length; i++)
            {
                lwma.Update(new IndicatorDataPoint(DateTime.UtcNow.AddSeconds(i), TimeZone.Utc, values[i]));
            }
            Assert.True(lwma.IsReady);
            Assert.NotEqual(0m, lwma.Current.Price);
            Assert.NotEqual(0, lwma.Samples);

            lwma.Reset();

            TestHelper.AssertIndicatorIsInDefaultState(lwma);
        }

        #endregion Public Methods
    }
}