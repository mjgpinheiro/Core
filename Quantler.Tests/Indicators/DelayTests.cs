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
using FluentAssertions;
using Xunit;

namespace Quantler.Tests.Indicators
{
    public class DelayTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "Delay")]
        public void DelayOneRepeatsFirstInputValue()
        {
            var delay = new Delay(1);

            var data = new IndicatorDataPoint(DateTime.UtcNow, TimeZone.Utc, 1m);
            delay.Update(data);
            Assert.Equal(1m, delay.Current.Price);

            data = new IndicatorDataPoint(DateTime.UtcNow.AddSeconds(1), TimeZone.Utc, 2m);
            delay.Update(data);
            Assert.Equal(1m, delay.Current.Price);

            data = new IndicatorDataPoint(DateTime.UtcNow.AddSeconds(1), TimeZone.Utc, 2m);
            delay.Update(data);
            Assert.Equal(2m, delay.Current.Price);
        }

        [Fact]
        [Trait("Quantler.Indicators", "Delay")]
        public void DelayTakesPeriodPlus2UpdatesToEmitNonInitialPoint()
        {
            int start = 1;
            int count = 10;
            for (int i = start; i < count + start; i++)
            {
                TestDelayTakesPeriodPlus2UpdatesToEmitNonInitialPoint(i);
            }
        }

        [Fact]
        [Trait("Quantler.Indicators", "Delay")]
        public void DelayZeroThrowsArgumentException()
        {
            Action act = () => new Delay(0);
            act.ShouldThrow<ArgumentException>()
                .Which.Message.Should().StartWith("RollingWindow must have size of at least 1");
        }

        [Fact]
        [Trait("Quantler.Indicators", "Delay")]
        public void ResetsProperly()
        {
            var delay = new Delay(2);

            foreach (var data in TestHelper.GetDataStream(3))
            {
                delay.Update(data);
            }
            Assert.True(delay.IsReady);
            Assert.Equal(3, delay.Samples);

            delay.Reset();

            TestHelper.AssertIndicatorIsInDefaultState(delay);
        }

        #endregion Public Methods

        #region Private Methods

        private void TestDelayTakesPeriodPlus2UpdatesToEmitNonInitialPoint(int period)
        {
            var delay = new Delay(period);
            for (int i = 0; i < period + 2; i++)
            {
                Assert.Equal(0m, delay.Current.Price);
                delay.Update(new IndicatorDataPoint(DateTime.Today.AddSeconds(i), TimeZone.Utc, i));
            }
            Assert.Equal(1m, delay.Current.Price);
        }

        #endregion Private Methods
    }
}