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

using Quantler.Api.IEX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Quantler.Api.IEX
{
    /// <summary>
    /// IEX API
    /// https://iextrading.com/developer/docs/#iex-api-1-0
    /// </summary>
    public class IEXApi : ApiClient
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IEXApi"/> class.
        /// </summary>
        public IEXApi() =>
            Endpoint = @"https://api.iextrading.com/1.0/";

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// IEX API returned date time format
        /// </summary>
        public static string DateTimeFormat => "yyyy-MM-ddTHH:mm:sszzz";

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Gets the chart data from the IEX API
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="range">The range.</param>
        /// <returns></returns>
        public async Task<IEnumerable<ChartModel>> GetChartAsync(string symbol, ChartRange range) =>
            await ExecuteRequestAsync<IEnumerable<ChartModel>>(Endpoint, $"stock/{symbol}/chart/{GetEnumValue(range)}");

        /// <summary>
        /// This call returns a company instance information for the given symbol
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns></returns>
        public async Task<CompanyModel> GetCompanyAsync(string symbol) =>
            await ExecuteRequestAsync<CompanyModel>(Endpoint, $"stock/{symbol}/company");

        /// <summary>
        /// Gets the historical dividends.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="range">The range.</param>
        /// <returns></returns>
        public async Task<DividendsModel[]> GetDividendsAsync(string symbol, ChartRange range) =>
            await ExecuteRequestAsync<DividendsModel[]>(Endpoint, $"stock/{symbol}/dividends/{GetEnumValue(range)}");

        /// <summary>
        /// Pulls data from the four most recent reported quarters.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns></returns>
        public async Task<EarningsModel> GetEarningsAsync(string symbol) =>
            await ExecuteRequestAsync<EarningsModel>(Endpoint, $"stock/{symbol}/earnings");

        /// <summary>
        /// Pulls income statement, balance sheet, and cash flow data from the four most recent reported quarters.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns></returns>
        public async Task<FinancialsModel> GetFinancialsAsync(string symbol) =>
            await ExecuteRequestAsync<FinancialsModel>(Endpoint, $"stock/{symbol}/financials");

        /// <summary>
        /// HIST will provide the output of IEX data products for download on a T+1 basis. Data will remain available for the trailing twelve months.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<HISTModel[]> GetHISTAsync(int date) =>
            await ExecuteRequestAsync<HISTModel[]>(Endpoint, $"hist?date={date}");

        /// <summary>
        /// This call returns key statistics for a company
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns></returns>
        public async Task<Dictionary<string, object>> GetKeyStatsAsync(string symbol) =>
            await ExecuteRequestAsync<Dictionary<string, object>>(Endpoint, $"stock/{symbol}/stats");

        /// <summary>
        /// This call returns the latest about a company
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="amount">Number of items to retrieve</param>
        /// <returns></returns>
        public async Task<NewsModel[]> GetNewsAsync(string symbol, int amount) =>
            await ExecuteRequestAsync<NewsModel[]>(Endpoint, $"stock/{symbol}/news/last/{amount}");

        /// <summary>
        /// Returns the official open and close for a give symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns></returns>
        public async Task<OpenCloseModel> GetOpenCloseAsync(string symbol) =>
            await ExecuteRequestAsync<OpenCloseModel>(Endpoint, $"stock/{symbol}/open-close");

        /// <summary>
        /// This returns previous day adjusted price data for a single stock, or an object keyed by symbol of price data for the whole market.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns></returns>
        public async Task<PreviousModel> GetPreviousAsync(string symbol) =>
            await ExecuteRequestAsync<PreviousModel>(Endpoint, $"stock/{symbol}/previous");

        /// <summary>
        /// This call returns a quote instance information for the given symbol
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns></returns>
        public async Task<QuoteModel> GetQuoteAsync(string symbol) =>
            await ExecuteRequestAsync<QuoteModel>(Endpoint, $"stock/{symbol}/quote");

        /// <summary>
        /// Gets the historical stock splits.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="range">The range.</param>
        /// <returns></returns>
        public async Task<SplitsModel[]> GetSplitsAsync(string symbol, ChartRange range) =>
            await ExecuteRequestAsync<SplitsModel[]>(Endpoint, $"stock/{symbol}/splits/{GetEnumValue(range)}");

        /// <summary>
        /// This call returns an array of symbols IEX supports for trading. This list is updated daily as of 7:45 a.m. ET. Symbols may be added or removed by IEX after the list was produced.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<SymbolModel>> GetSymbolsAsync() =>
            await ExecuteRequestAsync<IEnumerable<SymbolModel>>(Endpoint, "ref-data/symbols");

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Gets the enum value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private string GetEnumValue(Enum value) =>
            value.GetType()
                 .GetField(value.ToString())
                 .GetCustomAttributes<EnumValue>()
                 .Select(x => x.Value)
                 .FirstOrDefault();

        #endregion Private Methods
    }
}