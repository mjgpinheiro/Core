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
using Quantler.Account.Cash;
using Quantler.Messaging;
using Quantler.Messaging.Event;
using Quantler.Orders;

namespace Quantler.Interfaces
{
    /// <summary>
    /// Runs trough events send by the application to a desired front-end
    /// </summary>
    public interface EventRunner : QTask
    {
        #region Public Properties

        /// <summary>
        /// True if this instance has subscribers
        /// </summary>
        bool HasSubscribers { get; }

        /// <summary>
        /// Amount of messages currently in queue
        /// </summary>
        int InQueue { get; }

        /// <summary>
        /// Max items per run
        /// </summary>
        int MaxItemRunner { get; }

        /// <summary>
        /// Min time between runs of messages in milliseconds
        /// </summary>
        int MinWait { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Enqueue new message to be send
        /// </summary>
        /// <param name="message"></param>
        /// <param name="force"></param>
        void Enqueue(EventMessage message, bool force = false);

        /// <summary>
        /// Initialize this instance
        /// </summary>
        void Initialize(MessageInstance message);

        /// <summary>
        /// Sends the account information messages.
        /// </summary>
        /// <param name="portfolio">The portfolio.</param>
        void SendAccountInfoMessages(IPortfolio portfolio);

        /// <summary>
        /// Sends the fund information messages.
        /// </summary>
        /// <param name="quantfunds">The quant funds.</param>
        /// <param name="portfolioid"></param>
        void SendFundInfoMessages(string portfolioid, IQuantFund[] quantfunds);

        /// <summary>
        /// Sends the instance information message.
        /// </summary>
        /// <param name="connection">The connection.</param>
        void SendInstanceInfoMessage(BrokerConnection connection);

        /// <summary>
        /// Sends the pending order information messages.
        /// </summary>
        /// <param name="quantfunds"></param>
        /// <param name="ordertracker"></param>
        /// <param name="portfolioid"></param>
        void SendPendingOrderInfoMessages(IQuantFund[] quantfunds, OrderTracker ordertracker, string portfolioid);

        /// <summary>
        /// Sends the performance information messages.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="quantfunds">The quant funds.</param>
        void SendPerformanceInfoMessages(BrokerAccount account, IQuantFund[] quantfunds);

        /// <summary>
        /// Sends the position information messages.
        /// </summary>
        /// <param name="quantfunds">The quant funds.</param>
        /// <param name="cashmanager"></param>
        /// <param name="brokeraccount"></param>
        void SendPositionInformationMessages(IQuantFund[] quantfunds, CashManager cashmanager,
            BrokerAccount brokeraccount);

        #endregion Public Methods
    }
}