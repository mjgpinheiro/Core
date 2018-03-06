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
using Quantler.Api.IEX;
using Quantler.Api.IEX.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Quantler.Tests.Api
{
    /// <summary>
    /// General IEX api tests
    /// </summary>
    public class IEXApiTests
    {
        #region Public Properties

        /// <summary>
        /// System under test
        /// </summary>
        public IEXApi IexApi => new IEXApi();

        #endregion Public Properties

        #region Private Properties

        /// <summary>
        /// Gets the correct symbol.
        /// </summary>
        private string CorrectSymbol => "MSFT";

        /// <summary>
        /// Gets the incorrect symbol.
        /// </summary>
        private string IncorrectSymbol => "MCD!!";

        #endregion Private Properties

        #region Public Methods

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task IexApi_GetChart_Failed()
        {
            //Arrange

            //Act
            var result = await IexApi.GetChartAsync(IncorrectSymbol, ChartRange.SixMonths);

            //Assert
            result.Should().BeNullOrEmpty();
        }

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task IexApi_GetChart_Success()
        {
            //Arrange

            //Act
            var result = await IexApi.GetChartAsync(CorrectSymbol, ChartRange.SixMonths);

            //Assert
            result.Should().NotBeNull();
            result.Count().Should().BeGreaterThan(1);
            result.First().Close.Should().BeGreaterThan(0);
            result.First().Date.Should().BeAfter(DateTime.MinValue);
            result.First().Date.Date.Should().NotBe(DateTime.UtcNow.Date);
        }

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task IexApi_GetCompany_Failed()
        {
            //Arrange

            //Act
            var result = await IexApi.GetCompanyAsync(IncorrectSymbol);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task IexApi_GetCompany_Success()
        {
            //Arrange

            //Act
            var result = await IexApi.GetCompanyAsync(CorrectSymbol);

            //Assert
            result.Should().NotBeNull();
            result.CompanyName.Should().NotBeNullOrWhiteSpace();
            result.IssueType.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task IexApi_GetEarnings_Failed()
        {
            //Arrange

            //Act
            var result = await IexApi.GetEarningsAsync(IncorrectSymbol);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task IexApi_GetEarnings_Success()
        {
            //Arrange

            //Act
            var result = await IexApi.GetEarningsAsync(CorrectSymbol);

            //Assert
            result.Should().NotBeNull();
            result.earnings.Count.Should().BePositive();
            result.symbol.Should().Be(CorrectSymbol);
        }

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task IexApi_GetFinancials_Failed()
        {
            //Arrange

            //Act
            var result = await IexApi.GetFinancialsAsync(IncorrectSymbol);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task IexApi_GetFinancials_Success()
        {
            //Arrange

            //Act
            var result = await IexApi.GetFinancialsAsync(CorrectSymbol);

            //Assert
            result.Should().NotBeNull();
            result.Financials.Length.Should().BePositive();
            result.Symbol.Should().Be(CorrectSymbol);
            result.Financials.First().OperatingExpense.Should().BePositive();
            result.Financials.First().ReportDate.Should().BeAfter(DateTime.MinValue);
        }

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task IexApi_GetHIST_Failed()
        {
            //Arrange

            //Act
            var result = await IexApi.GetHISTAsync(0);

            //Assert
            result.Should().BeNullOrEmpty();
        }

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task IexApi_GetHIST_Success()
        {
            //Arrange

            //Act
            var result = await IexApi.GetHISTAsync(20171127);

            //Assert
            result.Should().NotBeNull();
            result.Length.Should().BePositive();
            result[0].Date.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task IexApi_GetKeyStats_Failed()
        {
            //Arrange

            //Act
            var result = await IexApi.GetKeyStatsAsync(IncorrectSymbol);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task IexApi_GetKeyStats_Success()
        {
            //Arrange

            //Act
            var result = await IexApi.GetKeyStatsAsync(CorrectSymbol);

            //Assert
            result.Should().NotBeNull();
            result["companyName"].ToString().Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task IexApi_GetNews_Failed()
        {
            //Arrange

            //Act
            var result = await IexApi.GetNewsAsync(IncorrectSymbol, 5);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task IexApi_GetNews_Success()
        {
            //Arrange

            //Act
            var result = await IexApi.GetNewsAsync(CorrectSymbol, 5);

            //Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.First().DateTimeUtc.Should().BeAfter(DateTime.MinValue);
            result.First().Headline.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task IexApi_GetQuote_Failed()
        {
            //Arrange

            //Act
            var result = await IexApi.GetQuoteAsync(IncorrectSymbol);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task IexApi_GetOpenClose_Success()
        {
            //Arrange

            //Act
            var result = await IexApi.GetOpenCloseAsync(CorrectSymbol);

            //Assert
            result.Should().NotBeNull();
            result.Close.DateTime.Should().BeAfter(DateTime.MinValue);
            result.Open.Price.Should().BePositive();
        }

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task IexApi_GetOpenClose_Failed()
        {
            //Arrange

            //Act
            var result = await IexApi.GetOpenCloseAsync(IncorrectSymbol);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task IexApi_GetDividends_Success()
        {
            //Arrange

            //Act
            var result = await IexApi.GetDividendsAsync(CorrectSymbol, ChartRange.ThreeMonths);

            //Assert
            result.Should().NotBeNull();
            var item = result.FirstOrDefault();
            item.Should().NotBeNull();
            item.Amount.Should().BePositive();
            item.DeclaredDate.Should().BeAfter(DateTime.MinValue);
            item.ExDate.Should().BeAfter(DateTime.MinValue);
            item.PaymentDate.Should().BeAfter(DateTime.MinValue);
            item.RecordDate.Should().BeAfter(DateTime.MinValue);
            item.Type.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task IexApi_GetDividends_Failed()
        {
            //Arrange

            //Act
            var result = await IexApi.GetDividendsAsync(IncorrectSymbol, ChartRange.FiveYears);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task IexApi_GetSplits_Success()
        {
            //Arrange

            //Act
            var result = await IexApi.GetSplitsAsync("ibb", ChartRange.FiveYears);

            //Assert
            result.Should().NotBeNull();
            var item = result.FirstOrDefault();
            item.Should().NotBeNull();
            item.Ratio.Should().BePositive();
            item.DeclaredDate.Should().BeAfter(DateTime.MinValue);
            item.ExDate.Should().BeAfter(DateTime.MinValue);
            item.PaymentDate.Should().BeAfter(DateTime.MinValue);
            item.RecordDate.Should().BeAfter(DateTime.MinValue);
        }

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task IexApi_GetSplits_Failed()
        {
            //Arrange

            //Act
            var result = await IexApi.GetDividendsAsync(IncorrectSymbol, ChartRange.FiveYears);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task IexApi_Previous_Success()
        {
            //Arrange

            //Act
            var result = await IexApi.GetPreviousAsync(CorrectSymbol);

            //Assert
            result.Should().NotBeNull();
            result.Date.Should().BeAfter(DateTime.MinValue);
            result.Symbol.Should().Be(CorrectSymbol);
            result.Close.Should().BePositive();
        }

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task IexApi_Previous_Failed()
        {
            //Arrange

            //Act
            var result = await IexApi.GetPreviousAsync(IncorrectSymbol);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task IexApi_GetQuote_Success()
        {
            //Arrange

            //Act
            var result = await IexApi.GetQuoteAsync("SRC");

            //Assert
            result.Should().NotBeNull();
            result.MarketCap.Should().BePositive();
            result.CompanyName.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task IexApi_GetSymbols_Success()
        {
            //Arrange

            //Act
            var result = await IexApi.GetSymbolsAsync();

            //Assert
            result.Count().Should().BePositive();
            result.First().Name.Should().NotBeNullOrWhiteSpace();
        }

        #endregion Public Methods
    }
}