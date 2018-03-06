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

using System.Threading.Tasks;
using Quantler.Api.Bittrex.Models;

namespace Quantler.Api.Bittrex
{
    /// <summary>
    /// Bittrex api
    /// https://bittrex.com/home/api
    /// </summary>
    public class BittrexApi : ApiClient
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BittrexApi"/> class.
        /// </summary>
        public BittrexApi() =>
            Endpoint = @"https://bittrex.com/api/v1.1/";

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Gets the markets.
        /// </summary>
        /// <returns></returns>
        public async Task<RestResultModel<MarketInfoModel>> GetMarketsAsync() =>
            await ExecuteRequestAsync<RestResultModel<MarketInfoModel>>(Endpoint, $"public/getmarkets");

        #endregion Public Methods
    }
}