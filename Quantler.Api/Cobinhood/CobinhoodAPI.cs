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

using Quantler.Api.Cobinhood.Models;
using System.Threading.Tasks;

namespace Quantler.Api.Cobinhood
{
    /// <summary>
    /// Cobinhood exchange API
    /// More info: https://cobinhood.github.io/api-public/#overview
    /// </summary>
    public class CobinhoodApi : ApiClient
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CobinhoodApi"/> class.
        /// </summary>
        public CobinhoodApi() =>
            Endpoint = @"https://api.cobinhood.com";

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Returns info for all currencies available for trade
        /// </summary>
        /// <returns></returns>
        public async Task<StatusModel> GetAllCurrencies() =>
            await ExecuteRequestAsync<StatusModel>(Endpoint, "/v1/market/currencies");

        /// <summary>
        /// Get info for all trading pairs
        /// </summary>
        /// <returns></returns>
        public async Task<StatusModel> GetAllTradingPairs() =>
            await ExecuteRequestAsync<StatusModel>(Endpoint, "/v1/market/trading_pairs");

        /// <summary>
        /// Get order book for the trading pair containing all asks/bids
        /// </summary>
        /// <param name="tradingpairid">The tradingpairid.</param>
        /// <returns></returns>
        public async Task<StatusModel> GetOrderBook(string tradingpairid) =>
            await ExecuteRequestAsync<StatusModel>(Endpoint, $"/v1/market/orderbooks/{tradingpairid}");

        /// <summary>
        /// Returns most recent trades for the specified trading pair
        /// </summary>
        /// <param name="tradingpairid">The tradingpairid.</param>
        /// <returns></returns>
        public async Task<StatusModel> GetRecentTrades(string tradingpairid) =>
            await ExecuteRequestAsync<StatusModel>(Endpoint, $"/v1/market/trades/{tradingpairid}");

        /// <summary>
        /// Returns ticker for specified trading pair
        /// </summary>
        /// <param name="tradingpairid">The tradingpairid.</param>
        /// <returns></returns>
        public async Task<StatusModel> GetTicker(string tradingpairid) =>
            await ExecuteRequestAsync<StatusModel>(Endpoint, $"/v1/market/tickers/{tradingpairid}");

        #endregion Public Methods
    }
}