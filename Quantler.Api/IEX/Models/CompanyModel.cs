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
    /// Company information model
    /// </summary>
    public class CompanyModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the ceo.
        /// </summary>
        [JilDirective(Name = "CEO")]
        public string CEO { get; set; }

        /// <summary>
        /// Gets or sets the name of the company.
        /// </summary>
        [JilDirective(Name = "companyName")]
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the company description.
        /// </summary>
        [JilDirective(Name = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the exchangeModel.
        /// </summary>
        [JilDirective(Name = "exchange")]
        public string Exchange { get; set; }

        /// <summary>
        /// Gets or sets the industry.
        /// </summary>
        [JilDirective(Name = "industry")]
        public string Industry { get; set; }

        /// <summary>
        /// Gets or sets the type of the issue.
        /// </summary>
        [JilDirective(Name = "issueType")]
        public string IssueType { get; set; }

        /// <summary>
        /// Gets or sets the sector.
        /// </summary>
        [JilDirective(Name = "sector")]
        public string Sector { get; set; }

        /// <summary>
        /// Gets or sets the symbol.
        /// </summary>
        [JilDirective(Name = "symbol")]
        public string Symbol { get; set; }

        /// <summary>
        /// Gets or sets the website.
        /// </summary>
        [JilDirective(Name = "website")]
        public string Website { get; set; }

        #endregion Public Properties
    }
}