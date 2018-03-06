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
using System;
using Quantler.Securities;

namespace Quantler.Data.Corporate
{
    /// <summary>
    /// Stock split
    /// </summary>
    /// <seealso cref="Quantler.Data.DataPointImpl" />
    [MessagePackObject]
    public class Split : DataPointImpl
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Split"/> class.
        /// </summary>
        public Split() =>
            DataType = DataType.Split;

        /// <summary>
        /// Initializes a new instance of the <see cref="Split"/> class.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <param name="date">The date.</param>
        /// <param name="price">The price.</param>
        /// <param name="factor">The factor.</param>
        public Split(TickerSymbol ticker, DateTime date, decimal price, decimal factor)
            : this()
        {
            Ticker = ticker;
            Occured = date;
            TimeZone = TimeZone.Utc;
            ReferencePrice = price;
            SplitFactor = factor;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the reference price.
        /// </summary>
        [Key(6)]
        public decimal ReferencePrice { get; set; }

        /// <summary>
        /// Gets or sets the split factor.
        /// </summary>
        [Key(7)]
        public decimal SplitFactor { get; set; }

        #endregion Public Properties
    }
}