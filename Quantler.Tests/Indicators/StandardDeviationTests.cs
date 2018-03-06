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
    public class StandardDeviationTests : CommonIndicatorTests<IndicatorDataPoint>
    {
        #region Protected Properties

        protected override Action<IndicatorBase<IndicatorDataPoint>, double> Assertion
        {
            get
            {
                return (indicator, expected) =>
                    ((double) indicator.Current.Price).Should().BeApproximately(Math.Sqrt(expected), 1e-6);
            }
        }

        protected override string TestColumnName => "Var";

        protected override string TestFileName => "spy_var.txt";

        #endregion Protected Properties

        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "StandardDeviation")]
        public void ComputesCorrectly()
        {
            // Indicator output was compared against the following function in Julia
            // stdpop(v) = sqrt(sum((v - mean(v)).^2) / length(v))
            var std = new StandardDeviation(3);
            var reference = DateTime.MinValue;

            std.Update(reference.AddDays(1), TimeZone.Utc, 1m);
            Assert.Equal(0m, std.Current.Price);

            std.Update(reference.AddDays(2), TimeZone.Utc, -1m);
            Assert.Equal(1m, std.Current.Price);

            std.Update(reference.AddDays(3), TimeZone.Utc, 1m);
            Assert.Equal(0.942809041582063m, std.Current.Price);

            std.Update(reference.AddDays(4), TimeZone.Utc, -2m);
            Assert.Equal(1.24721912892465m, std.Current.Price);

            std.Update(reference.AddDays(5), TimeZone.Utc, 3m);
            Assert.Equal(2.05480466765633m, std.Current.Price);
        }

        [Fact]
        [Trait("Quantler.Indicators", "StandardDeviation")]
        public void ResetsProperlyStandardDeviation()
        {
            var std = new StandardDeviation(3);
            std.Update(DateTime.Today, TimeZone.Utc, 1m);
            std.Update(DateTime.Today.AddSeconds(1), TimeZone.Utc, 5m);
            std.Update(DateTime.Today.AddSeconds(2), TimeZone.Utc, 1m);
            Assert.True(std.IsReady);

            std.Reset();
            TestHelper.AssertIndicatorIsInDefaultState(std);
        }

        #endregion Public Methods

        #region Protected Methods

        protected override IndicatorBase<IndicatorDataPoint> CreateIndicator()
        {
            return new StandardDeviation(10);
        }

        #endregion Protected Methods
    }
}