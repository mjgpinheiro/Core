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

namespace Quantler.Exchanges.Sessions
{
    /// <summary>
    /// Start and end time of a specific trading session
    /// </summary>
    public class LocalMarketSession
    {
        #region Public Constructors

        /// <summary>
        /// Create a market session
        /// </summary>
        /// <param name="type"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="comment"></param>
        public LocalMarketSession(MarketSessionType type, TimeSpan start, TimeSpan end, string comment = "")
        {
            Start = start;
            End = end;
            Type = type;
            Comment = comment;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Comment applicable to local market session
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Session end time
        /// </summary>
        public TimeSpan End { get; set; }

        /// <summary>
        /// Session start time
        /// </summary>
        public TimeSpan Start { get; set; }

        /// <summary>
        /// Market session type
        /// </summary>
        public MarketSessionType Type { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Check if the supplied timespan is within range of this session
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool Contains(TimeSpan time) =>
            End > time && Start < time;

        /// <summary>
        /// Get copy of current local market session
        /// </summary>
        /// <returns></returns>
        public LocalMarketSession Copy() =>
            new LocalMarketSession(Type, Start, End);

        #endregion Public Methods
    }
}