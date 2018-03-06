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
    public class SumTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "Sum")]
        public void ComputesCorrectly()
        {
            var sum = new Sum(2);
            var time = DateTime.UtcNow;

            sum.Update(time.AddDays(1), TimeZone.Utc, 1m);
            sum.Update(time.AddDays(1), TimeZone.Utc, 2m);
            sum.Update(time.AddDays(1), TimeZone.Utc, 3m);

            Assert.Equal(sum.Current.Price, 2m + 3m);
        }

        [Fact]
        [Trait("Quantler.Indicators", "Sum")]
        public void ResetsCorrectly()
        {
            var sum = new Sum(2);
            var time = DateTime.UtcNow;

            sum.Update(time.AddDays(1), TimeZone.Utc, 1m);
            sum.Update(time.AddDays(1), TimeZone.Utc, 2m);
            sum.Update(time.AddDays(1), TimeZone.Utc, 3m);

            Assert.True(sum.IsReady);

            sum.Reset();

            TestHelper.AssertIndicatorIsInDefaultState(sum);
            Assert.Equal(0m, sum.Current.Price);
            sum.Update(time.AddDays(1), TimeZone.Utc, 1);
            Assert.Equal(1m, sum.Current.Price);
        }

        #endregion Public Methods
    }
}