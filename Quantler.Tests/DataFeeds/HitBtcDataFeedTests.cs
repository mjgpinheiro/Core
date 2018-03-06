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
using Quantler.DataFeeds.HitBtcPublic;
using Quantler.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace Quantler.Tests.DataFeeds
{
    public class HitBtcDataFeedTests
    {
        #region Private Fields

        /// <summary>
        /// The output
        /// </summary>
        private readonly ITestOutputHelper _output;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HitBtcDataFeedTests"/> class.
        /// </summary>
        /// <param name="output">The output.</param>
        public HitBtcDataFeedTests(ITestOutputHelper output) =>
            _output = output;

        #endregion Public Constructors

        #region Private Properties

        /// <summary>
        /// Gets the sut.
        /// </summary>
        private HitBtcDataFeed SUT
        {
            get
            {
                var toreturn = new HitBtcDataFeed();
                toreturn.Initialize(null);
                return toreturn;
            }
        }

        #endregion Private Properties

        #region Public Methods

        [Fact]
        [Trait("Quantler.Core.Data.Market", "Tick")]
        public void HitBtcDataFeed_GetQuantlerTicker()
        {
            //Arrange
            string symbol = "UTTETH";

            //Act
            var result = SUT.GetQuantlerTicker(symbol);

            //Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("UTT.BC");
            result.Commodity.Should().Be("UTT");
            result.Currency.Should().Be(CurrencyType.ETH);
        }

        #endregion Public Methods
    }
}