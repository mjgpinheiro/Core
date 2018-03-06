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
using Xunit;
using Xunit.Abstractions;

namespace Quantler.Tests.Core.Data.Market
{
    public class TradingStatusTests
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
        public TradingStatusTests(ITestOutputHelper output) =>
            _output = output;

        #endregion Public Constructors

        #region Private Properties

        /// <summary>
        /// System under test
        /// </summary>
        private TradingStatus Sut => new TradingStatus
        {
            DataType = DataType.TradingStatus,
            IsBackfilling = false,
            Occured = DateTime.UtcNow,
            Reason = "Testing",
            Status = "H",
            Ticker = TickerSymbol.NIL("TST"),
            TimeZone = TimeZone.Auckland
        };

        #endregion Private Properties

        #region Public Methods

        [Fact]
        [Trait("Quantler.Core.Data.Market", "TradingStatus")]
        public void Tick_SerializeDeserialize()
        {
            //Act
            var serialized = Sut.Serialize();
            var deserialized = DataPointImpl.Deserialize(serialized, true) as TradingStatus;

            //Assert
            serialized.Length.Should().BePositive();
            deserialized.Should().NotBeNull();
            deserialized.OccuredUtc.Should().NotBe(Sut.Occured);
            deserialized.Reason.Should().Be(Sut.Reason);
            deserialized.Status.Should().Be(Sut.Status);
        }

        [Fact]
        [Trait("Quantler.Core.Data.Market", "TradingStatus")]
        public void Tick_SerializeJson()
        {
            //Act
            var result = Sut.SerializeJson();
            _output.WriteLine(result);

            //Assert
            result.Should().NotBeNullOrWhiteSpace();
        }

        #endregion Public Methods
    }
}