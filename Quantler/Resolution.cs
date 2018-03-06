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

namespace Quantler
{
    /// <summary>
    /// Indicator resolution
    /// </summary>
    public class Resolution
    {
        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="Resolution"/> class from being created.
        /// </summary>
        private Resolution()
        {
        }

        #endregion Private Constructors

        #region Internal Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Resolution"/> class.
        /// </summary>
        /// <param name="timespan">The timespan.</param>
        internal Resolution(TimeSpan? timespan)
        {
            IsTick = !timespan.HasValue;
            Ticks = !IsTick ? timespan.Value.Ticks : 0;
        }

        #endregion Internal Constructors

        #region Public Properties

        /// <summary>
        /// Daily resolution
        /// </summary>
        public static Resolution Daily => new Resolution
        {
            IsTick = false,
            Ticks = Time.OneDay.Ticks
        };

        /// <summary>
        /// Hourly resolution
        /// </summary>
        public static Resolution Hourly => new Resolution
        {
            IsTick = false,
            Ticks = Time.OneHour.Ticks
        };

        /// <summary>
        /// Millisecond resolution
        /// </summary>
        public static Resolution Millisecond => new Resolution
        {
            IsTick = false,
            Ticks = Time.OneMilliSecond.Ticks
        };

        /// <summary>
        /// Minute resolution
        /// </summary>
        public static Resolution Minute => new Resolution
        {
            IsTick = false,
            Ticks = Time.OneMinute.Ticks
        };

        /// <summary>
        /// Second resolution
        /// </summary>
        public static Resolution Second => new Resolution
        {
            IsTick = false,
            Ticks = Time.OneSecond.Ticks
        };

        /// <summary>
        /// Ticks based resolution
        /// </summary>
        public static Resolution Tick => new Resolution
        {
            IsTick = true,
            Ticks = 1
        };

        /// <summary>
        /// Gets or sets a value indicating whether this instance is tick based.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is tick; otherwise, <c>false</c>.
        /// </value>
        public bool IsTick { get; internal set; }

        /// <summary>
        /// Gets or sets the ticks (either time based or tick based).
        /// </summary>
        public long Ticks { get; internal set; }

        /// <summary>
        /// Gets the time span, if applicable
        /// </summary>
        public TimeSpan? TimeSpan => IsTick ? new TimeSpan?(new TimeSpan(Ticks)) : null;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Custom resolution with a custom timespan
        /// </summary>
        /// <param name="timespan">The timespan.</param>
        /// <returns></returns>
        public static Resolution Custom(TimeSpan timespan) =>
            new Resolution
            {
                IsTick = false,
                Ticks = timespan.Ticks
            };

        /// <summary>
        /// Custom resolution with a custom timespan
        /// </summary>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <returns></returns>
        public static Resolution Custom(int hour, int minute, int second) =>
            new Resolution
            {
                IsTick = false,
                Ticks = (new TimeSpan(0, hour, minute, second)).Ticks
            };

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public static Resolution operator *(Resolution left, int right) =>
            new Resolution
            {
                IsTick = left.IsTick,
                Ticks = left.Ticks * right
            };

        /// <summary>
        /// Implements the operator /.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public static Resolution operator /(Resolution left, int right) =>
            new Resolution
            {
                IsTick = left.IsTick,
                Ticks = left.Ticks / right
            };

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public static Resolution operator +(Resolution left, int right) =>
            new Resolution
            {
                IsTick = left.IsTick,
                Ticks = left.Ticks + right
            };

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public static Resolution operator +(Resolution left, Resolution right) =>
            new Resolution
            {
                IsTick = left.IsTick,
                Ticks = left.Ticks + right.Ticks
            };

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() =>
            IsTick ? $"{Ticks}" : TimeSpan.ToString();

        #endregion Public Methods
    }
}