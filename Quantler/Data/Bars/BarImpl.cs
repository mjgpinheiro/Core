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

using MessagePack;

namespace Quantler.Data.Bars
{
    /// <summary>
    /// Bar implementation for Open, High, Low and Close pricing
    /// </summary>
    /// <seealso cref="Quantler.Data.Bars.Bar" />
    [MessagePackObject]
    public class BarImpl : Bar
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BarImpl"/> class.
        /// </summary>
        public BarImpl()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BarImpl"/> class.
        /// </summary>
        /// <param name="open">The open price.</param>
        /// <param name="high">The high price.</param>
        /// <param name="low">The low price.</param>
        /// <param name="close">The close price.</param>
        public BarImpl(decimal open, decimal high, decimal low, decimal close)
        {
            Open = open;
            High = high;
            Low = low;
            Close = close;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Closing price of the bar. Defined as the price at Start Time + TimeSpan.
        /// </summary>
        [Key("Close")]
        public decimal Close { get; set; }

        /// <summary>
        /// High price of the bar during the time period.
        /// </summary>
        [Key("High")]
        public decimal High { get; set; }

        /// <summary>
        /// Low price of the bar during the time period.
        /// </summary>
        [Key("Low")]
        public decimal Low { get; set; }

        /// <summary>
        /// Opening price of the bar: Defined as the price at the start of the time period.
        /// </summary>
        [Key("Open")]
        public decimal Open { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Returns a clone of this bar
        /// </summary>
        public BarImpl Clone() =>
            new BarImpl(Open, High, Low, Close);

        /// <summary>
        /// Updates the bar with a new price. This will aggregate the OHLC bar
        /// </summary>
        /// <param name="price">The new price</param>
        public void Update(decimal price)
        {
            //Do not accept zero as a new price
            if (price == 0) return;

            if (Open == 0) Open = High = Low = Close = price;
            if (price > High) High = price;
            if (price < Low) Low = price;
            Close = price;
        }

        #endregion Public Methods
    }
}