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
using System;
using System.Globalization;

namespace Quantler.Api.IEX.Models
{
    /// <summary>
    /// IEX company news update
    /// </summary>
    public class NewsModel
    {
        #region Public Properties

        /// <summary>
        /// Gets the date time this news event occurred
        /// </summary>
        [JilDirective(true)]
        public DateTime DateTimeUtc => DateTimeOffset.TryParseExact(date, IEXApi.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset parsedDateTime) ?
            parsedDateTime.UtcDateTime : throw new Exception($"Could not parse date {date}");

        /// <summary>
        /// Gets or sets the date time this news event occurred
        /// </summary>
        [JilDirective(Name = "datetime")]
        public string date { get; set; }

        /// <summary>
        /// Gets or sets the headline.
        /// </summary>
        [JilDirective(Name = "headline")]
        public string Headline { get; set; }

        /// <summary>
        /// Gets or sets the related symbol tickers.
        /// </summary>
        [JilDirective(Name = "related")]
        public string Related { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        [JilDirective(Name = "source")]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        [JilDirective(Name = "summary")]
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        [JilDirective(Name = "url")]
        public string Url { get; set; }

        #endregion Public Properties
    }
}