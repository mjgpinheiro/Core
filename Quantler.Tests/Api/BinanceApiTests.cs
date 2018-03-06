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

using System.Threading.Tasks;
using FluentAssertions;
using Quantler.Api.Binance;
using Xunit;

namespace Quantler.Tests.Api
{
    /// <summary>
    /// Binance general api tests
    /// </summary>
    public class BinanceApiTests
    {
        #region Public Properties

        /// <summary>
        /// System under test
        /// </summary>
        public BinanceApi BinanceApi => new BinanceApi();

        #endregion Public Properties

        #region Public Methods

        [Fact]
        [Trait("Quantler.Api", "Binance")]
        //Test to see if we can get all ticker symbols correctly
        public async Task BinanceAPI_GetTickerSymbols()
        {
            //Arrange

            //Act
            var result = await BinanceApi.GetTickerSymbolsAsync();

            //Assert
            result.Length.Should().BeGreaterThan(0);
        }

        #endregion Public Methods
    }
}