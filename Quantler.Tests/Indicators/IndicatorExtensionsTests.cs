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
using System.Linq;
using Xunit;

namespace Quantler.Tests.Indicators
{
    public class IndicatorExtensionsTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "IndicatorExtensions")]
        public void MinusSubtractsLeftAndRightAfterBothUpdated()
        {
            var left = new Identity("left");
            var right = new Identity("right");
            var composite = left.Minus(right);

            left.Update(DateTime.Today, TimeZone.Utc, 1m);
            right.Update(DateTime.Today, TimeZone.Utc, 1m);
            Assert.Equal(0m, composite.Current.Price);

            left.Update(DateTime.Today, TimeZone.Utc, 2m);
            Assert.Equal(0m, composite.Current.Price);

            left.Update(DateTime.Today, TimeZone.Utc, 3m);
            Assert.Equal(0m, composite.Current.Price);

            right.Update(DateTime.Today, TimeZone.Utc, 4m);
            Assert.Equal(-1m, composite.Current.Price);
        }

        [Fact]
        [Trait("Quantler.Indicators", "IndicatorExtensions")]
        public void MultiChainEMA()
        {
            var identity = new Identity("identity");
            var delay = new Delay(2);

            // create the EMA of chained methods
            var ema = delay.Of(identity).EMA(2, 1);

            // Assert.True(ema. == 1);
            identity.Update(DateTime.UtcNow, TimeZone.Utc, 1m);
            Assert.True(identity.IsReady);
            Assert.False(delay.IsReady);
            Assert.False(ema.IsReady);

            identity.Update(DateTime.UtcNow, TimeZone.Utc, 2m);
            Assert.True(identity.IsReady);
            Assert.False(delay.IsReady);
            Assert.False(ema.IsReady);

            identity.Update(DateTime.UtcNow, TimeZone.Utc, 3m);
            Assert.True(identity.IsReady);
            Assert.True(delay.IsReady);
            Assert.False(ema.IsReady);

            identity.Update(DateTime.UtcNow, TimeZone.Utc, 4m);
            Assert.True(identity.IsReady);
            Assert.True(delay.IsReady);
            Assert.True(ema.IsReady);
        }

        [Fact]
        [Trait("Quantler.Indicators", "IndicatorExtensions")]
        public void MultiChainMAX()
        {
            var identity = new Identity("identity");
            var delay = new Delay(2);

            // create the MAX of the delay of the identity
            var max = delay.Of(identity).MAX(2);

            identity.Update(DateTime.UtcNow, TimeZone.Utc, 1m);
            Assert.True(identity.IsReady);
            Assert.False(delay.IsReady);
            Assert.False(max.IsReady);

            identity.Update(DateTime.UtcNow, TimeZone.Utc, 2m);
            Assert.True(identity.IsReady);
            Assert.False(delay.IsReady);
            Assert.False(max.IsReady);

            identity.Update(DateTime.UtcNow, TimeZone.Utc, 3m);
            Assert.True(identity.IsReady);
            Assert.True(delay.IsReady);
            Assert.False(max.IsReady);

            identity.Update(DateTime.UtcNow, TimeZone.Utc, 4m);
            Assert.True(identity.IsReady);
            Assert.True(delay.IsReady);
            Assert.True(max.IsReady);
        }

        [Fact]
        [Trait("Quantler.Indicators", "IndicatorExtensions")]
        public void MultiChainMIN()
        {
            var identity = new Identity("identity");
            var delay = new Delay(2);

            // create the MIN of the delay of the identity
            var min = delay.Of(identity).MIN(2);

            identity.Update(DateTime.UtcNow, TimeZone.Utc, 1m);
            Assert.True(identity.IsReady);
            Assert.False(delay.IsReady);
            Assert.False(min.IsReady);

            identity.Update(DateTime.UtcNow, TimeZone.Utc, 2m);
            Assert.True(identity.IsReady);
            Assert.False(delay.IsReady);
            Assert.False(min.IsReady);

            identity.Update(DateTime.UtcNow, TimeZone.Utc, 3m);
            Assert.True(identity.IsReady);
            Assert.True(delay.IsReady);
            Assert.False(min.IsReady);

            identity.Update(DateTime.UtcNow, TimeZone.Utc, 4m);
            Assert.True(identity.IsReady);
            Assert.True(delay.IsReady);
            Assert.True(min.IsReady);
        }

        [Fact]
        [Trait("Quantler.Indicators", "IndicatorExtensions")]
        public void MultiChainSMA()
        {
            var identity = new Identity("identity");
            var delay = new Delay(2);

            // create the SMA of the delay of the identity
            var sma = delay.Of(identity).SMA(2);

            identity.Update(DateTime.UtcNow, TimeZone.Utc, 1m);
            Assert.True(identity.IsReady);
            Assert.False(delay.IsReady);
            Assert.False(sma.IsReady);

            identity.Update(DateTime.UtcNow, TimeZone.Utc, 2m);
            Assert.True(identity.IsReady);
            Assert.False(delay.IsReady);
            Assert.False(sma.IsReady);

            identity.Update(DateTime.UtcNow, TimeZone.Utc, 3m);
            Assert.True(identity.IsReady);
            Assert.True(delay.IsReady);
            Assert.False(sma.IsReady);

            identity.Update(DateTime.UtcNow, TimeZone.Utc, 4m);
            Assert.True(identity.IsReady);
            Assert.True(delay.IsReady);
            Assert.True(sma.IsReady);

            Assert.Equal(1.5m, sma);
        }

        [Fact]
        [Trait("Quantler.Indicators", "IndicatorExtensions")]
        public void NewDataPushesToDerivedIndicators()
        {
            var identity = new Identity("identity");
            var sma = new SimpleMovingAverage(3);

            identity.Updated += (sender, consolidated) =>
            {
                sma.Update(consolidated);
            };

            identity.Update(DateTime.UtcNow, TimeZone.Utc, 1m);
            identity.Update(DateTime.UtcNow, TimeZone.Utc, 2m);
            Assert.False(sma.IsReady);

            identity.Update(DateTime.UtcNow, TimeZone.Utc, 3m);
            Assert.True(sma.IsReady);
            Assert.Equal(2m, sma);
        }

        [Fact]
        [Trait("Quantler.Indicators", "IndicatorExtensions")]
        public void OverDivdesLeftAndRightAfterBothUpdated()
        {
            var left = new Identity("left");
            var right = new Identity("right");
            var composite = left.Over(right);

            left.Update(DateTime.Today, TimeZone.Utc, 1m);
            right.Update(DateTime.Today, TimeZone.Utc, 1m);
            Assert.Equal(1m, composite.Current.Price);

            left.Update(DateTime.Today, TimeZone.Utc, 2m);
            Assert.Equal(1m, composite.Current.Price);

            left.Update(DateTime.Today, TimeZone.Utc, 3m);
            Assert.Equal(1m, composite.Current.Price);

            right.Update(DateTime.Today, TimeZone.Utc, 4m);
            Assert.Equal(3m / 4m, composite.Current.Price);
        }

        [Fact]
        [Trait("Quantler.Indicators", "IndicatorExtensions")]
        public void OverHandlesDivideByZero()
        {
            var left = new Identity("left");
            var right = new Identity("right");
            var composite = left.Over(right);
            var updatedEventFired = false;
            composite.Updated += delegate { updatedEventFired = true; };

            left.Update(DateTime.Today, TimeZone.Utc, 1m);
            Assert.False(updatedEventFired);
            right.Update(DateTime.Today, TimeZone.Utc, 0m);
            Assert.False(updatedEventFired);

            // submitting another update to right won't cause an update without corresponding update to left
            right.Update(DateTime.Today, TimeZone.Utc, 1m);
            Assert.False(updatedEventFired);
            left.Update(DateTime.Today, TimeZone.Utc, 1m);
            Assert.True(updatedEventFired);
        }

        [Fact]
        [Trait("Quantler.Indicators", "IndicatorExtensions")]
        public void PipesDataFirstWeightedBySecond()
        {
            const int period = 4;
            var value = new Identity("Value");
            var weight = new Identity("Weight");

            var third = value.WeightedBy(weight, period);

            var data = Enumerable.Range(1, 10).ToList();
            var window = Enumerable.Reverse(data).Take(period);
            var current = window.Sum(x => 2 * x * x) / (decimal)window.Sum(x => x);

            foreach (var item in data)
            {
                value.Update(new IndicatorDataPoint(DateTime.UtcNow, TimeZone.Utc, Convert.ToDecimal(2 * item)));
                weight.Update(new IndicatorDataPoint(DateTime.UtcNow, TimeZone.Utc, Convert.ToDecimal(item)));
            }

            Assert.Equal(current, third.Current.Price);
        }

        [Fact]
        [Trait("Quantler.Indicators", "IndicatorExtensions")]
        public void PipesDataUsingOfFromFirstToSecond()
        {
            var first = new SimpleMovingAverage(2);
            var second = new Delay(1);

            // this is a configuration step, but returns the reference to the second for method chaining
            second.Of(first);

            var data1 = new IndicatorDataPoint(DateTime.UtcNow, TimeZone.Utc, 1m);
            var data2 = new IndicatorDataPoint(DateTime.UtcNow, TimeZone.Utc, 2m);
            var data3 = new IndicatorDataPoint(DateTime.UtcNow, TimeZone.Utc, 3m);
            var data4 = new IndicatorDataPoint(DateTime.UtcNow, TimeZone.Utc, 4m);

            // sma has one item
            first.Update(data1);
            Assert.False(first.IsReady);
            Assert.Equal(0m, second.Current.Price);

            // sma is ready, delay will repeat this value
            first.Update(data2);
            Assert.True(first.IsReady);
            Assert.False(second.IsReady);
            Assert.Equal(1.5m, second.Current.Price);

            // delay is ready, and repeats its first input
            first.Update(data3);
            Assert.True(second.IsReady);
            Assert.Equal(1.5m, second.Current.Price);

            // now getting the delayed data
            first.Update(data4);
            Assert.Equal(2.5m, second.Current.Price);
        }

        [Fact]
        [Trait("Quantler.Indicators", "IndicatorExtensions")]
        public void PlusAddsLeftAndRightAfterBothUpdated()
        {
            var left = new Identity("left");
            var right = new Identity("right");
            var composite = left.Plus(right);

            left.Update(DateTime.Today, TimeZone.Utc, 1m);
            right.Update(DateTime.Today, TimeZone.Utc, 1m);
            Assert.Equal(2m, composite.Current.Price);

            left.Update(DateTime.Today, TimeZone.Utc, 2m);
            Assert.Equal(2m, composite.Current.Price);

            left.Update(DateTime.Today, TimeZone.Utc, 3m);
            Assert.Equal(2m, composite.Current.Price);

            right.Update(DateTime.Today, TimeZone.Utc, 4m);
            Assert.Equal(7m, composite.Current.Price);
        }

        [Fact]
        [Trait("Quantler.Indicators", "IndicatorExtensions")]
        public void TimesMultipliesLeftAndRightAfterBothUpdated()
        {
            var left = new Identity("left");
            var right = new Identity("right");
            var composite = left.Times(right);

            left.Update(DateTime.Today, TimeZone.Utc, 1m);
            right.Update(DateTime.Today, TimeZone.Utc, 1m);
            Assert.Equal(1m, composite.Current.Price);

            left.Update(DateTime.Today, TimeZone.Utc, 2m);
            Assert.Equal(1m, composite.Current.Price);

            left.Update(DateTime.Today, TimeZone.Utc, 3m);
            Assert.Equal(1m, composite.Current.Price);

            right.Update(DateTime.Today, TimeZone.Utc, 4m);
            Assert.Equal(12m, composite.Current.Price);
        }

        #endregion Public Methods
    }
}