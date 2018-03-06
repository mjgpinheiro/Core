#region License Header

/*
* QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
* Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
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
*
* Modifications Copyright 2018 Quantler B.V.
*
*/

#endregion License Header

using Quantler.Data.Bars;

namespace Quantler.Data
{
    /// <summary>
    /// Captured market prices
    /// </summary>
    public class Prices
    {
        #region Public Fields

        /// <summary>
        /// Current close price
        /// </summary>
        public readonly decimal Close;

        /// <summary>
        /// Current price
        /// </summary>
        public readonly decimal Current;

        /// <summary>
        /// Current high price
        /// </summary>
        public readonly decimal High;

        /// <summary>
        /// Current low price
        /// </summary>
        public readonly decimal Low;

        /// <summary>
        /// Current open price
        /// </summary>
        public readonly decimal Open;

        #endregion Public Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Prices"/> class.
        /// </summary>
        /// <param name="bar">The bar.</param>
        public Prices(Bar bar)
            : this(bar.Close, bar.Open, bar.High, bar.Low, bar.Close)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Prices"/> class.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="open">The open.</param>
        /// <param name="high">The high.</param>
        /// <param name="low">The low.</param>
        /// <param name="close">The close.</param>
        public Prices(decimal current, decimal open, decimal high, decimal low, decimal close)
        {
            Current = current;
            Open = open == 0 ? current : open;
            High = high == 0 ? current : high;
            Low = low == 0 ? current : low;
            Close = close == 0 ? current : close;
        }

        #endregion Public Constructors
    }
}