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
    public class MeanAbsoluteDeviationTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "MeanAbsoluteDeviation")]
        public void ComputesCorrectly()
        {
            // Indicator output was compared against the octave code:
            // mad = @(v) mean(abs(v - mean(v)));
            var std = new MeanAbsoluteDeviation(3);
            var reference = DateTime.MinValue;

            std.Update(reference.AddDays(1), TimeZone.Utc, 1m);
            Assert.Equal(0m, std.Current.Price);

            std.Update(reference.AddDays(2), TimeZone.Utc, -1m);
            Assert.Equal(1m, std.Current.Price);

            std.Update(reference.AddDays(3), TimeZone.Utc, 1m);
            Assert.Equal(0.888888888888889m, Decimal.Round(std.Current.Price, 15));

            std.Update(reference.AddDays(4), TimeZone.Utc, -2m);
            Assert.Equal(1.111111111111111m, Decimal.Round(std.Current.Price, 15));

            std.Update(reference.AddDays(5), TimeZone.Utc, 3m);
            Assert.Equal(1.777777777777778m, Decimal.Round(std.Current.Price, 15));
        }

        [Fact]
        [Trait("Quantler.Indicators", "MeanAbsoluteDeviation")]
        public void ResetsProperly()
        {
            var std = new MeanAbsoluteDeviation(3);
            std.Update(DateTime.Today, TimeZone.Utc, 1m);
            std.Update(DateTime.Today.AddSeconds(1), TimeZone.Utc, 2m);
            std.Update(DateTime.Today.AddSeconds(1), TimeZone.Utc, 1m);
            Assert.True(std.IsReady);

            std.Reset();

            TestHelper.AssertIndicatorIsInDefaultState(std);
            TestHelper.AssertIndicatorIsInDefaultState(std.Mean);
        }

        #endregion Public Methods
    }
}