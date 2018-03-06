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
using Xunit;

namespace Quantler.Tests.Indicators
{
    public class MomentumTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "Momentum")]
        public void ComputesCorrectly()
        {
            var mom = new Momentum(5);
            foreach (var data in TestHelper.GetDataStream(5))
            {
                mom.Update(data);
                Assert.Equal(data.Price, mom.Current.Price);
            }
        }

        [Fact]
        [Trait("Quantler.Indicators", "Momentum")]
        public void ResetsProperly()
        {
            var mom = new Momentum(5);
            foreach (var data in TestHelper.GetDataStream(6))
            {
                mom.Update(data);
            }
            Assert.True(mom.IsReady);

            mom.Reset();

            TestHelper.AssertIndicatorIsInDefaultState(mom);
        }

        #endregion Public Methods
    }
}