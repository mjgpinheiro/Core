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

using Quantler.Interfaces;
using Quantler.Tracker;
using System.Linq;

namespace Quantler.Account.Cash
{
    /// <summary>
    /// Calculated funds, based on known cash
    /// </summary>
    public class CalculatedFunds
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatedFunds"/> class.
        /// </summary>
        /// <param name="cashmanager">The cashmanager.</param>
        /// <param name="positiontracker">The positiontracker.</param>
        /// <param name="leverage">The leverage.</param>
        /// <param name="isMarkedDayTrading"></param>
        /// <param name="dayTradingTradesLeft"></param>
        public CalculatedFunds(CashManager cashmanager, PositionTracker positiontracker, int leverage, bool isMarkedDayTrading, int dayTradingTradesLeft)
        {
            Leverage = leverage;
            UnsettledCash = cashmanager.TotalUnsettledCash;
            MarginInUse = positiontracker.Sum(x => x.MarginInUse);
            BaseCurrency = cashmanager.BaseCurrency;
            Balance = cashmanager.TotalCash;
            Equity = Balance + positiontracker.Sum(x => x.NetProfit);
            IsMarkedDayTrading = isMarkedDayTrading;
            DayTradingTradesLeft = dayTradingTradesLeft;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatedFunds"/> class.
        /// </summary>
        /// <param name="unsettledCash">The unsettled funds.</param>
        /// <param name="balance">The balance.</param>
        /// <param name="basecurrency">The basecurrency.</param>
        /// <param name="positiontracker">The positiontracker.</param>
        /// <param name="leverage">The leverage.</param>
        /// <param name="isMarkedDayTrading"></param>
        /// <param name="dayTradingTradesLeft"></param>
        public CalculatedFunds(decimal unsettledCash, decimal balance, CurrencyType basecurrency,
            PositionTracker positiontracker, int leverage, bool isMarkedDayTrading, int dayTradingTradesLeft)
        {
            Leverage = leverage;
            UnsettledCash = unsettledCash;
            MarginInUse = positiontracker.Sum(x => x.MarginInUse);
            BaseCurrency = basecurrency;
            Balance = balance;
            Equity = Balance + positiontracker.Sum(x => x.NetProfit);
            IsMarkedDayTrading = isMarkedDayTrading;
            DayTradingTradesLeft = dayTradingTradesLeft;
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="CalculatedFunds"/> class from being created.
        /// </summary>
        private CalculatedFunds()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Current balance
        /// </summary>
        public decimal Balance { get; private set; }

        /// <summary>
        /// Default account currency
        /// </summary>
        public CurrencyType BaseCurrency { get; private set; }

        /// <summary>
        /// Current amount of buying power, taking margin into account
        /// </summary>
        public decimal BuyingPower => FreeMargin * Leverage;

        /// <summary>
        /// Gets the day trading trades left before this account is marked as day trading.
        /// </summary>
        public int DayTradingTradesLeft { get; private set; }

        /// <summary>
        /// Total amount of equity
        /// </summary>
        public decimal Equity { get; private set; }

        /// <summary>
        /// Total floating pnl
        /// </summary>
        public decimal FloatingPnl => Equity - Balance;

        /// <summary>
        /// Free margin for use
        /// </summary>
        public decimal FreeMargin => SettledCash - MarginInUse;

        /// <summary>
        /// Gets a value indicating whether this instance is marked day trading.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is marked day trading; otherwise, <c>false</c>.
        /// </value>
        public bool IsMarkedDayTrading { get; private set; }

        /// <summary>
        /// Account wide leverage
        /// </summary>
        public int Leverage { get; private set; }

        /// <summary>
        /// Current amount of margin use
        /// </summary>
        public decimal MarginInUse { get; private set; }

        /// <summary>
        /// Current margin level
        /// </summary>
        public decimal MarginLevel => (Equity / MarginInUse) * 100;

        /// <summary>
        /// Gets the settled funds.
        /// </summary>
        public decimal SettledCash => Balance - UnsettledCash;

        /// <summary>
        /// Total amount of unsettled funds
        /// </summary>
        public decimal UnsettledCash { get; private set; }

        #endregion Public Properties

        #region Internal Methods

        /// <summary>
        /// Converts the objects current currency used, to a new currency target.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <param name="targetcurrency">The target currency.</param>
        /// <returns></returns>
        internal CalculatedFunds ConvertCurrency(Currency currency, CurrencyType targetcurrency)
        {
            return new CalculatedFunds
            {
                BaseCurrency = targetcurrency,
                Balance = currency.Convert(Balance, BaseCurrency, targetcurrency),
                Equity = currency.Convert(Equity, BaseCurrency, targetcurrency),
                Leverage = Leverage,
                MarginInUse = currency.Convert(MarginInUse, BaseCurrency, targetcurrency),
                DayTradingTradesLeft = DayTradingTradesLeft,
                IsMarkedDayTrading = IsMarkedDayTrading,
                UnsettledCash = currency.Convert(UnsettledCash, BaseCurrency, targetcurrency)
            };
        }

        #endregion Public Methods
    }
}