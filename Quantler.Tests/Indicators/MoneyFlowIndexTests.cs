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

using Quantler.Data.Bars;
using Quantler.Indicators;
using System;
using Xunit;

namespace Quantler.Tests.Indicators
{
    public class MoneyFlowIndexTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "MoneyFlowIndex")]
        public void ComparesAgainstExternalData()
        {
            var mfi = new MoneyFlowIndex(20);
            TestHelper.TestIndicator(mfi, "spy_mfi.txt", "Money Flow Index 20");
        }

        [Fact]
        [Trait("Quantler.Indicators", "MoneyFlowIndex")]
        public void ResetsProperly()
        {
            var mfi = new MoneyFlowIndex(3);
            foreach (var data in TestHelper.GetDataStream(4))
            {
                var tradeBar = new TradeBar
                {
                    Open = data.Price,
                    Close = data.Price,
                    High = data.Price,
                    Low = data.Price,
                    Volume = Decimal.ToInt64(data.Price),
                    TimeZone = TimeZone.Utc
                };
                mfi.Update(tradeBar);
            }
            Assert.True(mfi.IsReady);
            Assert.True(mfi.PositiveMoneyFlow.IsReady);
            Assert.True(mfi.NegativeMoneyFlow.IsReady);
            Assert.NotEqual(0.0m, mfi.PreviousTypicalPrice);

            mfi.Reset();

            Assert.Equal(0.0m, mfi.PreviousTypicalPrice);
            TestHelper.AssertIndicatorIsInDefaultState(mfi);
            TestHelper.AssertIndicatorIsInDefaultState(mfi.PositiveMoneyFlow);
            TestHelper.AssertIndicatorIsInDefaultState(mfi.NegativeMoneyFlow);
        }

        [Fact]
        [Trait("Quantler.Indicators", "MoneyFlowIndex")]
        public void TestTradeBarsWithNoVolume()
        {
            var mfi = new MoneyFlowIndex(3);
            foreach (var data in TestHelper.GetDataStream(4))
            {
                var tradeBar = new TradeBar
                {
                    Open = data.Price,
                    Close = data.Price,
                    High = data.Price,
                    Low = data.Price,
                    TimeZone = TimeZone.Utc,
                    Volume = 0
                };
                mfi.Update(tradeBar);
            }

            Assert.Equal(100.0m, mfi.Current.Price);
        }

        #endregion Public Methods
    }
}