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
using Quantler.Api.Cobinhood;
using System.Threading.Tasks;
using Xunit;

namespace Quantler.Tests.Api
{
    public class CobinhoodApiTests
    {
        #region Private Fields

        /// <summary>
        /// The trading pair id under test
        /// </summary>
        private readonly string _tradingpairid = "COB-USD";

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// System under test
        /// </summary>
        public CobinhoodApi CobinhoodApi => new CobinhoodApi();

        #endregion Public Properties

        #region Public Methods

        [Fact(Skip = "Can fail during CI, gateway timeout? Enable later on")]
        [Trait("Quantler.Api", "Cobinhood")]
        public async Task CobinhoodAPI_GetAllCurrencies()
        {
            //Arrange

            //Act
            var result = await CobinhoodApi.GetAllCurrencies();

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Result.Currencies.Length.Should().BePositive();
        }

        [Fact(Skip = "Can fail during CI, gateway timeout? Enable later on")]
        [Trait("Quantler.Api", "Cobinhood")]
        public async Task CobinhoodAPI_GetAllTradingPairs()
        {
            //Arrange

            //Act
            var result = await CobinhoodApi.GetAllTradingPairs();

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Result.TradingPairs.Length.Should().BePositive();
        }

        [Fact(Skip = "Can fail during CI, gateway timeout? Enable later on")]
        [Trait("Quantler.Api", "Cobinhood")]
        public async Task CobinhoodAPI_GetOrderBook()
        {
            //Arrange

            //Act
            var result = await CobinhoodApi.GetOrderBook(_tradingpairid);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Result.Orderbook.Asks.Length.Should().BePositive();
        }

        [Fact(Skip = "Can fail during CI, gateway timeout? Enable later on")]
        [Trait("Quantler.Api", "Cobinhood")]
        public async Task CobinhoodAPI_GetRecentTrades()
        {
            //Arrange

            //Act
            var result = await CobinhoodApi.GetRecentTrades(_tradingpairid);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Result.Trades.Length.Should().BePositive();
        }

        [Fact(Skip = "Can fail during CI, gateway timeout? Enable later on")]
        [Trait("Quantler.Api", "Cobinhood")]
        public async Task CobinhoodAPI_GetTicker()
        {
            //Arrange

            //Act
            var result = await CobinhoodApi.GetTicker(_tradingpairid);

            //Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Result.Ticker.Should().NotBeNull();
            result.Result.Ticker.HighestBid.Should().NotBeNullOrWhiteSpace();
        }

        #endregion Public Methods
    }
}