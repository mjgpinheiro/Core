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
    public class WindowIdentityTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "WindowIdentity")]
        public void CompareAgainstExternalData()
        {
            var indicator = new WindowIdentity(14);
            TestHelper.TestIndicator(indicator, "Close", 1e-2);
        }

        [Fact]
        [Trait("Quantler.Indicators", "WindowIdentity")]
        public void IsReadyAfterPeriodUpdates()
        {
            var indicator = new WindowIdentity(3);

            indicator.Update(DateTime.UtcNow, TimeZone.Utc, 1m);
            indicator.Update(DateTime.UtcNow, TimeZone.Utc, 1m);
            Assert.False(indicator.IsReady);
            indicator.Update(DateTime.UtcNow, TimeZone.Utc, 1m);
            Assert.True(indicator.IsReady);
        }

        [Fact]
        [Trait("Quantler.Indicators", "WindowIdentity")]
        public void ResetsProperly()
        {
            var indicator = new WindowIdentity(3);

            foreach (var data in TestHelper.GetDataStream(4))
            {
                indicator.Update(data);
            }
            Assert.True(indicator.IsReady);

            indicator.Reset();

            TestHelper.AssertIndicatorIsInDefaultState(indicator);
            indicator.Update(DateTime.UtcNow, TimeZone.Utc, 2.0m);
            Assert.Equal(2.0m, indicator.Current.Price);
        }

        [Fact]
        [Trait("Quantler.Indicators", "WindowIdentity")]
        public void WindowIdentityComputesCorrectly()
        {
            var indicator = new WindowIdentity(4);
            var data = new[] { 1m, 10m, 100m, 1000m, 10000m, 1234m, 56789m };

            var seen = new List<decimal>();
            for (int i = 0; i < data.Length; i++)
            {
                var datum = data[i];
                seen.Add(datum);
                indicator.Update(new IndicatorDataPoint(DateTime.Now.AddSeconds(i), TimeZone.Utc, datum));
                Assert.Equal(seen.LastOrDefault(), indicator.Current.Price);
            }
        }

        #endregion Public Methods
    }
}