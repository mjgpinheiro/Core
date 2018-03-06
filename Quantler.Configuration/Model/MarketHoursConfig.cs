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

using System.Collections.Generic;

namespace Quantler.Configuration.Model
{
    public class MarketHoursConfig
    {
        #region Public Properties

        public List<string> Exchanges { get; set; }
        public List<MarketHoursSessionConfig> Friday { get; set; }
        public List<MarketHoursHolidayConfig> Holidays { get; set; }
        public List<MarketHoursSessionConfig> Monday { get; set; }
        public List<MarketHoursSessionConfig> Saturday { get; set; }
        public List<MarketHoursSessionConfig> Sunday { get; set; }
        public List<MarketHoursSessionConfig> Thursday { get; set; }
        public string TimeZone { get; set; }
        public List<MarketHoursSessionConfig> Tuesday { get; set; }
        public string Type { get; set; }
        public List<MarketHoursSessionConfig> Wednesday { get; set; }

        #endregion Public Properties
    }

    public class MarketHoursHolidayConfig
    {
        #region Public Properties

        public string Closed { get; set; }
        public string Comment { get; set; }
        public MarketHoursHolidayDateConfig Date { get; set; }
        public string Name { get; set; }
        public string Open { get; set; }

        #endregion Public Properties
    }

    public class MarketHoursHolidayDateConfig
    {
        #region Public Properties

        public string AfterHoliday { get; set; }
        public string BeforeHoliday { get; set; }
        public int? Day { get; set; }
        public int? DayOccurance { get; set; }
        public string DayOfWeek { get; set; }
        public int? DaysAfterHoliday { get; set; }
        public int? DaysBeforeHoliday { get; set; }
        public int? EveryXYear { get; set; }
        public bool? IsLastDayOccurance { get; set; }
        public int? Month { get; set; }
        public int? StartYear { get; set; }
        public int? WeekOfMonth { get; set; }

        #endregion Public Properties
    }

    public class MarketHoursSessionConfig
    {
        #region Public Properties

        public string End { get; set; }
        public string Session { get; set; }
        public string Start { get; set; }

        #endregion Public Properties
    }
}