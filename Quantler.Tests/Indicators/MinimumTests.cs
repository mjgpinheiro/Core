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
    public class MinimumTests : CommonIndicatorTests<IndicatorDataPoint>
    {
        #region Protected Properties

        protected override string TestColumnName => "MIN_5";

        protected override string TestFileName => "spy_min.txt";

        #endregion Protected Properties

        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "Minimum")]
        public void ComputesCorrectly()
        {
            var min = new Minimum(3);

            var reference = DateTime.UtcNow;

            min.Update(reference, TimeZone.Utc, 1m);
            Assert.Equal(1m, min.Current.Price);
            Assert.Equal(0, min.PeriodsSinceMinimum);

            min.Update(reference.AddDays(1), TimeZone.Utc, 2m);
            Assert.Equal(1m, min.Current.Price);
            Assert.Equal(1, min.PeriodsSinceMinimum);

            min.Update(reference.AddDays(2), TimeZone.Utc, -1m);
            Assert.Equal(-1m, min.Current.Price);
            Assert.Equal(0, min.PeriodsSinceMinimum);

            min.Update(reference.AddDays(3), TimeZone.Utc, 2m);
            Assert.Equal(-1m, min.Current.Price);
            Assert.Equal(1, min.PeriodsSinceMinimum);

            min.Update(reference.AddDays(4), TimeZone.Utc, 0m);
            Assert.Equal(-1m, min.Current.Price);
            Assert.Equal(2, min.PeriodsSinceMinimum);

            min.Update(reference.AddDays(5), TimeZone.Utc, 3m);
            Assert.Equal(0m, min.Current.Price);
            Assert.Equal(1, min.PeriodsSinceMinimum);

            min.Update(reference.AddDays(6), TimeZone.Utc, 2m);
            Assert.Equal(0m, min.Current.Price);
            Assert.Equal(2, min.PeriodsSinceMinimum);
        }

        [Fact]
        [Trait("Quantler.Indicators", "Minimum")]
        public void ResetsProperlyMinimum()
        {
            var min = new Minimum(3);
            min.Update(DateTime.Today, TimeZone.Utc, 1m);
            min.Update(DateTime.Today.AddSeconds(1), TimeZone.Utc, 2m);
            min.Update(DateTime.Today.AddSeconds(2), TimeZone.Utc, 1m);
            Assert.True(min.IsReady);

            min.Reset();
            Assert.Equal(0, min.PeriodsSinceMinimum);
            TestHelper.AssertIndicatorIsInDefaultState(min);
        }

        #endregion Public Methods

        #region Protected Methods

        protected override IndicatorBase<IndicatorDataPoint> CreateIndicator()
        {
            return new Minimum(5);
        }

        #endregion Protected Methods
    }
}