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

using NLog;
using Quantler.Account;
using Quantler.Account.Cash;
using Quantler.Broker;
using Quantler.Broker.Model;
using Quantler.Data;
using Quantler.Interfaces;
using Quantler.Orders;
using Quantler.Trades;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Quantler.Configuration;

namespace Quantler.Brokers.Simulated
{
    /// <summary>
    /// A simulated broker class for Quantler. Processes orders and fills them against external
    /// tick feed. (live or historical)
    /// </summary>
    [Export(typeof(BrokerConnection))]
    public class SimBroker : SimBrokerConnection
    {
        #region Private Fields

        /// <summary>
        /// The currently active orders
        /// </summary>
        private readonly ConcurrentDictionary<long, PendingOrder> _activeOrders = new ConcurrentDictionary<long, PendingOrder>();

        /// <summary>
        /// Thread locking
        /// </summary>
        private readonly object _locker = new object();

        /// <summary>
        /// Current instance logging
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The portfolio manager under test
        /// </summary>
        private IPortfolio _portfolio;

        /// <summary>
        /// Keeps track for if we need to use highly liquid fills
        /// </summary>
        private bool _usehighlyliquidfills;

        #endregion Private Fields

        #region Public Events

        /// <summary>
        /// Occurs when [account balance change].
        /// </summary>
        public event EventHandler<AccountAction> BalanceChange;

        /// <summary>
        /// Occurs when [order state change].
        /// </summary>
        public event EventHandler<OrderTicketEvent> OrderStateChange;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Gets the broker model.
        /// </summary>
        public BrokerModel BrokerModel => _portfolio.BrokerModel;

        /// <summary>
        /// Gets the type of the broker.
        /// </summary>
        public BrokerType BrokerType => BrokerModel.BrokerType;

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected => true;

        /// <summary>
        /// Gets a value indicating whether this instance is ready.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is ready; otherwise, <c>false</c>.
        /// </value>
        public bool IsReady => true;

        /// <summary>
        /// Gets the latency in ms.
        /// </summary>
        public int LatencyInMS => 0;

