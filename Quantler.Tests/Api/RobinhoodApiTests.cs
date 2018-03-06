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
using Quantler.Api.Robinhood;
using System.Threading.Tasks;
using Xunit;

namespace Quantler.Tests.Api
{
    public class RobinhoodApiTests
    {
        #region Public Properties

        /// <summary>
        /// System under test
        /// </summary>
        public RobinhoodApi RobinhoodApi => new RobinhoodApi();

        #endregion Public Properties

        #region Public Methods

        [Fact]
        [Trait("Quantler.Api", "IEX")]
        public async Task RobinhoodApi_GetInstruments()
        {
            //Arrange

            //Act
            var result = await RobinhoodApi.GetInstrumentsAsync();

            //Assert
            result.Should().NotBeNull();
            result.Results.Length.Should().NotBe(0);
        }

        #endregion Public Methods
    }
}