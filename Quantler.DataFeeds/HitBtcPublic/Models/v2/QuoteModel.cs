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
using Jil;

namespace Quantler.DataFeeds.HitBtcPublic.Models.v2
{
    /// <summary>
    /// JilDirective
    /// </summary>
    public class QuoteModel
    {
        #region Public Properties

        [JilDirective("price")]
        public string Price { get; set; }

        [JilDirective("size")]
        public string Size { get; set; }

        /// <summary>
        /// Gets the parsed price.
        /// </summary>
        public decimal DecimalPrice => decimal.Parse(Price, NumberStyles.Any, new CultureInfo("en-US"));

        /// <summary>
        /// Gets the parsed price.
        /// </summary>
        public double DoublePrice => double.Parse(Price, NumberStyles.Any, new CultureInfo("en-US"));

        /// <summary>
        /// Gets the parsed size.
        /// </summary>
        public double DoubleSize => double.Parse(Size, NumberStyles.Any, new CultureInfo("en-US"));

        /// <summary>
        /// Gets the parsed size.
        /// </summary>
        public decimal DecimalSize => decimal.Parse(Size, NumberStyles.Any, new CultureInfo("en-US"));

        #endregion Public Properties
    }
}