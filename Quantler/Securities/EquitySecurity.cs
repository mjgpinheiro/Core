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

using Quantler.Exchanges;
using Quantler.Interfaces;

namespace Quantler.Securities
{
    /// <summary>
    /// Equity Security Implementation
    /// </summary>
    public class EquitySecurity : SecurityBase
    {
        #region Public Constructors

        /// <summary>
        /// Create new Equity Security information
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="exchangeModel"></param>
        /// <param name="details"></param>
        /// <param name="conversion"></param>
        public EquitySecurity(TickerSymbol ticker, ExchangeModel exchangeModel, SecurityDetails details, Currency conversion)
            : base(ticker, exchangeModel, details, SecurityType.Equity, conversion)
        {
        }

        #endregion Public Constructors
    }
}