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
    /// Dividend amount
    /// </summary>
    [MessagePackObject]
    public class Dividend : DataPointImpl
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Dividend"/> class.
        /// </summary>
        public Dividend() =>
            DataType = DataType.Dividend;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dividend"/> class.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <param name="date">The date.</param>
        /// <param name="amount">The amount.</param>
        public Dividend(TickerSymbol ticker, DateTime date, decimal amount)
            : this()
        {
            Ticker = ticker;
            Occured = date;
            TimeZone = TimeZone.Utc;
            Amount = amount;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Amount distribution
        /// </summary>
        [Key(6)]
        public decimal Amount
        {
            get => Price;
            set => Price = Math.Round(Price, 2);
        }

        #endregion Public Properties
    }
}