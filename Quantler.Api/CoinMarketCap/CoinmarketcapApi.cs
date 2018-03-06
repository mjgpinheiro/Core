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
using Quantler.Api.CoinMarketCap.Models;
using Quantler.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quantler.Api.CoinMarketCap
{
    /// <summary>
    /// CoinMarketCap API (https://coinmarketcap.com/api/)
    /// Used for retrieving current price of crypto currencies in different base currencies
    /// </summary>
    public class CoinmarketcapApi : ApiClient
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CoinmarketcapApi"/> class.
        /// </summary>
        public CoinmarketcapApi() =>
            Endpoint = @"https://api.coinmarketcap.com/v1/";

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Get current currency rates from source
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <param name="currency">The currency.</param>
        /// <returns></returns>
        public async Task< Dictionary<string, string>> GetTickerAsync(string ticker, CurrencyType currency) =>
            JSON.Deserialize<IEnumerable<Dictionary<string, string>>>(await ExecuteRequestAsync(Endpoint, $"ticker/{ticker}?convert={currency}")).FirstOrDefault();

        /// <summary>
        /// Gets all the tickers and their associated information
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<TickerModel>> GetTickersAsync() =>
            JSON.Deserialize<TickerModel[]>(await ExecuteRequestAsync(Endpoint, $"ticker"));

        #endregion Public Methods
    }
}