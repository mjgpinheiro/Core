#region License Header

/*
* QUANTLER.COM - Quant Fund Development Platform
* Quantler Core Trading Engine. Copyright 2018 Quantler B.V.
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
*/

#endregion License Header

using FluentAssertions;
using Quantler.Data;
using Quantler.Data.Market;
using Quantler.Securities;
using System;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace Quantler.Tests.Core.Data.Market
{
    public class TickTests
    {
        #region Private Fields

        /// <summary>
        /// The output
        /// </summary>
        private readonly ITestOutputHelper _output;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TickTests"/> class.
        /// </summary>
        /// <param name="output">The output.</param>
        public TickTests(ITestOutputHelper output) =>
            _output = output;

        #endregion Public Constructors

        #region Public Methods

        [Fact]
        [Trait("Quantler.Core.Data.Market", "Tick")]
        public void Tick_CreateCorrectly()
        {
            //Act
            var sut = new Tick(TickerSymbol.NIL("Test"), DataSource.IEX);

            //Assert
            sut.AskSource.Should().Be(DataSource.IEX);
            sut.BidSource.Should().Be(DataSource.IEX);
            sut.Source.Should().Be(DataSource.IEX);
            sut.HasAsk.Should().BeFalse();
            sut.HasBid.Should().BeFalse();
            sut.HasTick.Should().BeFalse();
            sut.IsValid.Should().BeFalse();
            sut.IsFullQuote.Should().BeFalse();
            sut.IsIndex.Should().BeFalse();
            sut.IsQuote.Should().BeFalse();
            sut.IsTrade.Should().BeFalse();
            sut.IsBackfilling.Should().BeFalse();
        }

        [Fact]
        [Trait("Quantler.Core.Data.Market", "Tick")]
        public void Tick_CreateCorrectly_FullQuote()
        {
            //Act
            var sut = new Tick(TickerSymbol.NIL("Test"), DataSource.IEX)
            {
                AskPrice = 10,
                AskSize = 11,
                BidSize = 12,
                BidPrice = 13
            };

            //Assert
            sut.IsQuote.Should().BeTrue();
            sut.IsFullQuote.Should().BeTrue();
            sut.IsIndex.Should().BeFalse();
            sut.IsTrade.Should().BeFalse();
            sut.HasTick.Should().BeTrue();
        }

        [Fact]
        [Trait("Quantler.Core.Data.Market", "Tick")]
        public void Tick_CreateCorrectly_Index()
        {
            //Act
            var sut = new Tick(TickerSymbol.NIL("Test"), DataSource.IEX)
            {
                TradePrice = 12,
                Size = -10
            };

            //Assert
            sut.IsQuote.Should().BeFalse();
            sut.IsFullQuote.Should().BeFalse();
            sut.IsIndex.Should().BeTrue();
            sut.IsTrade.Should().BeFalse();
            sut.HasTick.Should().BeFalse();
        }

        [Fact]
        [Trait("Quantler.Core.Data.Market", "Tick")]
        public void Tick_CreateCorrectly_Quote()
        {
            //Act
            var sut = new Tick(TickerSymbol.NIL("Test"), DataSource.IEX)
            {
                AskPrice = 10,
                AskSize = 11
            };

            //Assert
            sut.IsQuote.Should().BeTrue();
            sut.IsFullQuote.Should().BeFalse();
            sut.IsIndex.Should().BeFalse();
            sut.IsTrade.Should().BeFalse();
            sut.HasTick.Should().BeTrue();
        }

        [Fact]
        [Trait("Quantler.Core.Data.Market", "Tick")]
        public void Tick_CreateCorrectly_Trade()
        {
            //Act
            var sut = new Tick(TickerSymbol.NIL("Test"), DataSource.IEX)
            {
                TradePrice = 12,
                Size = 13
            };

            //Assert
            sut.IsQuote.Should().BeFalse();
            sut.IsFullQuote.Should().BeFalse();
            sut.IsIndex.Should().BeFalse();
            sut.IsTrade.Should().BeTrue();
            sut.HasTick.Should().BeTrue();
        }

        [Fact]
        [Trait("Quantler.Core.Data.Market", "Tick")]
        public void Tick_SerializeDeserialize()
        {
            //Arrange
            DateTime occured = DateTime.UtcNow;
            var sut = new Tick(TickerSymbol.NIL("Test"), DataSource.IEX)
            {
                AskPrice = 10,
                AskSize = 11,
                BidSize = 12,
                BidPrice = 13,
                Occured = occured
            };

            //Act
            Stopwatch sw = new Stopwatch();
            sw.Start();
            byte[] serialized = null;
            int iterations = 10000;
            for (int i = 0; i < iterations; i++)
            {
                serialized = sut.Serialize();
            }
            sw.Stop();
            _output.WriteLine($"Speed: {sw.Elapsed / iterations}");
            (sw.Elapsed / iterations).Should().BeLessThan(TimeSpan.FromMilliseconds(1));
            var deserialized = DataPointImpl.Deserialize(serialized, true) as Tick;

            //Assert
            serialized.Length.Should().BePositive();
            deserialized.Should().NotBeNull();
            deserialized.AskPrice.Should().Be(sut.AskPrice);
            deserialized.BidPrice.Should().Be(sut.BidPrice);
            deserialized.Occured.Should().Be(occured);
            deserialized.OccuredUtc.Should().BeAfter(occured.AddDays(-1));
        }

        [Fact]
        [Trait("Quantler.Core.Data.Market", "Tick")]
        public void Tick_SerializeJson()
        {
            //Arrange
            var sut = new Tick(TickerSymbol.NIL("Test"), DataSource.IEX)
            {
                AskPrice = 10,
                AskSize = 11,
                BidSize = 12,
                BidPrice = 13,
                Occured = DateTime.UtcNow
            };

            //Act
            var result = sut.SerializeJson();
            _output.WriteLine(result);

            //Assert
            result.Should().NotBeNullOrWhiteSpace();
        }

        #endregion Public Methods
    }
}