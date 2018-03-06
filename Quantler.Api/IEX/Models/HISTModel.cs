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
    /// /hist?date=20170515
    /// </summary>
    public class HISTModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        [JilDirective(Name = "date")]
        public string Date { get; set; }

        /// <summary>
        /// Gets or sets the feed (DEEP/TOPS).
        /// </summary>
        [JilDirective(Name = "feed")]
        public string Feed { get; set; }

        /// <summary>
        /// Gets or sets the link.
        /// </summary>
        [JilDirective(Name = "link")]
        public string Link { get; set; }

        /// <summary>
        /// Gets or sets the protocol.
        /// </summary>
        [JilDirective(Name = "protocol")]
        public string Protocol { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        [JilDirective(Name = "size")]
        public string Size { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        [JilDirective(Name = "version")]
        public string Version { get; set; }

        #endregion Public Properties
    }
}