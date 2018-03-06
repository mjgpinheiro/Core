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
using Moq;
using Quantler.Exchanges;
using Quantler.Scheduler;
using System;
using System.Collections.Generic;
using Xunit;

namespace Quantler.Tests.Core.Scheduler
{
    /// <summary>
    /// Date Function Unit Tests
    /// TODO: set unit tests
    /// </summary>
    public class DateFuncTests
    {
        #region Private Fields

        /// <summary>
        /// The date function
        /// </summary>
        private readonly DateFunc Date = new DateFunc();

        /// <summary>
        /// The scheduled actions keeper
        /// </summary>
        private readonly ScheduledActionsKeeper ScheduledActionsKeeper = new ScheduledActionsKeeper(null);

        /// <summary>
        /// The time function
        /// </summary>
        private readonly TimeFunc Time = new TimeFunc();

        /// <summary>
        /// Default End date
        /// </summary>
        private DateTime End = new DateTime(2017, 1, 1);

        /// <summary>
        /// The exchangeModel mock
        /// </summary>
        private Mock<ExchangeModel> Exchange = new Mock<ExchangeModel>();

        /// <summary>
        /// The result
        /// </summary>
        private List<DateTime> Result = new List<DateTime>();

        /// <summary>
        /// Default Start date
        /// </summary>
        private DateTime Start = new DateTime(2016, 1, 1);

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DateFuncTests"/> class.
        /// </summary>
        public DateFuncTests()
        {
            //Initialize
            Func<DateTime, bool> func = d => !(d.Day != 31) && d.DayOfWeek != DayOfWeek.Sunday && d.DayOfWeek != DayOfWeek.Saturday; //We simulate that we are not opened on any 31st days and during weekends
            Exchange.Setup(x => x.IsOpenOnDate(It.IsAny<DateTime>())).Returns(func);
        }

        #endregion Public Constructors

