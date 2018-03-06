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
    public class RelativeStrengthIndexTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "RelativeStrengthIndex")]
        public void ComparesAgainstExternalData()
        {
            var rsi = new RelativeStrengthIndex("rsi", 14, MovingAverageType.Simple);
            TestHelper.TestIndicator(rsi, "RSI 14");
        }

        [Fact]
        [Trait("Quantler.Indicators", "RelativeStrengthIndex")]
        public void ResetsProperly()
        {
            var rsi = new RelativeStrengthIndex(2);
            rsi.Update(DateTime.Today, TimeZone.Utc, 1m);
            rsi.Update(DateTime.Today.AddSeconds(1), TimeZone.Utc, 2m);
            Assert.False(rsi.IsReady);

            rsi.Reset();
            TestHelper.AssertIndicatorIsInDefaultState(rsi);
            TestHelper.AssertIndicatorIsInDefaultState(rsi.AverageGain);
            TestHelper.AssertIndicatorIsInDefaultState(rsi.AverageLoss);
        }

        #endregion Public Methods
    }
}