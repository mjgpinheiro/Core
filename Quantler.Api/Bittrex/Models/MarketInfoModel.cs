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

namespace Quantler.Api.Bittrex.Models
{
    /// <summary>
    /// Market info
    /// </summary>
    public class MarketInfoModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the base currency.
        /// </summary>
        public string BaseCurrency { get; set; }

        /// <summary>
        /// Gets or sets the base currency long.
        /// </summary>
        public string BaseCurrencyLong { get; set; }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        public string Created { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the is sponsored.
        /// </summary>
        public bool? IsSponsored { get; set; }

        /// <summary>
        /// Gets or sets the logo URL.
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// Gets or sets the market currency.
        /// </summary>
        public string MarketCurrency { get; set; }

        /// <summary>
        /// Gets or sets the market currency long.
        /// </summary>
        public string MarketCurrencyLong { get; set; }

        /// <summary>
        /// Gets or sets the name of the market.
        /// </summary>
        public string MarketName { get; set; }

        /// <summary>
        /// Gets or sets the minimum size of the trade.
        /// </summary>
        public double MinTradeSize { get; set; }

        /// <summary>
        /// Gets or sets the notice.
        /// </summary>
        public string Notice { get; set; }

        #endregion Public Properties
    }
}