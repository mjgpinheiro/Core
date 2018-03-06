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
using Quantler.Account.Cash;
using Quantler.Interfaces;
using Quantler.Messaging.Event;
using Quantler.Orders;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Quantler.Account;
using Quantler.Messaging;

namespace Quantler.Bootstrap.MessagingEvents
{
    /// <summary>
    /// Base implementation for running trough events
    /// </summary>
    public abstract class EventRunnerBase : EventRunner
    {
        #region Protected Fields

        /// <summary>
        /// Currently enqueued messages
        /// </summary>
        protected readonly ConcurrentQueue<EventMessage> CurrentMessages = new ConcurrentQueue<EventMessage>();

        /// <summary>
        /// Checks if messages can pass
        /// </summary>
        protected readonly EventKeeper EventKeeper = new EventKeeper();

        #endregion Protected Fields

        #region Private Fields

        /// <summary>
        /// Local logger
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Locker
        /// </summary>
        private readonly object _locker = new object();

        /// <summary>
        /// Currently running thread
        /// </summary>
        private Timer _processItems;

        /// <summary>
        /// If true, we are in the process of stopping
        /// </summary>
        private bool _stopTriggered;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// If true, this runner has subscribers listening in for messages
        /// </summary>
        public bool HasSubscribers
        {
            protected set;
            get;
        }

        /// <summary>
        /// Amount of messages currently in queue
        /// </summary>
        public int InQueue => CurrentMessages.Count;

        /// <summary>
        /// If true, this instance is running
        /// </summary>
        public bool IsRunning
        {
            protected set;
            get;
        }

        /// <summary>
        /// Max amount of items to send for each iteration
        /// Default is unlimited
        /// </summary>
        public int MaxItemRunner
        {
            protected set;
            get;
        } = int.MaxValue;

        /// <summary>
        /// Min amount of milliseconds to wait before sending a message
        /// Default is 100 milliseconds
        /// </summary>
        public int MinWait
        {
            protected set;
            get;
        } = 100;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Enqueue new messages
        /// </summary>
        /// <param name="message">Message to enqueue before send</param>
        /// <param name="force"></param>
        public virtual void Enqueue(EventMessage message, bool force = false)
        {
            //Enqueue
            if ((EventKeeper.HasChanged(message) || force) && !_stopTriggered && HasSubscribers)
                CurrentMessages.Enqueue(message);
        }

        /// <summary>
        /// Initialize instance
        /// </summary>
        public abstract void Initialize(MessageInstance message);

        /// <summary>
        /// Sends the account information messages.
        /// </summary>
        /// <param name="portfolio">The portfolio.</param>
        public void SendAccountInfoMessages(IPortfolio portfolio)
        {
            if (!IsRunning || !HasSubscribers)
                return;

            //Portfolio
            Enqueue(AccountInfoMessage.Create(portfolio.Id, portfolio.BrokerAccount.Id, portfolio.BrokerAccount.Values, portfolio.Currency, portfolio.BrokerAccount.DisplayCurrency));

            //Quant Funds
            portfolio.QuantFunds.ForEach(x => Enqueue(AccountInfoMessage.Create(portfolio.Id, portfolio.BrokerAccount.Id, portfolio.CashManager.GetCalculatedFunds(x, portfolio.BrokerAccount), 
                portfolio.Currency, portfolio.BrokerAccount.DisplayCurrency, x.FundId)));
        }

        /// <summary>
        /// Sends the fund information messages.
        /// </summary>
        /// <param name="portfolioid"></param>
        /// <param name="quantfunds">The quantfunds.</param>
        public void SendFundInfoMessages(string portfolioid, IQuantFund[] quantfunds)
        {
            if (!IsRunning || !HasSubscribers)
                return;

            quantfunds.ForEach(x => Enqueue(FundInfoMessage.Generate(portfolioid, x)));
        }

        /// <summary>
        /// Sends the instance information message.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public void SendInstanceInfoMessage(BrokerConnection connection)
        {
            if (!IsRunning || !HasSubscribers)
                return;

            Enqueue(InstanceMessage.Generate(connection));
        }

        /// <summary>
        /// Sends the pending order information messages.
        /// </summary>
        /// <param name="quantfunds"></param>
        /// <param name="ordertracker"></param>
        /// <param name="portfolioid"></param>
        public void SendPendingOrderInfoMessages(IQuantFund[] quantfunds, OrderTracker ordertracker, string portfolioid)
        {
            if (!IsRunning || !HasSubscribers)
                return;

            ordertracker.PendingOrders.ForEach(p =>
            {
                var quantfund = quantfunds.FirstOrDefault(x => x.FundId == p.FundId);
                if(quantfund != null)
                    Enqueue(PendingOrderInfoMessage.Generate(p, quantfund.Portfolio.Id, quantfund.Name));
                else
                    Enqueue(PendingOrderInfoMessage.Generate(p, portfolioid, string.Empty));
            });
        }

        /// <summary>
        /// Sends the performance information messages.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="quantfunds">The quantfunds.</param>
        public void SendPerformanceInfoMessages(BrokerAccount account, IQuantFund[] quantfunds)
        {
            if (!IsRunning || !HasSubscribers)
                return;

            quantfunds.ForEach(x => Enqueue(PerformanceInfoMessage.Create(x.FundId, x.Results, account.DisplayCurrency)));
        }

        /// <summary>
        /// Sends the position information messages.
        /// </summary>
        /// <param name="quantfunds">The quantfunds.</param>
        /// <param name="cashmanager"></param>
        /// <param name="brokeraccount"></param>
        public void SendPositionInformationMessages(IQuantFund[] quantfunds, CashManager cashmanager, BrokerAccount brokeraccount)
        {
            if (!IsRunning || !HasSubscribers)
                return;

            quantfunds.ForEach(x =>
                x.Positions.ForEach(pos => Enqueue(PositionInfoMessage.Create(x.FundId, pos, x.Universe, cashmanager.GetCalculatedFunds(x, brokeraccount)))));
        }

        /// <summary>
        /// Start task
        /// </summary>
        public void Start()
        {
            if (!IsRunning)
            {
                _log.Info("Starting event runner");
                _processItems = new Timer(OnProcess, "", 0, MinWait);
                IsRunning = true;
            }
        }

        /// <summary>
        /// Stop task
        /// </summary>
        public void Stop()
        {
            if (IsRunning)
            {
                //Send to stop, so we will not retrieve any new messages
                _stopTriggered = true;

                //Turn off timer
                _processItems.Change(0, 0);

                //Release last items
                _log.Info("Stopping event runner and releasing last items");
                while (InQueue > 0)
                {
                    OnProcess("");
                }

                //Set to stopped state
                _log.Info("Event runner has been stopped");
                IsRunning = false;
            }
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Process in batches
        /// </summary>
        /// <param name="state"></param>
        protected virtual void OnProcess(object state)
        {
            lock (_locker)
            {
                int send = 0;
                while (CurrentMessages.TryDequeue(out EventMessage message) && send < MaxItemRunner)
                {
                    Send(message);
                    send++;
                }
            }
        }

        /// <summary>
        /// To be implemented sending logic
        /// </summary>
        /// <param name="message"></param>
        protected abstract void Send(EventMessage message);

        #endregion Protected Methods
    }
}