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
    /// TODO: set unit tests
    /// </summary>
    public class BaseExchangeTests
    {
        #region Public Methods

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if the regular days are correcly loaded
        public void BaseExchangeModel_IsOpen_IsFalse()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if the regular days are correcly loaded
        public void BaseExchangeModel_IsOpen_IsTrue()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if we are correcly closed during a holiday
        public void BaseExchangeModel_IsOpenBetween_IsFalse_HolidayOverlap()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if we are correcly closed during the weekend
        public void BaseExchangeModel_IsOpenBetween_IsFalse_WeekendOverlap()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if we are correcly opened on an expected date
        public void BaseExchangeModel_IsOpenOnDate_IsFalse_Holiday()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if we are correcly opened on an expected date
        public void BaseExchangeModel_IsOpenOnDate_IsFalse_Weekend()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if we are correcly opened on an expected date
        public void BaseExchangeModel_IsOpenOnDate_IsTrue()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if we are correcly opened on an expected date
        public void BaseExchangeModel_IsOpenOnDate_IsTrue_Partial_Holiday()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if we are correcly opened on an expected date
        public void BaseExchangeModel_IsOpenOnDateTime_IsFalse_Holiday()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if we are correcly opened on an expected date
        public void BaseExchangeModel_IsOpenOnDateTime_IsFalse_Partial_Holiday()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if we are correcly opened on an expected date
        public void BaseExchangeModel_IsOpenOnDateTime_IsFalse_Weekend()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if we are correcly opened on an expected date
        public void BaseExchangeModel_IsOpenOnDateTime_IsTrue()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if we are correcly opened on an expected date
        public void BaseExchangeModel_IsOpenOnDateTime_IsTrue_Partial_Holiday()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if we can get the correct nex market close timing
        public void BaseExchangeModel_NextMarketClose_Holiday()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if we can get the correct nex market close timing
        public void BaseExchangeModel_NextMarketClose_Regular()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if we can get the correct nex market close timing
        public void BaseExchangeModel_NextMarketClose_Weekend()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if we can get the correct nex market opening timing
        public void BaseExchangeModel_NextNextMarketOpen_After_Holiday()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if we can get the correct nex market opening timing
        public void BaseExchangeModel_NextNextMarketOpen_After_Regular()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if we can get the correct nex market opening timing
        public void BaseExchangeModel_NextNextMarketOpen_After_Weekend()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if we can get the correct nex market opening timing
        public void BaseExchangeModel_NextNextMarketOpen_During_Regular()
        {
        }

        #endregion Public Methods
    }
}