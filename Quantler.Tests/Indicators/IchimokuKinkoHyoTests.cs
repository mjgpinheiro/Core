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
using Xunit;

namespace Quantler.Tests.Indicators
{
    public class IchimokuKinkoHyoTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "IchimokuKinkoHyo")]
        public void ComparesWithExternalDataDelayedKijunSenkouA()
        {
            var ichimoku = new IchimokuKinkoHyo("Ichimoku", 9, 26, 26, 52, 26, 26);
            TestHelper.TestIndicator(
                ichimoku,
                "spy_with_ichimoku.csv",
                "DelayedKijunSenkouA",
                (ind, expected) => Assert.Equal(expected, (double)((IchimokuKinkoHyo)ind).DelayedKijunSenkouA.Current.Price)
                );
        }

        [Fact]
        [Trait("Quantler.Indicators", "IchimokuKinkoHyo")]
        public void ComparesWithExternalDataDelayedMaximumSenkouB()
        {
            var ichimoku = new IchimokuKinkoHyo("Ichimoku", 9, 26, 26, 52, 26, 26);
            TestHelper.TestIndicator(
                ichimoku,
                "spy_with_ichimoku.csv",
                "DelayedMaximumSenkouB",
                (ind, expected) => Assert.Equal(expected, (double)((IchimokuKinkoHyo)ind).DelayedMaximumSenkouB.Current.Price)
                );
        }

        [Fact]
        [Trait("Quantler.Indicators", "IchimokuKinkoHyo")]
        public void ComparesWithExternalDataDelayedMinimumSenkouB()
        {
            var ichimoku = new IchimokuKinkoHyo("Ichimoku", 9, 26, 26, 52, 26, 26);
            TestHelper.TestIndicator(
                ichimoku,
                "spy_with_ichimoku.csv",
                "DelayedMinimumSenkouB",
                (ind, expected) => Assert.Equal(expected, (double)((IchimokuKinkoHyo)ind).DelayedMinimumSenkouB.Current.Price)
                );
        }

        [Fact]
        [Trait("Quantler.Indicators", "IchimokuKinkoHyo")]
        public void ComparesWithExternalDataDelayedTenkanSenkouA()
        {
            var ichimoku = new IchimokuKinkoHyo("Ichimoku", 9, 26, 26, 52, 26, 26);
            TestHelper.TestIndicator(
                ichimoku,
                "spy_with_ichimoku.csv",
                "DelayedTenkanSenkouA",
                (ind, expected) => Assert.Equal(expected, (double)((IchimokuKinkoHyo)ind).DelayedTenkanSenkouA.Current.Price)
                );
        }

        [Fact]
        [Trait("Quantler.Indicators", "IchimokuKinkoHyo")]
        public void ComparesWithExternalDataKijun()
        {
            var ichimoku = new IchimokuKinkoHyo("Ichimoku", 9, 26, 26, 52, 26, 26);
            ichimoku.Current.Occured.ToString();
            TestHelper.TestIndicator(
                ichimoku,
                "spy_with_ichimoku.csv",
                "Kijun",
                (ind, expected) => Assert.Equal(expected, (double)((IchimokuKinkoHyo)ind).Kijun.Current.Price)
                );
        }

        [Fact]
        [Trait("Quantler.Indicators", "IchimokuKinkoHyo")]
        public void ComparesWithExternalDataKijunMaximum()
        {
            var ichimoku = new IchimokuKinkoHyo("Ichimoku", 9, 26, 26, 52, 26, 26);
            TestHelper.TestIndicator(
                ichimoku,
                "spy_with_ichimoku.csv",
                "KijunMaximum",
                (ind, expected) => Assert.Equal(expected, (double)((IchimokuKinkoHyo)ind).KijunMaximum.Current.Price)
                );
        }

        [Fact]
        [Trait("Quantler.Indicators", "IchimokuKinkoHyo")]
        public void ComparesWithExternalDataKijunMinimum()
        {
            var ichimoku = new IchimokuKinkoHyo("Ichimoku", 9, 26, 26, 52, 26, 26);
            TestHelper.TestIndicator(
                ichimoku,
                "spy_with_ichimoku.csv",
                "KijunMinimum",
                (ind, expected) => Assert.Equal(expected, (double)((IchimokuKinkoHyo)ind).KijunMinimum.Current.Price)
                );
        }

        [Fact]
        [Trait("Quantler.Indicators", "IchimokuKinkoHyo")]
        public void ComparesWithExternalDataSenkouA()
        {
            var ichimoku = new IchimokuKinkoHyo("Ichimoku", 9, 26, 26, 52, 26, 26);
            TestHelper.TestIndicator(
                ichimoku,
                "spy_with_ichimoku.csv",
                "Senkou A",
                (ind, expected) => Assert.Equal(expected, (double)((IchimokuKinkoHyo)ind).SenkouA.Current.Price)
                );
        }

        [Fact]
        [Trait("Quantler.Indicators", "IchimokuKinkoHyo")]
        public void ComparesWithExternalDataSenkouBMaximum()
        {
            var ichimoku = new IchimokuKinkoHyo("Ichimoku", 9, 26, 26, 52, 26, 26);
            TestHelper.TestIndicator(
                ichimoku,
                "spy_with_ichimoku.csv",
                "SenkouBMaximum",
                (ind, expected) => Assert.Equal(expected, (double)((IchimokuKinkoHyo)ind).SenkouBMaximum.Current.Price)
                );
        }

        [Fact]
        [Trait("Quantler.Indicators", "IchimokuKinkoHyo")]
        public void ComparesWithExternalDataSenkouBMinimum()
        {
            var ichimoku = new IchimokuKinkoHyo("Ichimoku", 9, 26, 26, 52, 26, 26);
            TestHelper.TestIndicator(
                ichimoku,
                "spy_with_ichimoku.csv",
                "SenkouBMinimum",
                (ind, expected) => Assert.Equal(expected, (double)((IchimokuKinkoHyo)ind).SenkouBMinimum.Current.Price)
                );
        }

        [Fact]
        [Trait("Quantler.Indicators", "IchimokuKinkoHyo")]
        public void ComparesWithExternalDataTenkan()
        {
            var ichimoku = new IchimokuKinkoHyo("Ichimoku", 9, 26, 26, 52, 26, 26);
            TestHelper.TestIndicator(
                ichimoku,
                "spy_with_ichimoku.csv",
                "Tenkan",
                (ind, expected) => Assert.Equal(expected, (double)((IchimokuKinkoHyo)ind).Tenkan.Current.Price)
                );
        }

        [Fact]
        [Trait("Quantler.Indicators", "IchimokuKinkoHyo")]
        public void ComparesWithExternalDataTenkanMaximum()
        {
            var ichimoku = new IchimokuKinkoHyo("Ichimoku", 9, 26, 26, 52, 26, 26);
            TestHelper.TestIndicator(
                ichimoku,
                "spy_with_ichimoku.csv",
                "TenkanMaximum",
                (ind, expected) => Assert.Equal(expected, (double)((IchimokuKinkoHyo)ind).TenkanMaximum.Current.Price)
                );
        }

        [Fact]
        [Trait("Quantler.Indicators", "IchimokuKinkoHyo")]
        public void ComparesWithExternalDataTenkanMinimum()
        {
            var ichimoku = new IchimokuKinkoHyo("Ichimoku", 9, 26, 26, 52, 26, 26);
            TestHelper.TestIndicator(
                ichimoku,
                "spy_with_ichimoku.csv",
                "TenkanMinimum",
                (ind, expected) => Assert.Equal(expected, (double)((IchimokuKinkoHyo)ind).TenkanMinimum.Current.Price)
                );
        }

        [Fact]
        [Trait("Quantler.Indicators", "IchimokuKinkoHyo")]
        public void ResetsProperly()
        {
            var ichimoku = new IchimokuKinkoHyo("Ichimoku", 9, 26, 26, 52, 26, 26);

            TestHelper.TestIndicatorReset(ichimoku, "spy_with_ichimoku.csv");
        }

        #endregion Public Methods
    }
}