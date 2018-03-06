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
    public class ConstantIndicatorTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "ConstantIndicator")]
        public void ComputesCorrectly()
        {
            var cons = new ConstantIndicator<IndicatorDataPoint>("c", 1m);
            Assert.Equal(1m, cons.Current.Price);
            Assert.True(cons.IsReady);

            cons.Update(DateTime.Today, TimeZone.Utc, 3m);
            Assert.Equal(1m, cons.Current.Price);
        }

        [Fact]
        [Trait("Quantler.Indicators", "ConstantIndicator")]
        public void ResetsProperly()
        {
            // constant reset should reset samples but the value should still be the same
            var cons = new ConstantIndicator<IndicatorDataPoint>("c", 1m);
            cons.Update(DateTime.Today, TimeZone.Utc, 3m);
            cons.Update(DateTime.Today.AddDays(1), TimeZone.Utc, 10m);

            cons.Reset();
            Assert.Equal(1m, cons.Current.Price);
            Assert.Equal(DateTime.MinValue, cons.Current.Occured);
            Assert.Equal(0, cons.Samples);
        }

        #endregion Public Methods
    }
}