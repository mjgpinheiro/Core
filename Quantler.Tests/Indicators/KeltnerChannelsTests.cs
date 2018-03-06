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

using FluentAssertions;
using Quantler.Indicators;
using Xunit;

namespace Quantler.Tests.Indicators
{
    public class KeltnerChannelsTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "KeltnerChannels")]
        public void ComparesWithExtenralDataMiddleBand()
        {
            var kch = new KeltnerChannels(20, 1.5m, MovingAverageType.Simple);
            TestHelper.TestIndicator(kch, "spy_with_keltner.csv", "Middle Band",
                (ind, expected) => ((double) ((KeltnerChannels) ind).MiddleBand.Current.Price).Should()
                    .BeApproximately(expected, 1e-3));
        }

        [Fact]
        [Trait("Quantler.Indicators", "KeltnerChannels")]
        public void ComparesWithExternalDataLowerBand()
        {
            var kch = new KeltnerChannels(20, 1.5m, MovingAverageType.Simple);
            TestHelper.TestIndicator(kch, "spy_with_keltner.csv", "Keltner Channels 20 Bottom",
                (ind, expected) => ((double) ((KeltnerChannels) ind).LowerBand.Current.Price).Should()
                    .BeApproximately(expected, 1e-3));
        }

        [Fact]
        [Trait("Quantler.Indicators", "KeltnerChannels")]
        public void ComparesWithExternalDataUpperBand()
        {
            var kch = new KeltnerChannels(20, 1.5m, MovingAverageType.Simple);
            TestHelper.TestIndicator(kch, "spy_with_keltner.csv", "Keltner Channels 20 Top",
                (ind, expected) => ((double) ((KeltnerChannels) ind).UpperBand.Current.Price).Should()
                    .BeApproximately(expected, 1e-3));
        }

        [Fact]
        [Trait("Quantler.Indicators", "KeltnerChannels")]
        public void ResetsProperly()
        {
            var kch = new KeltnerChannels(20, 1.5m, MovingAverageType.Simple);
            foreach (var data in TestHelper.GetTradeBarStream("spy_with_keltner.csv", false))
            {
                kch.Update(data);
            }

            Assert.True(kch.IsReady);
            Assert.True(kch.UpperBand.IsReady);
            Assert.True(kch.LowerBand.IsReady);
            Assert.True(kch.MiddleBand.IsReady);
            Assert.True(kch.AverageTrueRange.IsReady);

            kch.Reset();

            TestHelper.AssertIndicatorIsInDefaultState(kch);
            TestHelper.AssertIndicatorIsInDefaultState(kch.UpperBand);
            TestHelper.AssertIndicatorIsInDefaultState(kch.LowerBand);
            TestHelper.AssertIndicatorIsInDefaultState(kch.AverageTrueRange);
        }

        #endregion Public Methods
    }
}