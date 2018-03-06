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
    public class DonchianChannelTest
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "DonchianChannel")]
        public void CompareAgainstExternalDataForLowerBand()
        {
            var donchianChannel = new DonchianChannel("dch", 50);

            TestHelper.TestIndicator(donchianChannel, "spy_with_don50.txt", "Donchian Channels 50 Bottom",
                (ind, expected) => ((double) ((DonchianChannel) ind).LowerBand.Current.Price).Should().Be(expected));
        }

        [Fact]
        [Trait("Quantler.Indicators", "DonchianChannel")]
        public void CompareAgainstExternalDataForUpperBand()
        {
            var donchianChannel = new DonchianChannel("dch", 50);

            TestHelper.TestIndicator(donchianChannel, "spy_with_don50.txt", "Donchian Channels 50 Top",
                    (ind, expected) => Assert.Equal(expected, (double)((DonchianChannel)ind).UpperBand.Current.Price));
        }

        [Fact]
        [Trait("Quantler.Indicators", "DonchianChannel")]
        public void ComputesPrimaryOutputCorrectly()
        {
            var donchianChannel = new DonchianChannel("dch", 50);

            TestHelper.TestIndicator(donchianChannel, "spy_with_don50.txt", "Donchian Channels 50 Mean",
                    (ind, expected) => Assert.Equal(expected, (double)((DonchianChannel)ind).Current.Price));
        }

        [Fact]
        [Trait("Quantler.Indicators", "DonchianChannel")]
        public void ResetsProperly()
        {
            var donchianChannelIndicator = new DonchianChannel("DCH", 50);
            foreach (var data in TestHelper.GetTradeBarStream("spy_with_don50.txt", false))
            {
                donchianChannelIndicator.Update(data);
            }

            Assert.True(donchianChannelIndicator.IsReady);
            Assert.True(donchianChannelIndicator.UpperBand.IsReady);
            Assert.True(donchianChannelIndicator.LowerBand.IsReady);

            donchianChannelIndicator.Reset();

            TestHelper.AssertIndicatorIsInDefaultState(donchianChannelIndicator);
            TestHelper.AssertIndicatorIsInDefaultState(donchianChannelIndicator.UpperBand);
            TestHelper.AssertIndicatorIsInDefaultState(donchianChannelIndicator.LowerBand);
        }

        #endregion Public Methods
    }
}