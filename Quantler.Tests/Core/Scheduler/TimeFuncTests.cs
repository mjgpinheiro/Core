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
    /// Time based functions tests
    /// TODO: set unit tests
    /// </summary>
    public class TimeFuncTests
    {
        #region Private Fields

        /// <summary>
        /// The date function
        /// </summary>
        private readonly DateFunc Date = new DateFunc();

        /// <summary>
        /// The scheduled actions keeper
        /// </summary>
        private readonly ScheduledActionsKeeper ScheduledActionsKeeper;

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
        /// Initializes a new instance of the <see cref="TimeFuncTests"/> class.
        /// </summary>
        public TimeFuncTests()
        {
            //Initialize
            Func<DateTime, bool> func = d => !(d.Day != 31) && d.DayOfWeek != DayOfWeek.Sunday && d.DayOfWeek != DayOfWeek.Saturday; //We simulate that we are not opened on any 31st days and during weekends
            Exchange.Setup(x => x.IsOpenOnDate(It.IsAny<DateTime>())).Returns(func);
        }

        #endregion Public Constructors

        #region Public Methods

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Scheduler", "Quantler")]
        //Test to see if we can successfully test after market open timings
        public void TimeFunc_AfterMarketOpen_Hit_Success()
        {
            //Arrange
            List<DateTime> expected = new List<DateTime>()
            {
                new DateTime(2016, 01, 30, 01, 00, 00 ), //Check timing for each day in 1 month (January for instance)
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
                new DateTime(2016, 12, 31, 01, 00, 00 )
            };

            //SUT
            //ScheduledActionsKeeper.Event(Date.EveryDay(), Time.AfterMarketOpen(Exchange.Object, TimeSpan.FromMinutes(1)), (name, date) => Result.Add(date));

            //Act
            Run(new DateTime(2016, 01, 01), new DateTime(2016, 01, 31), TimeSpan.FromSeconds(1));

            //Assert
            expected.ForEach(x => expected.Should().Contain(x, "because we expected this to be hit"));
            expected.Count.Should().Be(Result.Count);
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Scheduler", "Quantler")]
        //Test to see if we can successfully test after market open timings
        public void TimeFunc_AfterMarketOpen_NoHit_Success()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Scheduler", "Quantler")]
        //Test to see if we can successfully test at a specific timespan
        public void TimeFunc_At_Timespan_Hit_Success()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Scheduler", "Quantler")]
        //Test to see if we can successfully test at a specific timespan
        public void TimeFunc_At_Timespan_NoHit_Success()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Scheduler", "Quantler")]
        //Test to see if we can successfully test at a specific timespan in another timezone
        public void TimeFunc_At_Timespan_Timezoned_Hit_Success()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Scheduler", "Quantler")]
        //Test to see if we can successfully test at a specific timespan in another timezone
        public void TimeFunc_At_Timespan_Timezoned_NoHit_Success()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Scheduler", "Quantler")]
        //Test to see if we can successfully test before market openings
        public void TimeFunc_BeforeMarketClose_Hit_Success()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Scheduler", "Quantler")]
        //Test to see if we can successfully test before market openings
        public void TimeFunc_BeforeMarketClose_NoHit_Success()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Scheduler", "Quantler")]
        //Test to see if we can successfully test interval timers
        public void TimeFunc_Every_Interval_Hit_Success()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Scheduler", "Quantler")]
        //Test to see if we can successfully initialize time func
        public void TimeFunc_Initialize_Success()
        {
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