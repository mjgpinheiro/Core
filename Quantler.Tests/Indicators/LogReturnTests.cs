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
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Quantler.Tests.Indicators
{
    public class LogReturnTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "LogReturn")]
        public void CompareAgainstExternalData()
        {
            var logr = new LogReturn(14);
            double epsilon = 1e-3;
            TestHelper.TestIndicator(logr, "spy_logr14.txt", "LOGR14",
                (ind, expected) => ((double) ind.Current.Price).Should().BeApproximately(expected, epsilon));
        }

        [Fact]
        [Trait("Quantler.Indicators", "LogReturn")]
        public void LOGRComputesCorrectly()
        {
            int period = 4;
            var logr = new LogReturn(period);
            var data = new[] { 1m, 10m, 100m, 1000m, 10000m, 1234m, 56789m };

            var seen = new List<decimal>();
            for (int i = 0; i < data.Length; i++)
            {
                var datum = data[i];
                var value0 = 0m;

                if (seen.Count >= 0 && seen.Count < period)
                    value0 = data[0];
                else if (seen.Count >= period)
                    value0 = data[i - period];

                var expected = (decimal)Math.Log((double)datum / (double)value0);

                seen.Add(datum);
                logr.Update(new IndicatorDataPoint(DateTime.Now.AddSeconds(i), TimeZone.Utc, datum));
                Assert.Equal(expected, logr.Current.Price);
            }
        }

        [Fact]
        [Trait("Quantler.Indicators", "LogReturn")]
        public void ResetsProperly()
        {
            var logr = new LogReturn(14);

            TestHelper.TestIndicatorReset(logr, "spy_logr14.txt");
        }

        #endregion Public Methods
    }
}