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
using Quantler.Securities;

namespace Quantler.Data.Bars
{
    /// <summary>
    /// Renko bar: Renko bars ignore time and focus solely on price changes that meet a minimum requirement.
    /// </summary>
    /// <seealso cref="Quantler.Data.DataPointImpl" />
    public class RenkoBar : DataPointImpl
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RenkoBar"/> class.
        /// </summary>
        public RenkoBar()
        {
            DataType = DataType.RenkoBar;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenkoBar"/> class.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <param name="time">The time.</param>
        /// <param name="barsize">The barsize.</param>
        /// <param name="open">The open.</param>
        /// <param name="volume">The volume.</param>
        public RenkoBar(TickerSymbol ticker, DateTime time, decimal barsize, decimal open, decimal volume)
            : this()
        {
            Ticker = ticker;
            Start = time;
            EndTime = time;
            BarSize = barsize;
            Open = open;
            Close = open;
            Volume = volume;
            High = open;
            Low = open;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenkoBar"/> class.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="barsize">The barsize.</param>
        /// <param name="open">The open.</param>
        /// <param name="high">The high.</param>
        /// <param name="low">The low.</param>
        /// <param name="close">The close.</param>
        public RenkoBar(TickerSymbol ticker, DateTime start, DateTime end, decimal barsize, decimal open, decimal high, decimal low, decimal close)
            : this()
        {
            Ticker = ticker;
            Start = start;
            EndTime = end;
            BarSize = barsize;
            Open = open;
            Close = close;
            Volume = 0;
            High = high;
            Low = low;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the height of the bar
        /// </summary>
        public decimal BarSize { get; }

        /// <summary>
        /// Gets the close.
        /// </summary>
        public decimal Close
        {
            get => Price;
            private set => Price = value;
        }

        /// <summary>
        /// Gets the brick end time
        /// </summary>
        public override DateTime EndTime { get; set; }

        /// <summary>
        /// Gets the brick high price
        /// </summary>
        public decimal High { get; }

        /// <summary>
        /// Gets a value indicating whether this brick is closed.
        /// </summary>
        public bool IsClosed { get; private set; }

        /// <summary>
        /// Gets the low price
        /// </summary>
        public decimal Low { get; }

        /// <summary>
        /// Gets the open price
        /// </summary>
        public decimal Open { get; }

        /// <summary>
        /// Gets the spread between the open and closing price of this brick
        /// </summary>
        public decimal Spread => Math.Abs(Close - Open);

        /// <summary>
        /// Gets the start date and time of this brick
        /// </summary>
        public DateTime Start
        {
            get => Occured;
            private set => Occured = value;
        }

        /// <summary>
        /// Gets the volume for this brick
        /// </summary>
        public decimal Volume { get; }

        #endregion Public Properties
    }
}