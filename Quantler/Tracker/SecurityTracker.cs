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

using Quantler.Securities;
using System.Collections.Generic;

namespace Quantler.Tracker
{
    /// <summary>
    /// Tracks and contains multiple securities, can be used as a single reference point for all securities used
    /// </summary>
    public class SecurityTracker : GenericTracker<Security>, IEnumerable<Security>
    {
        #region Private Fields

        /// <summary>
        /// Used for creating new securities
        /// </summary>
        private readonly SecurityFactory _securityFactory;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initialize a new securitytracker object
        /// </summary>
        /// <param name="factory"></param>
        public SecurityTracker(SecurityFactory factory) => _securityFactory = factory;

        #endregion Public Constructors

        #region Public Indexers

        /// <summary>
        /// Get the security object based on the symbol name
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        public new Security this[string ticker]
        {
            get
            {
                var idx = Getindex(ticker);
                if (idx >= 0) return this[idx];
                var security = _securityFactory.Create(ticker);
                AddSecurity(security);
                return security;
            }
        }

        /// <summary>
        /// Gets the <see cref="Security"/> with the specified ticker.
        /// </summary>
        /// <value>
        /// The <see cref="Security"/>.
        /// </value>
        /// <param name="ticker">The ticker.</param>
        /// <returns></returns>
        public Security this[TickerSymbol ticker] => this[ticker.Name];

        #endregion Public Indexers

        #region Private Indexers

        /// <summary>
        /// Get the security object based on the index location
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private new Security this[int idx] => idx < 0 ? _securityFactory.Create("UNKNOWN") : base[idx];

        #endregion Private Indexers

        #region Public Methods

        /// <summary>
        /// Add a new security to the security tracker
        /// </summary>
        /// <param name="security"></param>
        public void AddSecurity(Security security)
        {
            //Try and get the current index
            int idx = Getindex(security.Ticker.Name);

            //If index does not exist, add it
            if (idx < 0)
                Addindex(security.Ticker.Name, security);
        }

        /// <summary>
        /// Get object Enumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator<Security> IEnumerable<Security>.GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return this[i];
        }

        #endregion Public Methods
    }
}