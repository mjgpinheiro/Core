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

namespace Quantler.DataFeeds.IEXPublic.Models
{
    /// <summary>
    /// IEX retrieved tick model
    /// </summary>
    internal class TickModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the ask price.
        /// </summary>
        public decimal askPrice { get; set; }

        /// <summary>
        /// Gets or sets the size of the ask.
        /// </summary>
        public long askSize { get; set; }

        /// <summary>
        /// Gets or sets the bid price.
        /// </summary>
        public decimal bidPrice { get; set; }

        /// <summary>
        /// Gets or sets the size of the bid.
        /// </summary>
        public long bidSize { get; set; }

        /// <summary>
        /// Gets or sets the last sale price.
        /// </summary>
        public decimal lastSalePrice { get; set; }

        /// <summary>
        /// Gets or sets the last size of the sale.
        /// </summary>
        public long lastSaleSize { get; set; }

        /// <summary>
        /// Gets or sets the last updated.
        /// </summary>
        public long lastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the symbol.
        /// </summary>
        public string symbol { get; set; }

        #endregion Public Properties
    }
}