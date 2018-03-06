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
    public class SwissArmyKnifeTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "SwissArmyKnife")]
        public void Compares2PHPAgainstExternalData()
        {
            var indicator = new SwissArmyKnife("", 20, 0.1, SwissArmyKnifeTool.TwoPoleHighPass);
            RunTestIndicator(indicator, "2PHP", 0.01m);
        }

        [Fact]
        [Trait("Quantler.Indicators", "SwissArmyKnife")]
        public void ComparesBandPassAgainstExternalData()
        {
            var indicator = new SwissArmyKnife("", 20, 0.1, SwissArmyKnifeTool.BandPass);
            RunTestIndicator(indicator, "BP", 0.043m);
        }

        [Fact]
        [Trait("Quantler.Indicators", "SwissArmyKnife")]
        public void ComparesButterAgainstExternalData()
        {
            var indicator = new SwissArmyKnife("", 20, 0.1, SwissArmyKnifeTool.Butter);
            RunTestIndicator(indicator, "Butter", 0.01m);
        }

        [Fact]
        [Trait("Quantler.Indicators", "SwissArmyKnife")]
        public void ComparesGaussAgainstExternalData()
        {
            var indicator = new SwissArmyKnife("", 20, 0.1, SwissArmyKnifeTool.Gauss);
            RunTestIndicator(indicator, "Gauss", 0.01m);
        }

        [Fact]
        [Trait("Quantler.Indicators", "SwissArmyKnife")]
        public void ComparesHPAgainstExternalData()
        {
            var indicator = new SwissArmyKnife("", 20, 0.1, SwissArmyKnifeTool.HighPass);
            RunTestIndicator(indicator, "HP", 0.01m);
        }

        [Fact]
        [Trait("Quantler.Indicators", "SwissArmyKnife")]
        public void ResetsProperly()
        {
            var sak = new SwissArmyKnife(4, 0.1, SwissArmyKnifeTool.BandPass);

            foreach (var data in TestHelper.GetDataStream(5))
            {
                sak.Update(data);
            }
            Assert.True(sak.IsReady);
            Assert.NotEqual(0m, sak.Current.Price);
            Assert.NotEqual(0, sak.Samples);

            sak.Reset();

            TestHelper.AssertIndicatorIsInDefaultState(sak);
        }

        #endregion Public Methods

        #region Private Methods

        private static void AssertResult(double expected, decimal actual, decimal variance)
        {
            System.Diagnostics.Debug.WriteLine(expected + "," + actual + "," + Math.Abs((decimal)expected - actual));
            Assert.True(Math.Abs((decimal)expected - actual) < variance);
        }

        private static void RunTestIndicator(IndicatorBase<IndicatorDataPoint> indicator, string field, decimal variance)
        {
            TestHelper.TestIndicator(indicator, "spy_swiss.txt", field, (actual, expected) => { AssertResult(expected, actual.Current.Price, variance); });
        }

        #endregion Private Methods
    }
}