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
using NLog;
using Quantler.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quantler.Account.Cash
{
    /// <summary>
    /// Allocated funds manager
    /// </summary>
    public class CashManager
    {
        #region Public Fields

        /// <summary>
        /// Base Account (funds that are not allocated to any quant fund)
        /// </summary>
        public const string BaseAccount = "Base.Account";

        #endregion Public Fields

        #region Private Fields

        /// <summary>
        /// Currently known cash positions for quant funds
        /// </summary>
        private readonly Dictionary<string, Dictionary<CurrencyType, CashPosition>> _cashknown = new Dictionary<string, Dictionary<CurrencyType, CashPosition>>();

        /// <summary>
        /// Currency Conversion logic
        /// </summary>
        private readonly Currency _conversion;

        /// <summary>
        /// The leverage
        /// </summary>
        private readonly int _leverage;

        /// <summary>
        /// Locker object
        /// </summary>
        private readonly object _locker = new object();

        /// <summary>
        /// Current logging
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Attached portfolio
        /// </summary>
        private BrokerAccount _brokeraccount;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CashManager"/> class.
        /// </summary>
        /// <param name="conversion">The conversion.</param>
        /// <param name="leverage">The leverage.</param>
        public CashManager(Currency conversion, int leverage)
        {
            //Set conversion
            _conversion = conversion;
            _leverage = leverage;

            //Set base account
            _cashknown.Add(BaseAccount, new Dictionary<CurrencyType, CashPosition>());
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Base currency for account denomination
        /// </summary>
        public CurrencyType BaseCurrency => _brokeraccount.Currency;

        /// <summary>
        /// Total cash allocated to quant funds
        /// </summary>
        public decimal TotalAllocatedCash => AllocatedPositions.Sum(x => _conversion.Convert(x.TotalCash, x.BaseCurrency, BaseCurrency));

        /// <summary>
        /// Total amount of cash allocated
        /// </summary>
        public decimal TotalCash => CashPositions.Sum(x => _conversion.Convert(x.TotalCash, x.BaseCurrency, BaseCurrency));

        /// <summary>
        /// Total settled cash
        /// </summary>
        public decimal TotalSettledCash => CashPositions.Sum(x => _conversion.Convert(x.TotalSettledCash, x.BaseCurrency, BaseCurrency));

        /// <summary>
        /// Total cash unallocated to quant funds
        /// </summary>
        public decimal TotalUnallocatedCash => (_brokeraccount.Equity - TotalAllocatedCash);

        /// <summary>
        /// Total unsettled cash
        /// </summary>
        public decimal TotalUnsettledCash => CashPositions.Sum(x => _conversion.Convert(x.TotalUnsettledCash, x.BaseCurrency, BaseCurrency));

        #endregion Public Properties

        #region Private Properties

        /// <summary>
        /// Gets all allocated positions.
        /// </summary>
        private IEnumerable<CashPosition> AllocatedPositions => _cashknown.Where(x => x.Key != BaseAccount).Select(x => x.Value).SelectMany(x => x.Values);

        /// <summary>
        /// Get all current cash positions
        /// </summary>
        private IEnumerable<CashPosition> CashPositions => _cashknown.Values.SelectMany(x => x.Values);

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Add cash
        /// </summary>
        /// <param name="quantfund"></param>
        /// <param name="currency"></param>
        /// <param name="cash"></param>
        /// <param name="settlementutc"></param>
        public void AddCash(CurrencyType currency, decimal cash, IQuantFund quantfund = null, DateTime? settlementutc = null)
        {
            //Get funds for quant fund
            CashPosition cashposition;
            if (quantfund == null)
                cashposition = GetBaseAccount(currency);
            else
            {
                //Check if we know this quant fund
                if (!_cashknown.ContainsKey(quantfund.FundId))
                    _cashknown[quantfund.FundId] = new Dictionary<CurrencyType, CashPosition>();

                //Check if we know this currency type
                if (!_cashknown[quantfund.FundId].TryGetValue(currency, out cashposition))
                {
                    cashposition = new CashPosition(currency, 0);
                    _cashknown[quantfund.FundId].Add(currency, cashposition);
                }
            }

            //Set items
            cashposition.AddCash(settlementutc.HasValue
                ? new UnsettledCash(cash, settlementutc.Value)
                : new SettledCash(cash));
        }

        /// <summary>
        /// Get current buying power for quant fund
        /// </summary>
        /// <param name="quantfund">Corresponding quant fund to request buying power for</param>
        /// <returns></returns>
        public decimal GetBuyingPower(IQuantFund quantfund) =>
            _cashknown[quantfund.FundId].Values.Sum(x => _conversion.Convert(x.TotalSettledCash, x.BaseCurrency, BaseCurrency)) * _brokeraccount.Leverage;

        /// <summary>
        /// Gets the calculated funds.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <returns></returns>
        public CalculatedFunds GetCalculatedFunds(BrokerAccount account) =>
            new CalculatedFunds(this, account.Positions, _leverage, account.PatternDayTradingHit, account.DayTradingOrdersLeft);

        /// <summary>
        /// Gets the calculated funds.
        /// </summary>
        /// <param name="quantfund">The quantfund.</param>
        /// <param name="account">Current account object</param>
        /// <returns></returns>
        public CalculatedFunds GetCalculatedFunds(IQuantFund quantfund, BrokerAccount account) =>
            new CalculatedFunds(GetUnsettledCash(quantfund), GetCash(quantfund), BaseCurrency, quantfund.Positions,
                _leverage, account.PatternDayTradingHit, account.DayTradingOrdersLeft);

        /// <summary>
        /// Get total amount of cash for quant fund
        /// </summary>
        /// <param name="quantfund"></param>
        /// <returns></returns>
        public decimal GetCash(IQuantFund quantfund) => _cashknown[quantfund.FundId].Values.Sum(x => _conversion.Convert(x.TotalCash, x.BaseCurrency, BaseCurrency));

        /// <summary>
        /// Gets the cash positions that are part of a quant fund.
        /// </summary>
        /// <param name="quantfund">The quantfund.</param>
        /// <returns></returns>
        public Dictionary<CurrencyType, CashPosition> GetCashPositions(IQuantFund quantfund) => _cashknown[quantfund.FundId];

        /// <summary>
        /// Gets the cash positions for the entire account.
        /// </summary>
        /// <returns></returns>
        public Dictionary<CurrencyType, CashPosition> GetCashPositions()
        {
            //Get each funds per currencytype
            Dictionary<CurrencyType, CashPosition> toreturn = new Dictionary<CurrencyType, CashPosition>();
            foreach (var funds in _cashknown)
                foreach (var holder in funds.Value)
                {
                    if (!toreturn.ContainsKey(holder.Key))
                        toreturn.Add(holder.Key, holder.Value);
                    else
                        toreturn[holder.Key].AddCash(new SettledCash(holder.Value.TotalCash));
                }

            //Return results
            return toreturn;
        }

        /// <summary>
        /// Get total amount of unsettled cash for quant fund
        /// </summary>
        /// <param name="quantfund"></param>
        /// <returns></returns>
        public decimal GetUnsettledCash(IQuantFund quantfund) => _cashknown[quantfund.FundId].Values.Sum(x => _conversion.Convert(x.TotalUnsettledCash, x.BaseCurrency, BaseCurrency));

        /// <summary>
        /// Update cash position based on account action
        /// TODO: add sync action to scheduler for live trading (get current funds from account via broker API) => will be done via tickethandler (it will contact the api)
        /// </summary>
        /// <param name="action"></param>
        /// <param name="currency"></param>
        /// <param name="amount"></param>
        /// <param name="quantfund">In case the cash position affects a specific quant fund</param>
        public void Process(AccountActionType action, CurrencyType currency, decimal amount, IQuantFund quantfund = null)
        {
            //Check action type and process
            if (action == AccountActionType.Sync)
                SyncFunds(currency, amount);
            else if (action == AccountActionType.Credit)
                throw new NotImplementedException();
            else if (action == AccountActionType.Deposit)
                throw new NotImplementedException();
            else if (action == AccountActionType.Dividend)
                throw new NotImplementedException();
            else if (action == AccountActionType.Withdrawal)
                throw new NotImplementedException();
            //TODO: What to do with currently set funds and fund withdrawals
        }

        /// <summary>
        /// Remove quant fund from following cash positions
        /// </summary>
        /// <param name="quantfund"></param>
        public void RemoveQuantFund(IQuantFund quantfund)
        {
            lock (_locker)
            {
                //Check if we know this quantfund
                if (!_cashknown.ContainsKey(quantfund.FundId))
                    return;

                //Remove balance from quant fund
                var cashholder = GetCashPositions(quantfund)[BaseCurrency];
                var baseholder = GetCashPositions()[BaseCurrency];

                //Move from cash holder and add to base
                baseholder.AddCash(new SettledCash(cashholder.TotalSettledCash));
                baseholder.AddCash(new UnsettledCash(cashholder.TotalUnsettledCash, cashholder.DateTimeAllIsSettledUtc));

                //Remove fund
                _cashknown.Remove(quantfund.FundId);
            }
        }

        /// <summary>
        /// Sets the broker account.
        /// </summary>
        /// <param name="account">The account.</param>
        public void SetBrokerAccount(BrokerAccount account) =>
            _brokeraccount = account;

        /// <summary>
        /// Set initial fund allocation to quant fund, can only be set once
        /// </summary>
        /// <param name="quantfund"></param>
        /// <param name="basecurrency"></param>
        /// <param name="cash"></param>
        public void AddQuantFund(IQuantFund quantfund, CurrencyType basecurrency, decimal cash)
        {
            lock (_locker)
            {

                //Can only be set once
                if (_cashknown.ContainsKey(quantfund.FundId))
                    return;

                //Check cash integrity
                cash = Math.Abs(cash);

                //Get base balance
                var baseposition = GetCashPositions()[basecurrency];

                //Check if we have enough funds
                if (baseposition.TotalCash < cash)
                    _log.Warn($"You are allocating more cash than the account has on cash available. Avalaible = {baseposition.TotalCash}, Allocate ({quantfund.FundId}) = {cash}");

                _cashknown.Add(quantfund.FundId,
                    new Dictionary<CurrencyType, CashPosition>
                    {
                        { basecurrency, new CashPosition(basecurrency, cash) }
                    });

                //Some logging
                _log.Info($"Allocated {cash} {basecurrency} amount to fund with name {quantfund.Name}");

                //Remove from base
                baseposition.AddCash(new SettledCash(-cash));
            }
        }

        /// <summary>
        /// Update any unsettled funds
        /// </summary>
        /// <param name="momentutc"></param>
        public void Update(DateTime momentutc) =>
                    CashPositions.ForEach(x => x.UpdateCash(momentutc));

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Gets the base account cash position
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <returns></returns>
        private CashPosition GetBaseAccount(CurrencyType currency)
        {
            //See if this currency is known
            if (!_cashknown[BaseAccount].TryGetValue(currency, out CashPosition value))
            {
                value = new CashPosition(currency, 0);
                _cashknown[BaseAccount].Add(currency, value);
            }

            //Return value
            return value;
        }

        /// <summary>
        /// Synchronizes the cash positions compared to current account.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <param name="amount">The amount.</param>
        private void SyncFunds(CurrencyType currency, decimal amount)
        {
            lock (_locker)
            {
                //Get all holders
                decimal baseamount = GetBaseAccount(currency).TotalCash;
                _log.Info($"Syncing funds for currency {currency} and received amount {amount}. Current base amount = {baseamount}, currently allocated = {TotalAllocatedCash}");

                //If we have a lower amount
                if (amount > TotalCash || TotalAllocatedCash < amount)
                {
                    decimal adjust = amount - TotalCash;
                    GetBaseAccount(currency).AddCash(new SettledCash(adjust));
                    _log.Info($"Adjusted current base account with amount {adjust}");
                }
                //If we have more allocated than currently in funds, this should be reset proportionally
                else if (TotalAllocatedCash > amount)
                {
                    //Calculate proportions
                    _log.Warn($"The base account balance is lower than we have allocated in amount (base = {amount}, allocated = {TotalAllocatedCash}");
                    Dictionary<string, decimal> proportion = new Dictionary<string, decimal>();
                    _cashknown.Where(x => x.Key != BaseAccount).ForEach(x =>
                    {
                        decimal allocated = x.Value.Sum(funds => _conversion.Convert(funds.Value.TotalCash, funds.Value.BaseCurrency, currency));
                        proportion.Add(x.Key, allocated / amount);
                    });

                    //Adjust funds
                    proportion.ForEach(x =>
                    {
                        decimal current = _cashknown[x.Key][currency].TotalCash;
                        decimal adjusted = current - (current * x.Value);
                        _cashknown[x.Key][currency].AddCash(new SettledCash(adjusted));
                        _log.Warn($"Adjusting funds for quant fund with id {x.Key} current = {current} new amount = {current + adjusted}, adjusted amount = {adjusted}");
                    });
                }
                else
                    _log.Info($"Funds are all in sync, no actions needed");

                //Logging
                _log.Info($"Syncing of funds finished, TotalFunds = {TotalCash}, TotalAllocatedFunds = {TotalAllocatedCash}, #QuantFunds = {_cashknown.Count - 1}, TotalUnallocatedFunds = {TotalUnallocatedCash}");
            }
        }

        #endregion Private Methods
    }
}