        /// <summary>
        /// Gets a value indicating whether this is a live feed with live updates.
        /// </summary>
        /// <value>
        /// <c>true</c> if [live feed]; otherwise, <c>false</c>.
        /// </value>
        public bool LiveFeed => true;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Cancels the order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        public bool CancelOrder(PendingOrder order)
        {
            //Check if we can remove this order
            if (!_activeOrders.TryRemove(order.OrderId, out PendingOrder active))
                return false; //Cannot find order

            //Send cancelled order notification
            OrderStateChange?.Invoke(this, OrderTicketEvent.Cancelled(order.OrderId, "Order was cancelled"));
            return true;
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        public void Connect()
        {
            //Not needed
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public void Disconnect()
        {
            //Not needed
        }

        /// <summary>
        /// Gets the account funds currently known.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<CashPosition> GetAccountFunds() =>
            _portfolio.CashManager.GetCashPositions().Values.ToList();

        /// <summary>
        /// Gets the currently active orders.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<PendingOrder> GetActiveOrders() =>
            _activeOrders.Values.ToList();

        /// <summary>
        /// Gets the position overview.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<Position> GetPositionOverview() =>
            _portfolio.BrokerAccount.Positions.ToList();

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            _log.Info($"Initializing sim broker");
            _usehighlyliquidfills = Config.SimulationConfig.HighlyLiquidFills;
        }

        /// <summary>
        /// Processes the market data.
        /// </summary>
        /// <param name="dataupdates">The data updates.</param>
        public void ProcessMarketData(DataUpdates dataupdates)
        {
            //Only accept market data
            if (_activeOrders.Count == 0)
                return;

            //Check if we have any data
            if (!dataupdates.HasUpdates)
                return;

            lock (_locker)
            {
                foreach (var pkv in _activeOrders.OrderBy(x => x.Key))
                {
                    //get the order
                    var pendingorder = pkv.Value;
                    var order = pendingorder.Order;

                    //Get datapoint
                    if (!dataupdates[order.Security].HasUpdates)
                        continue;

                    var datapoint = dataupdates[order.Security].Ticks.Count > 0
                        ? dataupdates[order.Security].Ticks.First().Value.First() as DataPoint
                        : dataupdates[order.Security].QuoteBars.Count > 0
                            ? dataupdates[order.Security].QuoteBars.First().Value as DataPoint
                            : dataupdates[order.Security].TradeBars.Count > 0
                                ? dataupdates[order.Security].TradeBars.First().Value as DataPoint
                                : null;

                    if (datapoint == null)
                        continue;

                    //Check if order is already done
                    if (order.State.IsDone())
                    {
                        _activeOrders.TryRemove(pkv.Key, out pendingorder);
                        continue;
                    }

                    //Check if we have enough buying power
                    if (!_portfolio.OrderTicketHandler.GetSufficientCapitalForOrder(pendingorder))
                    {
                        //Remove order from active orders, as it is cancelled by the broker instance
                        _activeOrders.TryRemove(pkv.Key, out pendingorder);
                        _portfolio.Log(LogLevel.Error, $"Insufficient funds to process order by sim broker");
                        OrderStateChange?.Invoke(this, OrderTicketEvent.Cancelled(pendingorder.OrderId, "Insufficient funds to process order by sim broker"));
                    }

                    //Check if we need to fill this order
                    var fillmodel = BrokerModel.GetFillModel(order);
                    Fill filledresult = Fill.NoFill();

                    try
                    {
                        filledresult = fillmodel.FillOrder(BrokerModel, datapoint, pendingorder, _usehighlyliquidfills);
                    }
                    catch (Exception exc)
                    {
                        _log.Error(exc);
                        _portfolio.Log(LogLevel.Error, string.Format("Order Error: id: {0}, Transaction model failed to fill for order type: {1} with error: {2}", order.InternalId, order.Type, exc.Message));
                        OrderStateChange?.Invoke(this, OrderTicketEvent.Cancelled(pendingorder.OrderId, "Exception during processing fill for this order, please check logs"));
                    }

                    //Check for any full or partial fills
                    if (filledresult.FillQuantity > 0)
                    {
                        if (filledresult.Status == OrderState.Filled)
                            OrderStateChange?.Invoke(this, OrderTicketEvent.Filled(order.InternalId, filledresult));
                        else if (filledresult.Status == OrderState.PartialFilled)
                            OrderStateChange?.Invoke(this, OrderTicketEvent.PartiallyFilled(order.InternalId, filledresult));
                    }

                    //Check if order is done
                    if (filledresult.Status.IsDone())
                        _activeOrders.TryRemove(pkv.Key, out pendingorder);
                }
            }
        }

        /// <summary>
        /// Sets the broker model.
        /// </summary>
        /// <param name="portfolio">The Portfolio Manager.</param>
        public void SetPortfolio(IPortfolio portfolio) =>
            _portfolio = portfolio;

        /// <summary>
        /// Submits the order.
        /// </summary>
        /// <param name="pendingorder">The order.</param>
        /// <returns></returns>
        public bool SubmitOrder(PendingOrder pendingorder)
        {
            //get the underlying order
            var order = pendingorder.Order;

            //Check current order state
            if (order.State == OrderState.New)
            {
                //Set order
                lock (_locker)
                {
                    _activeOrders[pendingorder.OrderId] = pendingorder;
                }

                //Check order id
                if (order.BrokerId.Contains(order.InternalId.ToString()))
                    order.BrokerId.Add(order.InternalId.ToString());

                //Order event
                OrderStateChange?.Invoke(this, OrderTicketEvent.Submitted(order.InternalId));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tests the connection latency between the current instance and the broker.
        /// </summary>
        public void TestLatency()
        {
            //Not Applicable
        }

        /// <summary>
        /// Updates the order.
        /// </summary>
        /// <param name="pendingorder">The order.</param>
        /// <returns></returns>
        public bool UpdateOrder(PendingOrder pendingorder)
        {
            //Check if we can find this order
            if (!_activeOrders.TryGetValue(pendingorder.OrderId, out PendingOrder active))
                return false; //Cannot find order

            lock (_locker)
            {
                //Update order instance
                pendingorder.UpdateOrder(active.Order);
            }

            //Send updated order notification
            OrderStateChange?.Invoke(this, OrderTicketEvent.Updated(active.OrderId, "Order was updated"));
            return true;
        }

        #endregion Public Methods
    }
}