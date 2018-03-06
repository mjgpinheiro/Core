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
using System.Collections.Generic;

namespace Quantler.Messaging
{
    /// <summary>
    /// Run backtest request
    /// </summary>
    public class SimulationMessage : MessageImpl
    {
        #region Public Properties

        /// <summary>
        /// Account type (Cash or Margin)
        /// </summary>
        public string AccountType { get; set; }

        /// <summary>
        /// Base currency for account (USD/EUR/HUF etc..)
        /// </summary>
        public string BaseCurrency { get; set; }

        /// <summary>
        /// Associated broker type and therefore broker model to use
        /// </summary>
        public string BrokerType { get; set; }

        /// <summary>
        /// Additional custom data that can be filled in
        /// </summary>
        public Dictionary<string, string> CustomData { get; set; }

        /// <summary>
        /// Backtest end date and time
        /// </summary>
        public DateTime EndDateTime { get; set; }

        /// <summary>
        /// If true, make use of extended market hours
        /// </summary>
        public bool ExtendedMarketHours { get; set; }

        /// <summary>
        /// Gets or sets the leverage to use
        /// TODO: check how we are going to implement this
        /// </summary>
        public int Leverage { get; set; }

        /// <summary>
        /// Type of message
        /// </summary>
        public override MessageType MessageType { get; set; } = MessageType.Backtest;

        /// <summary>
        /// Associated portfolio id
        /// </summary>
        public string PortfolioId { get; set; }

        /// <summary>
        /// Gets or sets the quant fund.
        /// </summary>
        public AddFundMessage QuantFund { get; set; }

        /// <summary>
        /// Simulation start date and time
        /// </summary>
        public DateTime StartDateTime { get; set; }

        #endregion Public Properties
    }
}