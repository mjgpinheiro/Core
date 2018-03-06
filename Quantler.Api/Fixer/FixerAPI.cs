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

using System;
using Quantler.Configuration.Model;
using Quantler.Interfaces;
using System.Threading.Tasks;

namespace Quantler.Api.Fixer
{
    /// <summary>
    /// Fixer API (http://fixer.io/)
    /// Used for retrieving current price of fiat currencies in different base currencies
    /// </summary>
    public class FixerApi : ApiClient
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FixerApi"/> class.
        /// </summary>
        public FixerApi() =>
            Endpoint = @"http://api.fixer.io";

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Get current currency rates from source
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public async Task<CurrencyRatesConfig> GetLatestConversionRatesAsync(CurrencyType currency) =>
            await ExecuteRequestAsync<CurrencyRatesConfig>(Endpoint, $"latest?base={currency.ToString()}");

        /// <summary>
        /// Get currency rates for a specific date from source
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public async Task<CurrencyRatesConfig> GetDateConversionRatesAsync(CurrencyType currency, DateTime date) =>
            await ExecuteRequestAsync<CurrencyRatesConfig>(Endpoint, $"{date:yyyy-MM-dd}?base={currency.ToString()}");

        #endregion Public Methods
    }
}