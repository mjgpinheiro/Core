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
using Quantler.Api.Fixer;
using Quantler.Interfaces;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Quantler.Tests.Api
{
    /// <summary>
    /// Fixer api general tests
    /// </summary>
    public class FixerApiTests
    {
        #region Public Properties

        /// <summary>
        /// System under test
        /// </summary>
        public FixerApi FixerApi => new FixerApi();

        #endregion Public Properties

        #region Public Methods

        [Fact]
        [Trait("Quantler.Api", "Fixer")]
        //Get latest conversion rates
        public async Task FixerApi_GetLatestConversionRates_USD()
        {
            //Arrange

            //Act
            var result = await FixerApi.GetLatestConversionRatesAsync(CurrencyType.BRL);

            //Assert
            result.Should().NotBeNull();
            result.Base.Should().NotBeNullOrWhiteSpace();
            result.Date.Should().BeAfter(DateTime.Today.AddDays(-15));
            result.Rates.Count.Should().BeGreaterThan(1);
        }

        [Fact]
        [Trait("Quantler.Api", "Fixer")]
        //Get latest conversion rates
        public async Task FixerApi_GetDateConversionRates_USD()
        {
            //Arrange
            var date = new DateTime(2018, 1, 3);

            //Act
            var result = await FixerApi.GetDateConversionRatesAsync(CurrencyType.BRL, date);

            //Assert
            result.Should().NotBeNull();
            result.Base.Should().NotBeNullOrWhiteSpace();
            result.Rates.Count.Should().BeGreaterThan(1);
            result.Date.Date.Should().Be(date.Date);
        }

        #endregion Public Methods
    }
}