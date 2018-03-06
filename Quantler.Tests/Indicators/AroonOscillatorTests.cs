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
using FluentAssertions;
using Xunit;

namespace Quantler.Tests.Indicators
{
    public class AroonOscillatorTests
    {
        #region Public Methods

        [Fact]
        public void ComparesWithExternalData()
        {
            var aroon = new AroonOscillator(14, 14);
            TestHelper.TestIndicator(aroon, "spy_aroon_oscillator.txt", "Aroon Oscillator 14",
                (i, expected) => ((double) aroon.Current.Price).Should().BeApproximately(expected, 1e-3));
        }

        [Fact]
        public void ResetsProperly()
        {
            var aroon = new AroonOscillator(3, 3);
            aroon.Update(new TradeBar
            {
                Ticker = Props.TickerSymbol,
                Occured = DateTime.Today,
                Open = 3m,
                High = 7m,
                Low = 2m,
                Close = 5m,
                Volume = 10,
                TimeZone = TimeZone.Utc
            });
            aroon.Update(new TradeBar
            {
                Ticker = Props.TickerSymbol,
                Occured = DateTime.Today.AddSeconds(1),
                Open = 3m,
                High = 7m,
                Low = 2m,
                Close = 5m,
                Volume = 10,
                TimeZone = TimeZone.Utc
            });
            aroon.Update(new TradeBar
            {
                Ticker = Props.TickerSymbol,
                Occured = DateTime.Today.AddSeconds(2),
                Open = 3m,
                High = 7m,
                Low = 2m,
                Close = 5m,
                Volume = 10,
                TimeZone = TimeZone.Utc
            });
            Assert.False(aroon.IsReady);
            aroon.Update(new TradeBar
            {
                Ticker = Props.TickerSymbol,
                Occured = DateTime.Today.AddSeconds(3),
                Open = 3m,
                High = 7m,
                Low = 2m,
                Close = 5m,
                Volume = 10,
                TimeZone = TimeZone.Utc
            });
            Assert.True(aroon.IsReady);

            aroon.Reset();
            TestHelper.AssertIndicatorIsInDefaultState(aroon);
            TestHelper.AssertIndicatorIsInDefaultState(aroon.AroonUp);
            TestHelper.AssertIndicatorIsInDefaultState(aroon.AroonDown);
        }

        #endregion Public Methods
    }
}