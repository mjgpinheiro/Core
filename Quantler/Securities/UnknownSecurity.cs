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

namespace Quantler.Securities
{
    /// <summary>
    /// Unknown security type
    /// </summary>
    public class UnknownSecurity : SecurityBase
    {
        #region Public Constructors

        /// <summary>
        /// Create unknown security type, cannot be used anywhere
        /// </summary>
        /// <param name="ticker"></param>
        public UnknownSecurity(string ticker)
            : base(TickerSymbol.NIL(ticker), null, SecurityDetails.NIL(), SecurityType.NIL, null)
        {

        }

        #endregion Public Constructors
    }
}