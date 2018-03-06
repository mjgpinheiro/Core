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

using Flurl.Http;
using Quantler.Api.Robinhood.Models;
using System;
using System.Threading.Tasks;

namespace Quantler.Api.Robinhood
{
    /// <summary>
    /// The public Robinhood Api
    /// </summary>
    /// <seealso cref="Quantler.Api.ApiClient" />
    public class RobinhoodApi : ApiClient
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RobinhoodApi"/> class.
        /// </summary>
        public RobinhoodApi() =>
            Endpoint = "https://api.robinhood.com/";

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Gets the instruments available on Robinhood, paginated.
        /// </summary>
        /// <param name="cursor">The cursor.</param>
        /// <returns></returns>
        public async Task<PaginationModel<InstrumentModel>> GetInstrumentsAsync(string cursor = "") =>
            await ($"{Endpoint}instruments/" +
                   (!string.IsNullOrWhiteSpace(cursor) ? $"?cursor={cursor}" : String.Empty))
                .GetJsonAsync<PaginationModel<InstrumentModel>>();

        #endregion Public Methods
    }
}