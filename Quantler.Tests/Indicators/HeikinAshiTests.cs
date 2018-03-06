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

using FluentAssertions;
using Quantler.Data.Bars;
using Quantler.Indicators;
using Xunit;

namespace Quantler.Tests.Indicators
{
    public class HeikinAshiTests : CommonIndicatorTests<TradeBar>
    {
        #region Protected Properties

        protected override string TestColumnName => "";

        protected override string TestFileName => "spy_heikin_ashi.txt";

        #endregion Protected Properties

        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "HeikinAshi")]
        public override void ComparesAgainstExternalData()
        {
            TestHelper.TestIndicator(new HeikinAshi(), TestFileName, "HA_Open",
                (ind, expected) => ((double) ((HeikinAshi) ind).Open.Current.Price).Should()
                    .BeApproximately(expected, 1e-3));
            TestHelper.TestIndicator(new HeikinAshi(), TestFileName, "HA_High",
                (ind, expected) => ((double) ((HeikinAshi) ind).High.Current.Price).Should()
                    .BeApproximately(expected, 1e-3));
            TestHelper.TestIndicator(new HeikinAshi(), TestFileName, "HA_Low",
                (ind, expected) =>
                    ((double) ((HeikinAshi) ind).Low.Current.Price).Should().BeApproximately(expected, 1e-3));
            TestHelper.TestIndicator(new HeikinAshi(), TestFileName, "HA_Close",
                (ind, expected) => ((double) ((HeikinAshi) ind).Close.Current.Price).Should()
                    .BeApproximately(expected, 1e-3));
            TestHelper.TestIndicator(new HeikinAshi(), TestFileName, "Volume",
                (ind, expected) => ((double) ((HeikinAshi) ind).Volume.Current.Price).Should()
                    .BeApproximately(expected, 1e-3));
        }

        [Fact]
        [Trait("Quantler.Indicators", "HeikinAshi")]
        public override void ComparesAgainstExternalDataAfterReset()
        {
            var indicator = new HeikinAshi();
            for (var i = 1; i <= 2; i++)
            {
                TestHelper.TestIndicator(indicator, TestFileName, "HA_Open",
                    (ind, expected) => ((double) ((HeikinAshi) ind).Open.Current.Price).Should()
                        .BeApproximately(expected, 1e-3));
                indicator.Reset();
                TestHelper.TestIndicator(indicator, TestFileName, "HA_High",
                    (ind, expected) => ((double) ((HeikinAshi) ind).High.Current.Price).Should()
                        .BeApproximately(expected, 1e-3));
                indicator.Reset();
                TestHelper.TestIndicator(indicator, TestFileName, "HA_Low",
                    (ind, expected) => ((double) ((HeikinAshi) ind).Low.Current.Price).Should()
                        .BeApproximately(expected, 1e-3));
                indicator.Reset();
                TestHelper.TestIndicator(indicator, TestFileName, "HA_Close",
                    (ind, expected) => ((double) ((HeikinAshi) ind).Close.Current.Price).Should()
                        .BeApproximately(expected, 1e-3));
                indicator.Reset();
                TestHelper.TestIndicator(indicator, TestFileName, "Volume",
                    (ind, expected) => ((double) ((HeikinAshi) ind).Volume.Current.Price).Should()
                        .BeApproximately(expected, 1e-3));
                indicator.Reset();
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected override IndicatorBase<TradeBar> CreateIndicator()
        {
            return new HeikinAshi();
        }

        #endregion Protected Methods
    }
}