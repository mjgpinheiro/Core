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
using System;
using Xunit;

namespace Quantler.Tests.Indicators
{
    public class StochasticTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "Stochastic")]
        public void ComparesAgainstExternalDataOnStochasticsD()
        {
            var stochastics = new Stochastic("sto", 12, 3, 5);

            const double epsilon = 1e-3;
            TestHelper.TestIndicator(stochastics, "spy_with_stoch12k3.txt", "%D 5",
                (ind, expected) => ((double)((Stochastic)ind).StochD.Current.Price).Should()
                    .BeApproximately(expected, epsilon));
        }

        [Fact]
        [Trait("Quantler.Indicators", "Stochastic")]
        public void ComparesAgainstExternalDataOnStochasticsK()
        {
            var stochastics = new Stochastic("sto", 12, 3, 5);

            const double epsilon = 1e-3;

            TestHelper.TestIndicator(stochastics, "spy_with_stoch12k3.txt", "Stochastics 12 %K 3",
                (ind, expected) => ((double)((Stochastic)ind).StochK.Current.Price).Should()
                    .BeApproximately(expected, epsilon));
        }

        [Fact]
        [Trait("Quantler.Indicators", "Stochastic")]
        public void HandlesEqualMinAndMax()
        {
            var reference = new DateTime(2015, 09, 01);
            var stochastics = new Stochastic("sto", 2, 2, 2);
            for (int i = 0; i < 4; i++)
            {
                var bar = new TradeBar { Occured = reference.AddSeconds(i), TimeZone = TimeZone.Utc };
                bar.Open = bar.Close = bar.High = bar.Low = bar.Volume = 1;
                stochastics.Update(bar);
                Assert.Equal(0m, stochastics.Current.Price);
            }
        }

        [Fact]
        [Trait("Quantler.Indicators", "Stochastic")]
        public void PrimaryOutputIsFastStochProperty()
        {
            var stochastics = new Stochastic("sto", 12, 3, 5);

            TestHelper.TestIndicator(stochastics, "spy_with_stoch12k3.txt", "Stochastics 12 %K 3",
                (ind, expected) => Assert.Equal(((Stochastic)ind).FastStoch.Current.Price, ind.Current.Price));
        }

        [Fact]
        [Trait("Quantler.Indicators", "Stochastic")]
        public void ResetsProperly()
        {
            var stochastics = new Stochastic("sto", 12, 3, 5);

            foreach (var bar in TestHelper.GetTradeBarStream("spy_with_stoch12k3.txt", false))
            {
                stochastics.Update(bar);
            }
            Assert.True(stochastics.IsReady);
            Assert.True(stochastics.FastStoch.IsReady);
            Assert.True(stochastics.StochK.IsReady);
            Assert.True(stochastics.StochD.IsReady);

            stochastics.Reset();

            TestHelper.AssertIndicatorIsInDefaultState(stochastics);
            TestHelper.AssertIndicatorIsInDefaultState(stochastics.FastStoch);
            TestHelper.AssertIndicatorIsInDefaultState(stochastics.StochK);
            TestHelper.AssertIndicatorIsInDefaultState(stochastics.StochD);
        }

        #endregion Public Methods
    }
}