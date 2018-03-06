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

using Quantler.Interfaces;
using System;
using MessagePack;

namespace Quantler.Securities
{
    /// <summary>
    /// Ticker Symbol
    /// </summary>
    [MessagePackObject]
    public class TickerSymbol
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TickerSymbol"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="currency">The currency.</param>
        /// <param name="commodity"></param>
        public TickerSymbol(string name, string commodity, CurrencyType currency)
        {
            Name = name;
            Currency = currency;
            Commodity = commodity;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the underlying commodity name
        /// </summary>
        [Key(1)]
        public string Commodity { get; }

        /// <summary>
        /// Base Currency
        /// </summary>
        [Key(2)]
        public CurrencyType Currency { get; }

        /// <summary>
        /// Ticker name
        /// </summary>
        [Key(0)]
        public string Name { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// All ticker symbol instance
        /// </summary>
        /// <returns></returns>
        public static TickerSymbol All() => new TickerSymbol("*", "*", CurrencyType.USD);

        /// <summary>
        /// Returns an empty ticker symbol
        /// </summary>
        /// <param name="tickername">The tickername.</param>
        /// <returns></returns>
        public static TickerSymbol NIL(string tickername) => new TickerSymbol(tickername, "", CurrencyType.USD);

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(TickerSymbol left, string right) => !(left == right);

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(string left, TickerSymbol right) => !(left == right);

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(TickerSymbol left, TickerSymbol right) =>
            left.Name == right.Name && left.Commodity == right.Commodity && left.Currency == right.Currency;

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(TickerSymbol left, TickerSymbol right) => !(left == right);

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(TickerSymbol left, string right) =>
            string.Compare(left?.Name, right, StringComparison.OrdinalIgnoreCase) == 0;

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(string left, TickerSymbol right) => (right == left);

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() =>
            Name;

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) =>
            obj is TickerSymbol tickersymbol && tickersymbol == this;

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash *= Commodity.GetHashCode();
                hash *= Currency.GetHashCode();
                hash *= Name.GetHashCode();
                return hash;
            }
        }
            

        #endregion Public Methods
    }
}