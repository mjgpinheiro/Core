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
using Quantler.Securities;
using System;
using Quantler.Account;

namespace Quantler.Broker.SettlementModels
{
    /// <summary>
    /// Delayed settlement by T+N period
    /// </summary>
    public class DelayedSettlementModel : SettlementModel
    {
        #region Public Constructors

        /// <summary>
        /// Create a delayed settlement model implementation
        /// </summary>
        /// <param name="daysdelayed">Amount of days before settlement</param>
        /// <param name="timeofday">Time of day settlement will occur</param>
        public DelayedSettlementModel(int daysdelayed, TimeSpan timeofday)
        {
            if (daysdelayed <= 0)
                throw new ArgumentException("Provided days should be positive for a delayed model");
            DelayedDays = daysdelayed;
            TimeOfDay = timeofday;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Amount of days unsettled funds should be delayed before settlement
        /// </summary>
        public int DelayedDays { get; }

        /// <summary>
        /// Time of day settlement should occur
        /// </summary>
        public TimeSpan TimeOfDay { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Settle funds, delayed
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="quantfund">The quantfund.</param>
        /// <param name="security">The security.</param>
        /// <param name="occureddtutc">The occureddtutc.</param>
        /// <param name="amount">The amount.</param>
        public void SettleFunds(BrokerAccount account, Security security, DateTime occureddtutc, decimal amount, IQuantFund quantfund = null)
        {
            //Added funds
            if (amount > 0)
            {
                //Get exchangeModel based local time
                DateTime settlementdate = security.Exchange.LocalTime;

                //Check for date based on market opened date and time
                for (int i = 0; i < DelayedDays; i++)
                {
                    settlementdate = settlementdate.AddDays(i);

                    if (!security.Exchange.IsOpenOnDate(settlementdate))
                        i--;
                }

                //Get correct date and time
                settlementdate = settlementdate.Add(TimeOfDay);

                //Convert time of day from local exchangeModel timezone to utc based time
                settlementdate = settlementdate.ConvertTo(security.Exchange.TimeZone, TimeZone.Utc);

                //Add unsettled funds (to be settled on a later time and date)
                account.Cash.AddCash(security.BaseCurrency, amount, quantfund, settlementdate);
            }
            else //Used funds, settle right away
                account.Cash.AddCash(security.BaseCurrency, amount, quantfund);
        }

        #endregion Public Methods
    }
}