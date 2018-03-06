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

using System;
using Xunit;

namespace Quantler.Tests.Core.Exchanges
{
    /// <summary>
    /// TODO: set unit tests
    /// </summary>
    public class TradingSessionDayTests
    {
        #region Public Methods

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if the exchangeModel name is correctly set
        public void LocalMarketSessionDay_ExchangeName_IsSet()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if we can correctly get a failed initialize the local market sessions
        public void LocalMarketSessionDay_Initialize_Failed()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if we can succesfully initialize the local market sessions
        public void LocalMarketSessionDay_Initialize_Success()
        {
        }

        [Theory(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        [InlineData(DayOfWeek.Monday, 9, 14, false, true)]          //Regular monday (open)
        [InlineData(DayOfWeek.Tuesday, 9, 14, false, true)]         //Regular tuesday (open)
        [InlineData(DayOfWeek.Wednesday, 9, 14, false, true)]       //Regular wednesday (open)
        [InlineData(DayOfWeek.Thursday, 9, 14, false, true)]        //Regular thursday (open)
        [InlineData(DayOfWeek.Friday, 9, 14, false, true)]          //Regular friday (open)
        [InlineData(DayOfWeek.Saturday, 9, 14, false, false)]       //Regular saturday (closed)
        [InlineData(DayOfWeek.Sunday, 9, 14, false, false)]         //Regular sunday (closed)
        [InlineData(DayOfWeek.Monday, 5, 13, false, false)]         //Regular monday non exteneded (closed) -- early
        [InlineData(DayOfWeek.Monday, 5, 13, true, true)]           //Regular monday extended (opened) -- early
        [InlineData(DayOfWeek.Monday, 17, 19, false, false)]        //Regular monday non extended (closed -- late
        [InlineData(DayOfWeek.Monday, 17, 19, true, true)]          //Regular monday extended (opened) -- late
        [InlineData(DayOfWeek.Monday, 03.50, 10.30, true, false)]   //Pre-market, but not opened yet
        [InlineData(DayOfWeek.Monday, 04.00, 10.30, true, true)]    //Pre-market, opened
        [InlineData(DayOfWeek.Monday, 09.50, 10.30, true, true)]    //Normal market, opened
        [InlineData(DayOfWeek.Monday, 09.50, 10.30, false, true)]   //Normal market, opened
        [InlineData(DayOfWeek.Monday, 15.00, 16.01, false, false)]  //After market, closed, non extended
        [InlineData(DayOfWeek.Monday, 15.00, 16.01, true, true)]    //Normal market, opened
        [InlineData(DayOfWeek.Monday, 16.00, 20.01, true, false)]   //After market, too late, closed
        //Test to see if the market is open between two moments of a day we expect it to be open or closed
        public void LocalMarketSessionDay_IsOpenBetween(DayOfWeek dow, double starthour, double endhour, bool extended, bool expected)
        {
            //Arrange
            TimeSpan start = TimeSpan.FromHours(starthour);
            TimeSpan end = TimeSpan.FromHours(endhour);
        }

        [Theory(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        [InlineData(DayOfWeek.Monday, 03.55, true, false)]      //Extended, not yet opened
        [InlineData(DayOfWeek.Monday, 03.55, false, false)]     //Nonextended, not open yet
        [InlineData(DayOfWeek.Monday, 04.01, true, true)]       //Extended, opened
        [InlineData(DayOfWeek.Monday, 03.55, false, false)]     //Nonextended, not open yet
        [InlineData(DayOfWeek.Monday, 09.50, true, true)]       //Normal, opened
        [InlineData(DayOfWeek.Monday, 09.50, false, true)]      //Normal, opened
        [InlineData(DayOfWeek.Monday, 16.01, false, false)]     //Extended, not opened
        [InlineData(DayOfWeek.Monday, 16.01, true, true)]       //Extended opened
        [InlineData(DayOfWeek.Monday, 20.01, true, false)]      //Extended, not opened
        //Test to see if the market is open on a given time of day in the week
        public void LocalMarketSessionDay_IsOpenOn(DayOfWeek dow, double hour, bool extended, bool expected)
        {
            //Arrange
            TimeSpan start = TimeSpan.FromHours(hour);
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if the regular days are correcly loaded
        public void LocalMarketSessionDay_IsRegularLoaded_IsTrue()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if the market is closed on the time expected
        public void LocalMarketSessionDay_MarketClosedTime_Holiday()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if the market is closed on the time expected
        public void LocalMarketSessionDay_MarketClosedTime_Regular()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if the market is closed on the time expected
        public void LocalMarketSessionDay_MarketClosedTime_Weekend()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if the market is opening on the time expected
        public void LocalMarketSessionDay_MarketOpenedTime_Holiday()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if the market is opening on the time expected
        public void LocalMarketSessionDay_MarketOpenedTime_Regular()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.ExchangeModels", "Quantler")]
        //Test to see if the market is opening on the time expected
        public void LocalMarketSessionDay_MarketOpenedTime_Weekend()
        {
        }

        #endregion Public Methods
    }
}