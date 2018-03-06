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
    public class CompositeIndicatorTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "CompositeIndicator")]
        public void CallsDelegateCorrectly()
        {
            var left = new Identity("left");
            var right = new Identity("right");
            var composite = new CompositeIndicator<IndicatorDataPoint>(left, right, (l, r) =>
            {
                Assert.Equal(left, l);
                Assert.Equal(right, r);
                return l + r;
            });

            left.Update(DateTime.Today, TimeZone.Utc, 1m);
            right.Update(DateTime.Today, TimeZone.Utc, 1m);
            Assert.Equal(2m, composite.Current.Price);
        }

        [Fact]
        [Trait("Quantler.Indicators", "CompositeIndicator")]
        public void CompositeIsReadyWhenBothAre()
        {
            var left = new Delay(1);
            var right = new Delay(2);
            var composite = new CompositeIndicator<IndicatorDataPoint>(left, right, (l, r) => l + r);

            left.Update(DateTime.Today.AddSeconds(0), TimeZone.Utc, 1m);
            right.Update(DateTime.Today.AddSeconds(0), TimeZone.Utc, 1m);
            Assert.False(composite.IsReady);
            Assert.False(composite.Left.IsReady);
            Assert.False(composite.Right.IsReady);

            left.Update(DateTime.Today.AddSeconds(1), TimeZone.Utc, 2m);
            right.Update(DateTime.Today.AddSeconds(1), TimeZone.Utc, 2m);
            Assert.False(composite.IsReady);
            Assert.True(composite.Left.IsReady);
            Assert.False(composite.Right.IsReady);

            left.Update(DateTime.Today.AddSeconds(2), TimeZone.Utc, 3m);
            right.Update(DateTime.Today.AddSeconds(2), TimeZone.Utc, 3m);
            Assert.True(composite.IsReady);
            Assert.True(composite.Left.IsReady);
            Assert.True(composite.Right.IsReady);

            left.Update(DateTime.Today.AddSeconds(3), TimeZone.Utc, 4m);
            right.Update(DateTime.Today.AddSeconds(3), TimeZone.Utc, 4m);
            Assert.True(composite.IsReady);
            Assert.True(composite.Left.IsReady);
            Assert.True(composite.Right.IsReady);
        }

        [Fact]
        [Trait("Quantler.Indicators", "CompositeIndicator")]
        public void ResetsProperly()
        {
            var left = new Maximum("left", 2);
            var right = new Minimum("right", 2);
            var composite = new CompositeIndicator<IndicatorDataPoint>(left, right, (l, r) => l + r);

            left.Update(DateTime.Today, TimeZone.Utc, 1m);
            right.Update(DateTime.Today, TimeZone.Utc, -1m);

            left.Update(DateTime.Today.AddDays(1), TimeZone.Utc, -1m);
            right.Update(DateTime.Today.AddDays(1), TimeZone.Utc, 1m);

            Assert.Equal(1, left.PeriodsSinceMaximum);
            Assert.Equal(1, right.PeriodsSinceMinimum);

            composite.Reset();
            TestHelper.AssertIndicatorIsInDefaultState(composite);
            TestHelper.AssertIndicatorIsInDefaultState(left);
            TestHelper.AssertIndicatorIsInDefaultState(right);
            Assert.Equal(0, left.PeriodsSinceMaximum);
            Assert.Equal(0, right.PeriodsSinceMinimum);
        }

        #endregion Public Methods
    }
}