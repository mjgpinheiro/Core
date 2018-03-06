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

using Xunit;

namespace Quantler.Tests.Core.Exchanges
{
    /// <summary>
    /// Unit tests for holiday functions
    /// </summary>
    public class HolidayTests
    {
        #region Public Methods

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Check if we can get the correct holiday session information
        public void LocalHoliday_GetHolidaySession_IsHoliday()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Check if we can get the correct holiday session information, when it is in fact not a holiday
        public void LocalHoliday_GetHolidaySession_IsHoliday_Incorrect()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see behaviour on failed initialization of public holiday instance
        public void LocalHoliday_Initialize_Failed_BadInput()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if we can succesfully initialize the local holiday instance
        public void LocalHoliday_Initialize_Success()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Check if this date has a holiday, expected correct
        public void LocalHoliday_IsHoliday_Correct()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Check if this date has a holiday, expected incorrect
        public void LocalHoliday_IsHoliday_InCorrect()
        {
        }

        #endregion Public Methods
    }
}