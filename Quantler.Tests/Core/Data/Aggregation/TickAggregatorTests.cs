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
using Quantler.Data.Aggegrate;
using Quantler.Data.Bars;
using Quantler.Data.Market;
using Quantler.Securities;
using System;
using Xunit;

namespace Quantler.Tests.Core.Data.Aggregation
{
    public class TickAggregatorTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Core.Data.Aggregation", "TickAggregator")]
        public void AggregatesNewTicksInPeriodWithRoundedTime()
        {
            //Arrange
            TradeBar tradeBar = null;
            var tickersymbol = TickerSymbol.NIL("TST");
            var tickAggregator = new TickAggregator(TimeSpan.FromMinutes(1));
            tickAggregator.DataAggregated += (sender, bar) =>
            {
                tradeBar = bar;
            };

            //Act
            var reference = new DateTime(2015, 06, 02);
            var tick1 = new Tick
            {
                Ticker = tickersymbol,
                Occured = reference.AddSeconds(3),
                Price = 1.1000m
            };

            tickAggregator.Feed(tick1);
            tradeBar.Should().BeNull();

            var tick2 = new Tick
            {
                Ticker = tickersymbol,
                Occured = reference.AddSeconds(10),
                Price = 1.1005m
            };

            tickAggregator.Feed(tick2);
            tradeBar.Should().BeNull();

            var tick3 = new Tick
            {
                Ticker = tickersymbol,
                Occured = reference.AddSeconds(61),
                Price = 1.1010m
            };

            tickAggregator.Feed(tick3);
            tradeBar.Should().NotBeNull();

            tradeBar.Occured.Should().Be(reference);
            tradeBar.Open.Should().Be(tick1.Price);
            tradeBar.Close.Should().Be(tick2.Price);

            var tick4 = new Tick
            {
                Ticker = tickersymbol,
                Occured = reference.AddSeconds(70),
                Price = 1.1015m
            };

            tickAggregator.Feed(tick4);
            tradeBar.Should().NotBeNull();

            var tick5 = new Tick
            {
                Ticker = tickersymbol,
                Occured = reference.AddSeconds(118),
                Price = 1.1020m
            };

            tickAggregator.Feed(tick5);
            tradeBar.Should().NotBeNull();

            var tick6 = new Tick
            {
                Ticker = tickersymbol,
                Occured = reference.AddSeconds(140),
                Price = 1.1025m
            };

            tickAggregator.Feed(tick6);

            //Assert
            tradeBar.Should().NotBeNull();
            tradeBar.Occured.Should().Be(reference.AddSeconds(60));
            tradeBar.Open.Should().Be(tick3.Price);
            tradeBar.Close.Should().Be(tick5.Price);
        }

        [Fact]
        [Trait("Quantler.Core.Data.Aggregation", "TickAggregator")]
        public void AggregatesNewTradeBarsProperly()
        {
            //Arrange
            TradeBar newTradeBar = null;
            var tickAggregator = new TickAggregator(4);
            tickAggregator.DataAggregated += (sender, tradeBar) =>
            {
                newTradeBar = tradeBar;
            };

            //Act
            var reference = DateTime.Today;
            var tickersymbol = TickerSymbol.NIL("TST");
            var bar1 = new Tick
            {
                Ticker = tickersymbol,
                Occured = reference,
                Price = 5,
                Size = 10
            };

            tickAggregator.Feed(bar1);
            newTradeBar.Should().BeNull();

            var bar2 = new Tick
            {
                Ticker = tickersymbol,
                Occured = reference.AddHours(1),
                Price = 10,
                Size = 20
            };

            tickAggregator.Feed(bar2);
            newTradeBar.Should().BeNull();

            var bar3 = new Tick
            {
                Ticker = tickersymbol,
                Occured = reference.AddHours(2),
                Price = 1,
                Size = 10
            };

            tickAggregator.Feed(bar3);
            newTradeBar.Should().BeNull();

            var bar4 = new Tick
            {
                Ticker = tickersymbol,
                Occured = reference.AddHours(3),
                Price = 9,
                Size = 20
            };

            tickAggregator.Feed(bar4);

            //Assert
            newTradeBar.Should().NotBeNull();
            newTradeBar.Ticker.Should().Be(tickersymbol);
            bar1.Occured.Should().Be(newTradeBar.Occured);
            bar1.Price.Should().Be(newTradeBar.Open);
            bar2.Price.Should().Be(newTradeBar.High);
            bar3.Price.Should().Be(newTradeBar.Low);
            bar4.Price.Should().Be(newTradeBar.Close);
            (bar1.Size + bar2.Size + bar3.Size + bar4.Size).Should().Be(newTradeBar.Volume);
        }

