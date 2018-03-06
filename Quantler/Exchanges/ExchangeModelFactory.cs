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

using NLog;
using System;

namespace Quantler.Exchanges
{
    /// <summary>
    /// Exchange model factory
    /// </summary>
    public class ExchangeModelFactory
    {
        #region Private Fields

        /// <summary>
        /// Current logging instance
        /// </summary>
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Gets the exchange.
        /// </summary>
        /// <param name="clock">The clock.</param>
        /// <param name="exchangename">The exchangename.</param>
        /// <param name="extendedhours">if set to <c>true</c> [extendedhours].</param>
        /// <returns></returns>
        public static ExchangeModel GetExchange(WorldClock clock, string exchangename, bool extendedhours)
        {
            try
            {
                if (exchangename.ToLower() == "crypto")
                    return new CryptoExchangeModel(clock);
                else
                    return new EquityExchangeModel(exchangename, clock, extendedhours);
            }
            catch (Exception exc)
            {
                Log.Error(exc, $"Could not create exchange model {exchangename} due to exception");
                throw;
            }
        }

        #endregion Public Methods
    }
}