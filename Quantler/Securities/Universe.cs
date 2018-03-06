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

using MoreLinq;
using Quantler.Exchanges;
using Quantler.Tracker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace Quantler.Securities
{
    /// <summary>
    /// Universe collection of securities and their respective weights
    /// TODO: change to universe module and make use of static universe property in config file (so we have a static universe and regular universe modules)
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IEnumerable{Security}" />
    public class Universe : IEnumerable<Security>
    {
        #region Private Fields

        /// <summary>
        /// The securities
        /// </summary>
        private readonly Dictionary<Security, decimal> _securities;

        /// <summary>
        /// Current logging instance
        /// </summary>
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        #endregion Private Fields

        #region Private Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Universe"/> class.
        /// </summary>
        /// <param name="securities">The securities.</param>
        private Universe(Dictionary<Security, decimal> securities)
        {
            _securities = securities;
            Exchanges = _securities.Select(x => x.Key.Exchange).DistinctBy(x => x.Name).ToArray();
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets the exchanges.
        /// </summary>
        public ExchangeModel[] Exchanges { get; }

        /// <summary>
        /// Gets or sets the name of this universe.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Securities attached tho this universe
        /// </summary>
        public Security[] Securities => _securities.Keys.ToArray();

        #endregion Public Properties

        #region Public Indexers

        /// <summary>
        /// Get security based on ticker name
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        public Security this[TickerSymbol ticker] =>
            GetSecurities(x => x.Ticker.Name == ticker.Name).FirstOrDefault();

        #endregion Public Indexers

        #region Public Methods

        /// <summary>
        /// Creates the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="securitytracker">The securitytracker.</param>
        /// <param name="tickerweights">The tickerweights.</param>
        /// <returns></returns>
        public static Universe Create(string name, SecurityTracker securitytracker, Dictionary<string, decimal> tickerweights)
        {
            //Check if all securities are loaded
            Dictionary<Security, decimal> loadedsecurities = new Dictionary<Security, decimal>();
            foreach (var ticker in tickerweights.Keys)
            {
                var security = securitytracker[ticker];
                if (security is UnknownSecurity)
                    Log.Warn($"Security with ticker name {ticker} is unknown for the current broker model, it will not be used in universe with name {name}");
                else
                    loadedsecurities[security] = tickerweights[ticker];
            }

            //Create new universe object
            var output = new Universe(loadedsecurities)
            {
                Name = name
            };

            //Return result
            return output;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Security> GetEnumerator() =>
                    _securities.Keys.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() =>
                    _securities.Keys.GetEnumerator();

        /// <summary>
        /// Gets the securities by exchangeModel.
        /// </summary>
        /// <param name="exchangeModel">The exchangeModel.</param>
        /// <returns></returns>
        public Security[] GetSecurities(ExchangeModel exchangeModel) =>
            _securities.Where(x => x.Key.Exchange.Name == exchangeModel.Name).Select(x => x.Key).ToArray();

        /// <summary>
        /// Gets the securities by security type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public Security[] GetSecurities(SecurityType type) =>
            _securities.Where(x => x.Key.Type == type).Select(x => x.Key).ToArray();

        /// <summary>
        /// Gets the securities based on a search function.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <returns></returns>
        public Security[] GetSecurities(Func<Security, bool> search) =>
            _securities.Keys.Where(search).ToArray();

        /// <summary>
        /// Returns the weight of this security set for this universe
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        public decimal GetWeight(Security security) =>
            _securities.ContainsKey(security) ? _securities[security] : 0m;

        /// <summary>
        /// Returns the weight of this security set for this universe
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        public decimal GetWeight(TickerSymbol ticker) =>
            _securities.Select(x => x.Key.Ticker).Contains(ticker)
                ? _securities.First(x => x.Key.Ticker.Name == ticker.Name).Value
                : 0m;

        #endregion Public Methods
    }
}