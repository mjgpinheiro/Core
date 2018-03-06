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
    public class BollingerBandsTests
    {
        #region Public Methods

        [Fact(Skip = "Fix Me")]
        [Trait("Quantler.Indicators", "BollingerBands")]
        public void ComparesWithExternalDataLowerBand()
        {
            var bb = new BollingerBands(20, 2.0m, MovingAverageType.Simple);
            TestHelper.TestIndicator(bb, "spy_bollinger_bands.txt", "Bollinger Bands® 20 2 Bottom", (BollingerBands ind) => (double)ind.LowerBand.Current.Price);
        }

        [Fact(Skip = "Fix Me")]
        [Trait("Quantler.Indicators", "BollingerBands")]
        public void ComparesWithExternalDataMiddleBand()
        {
            var bb = new BollingerBands(20, 2.0m, MovingAverageType.Simple);
            TestHelper.TestIndicator(bb, "spy_bollinger_bands.txt", "Moving Average 20", (BollingerBands ind) => (double)ind.MiddleBand.Current.Price);
        }

        [Fact(Skip = "Fix Me")]
        [Trait("Quantler.Indicators", "BollingerBands")]
        public void ComparesWithExternalDataUpperBand()
        {
            var bb = new BollingerBands(20, 2.0m, MovingAverageType.Simple);
            TestHelper.TestIndicator(bb, "spy_bollinger_bands.txt", "Bollinger Bands® 20 2 Top", (BollingerBands ind) => (double)ind.UpperBand.Current.Price);
        }

        [Fact]
        [Trait("Quantler.Indicators", "BollingerBands")]
        public void ResetsProperly()
        {
            var bb = new BollingerBands(2, 2m);
            bb.Update(DateTime.Today, TimeZone.Utc, 1m);

            Assert.False(bb.IsReady);
            bb.Update(DateTime.Today.AddSeconds(1), TimeZone.Utc, 2m);
            Assert.True(bb.IsReady);
            Assert.True(bb.StandardDeviation.IsReady);
            Assert.True(bb.LowerBand.IsReady);
            Assert.True(bb.MiddleBand.IsReady);
            Assert.True(bb.UpperBand.IsReady);

            bb.Reset();
            TestHelper.AssertIndicatorIsInDefaultState(bb);
            TestHelper.AssertIndicatorIsInDefaultState(bb.StandardDeviation);
            TestHelper.AssertIndicatorIsInDefaultState(bb.LowerBand);
            TestHelper.AssertIndicatorIsInDefaultState(bb.MiddleBand);
            TestHelper.AssertIndicatorIsInDefaultState(bb.UpperBand);
        }

        #endregion Public Methods
    }
}