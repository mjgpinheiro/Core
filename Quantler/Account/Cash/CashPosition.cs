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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quantler.Account.Cash
{
    /// <summary>
    /// Holder logic for available cash
    /// </summary>
    public class CashPosition
    {
        #region Private Fields

        /// <summary>
        /// Holder of settled funds
        /// </summary>
        private readonly SettledCash _settledcash;

        /// <summary>
        /// Holder of unsettled funds
        /// </summary>
        private readonly List<UnsettledCash> _unsettledcash = new List<UnsettledCash>();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initialize cash holder for a quant fund
        /// </summary>
        /// <param name="basecurrency"></param>
        /// <param name="initialcash"></param>
        public CashPosition(CurrencyType basecurrency, decimal initialcash)
        {
            InitialCash = initialcash;
            BaseCurrency = basecurrency;

            //Add initial cash
            _settledcash = new SettledCash(initialcash);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Base currency denomination for this cash
        /// </summary>
        public CurrencyType BaseCurrency { get; }

        /// <summary>
        /// Initially set cash, allocated to this quant fund
        /// </summary>
        public decimal InitialCash { get; }

        /// <summary>
        /// Total amount cash allocated
        /// </summary>
        public decimal TotalCash => TotalSettledCash + TotalUnsettledCash;

        /// <summary>
        /// Total amount of settled cash
        /// </summary>
        public decimal TotalSettledCash => _settledcash.Amount;

        /// <summary>
        /// Total amount of unsettled cash
        /// </summary>
        public decimal TotalUnsettledCash => _unsettledcash.Sum(x => x.Amount);

        /// <summary>
        /// Gets the date time all is settled in UTC.
        /// </summary>
        public DateTime DateTimeAllIsSettledUtc =>
            _unsettledcash.Count > 0 ? _unsettledcash.Max(x => x.SettlementUtc) : DateTime.MinValue;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Add cash to this holder of cash
        /// </summary>
        /// <param name="cash"></param>
        public void AddCash(SettledCash cash)
        {
            //Check if this settled or unsettled cash
            if (cash is UnsettledCash unsettledfunds)
                _unsettledcash.Add(unsettledfunds);
            else
                _settledcash.AddFunds(cash.Amount);
        }

        /// <summary>
        /// Update current cash
        /// </summary>
        /// <param name="timeutc">Current Date and Time in UTC!</param>
        public void UpdateCash(DateTime timeutc)
        {
            foreach (var unsettled in _unsettledcash.Where(x => timeutc >= x.SettlementUtc))
            {
                //Add to available cash
                _settledcash.AddFunds(unsettled.Amount);

                //Remove as unsettled
                _unsettledcash.Remove(unsettled);
            }
        }

        #endregion Public Methods
    }
}