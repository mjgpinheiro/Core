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
using Quantler.Securities;
using System;
using Xunit;

namespace Quantler.Tests.Core.Data.Aggregation
{
    public class QuoteBarAggregatorTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Core.Data.Aggregation", "QuoteBarAggregator")]
        public void AggregatesNewQuoteBarProperly()
        {
            //Arrange
            QuoteBar quoteBar = null;
            var quoteBarAggregator = new QuoteBarAggregator(4);
            quoteBarAggregator.DataAggregated += (sender, args) =>
            {
                quoteBar = args;
            };

            //Act
            var time = DateTime.Today;
            var tickersymbol = TickerSymbol.NIL("TST");
            var bar1 = new QuoteBar
            {
                Occured = time,
                Ticker = tickersymbol,
                Bid = new BarImpl(1, 2, 0.75m, 1.25m),
                LastBidSize = 3,
                Ask = null,
                LastAskSize = 0
            };

            quoteBarAggregator.Feed(bar1);
            quoteBar.Should().BeNull();

            var bar2 = new QuoteBar
            {
                Occured = time,
                Ticker = tickersymbol,
                Bid = new BarImpl(1.1m, 2.2m, 0.9m, 2.1m),
                LastBidSize = 3,
                Ask = new BarImpl(2.2m, 4.4m, 3.3m, 3.3m),
                LastAskSize = 0
            };

            quoteBarAggregator.Feed(bar2);
            quoteBar.Should().BeNull();

            var bar3 = new QuoteBar
            {
                Occured = time,
                Ticker = tickersymbol,
                Bid = new BarImpl(1, 2, 0.5m, 1.75m),
                LastBidSize = 3,
                Ask = null,
                LastAskSize = 0
            };

            quoteBarAggregator.Feed(bar3);
            quoteBar.Should().BeNull();

            var bar4 = new QuoteBar
            {
                Occured = time,
                Ticker = tickersymbol,
                Bid = null,
                LastBidSize = 0,
                Ask = new BarImpl(1, 7, 0.5m, 4.4m),
                LastAskSize = 4,
            };

            //Assert
            quoteBarAggregator.Feed(bar4);
            quoteBar.Should().NotBeNull();
            bar1.Ticker.Should().Be(quoteBar.Ticker);
            bar1.Bid.Open.Should().Be(quoteBar.Bid.Open);
            bar2.Ask.Open.Should().Be(quoteBar.Ask.Open);
            bar2.Bid.High.Should().Be(quoteBar.Bid.High);
            bar4.Ask.High.Should().Be(quoteBar.Ask.High);
            bar3.Bid.Low.Should().Be(quoteBar.Bid.Low);
            bar4.Ask.Low.Should().Be(quoteBar.Ask.Low);
            bar3.Bid.Close.Should().Be(quoteBar.Bid.Close);
            bar4.Ask.Close.Should().Be(quoteBar.Ask.Close);
            bar3.LastBidSize.Should().Be(quoteBar.LastBidSize);
            bar4.LastAskSize.Should().Be(quoteBar.LastAskSize);
            bar1.Price.Should().Be(quoteBar.Price);
        }

        #endregion Public Methods
    }
}