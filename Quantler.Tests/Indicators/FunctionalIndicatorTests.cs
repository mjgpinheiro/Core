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
    public class FunctionalIndicatorTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "FunctionalIndicator")]
        public void ComputesDelegateCorrectly()
        {
            var func = new FunctionalIndicator<IndicatorDataPoint>("f", data => data.Price, @this => @this.Samples > 1, () => {/*no reset action required*/});
            func.Update(DateTime.Today, TimeZone.Utc, 1m);
            Assert.False(func.IsReady);
            Assert.Equal(1m, func.Current.Price);

            func.Update(DateTime.Today.AddSeconds(1), TimeZone.Utc, 2m);
            Assert.True(func.IsReady);
            Assert.Equal(2m, func.Current.Price);
        }

        [Fact]
        [Trait("Quantler.Indicators", "FunctionalIndicator")]
        public void ResetsProperly()
        {
            var inner = new SimpleMovingAverage(2);
            var func = new FunctionalIndicator<IndicatorDataPoint>("f", data =>
            {
                inner.Update(data);
                return inner.Current.Price * 2;
            },
            @this => inner.IsReady,
            () => inner.Reset()
            );

            func.Update(DateTime.Today, TimeZone.Utc, 1m);
            func.Update(DateTime.Today.AddSeconds(1), TimeZone.Utc, 2m);
            Assert.True(func.IsReady);

            func.Reset();
            TestHelper.AssertIndicatorIsInDefaultState(inner);
            TestHelper.AssertIndicatorIsInDefaultState(func);
        }

        #endregion Public Methods
    }
}