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

using Quantler.Account.Cash;
using Quantler.Data;
using Quantler.Interfaces;
using Quantler.Tracker;
using System.Collections.Generic;

namespace Quantler.Account
{
    /// <summary>
    /// Account information and reference for current states (Simulated account, used for backtesting and papertrading)
    /// </summary>
    public class BrokerAccount
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BrokerAccount"/> class.
        /// </summary>
        /// <param name="accountid">The accountid.</param>
        /// <param name="cashmanager">The cashmanager.</param>
        /// <param name="securitytracker">The securitytracker.</param>
        /// <param name="accountcurrency">The accountcurrency.</param>
        /// <param name="leverage">The leverage.</param>
        /// <param name="displaycurrency">The display currency.</param>
        public BrokerAccount(
            string accountid,
            CashManager cashmanager,
            SecurityTracker securitytracker,
            CurrencyType accountcurrency,
            CurrencyType displaycurrency,
            int leverage)
        {
            Cash = cashmanager;
            Securities = securitytracker;
            Positions = new PositionTracker(this);
            Currency = accountcurrency;
            Leverage = leverage;
            Id = accountid;
            DisplayCurrency = displaycurrency;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Current balance
        /// </summary>
        public decimal Balance => Values?.Balance ?? 0;

        /// <summary>
        /// Current amount of buying power, taking margin into account
        /// </summary>
        public decimal BuyingPower => Values?.BuyingPower ?? 0;

        /// <summary>
        /// CashManager
        /// </summary>
        public CashManager Cash { get; }

        /// <summary>
        /// Base account currency
        /// </summary>
        public CurrencyType Currency { get; }

        /// <summary>
        /// Gets the display currency.
        /// </summary>
        public CurrencyType DisplayCurrency { get; }

        /// <summary>
        /// Total amount of equity
        /// </summary>
        public decimal Equity => Values?.Equity ?? 0;

        /// <summary>
        /// Total floating pnl
        /// </summary>
        public decimal FloatingPnl => Values?.FloatingPnl ?? 0;

        /// <summary>
        /// Free margin for use
        /// </summary>
        public decimal FreeMargin => Values?.FreeMargin ?? 0;

        /// <summary>
        /// Account id
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Account wide leverage
        /// </summary>
        public int Leverage { get; }

        /// <summary>
        /// Level at which margin calls will be thrown
        /// TODO: set value from somewhere?
        /// </summary>
        public decimal MarginCallLevel { get; private set; }

        /// <summary>
        /// Current amount of margin use
        /// </summary>
        public decimal MarginInUse => Values?.MarginInUse ?? 0;

        /// <summary>
        /// Current margin level
        /// </summary>
        public decimal MarginLevel => Values?.MarginLevel ?? 0;

        /// <summary>
        /// Local positions tracker
        /// </summary>
        public PositionTracker Positions { get; }

        /// <summary>
        /// Security tracker
        /// </summary>
        public SecurityTracker Securities { get; }

        /// <summary>
        /// Type of account
        /// </summary>
        public AccountType Type => Leverage > 1 ? AccountType.Margin : AccountType.Cash;

        /// <summary>
        /// Total amount of unsettled cash
        /// </summary>
        public decimal UnsettledCash => Values?.UnsettledCash ?? 0;

        /// <summary>
        /// Gets the last calculated values.
        /// </summary>
        public CalculatedFunds Values => Cash.GetCalculatedFunds(this);

        #endregion Public Properties

        #region Internal Properties

        /// <summary>
        /// Gets or sets the day trading orders left before the patter day trading indication has been hit.
        /// </summary>
        internal int DayTradingOrdersLeft { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [pattern day trading hit].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [pattern day trading hit]; otherwise, <c>false</c>.
        /// </value>
        internal bool PatternDayTradingHit { get; set; }

        #endregion Internal Properties

        #region Public Methods

        /// <summary>
        /// Update security information in account
        /// TODO: add in the flow
        /// </summary>
        /// <param name="datapoints"></param>
        public void OnData(IEnumerable<DataPoint> datapoints)
        {
            foreach (var datapoint in datapoints)
            {
                //Update current pricing
                var security = Securities[datapoint.Ticker];
                security.UpdatePrice(datapoint);
            }
        }

        #endregion Public Methods
    }
}