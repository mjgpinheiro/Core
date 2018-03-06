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
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Quantler.Tests.Indicators
{
    public class SimpleMovingAverageTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "SimpleMovingAverage")]
        public void CompareAgainstExternalData()
        {
            var sma = new SimpleMovingAverage(14);
            TestHelper.TestIndicator(sma, "SMA14", 1e-2); // test file only has
        }

        [Fact]
        [Trait("Quantler.Indicators", "SimpleMovingAverage")]
        public void IsReadyAfterPeriodUpdates()
        {
            var sma = new SimpleMovingAverage(3);

            sma.Update(DateTime.UtcNow, TimeZone.Utc, 1m);
            sma.Update(DateTime.UtcNow, TimeZone.Utc, 1m);
            Assert.False(sma.IsReady);
            sma.Update(DateTime.UtcNow, TimeZone.Utc, 1m);
            Assert.True(sma.IsReady);
        }

        [Fact]
        [Trait("Quantler.Indicators", "SimpleMovingAverage")]
        public void ResetsProperly()
        {
            var sma = new SimpleMovingAverage(3);

            foreach (var data in TestHelper.GetDataStream(4))
            {
                sma.Update(data);
            }
            Assert.True(sma.IsReady);

            sma.Reset();

            TestHelper.AssertIndicatorIsInDefaultState(sma);
            TestHelper.AssertIndicatorIsInDefaultState(sma.RollingSum);
            sma.Update(DateTime.UtcNow, TimeZone.Utc, 2.0m);
            Assert.Equal(2.0m, sma.Current.Price);
        }

        [Fact]
        [Trait("Quantler.Indicators", "SimpleMovingAverage")]
        public void SMAComputesCorrectly()
        {
            var sma = new SimpleMovingAverage(4);
            var data = new[] { 1m, 10m, 100m, 1000m, 10000m, 1234m, 56789m };

            var seen = new List<decimal>();
            for (int i = 0; i < data.Length; i++)
            {
                var datum = data[i];
                seen.Add(datum);
                sma.Update(new IndicatorDataPoint(DateTime.Now.AddSeconds(i), TimeZone.Utc, datum));
                Assert.Equal(Enumerable.Reverse(seen).Take(sma.Period).Average(), sma.Current.Price);
            }
        }

        #endregion Public Methods
    }
}