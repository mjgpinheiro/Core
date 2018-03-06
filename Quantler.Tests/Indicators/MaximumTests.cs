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
    public class MaximumTests : CommonIndicatorTests<IndicatorDataPoint>
    {
        #region Protected Properties

        protected override string TestColumnName => "MAX_5";

        protected override string TestFileName => "spy_max.txt";

        #endregion Protected Properties

        #region Public Methods

        [Fact]
        public void ComputesCorrectly()
        {
            var max = new Maximum(3);

            var reference = DateTime.MinValue;

            max.Update(reference.AddDays(1), TimeZone.Utc, 1m);
            Assert.Equal(1m, max.Current.Price);
            Assert.Equal(0, max.PeriodsSinceMaximum);

            max.Update(reference.AddDays(2), TimeZone.Utc, -1m);
            Assert.Equal(1m, max.Current.Price);
            Assert.Equal(1, max.PeriodsSinceMaximum);

            max.Update(reference.AddDays(3), TimeZone.Utc, 0m);
            Assert.Equal(1m, max.Current.Price);
            Assert.Equal(2, max.PeriodsSinceMaximum);

            max.Update(reference.AddDays(4), TimeZone.Utc, -2m);
            Assert.Equal(0m, max.Current.Price);
            Assert.Equal(1, max.PeriodsSinceMaximum);

            max.Update(reference.AddDays(5), TimeZone.Utc, -2m);
            Assert.Equal(0m, max.Current.Price);
            Assert.Equal(2, max.PeriodsSinceMaximum);
        }

        [Fact]
        public void ComputesCorrectlyMaximum()
        {
            const int period = 5;
            var max = new Maximum(period);

            Assert.Equal(0m, max.Current.Price);

            // test an increasing stream of data
            for (int i = 0; i < period; i++)
            {
                max.Update(DateTime.Now.AddDays(i), TimeZone.Utc, i);
                Assert.Equal(i, max.Current.Price);
                Assert.Equal(0, max.PeriodsSinceMaximum);
            }

            // test a decreasing stream of data
            for (int i = 0; i < period; i++)
            {
                max.Update(DateTime.Now.AddDays(period + i), TimeZone.Utc, period - i - 1);
                Assert.Equal(period - 1, max.Current.Price);
                Assert.Equal(i, max.PeriodsSinceMaximum);
            }

            Assert.Equal(max.Period, max.PeriodsSinceMaximum + 1);
        }

        [Fact]
        public void ResetsProperlyMaximum()
        {
            var max = new Maximum(3);
            max.Update(DateTime.Today, TimeZone.Utc, 1m);
            max.Update(DateTime.Today.AddSeconds(1), TimeZone.Utc, 2m);
            max.Update(DateTime.Today.AddSeconds(2), TimeZone.Utc, 1m);
            Assert.True(max.IsReady);

            max.Reset();
            Assert.Equal(0, max.PeriodsSinceMaximum);
            TestHelper.AssertIndicatorIsInDefaultState(max);
        }

        #endregion Public Methods

        #region Protected Methods

        protected override IndicatorBase<IndicatorDataPoint> CreateIndicator()
        {
            return new Maximum(5);
        }

        #endregion Protected Methods
    }
}