        [Fact]
        [Trait("Quantler.Core.Data.Aggregation", "TickAggregator")]
        public void AggregatesPeriodInCountModeWithDailyData()
        {
            //Arrange
            TradeBar tradeBar = null;
            var tickAggregator = new TickAggregator(2);
            tickAggregator.DataAggregated += (sender, bar) =>
            {
                tradeBar = bar;
            };

            //Act
            var reference = new DateTime(2015, 04, 13);
            tickAggregator.Feed(new Tick { Occured = reference });
            tradeBar.Should().BeNull();

            tickAggregator.Feed(new Tick { Occured = reference.AddMilliseconds(1) });
            tradeBar.Should().NotBeNull();

            // sadly the first emit will be off by the data resolution since we 'swallow' a point, so to
            tradeBar.Period.Should().Be(TimeSpan.FromMilliseconds(1));
            tradeBar = null;

            tickAggregator.Feed(new Tick { Occured = reference.AddMilliseconds(2) });
            tradeBar.Should().BeNull();
            tickAggregator.Feed(new Tick { Occured = reference.AddMilliseconds(3) });

            //Assert
            tradeBar.Should().NotBeNull();
            tradeBar.Period.Should().Be(TimeSpan.FromMilliseconds(2));
        }

        [Fact]
        [Trait("Quantler.Core.Data.Aggregation", "TickAggregator")]
        public void AggregatesPeriodInPeriodModeWithDailyData()
        {
            //Arrange
            TradeBar tradeBar = null;
            var tickAggregator = new TickAggregator(TimeSpan.FromDays(1));
            tickAggregator.DataAggregated += (sender, bar) =>
            {
                tradeBar = bar;
            };

            //Act
            var reference = new DateTime(2015, 04, 13);
            tickAggregator.Feed(new Tick { Occured = reference });
            tradeBar.Should().BeNull();

            tickAggregator.Feed(new Tick { Occured = reference.AddDays(1) });
            tradeBar.Should().NotBeNull();
            tradeBar.Period.Should().Be(TimeSpan.FromDays(1));
            tradeBar = null;

            tickAggregator.Feed(new Tick { Occured = reference.AddDays(2) });
            tradeBar.Should().NotBeNull();
            tradeBar.Period.Should().Be(TimeSpan.FromDays(1));
            tradeBar = null;

            tickAggregator.Feed(new Tick { Occured = reference.AddDays(3) });

            //Assert
            tradeBar.Should().NotBeNull();
            tradeBar.Period.Should().Be(TimeSpan.FromDays(1));
        }

        [Fact]
        [Trait("Quantler.Core.Data.Aggregation", "TickAggregator")]
        public void AggregatesPeriodInPeriodModeWithDailyDataAndRoundedTime()
        {
            //Arrange
            TradeBar tradeBar = null;
            var tickAggregator = new TickAggregator(TimeSpan.FromDays(1));
            tickAggregator.DataAggregated += (sender, bar) =>
            {
                tradeBar = bar;
            };

            //Act
            var reference = new DateTime(2015, 04, 13);
            tickAggregator.Feed(new Tick { Occured = reference.AddSeconds(5) });
            tradeBar.Should().BeNull();

            tickAggregator.Feed(new Tick { Occured = reference.AddDays(1).AddSeconds(15) });
            tradeBar.Should().NotBeNull();
            tradeBar.Period.Should().Be(TimeSpan.FromDays(1));
            tradeBar.Occured.Should().Be(reference);
            tradeBar = null;

            tickAggregator.Feed(new Tick { Occured = reference.AddDays(2).AddMinutes(1) });
            tradeBar.Should().NotBeNull();
            tradeBar.Period.Should().Be(TimeSpan.FromDays(1));
            tradeBar.Occured.Should().Be(reference.AddDays(1));
            tradeBar = null;

            tickAggregator.Feed(new Tick { Occured = reference.AddDays(3).AddMinutes(5) });

            //Assert
            tradeBar.Should().NotBeNull();
            tradeBar.Period.Should().Be(TimeSpan.FromDays(1));
            tradeBar.Occured.Should().Be(reference.AddDays(2));
        }

        #endregion Public Methods
    }
}