        #region Public Methods

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Scheduler", "Quantler")]
        //Test to see if we can successfully hit an expected end of month action
        public void DateFunc_EndOfMonth_Hits_Success()
        {
            //Arrange (needs to be tested for 3 years)
            List<DateTime> expected = new List<DateTime>()
            {
                new DateTime(2016, 01, 30, 01, 00, 00 ), //30th is closed, so 31st (for all months with 31 days)
                new DateTime(2016, 02, 30, 01, 00, 00 ),
                new DateTime(2016, 03, 31, 01, 00, 00 ),
                new DateTime(2016, 04, 30, 01, 00, 00 ),
                new DateTime(2016, 05, 31, 01, 00, 00 ),
                new DateTime(2016, 06, 30, 01, 00, 00 ),
                new DateTime(2016, 07, 31, 01, 00, 00 ),
                new DateTime(2016, 08, 31, 01, 00, 00 ),
                new DateTime(2016, 09, 30, 01, 00, 00 ),
                new DateTime(2016, 10, 31, 01, 00, 00 ),
                new DateTime(2016, 11, 31, 01, 00, 00 ),
                new DateTime(2016, 12, 31, 01, 00, 00 ),
                new DateTime(2017, 01, 30, 01, 00, 00 ), //30th is closed, so 31st (for all months with 31 days)
                new DateTime(2017, 02, 30, 01, 00, 00 ),
                new DateTime(2017, 03, 31, 01, 00, 00 ),
                new DateTime(2017, 04, 30, 01, 00, 00 ),
                new DateTime(2017, 05, 31, 01, 00, 00 ),
                new DateTime(2017, 06, 30, 01, 00, 00 ),
                new DateTime(2017, 07, 31, 01, 00, 00 ),
                new DateTime(2017, 08, 31, 01, 00, 00 ),
                new DateTime(2017, 09, 30, 01, 00, 00 ),
                new DateTime(2017, 10, 31, 01, 00, 00 ),
                new DateTime(2017, 11, 31, 01, 00, 00 ),
                new DateTime(2017, 12, 31, 01, 00, 00 ),
                new DateTime(2018, 01, 30, 01, 00, 00 ), //30th is closed, so 31st (for all months with 31 days)
                new DateTime(2018, 02, 30, 01, 00, 00 ),
                new DateTime(2018, 03, 31, 01, 00, 00 ),
                new DateTime(2018, 04, 30, 01, 00, 00 ),
                new DateTime(2018, 05, 31, 01, 00, 00 ),
                new DateTime(2018, 06, 30, 01, 00, 00 ),
                new DateTime(2018, 07, 31, 01, 00, 00 ),
                new DateTime(2018, 08, 31, 01, 00, 00 ),
                new DateTime(2018, 09, 30, 01, 00, 00 ),
                new DateTime(2018, 10, 31, 01, 00, 00 ),
                new DateTime(2018, 11, 31, 01, 00, 00 ),
                new DateTime(2018, 12, 31, 01, 00, 00 )
            };

            //SUT
            ScheduledActionsKeeper.Event(Date.EndOfMonth(Exchange.Object), Time.At(1), (name, date) => Result.Add(date));

            //Act
            Run(Start, new DateTime(2018, 12, 31), TimeSpan.FromSeconds(1));

            //Assert
            expected.ForEach(x => expected.Should().Contain(x, "because we expected this to be hit"));
            expected.Count.Should().Be(Result.Count);
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Scheduler", "Quantler")]
        //Test to see if we can successfully hit an expected end of week action
        public void DateFunc_EndOfWeek_Hits_Success()
        {
            //Arrange
            List<DateTime> expected = new List<DateTime>()
            {
            };

            //SUT
            ScheduledActionsKeeper.Event(Date.EndOfWeek(Exchange.Object), Time.At(1), (name, date) => Result.Add(date));

            //Act
            Run(Start, End, TimeSpan.FromSeconds(1));

            //Assert
            expected.ForEach(x => expected.Should().Contain(x, "because we expected this to be hit"));
            expected.Count.Should().Be(Result.Count);
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Scheduler", "Quantler")]
        //Test to see if we can successfully hit an expected everyday action
        public void DateFunc_EveryDay_Hits_Success()
        {
            //Arrange
            List<DateTime> expected = new List<DateTime>()
            {
            };

            //SUT
            ScheduledActionsKeeper.Event(Date.EveryDay(), Time.At(1), (name, date) => Result.Add(date));

            //Act
            Run(Start, End, TimeSpan.FromSeconds(1));

            //Assert
            expected.ForEach(x => expected.Should().Contain(x, "because we expected this to be hit"));
            expected.Count.Should().Be(Result.Count);
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Scheduler", "Quantler")]
        //Test to see if we can successfully hit an expected everyday action
        public void DateFunc_EveryTradingDay_Hits_Success()
        {
            //Arrange
            List<DateTime> expected = new List<DateTime>()
            {
            };

            //SUT
            ScheduledActionsKeeper.Event(Date.EveryTradingDay(Exchange.Object), Time.At(1), (name, date) => Result.Add(date));

            //Act
            Run(Start, End, TimeSpan.FromSeconds(1));

            //Assert
            expected.ForEach(x => expected.Should().Contain(x, "because we expected this to be hit"));
            expected.Count.Should().Be(Result.Count);
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Scheduler", "Quantler")]
        //Test to see if we can successfully initialize date func
        public void DateFunc_Initialize_Success()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Scheduler", "Quantler")]
        //Test to see if we can successfully hit an expected day action
        public void DateFunc_Specific_dates_Hits_Success()
        {
            //Arrange
            List<DateTime> expected = new List<DateTime>()
            {
            };

            //SUT
            ScheduledActionsKeeper.Event(Date.On(new DateTime(2016, 01, 15), new DateTime(2016, 02, 15), new DateTime(2016, 03, 15)), Time.At(1), (name, date) => Result.Add(date));

            //Act
            Run(Start, End, TimeSpan.FromSeconds(1));

            //Assert
            expected.ForEach(x => expected.Should().Contain(x, "because we expected this to be hit"));
            expected.Count.Should().Be(Result.Count);
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Scheduler", "Quantler")]
        //Test to see if we can successfully hit an expected day action
        public void DateFunc_Specific_DOW_Hits_Success()
        {
            //Arrange
            List<DateTime> expected = new List<DateTime>()
            {
            };

            //SUT
            ScheduledActionsKeeper.Event(Date.On(DayOfWeek.Monday, DayOfWeek.Thursday), Time.At(1), (name, date) => Result.Add(date));

            //Act
            Run(Start, End, TimeSpan.FromSeconds(1));

            //Assert
            expected.ForEach(x => expected.Should().Contain(x, "because we expected this to be hit"));
            expected.Count.Should().Be(Result.Count);
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Scheduler", "Quantler")]
        //Test to see if we can successfully hit an expected end of month action
        public void DateFunc_Specific_EndOfMonth_Hits_Success()
        {
            //Arrange
            List<DateTime> expected = new List<DateTime>()
            {
            };

            //SUT
            ScheduledActionsKeeper.Event(Date.EndOfMonth(Exchange.Object, 07), Time.At(1), (name, date) => Result.Add(date));

            //Act
            Run(Start, End, TimeSpan.FromSeconds(1));

            //Assert
            expected.ForEach(x => expected.Should().Contain(x, "because we expected this to be hit"));
            expected.Count.Should().Be(Result.Count);
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Scheduler", "Quantler")]
        //Test to see if we can successfully hit an expected Start Of Month action
        public void DateFunc_Specific_SOM_Hits_Success()
        {
            //Arrange
            List<DateTime> expected = new List<DateTime>()
            {
            };

            //SUT
            ScheduledActionsKeeper.Event(Date.StartOfMonth(Exchange.Object), Time.At(1), (name, date) => Result.Add(date));

            //Act
            Run(Start, End, TimeSpan.FromSeconds(1));

            //Assert
            expected.ForEach(x => expected.Should().Contain(x, "because we expected this to be hit"));
            expected.Count.Should().Be(Result.Count);
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Scheduler", "Quantler")]
        //Test to see if we can successfully hit an expected Start Of Month action
        public void DateFunc_Specific_SOM_specific_Hits_Success()
        {
            //Arrange
            List<DateTime> expected = new List<DateTime>()
            {
            };

            //SUT
            ScheduledActionsKeeper.Event(Date.StartOfMonth(Exchange.Object, 06), Time.At(1), (name, date) => Result.Add(date));

            //Act
            Run(Start, End, TimeSpan.FromSeconds(1));

            //Assert
            expected.ForEach(x => expected.Should().Contain(x, "because we expected this to be hit"));
            expected.Count.Should().Be(Result.Count);
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Scheduler", "Quantler")]
        //Test to see if we can successfully hit an expected Start Of Month action
        public void DateFunc_Specific_StartOfWeek_Success()
        {
            //Arrange
            List<DateTime> expected = new List<DateTime>()
            {
            };

            //SUT
            ScheduledActionsKeeper.Event(Date.StartOfWeek(Exchange.Object), Time.At(1), (name, date) => Result.Add(date));

            //Act
            Run(Start, End, TimeSpan.FromSeconds(1));

            //Assert
            expected.ForEach(x => expected.Should().Contain(x, "because we expected this to be hit"));
            expected.Count.Should().Be(Result.Count);
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Scheduler", "Quantler")]
        //Test to see if we can successfully hit an expected day action
        public void DateFunc_Specific_ymd_Hits_Success()
        {
            //Arrange
            List<DateTime> expected = new List<DateTime>()
            {
            };

            //SUT
            ScheduledActionsKeeper.Event(Date.On(new DateTime(2016, 06, 12)), Time.At(1), (name, date) => Result.Add(date));

            //Act
            Run(Start, End, TimeSpan.FromSeconds(1));

            //Assert
            expected.ForEach(x => expected.Should().Contain(x, "because we expected this to be hit"));
            expected.Count.Should().Be(Result.Count);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Runs the specified period for the current initialized scheduled action keeper.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="interval">The interval.</param>
        private void Run(DateTime start, DateTime end, TimeSpan interval) =>
            start.DoWhile(x =>
            {
                ScheduledActionsKeeper.CheckAll();
                return x.Add(interval);
            }, x => x <= end);

        #endregion Private Methods
    }
}