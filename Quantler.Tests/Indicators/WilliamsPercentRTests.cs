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
    public class WilliamsPercentRTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "WilliamsPercentR")]
        public void ComputesCorrectly()
        {
            var wilr = new WilliamsPercentR(14);
            TestHelper.TestIndicator(wilr, "spy_with_williamsR14.txt", "Williams %R 14", (ind, expected) =>
                ((double) ind.Current.Price).Should().BeApproximately(expected, 1e-3));
        }

        [Fact]
        [Trait("Quantler.Indicators", "WilliamsPercentR")]
        public void ResetsProperly()
        {
            var wilr = new WilliamsPercentR(14);
            foreach (var data in TestHelper.GetTradeBarStream("spy_with_williamsR14.txt", false))
            {
                wilr.Update(data);
            }
            Assert.True(wilr.IsReady);

            wilr.Reset();

            TestHelper.AssertIndicatorIsInDefaultState(wilr);
        }

        #endregion Public Methods
    }
}