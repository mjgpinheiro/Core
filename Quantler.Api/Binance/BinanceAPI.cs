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
using Quantler.Api.Binance.Models;

namespace Quantler.Api.Binance
{
    /// <summary>
    /// Binance api
    /// https://www.binance.com/restapipub.html
    /// </summary>
    public class BinanceApi : ApiClient
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BinanceApi"/> class.
        /// </summary>
        public BinanceApi() =>
            Endpoint = @"https://api.binance.com";

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Gets the markets.
        /// </summary>
        /// <returns></returns>
        public async Task<TickerModel[]> GetTickerSymbolsAsync() =>
            await ExecuteRequestAsync<TickerModel[]>(Endpoint, $"/api/v1/ticker/allBookTickers");

        #endregion Public Methods
    }
}