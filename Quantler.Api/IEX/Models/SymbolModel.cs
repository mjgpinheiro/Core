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

using Jil;

namespace Quantler.Api.IEX.Models
{
    /// <summary>
    /// /ref-data/symbols
    /// </summary>
    public class SymbolModel
    {
        #region Public Properties

        /// <summary>
        /// refers to the symbol represented in Nasdaq Integrated symbology (INET).
        /// </summary>
        [JilDirective(Name = "date")]
        public string Date { get; set; }

        /// <summary>
        /// refers to the name of the company or security.
        /// </summary>
        [JilDirective(Name = "isEnabled")]
        public bool IsEnabled { get; set; }

        /// <summary>
        /// refers to the date the symbol reference data was generated.
        /// </summary>
        [JilDirective(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// will be true if the symbol is enabled for trading on IEX.
        /// </summary>
        [JilDirective(Name = "symbol")]
        public string Symbol { get; set; }

        #endregion Public Properties
    }
}