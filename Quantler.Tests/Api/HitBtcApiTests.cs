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
using Quantler.Api.HitBtc;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Quantler.Tests.Api
{
    /// <summary>
    /// General hit btc api tests
    /// </summary>
    public class HitBtcApiTests
    {
        #region Public Properties

        /// <summary>
        /// System under test
        /// </summary>
        public HitBtcApi HitBtcApi => new HitBtcApi();

        #endregion Public Properties

        #region Public Methods

        [Fact]
        [Trait("Quantler.Api", "HitBtc")]
        public async Task HitBtcApi_GetSymbols()
        {
            //Arrange

            //Act
            var result = await HitBtcApi.GetSymbolsAsync();

            //Assert
            result.Should().NotBeNull();
            result.Count().Should().BeGreaterThan(1);
            result.First().Id.Should().NotBeNullOrWhiteSpace();
        }

        #endregion Public Methods
    }
}