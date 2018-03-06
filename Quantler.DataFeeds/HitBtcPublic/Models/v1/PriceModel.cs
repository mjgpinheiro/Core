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

using System.Globalization;

namespace Quantler.DataFeeds.HitBtcPublic.Models.v1
{
    /// <summary>
    /// Bid or ask prices from feed
    /// </summary>
    public class PriceModel
    {
        #region Public Properties

        /// <summary>
        /// Gets the parsed price.
        /// </summary>
        public decimal DecimalPrice => decimal.Parse(price, NumberStyles.Any, new CultureInfo("en-US"));

        /// <summary>
        /// Gets the parsed price.
        /// </summary>
        public double DoublePrice => double.Parse(price, NumberStyles.Any, new CultureInfo("en-US"));

        /// <summary>
        /// Current price (bid/ask/trade)
        /// </summary>
        public string price { get; set; }

        /// <summary>
        /// Current size (bid/ask/trade)
        /// </summary>
        public long size { get; set; }

        /// <summary>
        /// UTC timestamp, in milliseconds
        /// </summary>
        public long timestamp { get; set; }

        #endregion Public Properties
    }
}