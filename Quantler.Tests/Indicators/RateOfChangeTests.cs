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
    public class RateOfChangeTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "RateOfChange")]
        public void ComputesCorrectly()
        {
            var roc = new RateOfChange(50);
            double epsilon = 1e-3;
            TestHelper.TestIndicator(roc, "spy_with_rocp50.txt", "Rate of Change % 50",
                (ind, expected) => ((double) ind.Current.Price * 100).Should().BeApproximately(expected, epsilon));
        }

        [Fact]
        [Trait("Quantler.Indicators", "RateOfChange")]
        public void ResetsProperly()
        {
            var roc = new RateOfChange(50);
            foreach (var data in TestHelper.GetDataStream(51))
            {
                roc.Update(data);
            }
            Assert.True(roc.IsReady);

            roc.Reset();

            TestHelper.AssertIndicatorIsInDefaultState(roc);
        }

        #endregion Public Methods
    }
}