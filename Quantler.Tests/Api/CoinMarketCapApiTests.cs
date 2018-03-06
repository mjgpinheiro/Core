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
using Quantler.Api.CoinMarketCap;
using Quantler.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Quantler.Tests.Api
{
    /// <summary>
    /// Coinmarket cap general api tests
    /// </summary>
    public class CoinMarketCapApiTests
    {
        #region Public Properties

        /// <summary>
        /// System under test
        /// </summary>
        public CoinmarketcapApi CoinmarketcapApi => new CoinmarketcapApi();

        #endregion Public Properties

        #region Public Methods

        [Fact]
        [Trait("Quantler.Api", "CoinMarketCap")]
        //Get ticker from coinmarket cap
        public async Task CoinmarketcapApi_GetTicker()
        {
            //Arrange
            string ticker = "iconomi";

            //Act
            var result = await CoinmarketcapApi.GetTickerAsync(ticker, CurrencyType.USD);

            //Assert
            result.Count.Should().BeGreaterThan(0);
            result["id"].Should().Be(ticker);
            result["symbol"].Should().NotBeNullOrWhiteSpace();
            int.Parse(result["rank"]).Should().NotBe(1);
        }

        [Fact]
        [Trait("Quantler.Api", "CoinMarketCap")]
        //Get ticker from coinmarket cap
        public async Task CoinmarketcapApi_GetTickers()
        {
            //Arrange

            //Act
            var result = await CoinmarketcapApi.GetTickersAsync();

            //Assert
            result.Count().Should().BeGreaterThan(0);
            result.First().Name.Should().NotBeNullOrWhiteSpace();
        }

        #endregion Public Methods
    }
}