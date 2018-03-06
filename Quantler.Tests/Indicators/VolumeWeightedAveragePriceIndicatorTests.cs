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
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Quantler.Tests.Indicators
{
    public class VolumeWeightedAveragePriceIndicatorTests
    {
        #region Public Methods

        [Fact(Skip = "Fix Me")]
        [Trait("Quantler.Indicators", "VolumeWeightedAveragePriceIndicator")]
        public void CompareAgainstExternalData()
        {
            var ind = new VolumeWeightedAveragePriceIndicator(50);
            TestHelper.TestIndicator(ind, "spy_with_vwap.txt", "Moving VWAP 50",
                (x, expected) => ((double) ((VolumeWeightedAveragePriceIndicator) x).Current.Price).Should()
                    .BeApproximately(expected, 1e-3));
        }

        [Fact]
        [Trait("Quantler.Indicators", "VolumeWeightedAveragePriceIndicator")]
        public void IsReadyAfterPeriodUpdates()
        {
            var ind = new VolumeWeightedAveragePriceIndicator(3);

            ind.Update(new TradeBar(DateTime.UtcNow, TimeZone.Utc, Props.TickerSymbol, 1m, 1m, 1m, 1m, 1));
            ind.Update(new TradeBar(DateTime.UtcNow, TimeZone.Utc, Props.TickerSymbol, 1m, 1m, 1m, 1m, 1));
            Assert.False(ind.IsReady);
            ind.Update(new TradeBar(DateTime.UtcNow, TimeZone.Utc, Props.TickerSymbol, 1m, 1m, 1m, 1m, 1));
            Assert.True(ind.IsReady);
        }

        [Fact]
        [Trait("Quantler.Indicators", "VolumeWeightedAveragePriceIndicator")]
        public void ResetsProperly()
        {
            var ind = new VolumeWeightedAveragePriceIndicator(50);

            foreach (var data in TestHelper.GetTradeBarStream("spy_with_vwap.txt"))
            {
                ind.Update(data);
            }
            Assert.True(ind.IsReady);

            ind.Reset();

            TestHelper.AssertIndicatorIsInDefaultState(ind);
            ind.Update(new TradeBar(DateTime.UtcNow, TimeZone.Utc, Props.TickerSymbol, 2m, 2m, 2m, 2m, 1));
            Assert.Equal(2m, ind.Current.Price);
        }

        [Fact]
        [Trait("Quantler.Indicators", "VolumeWeightedAveragePriceIndicator")]
        public void VWAPComputesCorrectly()
        {
            const int period = 4;
            const int volume = 100;
            var ind = new VolumeWeightedAveragePriceIndicator(period);
            var data = new[] { 1m, 10m, 100m, 1000m, 10000m, 1234m, 56789m };

            var seen = new List<decimal>();
            for (int i = 0; i < data.Length; i++)
            {
                var datum = data[i];
                seen.Add(datum);
                ind.Update(new TradeBar(DateTime.Now.AddSeconds(i), TimeZone.Utc, Props.TickerSymbol, datum, datum, datum, datum, volume));
                // When volume is constant, VWAP is a simple moving average
                Assert.Equal(Enumerable.Reverse(seen).Take(period).Average(), ind.Current.Price);
            }
        }

        #endregion Public Methods
    }
}