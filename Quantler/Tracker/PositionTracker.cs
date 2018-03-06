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

using Quantler.Account;
using Quantler.Orders;
using Quantler.Performance;
using Quantler.Securities;
using Quantler.Trades;
using System.Collections.Generic;
using System.Linq;

namespace Quantler.Tracker
{
    /// <summary>
    /// easily trade positions for a collection of securities. automatically builds positions from new fills.
    /// </summary>
    public class PositionTracker : GenericTracker<Position>, IEnumerable<Position>
    {
        #region Private Fields

        /// <summary>
        /// Fund id, in case this tracker is used in association with a quant fund
        /// </summary>
        private readonly string _fundId;

        /// <summary>
        /// Known universe, if applicable
        /// </summary>
        private TickerSymbol[] _universe = new TickerSymbol[0];

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initialize position tracker
        /// </summary>
        /// <param name="account"></param>
        /// <param name="fundid"></param>
        /// <param name="benchmark"></param>
        /// <param name="initialcapital"></param>
        public PositionTracker(BrokerAccount account, string fundid = "", Benchmark benchmark = null, decimal initialcapital = 0m)
        {
            Account = account;
            _fundId = fundid;

            Result = new Result(initialcapital, benchmark);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Current performance results
        /// </summary>
        public Result Result
        {
            get;
        }

        /// <summary>
        /// Total amount of fills occurred
        /// </summary>
        public int TotalFillCount
        {
            private set;
            get;
        }

        /// <summary>
        /// Total amount of fill value (in account currency)
        /// </summary>
        public decimal TotalFillValue
        {
            private set;
            get;
        }

        #endregion Public Properties

        #region Private Properties

        /// <summary>
        /// Associated account
        /// </summary>
        private BrokerAccount Account
        {
            get;
        }

        #endregion Private Properties

        #region Public Indexers

        /// <summary>
        /// Return position based on security
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        public Position this[Security security]
        {
            get
            {
                int idx = Getindex(security.Ticker.Name);
                if (idx < 0)
                {
                    //Check for universe if position is applicable
                    if (_universe.Length > 0 && _universe.All(x => x.Name != security.Ticker.Name))
                        return new Position(_fundId, Account.DisplayCurrency, Account.Currency, security);

                    //Add otherwise
                    var sec = Account.Securities[security.Ticker];
                    var toadd = new Position(_fundId, Account.DisplayCurrency, Account.Currency, security);
                    Addindex(sec.Ticker.Name, toadd);
                    return toadd;
                }
                return this[idx];
            }
        }

        #endregion Public Indexers

        #region Public Methods

        /// <summary>
        /// Get all current positions
        /// </summary>
        /// <returns></returns>
        IEnumerator<Position> IEnumerable<Position>.GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return this[i];
        }

        /// <summary>
        /// Sets the known universe to track position information.
        /// </summary>
        /// <param name="tickerSymbols">The ticker symbols.</param>
        public void SetUniverse(TickerSymbol[] tickerSymbols)
        {
            if (_universe.Length == 0)
                _universe = tickerSymbols;
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Add fill to position tracker
        /// </summary>
        /// <param name="fill"></param>
        internal void Adjust(Fill fill)
        {
            //Add to known fills
            TotalFillCount++;
            TotalFillValue += fill.Security.ConvertValue(fill.FillValue, Account.Currency);

            //Get position
            var pos = this[fill.Security];

            //Set new fill
            var trade = pos.Adjust(fill);

            //Set results (in case the fill triggered a trade)
            if (trade != null)
                Result.Update(trade);
        }

        /// <summary>
        /// Update results
        /// </summary>
        internal void UpdateResult() => Result.Update(this);

        #endregion Internal Methods
    }
}