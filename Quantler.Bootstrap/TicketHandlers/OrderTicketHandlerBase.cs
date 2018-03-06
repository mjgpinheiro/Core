#region License Header

/*
* QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
* Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
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
*
* Modifications Copyright 2018 Quantler B.V.
*
*/

#endregion License Header

using NLog;
using Quantler.Account;
using Quantler.Account.Cash;
using Quantler.Common;
using Quantler.Data;
using Quantler.Interfaces;
using Quantler.Orders;
using Quantler.Orders.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MoreLinq;

namespace Quantler.Bootstrap.TicketHandlers
{
    /// <summary>
    /// Base implementation of the order ticket handler
    /// TODO: this currently does no async work (extend implementation to support async processing of orders)
    /// TODO: implement wait for order
    /// </summary>
    /// <seealso cref="OrderTicketHandler" />
    public abstract class OrderTicketHandlerBase : OrderTicketHandler
    {
        #region Private Fields

        /// <summary>
        /// Time at which we check for balance differences
        /// </summary>
        private readonly TimeSpan _balancecheck = new TimeSpan(6, 0, 0);

        /// <summary>
        /// The balance check locker
        /// </summary>
        private readonly object _balancechecklocker = new object();

        /// <summary>
        /// The cancellation token source
        /// </summary>
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Current instance logging
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The order request queue
        /// </summary>
        private readonly BusyBlockingCollection<OrderTicket> _orderTicketQueue = new BusyBlockingCollection<OrderTicket>();

        /// <summary>
        /// The last balance synchronization performed
        /// </summary>
        private DateTime _lastBalanceSync = DateTime.MinValue;

        /// <summary>
        /// Indicates if the rounding off warning has been send (so it is send once)
        /// </summary>
        private bool _roundingoffwarningsend;

        #endregion Private Fields

        #region Protected Fields

        /// <summary>
        /// The portfolio manager
        /// </summary>
        protected IPortfolio Portfolio;

        #endregion Protected Fields

        #region Public Properties

        /// <summary>
        /// Associated broker connection
        /// </summary>
        public BrokerConnection BrokerConnection => Portfolio.BrokerConnection;

        /// <summary>
        /// Gets the fund manager.
        /// </summary>
        public CashManager CashManager => Portfolio.CashManager;

        /// <summary>
        /// True if this task is running
        /// </summary>
        public bool IsRunning
        {
            protected set;
            get;
        }

        /// <summary>
        /// Gets the last interal order identifier.
        /// </summary>
        public int LastInteralOrderId => throw new NotImplementedException();

        /// <summary>
        /// Gets the market order fill timeout.
        /// </summary>
        public TimeSpan MarketOrderFillTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the current order count.
        /// </summary>
        public int OrderCount => OrderTracker.PendingOrders.Length;

        /// <summary>
        /// Gets the order factory.
        /// </summary>
        public OrderFactory OrderFactory => Portfolio.OrderFactory;

        /// <summary>
        /// Gets the order tracker.
        /// </summary>
        public OrderTracker OrderTracker => Portfolio.OrderTracker;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderTicketHandlerBase"/> class.
        /// </summary>
        /// <param name="portfolio">The portfoliomanager.</param>
        public virtual void Initialize(IPortfolio portfolio)
        {
            //Check if we are already initialized
            if(Portfolio != null)
                return;

            //Set portfolio
            Portfolio = portfolio;

            //Set events
            BrokerConnection.OrderStateChange += (sender, orderticketevent) =>
            {
                if (!portfolio.IsBacktesting)
                    _log.Trace($"Order Ticket Event received: {orderticketevent}");
                HandleOrderTicketEvent(orderticketevent);
            };

            BrokerConnection.BalanceChange += (sender, accountaction) => HandleAccountEvent(accountaction);

            //Get current funds
            _log.Debug($"Performing initial sync of funds");
            SyncbrokerageFunds();
            _log.Debug($"Known Funds: ");
            portfolio.CashManager.GetCashPositions().ForEach(pos => _log.Debug($"\t {pos.Key}: {pos.Value.TotalCash}"));
        }

        /// <summary>
        /// Cancels the open orders.
        /// </summary>
        /// <param name="orders">The orders.</param>
        /// <returns></returns>
        public List<PendingOrder> CancelOpenOrders(PendingOrder[] orders)
        {
            var cancelled = new List<PendingOrder>();
            foreach (var po in orders)
            {
                po.Cancel();
                cancelled.Add(po);
            }
            return cancelled;
        }

        /// <summary>
        /// Thread entry for executing all current order tickets
        /// TODO: who kicks off execute?
        /// </summary>
        public void Execute()
        {
            try
            {
                foreach (var ticket in _orderTicketQueue.GetConsumingEnumerable(_cancellationTokenSource.Token))
                {
                    OrderTicketResponse response;
                    switch (ticket.Type)
                    {
                        case OrderTicketType.Submit:
                            response = SubmitOrder(ticket as SubmitOrderTicket);
                            break;

                        case OrderTicketType.Cancel:
                            response = CancelOrder(ticket as CancelOrderTicket);
                            break;

                        case OrderTicketType.Update:
                            response = UpdateOrder(ticket as UpdateOrderTicket);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException($"Unknown order ticket type {ticket.Type}");
                    }

                    //set response
                    ticket.SetResponse(response, OrderTicketState.Processed);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc);
                Portfolio.ExceptionHandler.HandleException(exc);
            }
        }

        /// <summary>
        /// Gets the next internal order identifier.
        /// </summary>
        /// <returns></returns>
        public long GetNextInternalOrderId() =>
            OrderTracker.GetNextInternalOrderId();

        /// <summary>
        /// Gets the open orders.
        /// </summary>
        /// <returns></returns>
        public List<Order> GetOpenOrders() =>
            OrderTracker.PendingOrders.Where(x => !x.OrderState.IsDone()).Select(x => x.Order.Clone()).ToList();

        /// <summary>
        /// Gets the order by broker identifier.
        /// </summary>
        /// <param name="brokerid">The brokerid.</param>
        /// <returns></returns>
        public Order GetOrderByBrokerId(string brokerid) =>
            OrderTracker.PendingOrders.Where(x => x.Order.BrokerId.Contains(brokerid)).Select(x => x.Order.Clone()).FirstOrDefault();

        /// <summary>
        /// Gets the order by identifier.
        /// </summary>
        /// <param name="orderid">The orderid.</param>
        /// <returns></returns>
        public Order GetOrderById(long orderid) =>
            OrderTracker.PendingOrders.Where(x => x.Order.InternalId == orderid).Select(x => x.Order.Clone()).FirstOrDefault();

        /// <summary>
        /// Gets the orders trough search.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <returns></returns>
        public IEnumerable<Order> GetOrders(Func<Order, bool> search = null) =>
            search != null ?
            OrderTracker.PendingOrders.Select(x => x.Order.Clone()).Where(search) :
            OrderTracker.PendingOrders.Select(x => x.Order.Clone());

        /// <summary>
        /// Gets if there is sufficient capital for the specified pending order.
        /// </summary>
        /// <param name="pendingorder">The pending order.</param>
        /// <returns></returns>
        public bool GetSufficientCapitalForOrder(PendingOrder pendingorder)
        {
            //Check if order has size
            if (pendingorder.OrderQuantity == 0 || pendingorder.Order.Direction == Direction.Flat)
                return true;

            //Get Quant Fund (if applicable)
            //TODO: when is this not applicable?
            var quantfund = string.IsNullOrWhiteSpace(pendingorder.FundId)
                ? null
                : Portfolio.QuantFunds.FirstOrDefault(x => x.FundId == pendingorder.FundId);

            //get current position tracker for this pending order
            var positions = (quantfund != null ? quantfund.Positions : Portfolio.BrokerAccount.Positions)[pendingorder.Security];

            //Closing a position is always allowed
            if (positions.Direction != pendingorder.Order.Direction)
                return true;

            //Check if we have enough buying power for adding a position
            try
            {
                var marginmodel = Portfolio.BrokerModel.GetMarginModel(pendingorder.Security);
                var freemargin = marginmodel.GetRemainingMargin(quantfund);
                var initialmarginrequired = marginmodel.GetInitialMarginRequired(pendingorder.Security);

                // pro-rate the initial margin required for order based on how much has already been filled
                var percunfilled = (Math.Abs(pendingorder.OrderQuantity) - Math.Abs(pendingorder.OrderFilledQuantity)) / Math.Abs(pendingorder.OrderQuantity);
                var initialMarginRequiredForRemainderOfOrder = percunfilled * initialmarginrequired;

                if (Math.Abs(initialMarginRequiredForRemainderOfOrder) > freemargin)
                {
                    _log.Error($"Insufficient margin for order: Id: {pendingorder.OrderId}, Initial Margin: {initialmarginrequired}, Free Margin: {freemargin}");
                    return false;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, $"Could not process margin model requests, please check the implemented margin model");
                throw exc;
            }

            return true;
        }

        /// <summary>
        /// On data point received (for simulated order types)
        /// </summary>
        /// <param name="updates">The data updates.</param>
        public void OnData(DataUpdates updates) => ProcessSimulatedOrders(updates.Data);

        /// <summary>
        /// Processes the specified ticket.
        /// </summary>
        /// <param name="ticket">The ticket.</param>
        /// <returns></returns>
        public virtual OrderTicket Process(OrderTicket ticket)
        {
            //Logging
            if (!Portfolio.IsBacktesting)
                _log.Trace($"Processing order ticket: {ticket}");

            //Try and get quant fund, if applicable
            var quantfund = Portfolio.QuantFunds.FirstOrDefault(x => x.FundId == ticket.FundId);

            //Add to queue
            if (ticket.Type == OrderTicketType.Submit && ticket is SubmitOrderTicket submitOrderTicket)
                return ProcessSubmitTicket(submitOrderTicket, quantfund);
            else if (ticket.Type == OrderTicketType.Update && ticket is UpdateOrderTicket updateOrderTicket)
                return ProcessUpdateTicket(updateOrderTicket, quantfund);
            else if (ticket.Type == OrderTicketType.Cancel && ticket is CancelOrderTicket cancelOrderTicket)
                return ProcessCancelTicket(cancelOrderTicket, quantfund);
            else
            {
                ticket.SetResponse(OrderTicketResponse.Error(ticket.OrderId, OrderTicketResponseErrorCode.ProcessingError, "Could not process order ticket"));
                return ticket;
            }
        }

        /// <summary>
        /// Removes the order.
        /// </summary>
        /// <param name="orderid">The orderid.</param>
        /// <param name="comment">The comment.</param>
        /// <returns></returns>
        public void RemoveOrder(long orderid, string comment = null) =>
            //Remove from order tracker
            OrderTracker.TryRemoveOrder(orderid);

        /// <summary>
        /// Start task
        /// </summary>
        public virtual void Start()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Stop task
        /// </summary>
        public virtual void Stop()
        {
            if (!_orderTicketQueue.WaitHandle.WaitOne(TimeSpan.FromMinutes(1)))
                _log.Error($"Exceeded timeout of 1 minute");

            //Cancel process
            _cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Waits for order to be processed by the broker.
        /// </summary>
        /// <param name="orderid">The orderid.</param> =
        /// <returns></returns>
        public virtual bool WaitForOrder(long orderid)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Process submit order
        /// </summary>
        /// <param name="ticket"></param>
        protected virtual OrderTicketResponse CancelOrder(CancelOrderTicket ticket)
        {
            //Check if we can get the order
            if (!OrderTracker.TryGetOrder(ticket.OrderId, out PendingOrder pendingorder))
            {
                _log.Error($"Unable to cancel order with ID {ticket.OrderId}, order id unkown.");
                return OrderTicketResponse.Error(ticket.OrderId, OrderTicketResponseErrorCode.UnableToFindOrder);
            }

            //Check if order is closed
            if (pendingorder.Order.State.IsDone())
                return OrderTicketResponse.Error(ticket.OrderId, OrderTicketResponseErrorCode.InvalidOrderStatus);

            //try and cancel the order
            bool ordercancelled = false;
            try
            {
                ordercancelled = BrokerConnection.CancelOrder(pendingorder);
            }
            catch (Exception exc)
            {
                _log.Error(exc, $"Could not cancel order with id {ticket.OrderId} broker connection refused to do so");
            }

            //If not managed to cancel
            if (!ordercancelled)
            {
                var message = $"Brokerconnection failed to cancel order with id {ticket.OrderId}";
                Portfolio.Log(LogLevel.Error, message);
                HandleOrderTicketEvent(OrderTicketEvent.Error(ticket.OrderId));
                return OrderTicketResponse.Error(ticket.OrderId, OrderTicketResponseErrorCode.BrokerageFailedToCancelOrder);
            }

            //Order was cancelled
            var order = pendingorder.Order as OrderImpl;
            order.State = OrderState.Cancelled;
            pendingorder.UpdateOrder(order);

            //Return result for order ticket
            return OrderTicketResponse.Processed(ticket.OrderId);
        }

        /// <summary>
        /// Determines whether this instance [can update order] the specified order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can update order] the specified order; otherwise, <c>false</c>.
        /// </returns>
        protected bool CanUpdateOrder(Order order) =>
            order.State != OrderState.Filled &&
            order.State != OrderState.Cancelled &&
            order.State != OrderState.PartialFilled &&
            order.State != OrderState.Invalid &&
            order.State != OrderState.Error;

        /// <summary>
        /// Process any simulated orders
        /// </summary>
        /// <param name="datapoints"></param>
        protected virtual void ProcessSimulatedOrders(IEnumerable<DataPoint> datapoints)
        {
            //Check if we need to check this
            if (OrderCount == 0)
                return;

            //Only check on tick data and bars
            var datapointsused = datapoints.Where(x => x.DataType == DataType.QuoteBar || x.DataType == DataType.TradeBar || x.DataType == DataType.Tick);

            //Triggered orders
            List<PendingOrder> triggered = new List<PendingOrder>();

            //Get all orders that need to be simulated internally
            foreach (var item in OrderTracker.PendingOrders
                                    .Where(x => !x.OrderState.IsDone())
                                    .Where(x => x.IsSimulated)
                                    .Join(datapointsused, x => x.Security.Ticker, x => x.Ticker, (po, dp) => new { PendingOrder = po, DataPoint = dp })
                                    .ToArray())
            {
                //Check if already triggered
                if (triggered.Select(x => x.OrderId).Contains(item.PendingOrder.OrderId))
                    continue;

                //Get Data
                var currentprices = item.DataPoint.OrderPrice(item.PendingOrder.Security, item.PendingOrder.Order.Direction);
                var order = item.PendingOrder.Order;

                //Create function for converting to a market order
                SubmitOrderTicket CreateSubmitTicket() =>
                    SubmitOrderTicket.MarketOrder(item.PendingOrder.FundId, order.Security, order.Quantity);

                //logging function
                void Log(decimal price) => _log.Debug($"Simulated order of type {order.Type} with id {order.InternalId} was triggered at price {price} for processing");
                string CreateComment() => 
                    $"Sending market order for simulated order {item.PendingOrder.OrderId} and order type {OrderType.Limit}. OldComment: {item.PendingOrder.Comment}";

                //Check order type and logic
                switch (order)
                {
                    case LimitOrder limitorder:
                        {
                            //Check if limit order price is triggered
                            if (limitorder.IsTriggered(currentprices, out decimal price))
                            {
                                //create a new order from this order, as it will be converted to a market order
                                Log(price);
                                var norder = CreateSubmitTicket();
                                norder.Comment = CreateComment();
                                SubmitOrder(norder);

                                //Add to triggered
                                triggered.Add(item.PendingOrder);
                            }
                            break;
                        }
                    case StopLimitOrder stoplimitorder:
                        {
                            //Check if stop limit order price is triggered
                            if (stoplimitorder.IsTriggered(currentprices, out decimal price))
                            {
                                //create a new order from this order, as it will be converted to a market order
                                Log(price);
                                var norder = CreateSubmitTicket();
                                norder.Comment = CreateComment();
                                SubmitOrder(norder);

                                //Add to triggered
                                triggered.Add(item.PendingOrder);
                            }
                            break;
                        }
                    case StopMarketOrder stoporder:
                        {
                            //Check if stop order price is triggered
                            if (stoporder.IsTriggered(currentprices, out decimal price))
                            {
                                //create a new order from this order, as it will be converted to a market order
                                Log(price);
                                var norder = CreateSubmitTicket();
                                norder.Comment = CreateComment();
                                SubmitOrder(norder);

                                //Add to triggered
                                triggered.Add(item.PendingOrder);
                            }
                            break;
                        }
                    case MarketOnCloseOrder marketOnCloseOrder:
                        {
                            //Check if stop order price is triggered
                            if (marketOnCloseOrder.IsTriggered())
                            {
                                //create a new order from this order, as it will be converted to a market order
                                var norder = CreateSubmitTicket();
                                norder.Comment = CreateComment();
                                SubmitOrder(norder);

                                //Add to triggered
                                triggered.Add(item.PendingOrder);
                            }
                            break;
                        }
                    case MarketOnOpenOrder marketOnOpenOrder:
                    {
                        //Check if stop order price is triggered
                        if (marketOnOpenOrder.IsTriggered())
                        {
                            //create a new order from this order, as it will be converted to a market order
                            var norder = CreateSubmitTicket();
                            norder.Comment = CreateComment();
                            SubmitOrder(norder);

                            //Add to triggered
                            triggered.Add(item.PendingOrder);
                        }
                        break;
                    }
                    default:
                        _log.Error($"Simulated order of type {item.PendingOrder.Order.Type} is not supported, removing order!");
                        triggered.Add(item.PendingOrder);
                        break;
                }
            }

            //Remove all triggered orders
            triggered.ForEach(x => OrderTracker.TryRemoveOrder(x.OrderId));
        }

        /// <summary>
        /// Rounds the size of a pending order to the correct lot size.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        protected virtual void RoundLotSize(OrderImpl order)
        {
            var orderLotMod = order.Quantity % Convert.ToInt32(order.Security.Details.LotSize);

            if (orderLotMod != 0)
            {
                order.Quantity = order.Quantity - orderLotMod;
                if (!_roundingoffwarningsend)
                {
                    Portfolio.Log(LogLevel.Error,
                        $"Warning: Due to brokerage limitations, orders will be rounded to the nearest lot size of {Convert.ToInt32(order.Security.Details.LotSize)}");
                    _roundingoffwarningsend = true;
                }
            }
        }

        /// <summary>
        /// Round the prices to a correct number for this order and its associated security
        /// </summary>
        /// <param name="order">The order.</param>
        protected virtual void RoundOrderPrices(OrderImpl order)
        {
            //check if we need to round order prices
            if (order.Type == OrderType.Market ||
                order.Type == OrderType.MarketOnClose ||
                order.Type == OrderType.MarketOnOpen)
                return;

            decimal minimumincrement = Portfolio.BrokerModel.GetMinimumPriceIncrement(order.Security);
            if (minimumincrement == 0)
                return;

            decimal limitprice = 0;
            decimal limitchangedprice = 0;
            decimal stopprice = 0;
            decimal stopchangedprice = 0;

            switch (order.Type)
            {
                case OrderType.Limit:
                    limitprice = ((LimitOrder)order).LimitPrice;
                    limitchangedprice = Math.Round(((LimitOrder)order).LimitPrice / minimumincrement) * minimumincrement;
                    ((LimitOrder)order).LimitPrice = limitchangedprice;
                    break;

                case OrderType.StopMarket:
                    stopprice = ((StopMarketOrder)order).StopPrice;
                    stopchangedprice = Math.Round(((StopMarketOrder)order).StopPrice / minimumincrement) * minimumincrement;
                    ((StopMarketOrder)order).StopPrice = stopchangedprice;
                    break;

                case OrderType.StopLimit:
                    limitprice = ((StopLimitOrder)order).LimitPrice;
                    stopprice = ((StopLimitOrder)order).StopPrice;
                    limitchangedprice = Math.Round(((StopLimitOrder)order).LimitPrice / minimumincrement) * minimumincrement;
                    stopchangedprice = Math.Round(((StopLimitOrder)order).StopPrice / minimumincrement) * minimumincrement;
                    ((StopLimitOrder)order).LimitPrice = limitchangedprice;
                    ((StopLimitOrder)order).StopPrice = stopchangedprice;
                    break;
            }

            //check for changes to be notified
            void Message(string type, decimal oldvalue, decimal newvalue) => Portfolio.Log(LogLevel.Error, $"Warning: To meet brokerage precision requirements, order with id {order.InternalId} and its {type}Price was rounded to {newvalue} from {oldvalue}");
            if (limitprice != limitchangedprice)
                Message("Limit", limitprice, limitchangedprice);
            if (stopprice != stopchangedprice)
                Message("Stop", stopprice, stopchangedprice);
        }

        /// <summary>
        /// Process submit order
        /// </summary>
        /// <param name="ticket"></param>
        protected virtual OrderTicketResponse SubmitOrder(SubmitOrderTicket ticket)
        {
            //Get order from factory
            PendingOrder pendingorder = OrderFactory.CreateOrder(ticket);
            OrderImpl order = pendingorder.Order as OrderImpl;

            //Round off order quantity for correct amounts
            RoundLotSize(order);

            //try and get the order from the order tracker
            if (!OrderTracker.TryAddOrder(pendingorder))
            {
                _log.Error($"Unable to add new order, order with id {pendingorder.OrderId} was already submitted");
                return OrderTicketResponse.Error(pendingorder.OrderId, OrderTicketResponseErrorCode.OrderAlreadyExists, $"Current order with id {pendingorder.OrderId} was already submitted");
            }
            if (!OrderTracker.TryGetOrder(pendingorder.OrderId, out pendingorder))
            {
                _log.Error($"Unable to retrieve newly added order, order with id {pendingorder.OrderId} was cannot be processed properly");
                return OrderTicketResponse.Error(pendingorder.OrderId, OrderTicketResponseErrorCode.UnableToFindOrder, $"Current order with id {pendingorder.OrderId} cannot be found");
            }

            //Round of prices
            RoundOrderPrices(order);

            //Set our new order
            pendingorder.UpdateOrder(order);

            //Check for correct size
            if (order.Quantity == 0)
            {
                order.State = OrderState.Invalid;
                var ticketresponse = OrderTicketResponse.Error(order.InternalId, OrderTicketResponseErrorCode.OrderQuantityZero);
                Portfolio.Log(LogLevel.Error, ticketresponse.ErrorMessage);
                return ticketresponse;
            }

            //Check if we have enough capital for an order
            bool sufficientcapital = GetSufficientCapitalForOrder(pendingorder);
            if (!sufficientcapital)
            {
                //Not enough capital to execute this order
                order.State = OrderState.Invalid;
                var response = OrderTicketResponse.Error(order.InternalId,
                    OrderTicketResponseErrorCode.InsufficientBuyingPower, $"Cannot execute order with id {order.InternalId}, insufficient funds to execute order.");
                Portfolio.Log(LogLevel.Error, response.ErrorMessage);
                HandleOrderTicketEvent(OrderTicketEvent.Error(order.InternalId, $"Insufficent capital to execute order"));
                return response;
            }

            //Check if broker accepts order at this moment
            try
            {
                if (!Portfolio.BrokerModel.CanSubmitOrder(order, out var message))
                {
                    //Broker model did not accept this order
                    order.State = OrderState.Invalid;
                    var response = OrderTicketResponse.Error(order.InternalId,
                        OrderTicketResponseErrorCode.BrokerageModelRefusedToSubmitOrder, $"Order with id {order.InternalId}: {message}");
                    Portfolio.Log(LogLevel.Error, "");
                    HandleOrderTicketEvent(OrderTicketEvent.Error(order.InternalId, $"Broker model of type {Portfolio.BrokerModel.BrokerType} declared order cannot be submitted, message: {message}"));
                    return response;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, $"Could not run CanSubmitOrder on order with id {order.InternalId}, please check the implemented logic");
                order.State = OrderState.Invalid;
                return OrderTicketResponse.Error(pendingorder.OrderId, OrderTicketResponseErrorCode.ProcessingError, $"Current order with id {pendingorder.OrderId} cannot be processed due to error");
            }

            //Try to execute this order to the broker connection attached
            bool ordersuccess = false;
            try
            {
                ordersuccess = BrokerConnection.SubmitOrder(pendingorder);
            }
            catch (Exception exc)
            {
                _log.Error(exc);
            }

            //Check if placing the order was a success
            if (!ordersuccess)
            {
                order.State = OrderState.Invalid;
                var submitmessage = "BrokerConnection failed to place order";
                var response = OrderTicketResponse.Error(order.InternalId,
                    OrderTicketResponseErrorCode.BrokerageFailedToSubmitOrder, submitmessage);
                Portfolio.Log(LogLevel.Error, submitmessage);
                HandleOrderTicketEvent(OrderTicketEvent.Error(order.InternalId, submitmessage));
                return response;
            }

            order.State = OrderState.Submitted;
            return OrderTicketResponse.Processed(order.InternalId);
        }

        /// <summary>
        /// Synchronizes the current brokerage funds with the fund manager.
        /// TODO: needs to be done regularly
        /// </summary>
        protected virtual void SyncbrokerageFunds()
        {
            try
            {
                if (!Monitor.TryEnter(_balancechecklocker))
                    return;

                //Logging
                _log.Trace($"Performing brokerage funds synchronization for broker {BrokerConnection.BrokerType}");
                IReadOnlyList<CashPosition> currentfunds = null;
                try
                {
                    currentfunds = BrokerConnection.GetAccountFunds();
                }
                catch (Exception exc)
                {
                    _log.Error(exc);
                }

                //Check if we have data
                if (currentfunds == null || currentfunds.Count == 0)
                    return;

                //Perform sync to fund manager
                foreach (var funds in currentfunds)
                    CashManager.Process(AccountActionType.Sync, funds.BaseCurrency, funds.TotalCash);

                //Set to already synced
                _lastBalanceSync = Portfolio.Clock.CurrentUtc;
            }
            finally
            {
                Monitor.Exit(_balancechecklocker);
            }
        }

        /// <summary>
        /// Submit update order
        /// </summary>
        /// <param name="ticket"></param>
        protected virtual OrderTicketResponse UpdateOrder(UpdateOrderTicket ticket)
        {
            //Get current pending order
            if (!OrderTracker.TryGetOrder(ticket.OrderId, out PendingOrder pendingorder))
            {
                _log.Error($"Unable to retrieve order with id {ticket.OrderId} could not proceed");
                return OrderTicketResponse.Error(pendingorder.OrderId, OrderTicketResponseErrorCode.UnableToFindOrder, $"Current order with id {pendingorder.OrderId} cannot be found");
            }

            //Get the order
            var order = pendingorder.Order as OrderImpl;

            //Check if we can update this order
            if (!CanUpdateOrder(order))
                return OrderTicketResponse.Error(order.InternalId, OrderTicketResponseErrorCode.InvalidOrderStatus,
                    $"Unable to update order with id {order.InternalId} and current state {order.State}");

            //Check if we can process this order
            try
            {
                if (!Portfolio.BrokerModel.CanUpdateOrder(order, out string message))
                {
                    //Notify we cannot update this order
                    order.State = OrderState.Invalid;
                    var response = OrderTicketResponse.Error(order.InternalId,
                        OrderTicketResponseErrorCode.BrokerageModelRefusedToUpdateOrder, $"Cannot update order {order.InternalId}: {message}");
                    Portfolio.Log(LogLevel.Error, response.ErrorMessage);
                    HandleOrderTicketEvent(OrderTicketEvent.Error(order.InternalId, response.ErrorMessage));
                    return response;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, $"Could not run CanUpdateOrder on order with id {order.InternalId}, please check the implemented logic");
                order.State = OrderState.Invalid;
                return OrderTicketResponse.Error(pendingorder.OrderId, OrderTicketResponseErrorCode.ProcessingError, $"Current order with id {pendingorder.OrderId} cannot be processed due to error");
            }

            //Check order quantity and pricing in case this is out of range
            RoundLotSize(order);
            RoundOrderPrices(order);

            //Try and update this order
            bool orderupdatesucceeded = false;
            try
            {
                orderupdatesucceeded = BrokerConnection.UpdateOrder(pendingorder);
            }
            catch (Exception exc)
            {
                _log.Error(exc);
            }

            //Process a failed order update event
            if (!orderupdatesucceeded)
            {
                var errormessage =
                    $"Broker connection failed to update order with id {order.InternalId} with connection type {BrokerConnection.BrokerType}";
                Portfolio.Log(LogLevel.Error, errormessage);
                HandleOrderTicketEvent(OrderTicketEvent.Error(order.InternalId, "Failed to update order"));
                return OrderTicketResponse.Error(order.InternalId,
                    OrderTicketResponseErrorCode.BrokerageFailedToUpdateOrder, errormessage);
            }
            else
            {
                //Apply updates to order
                order.Update(ticket);
                pendingorder.UpdateOrder(order);
            }

            return OrderTicketResponse.Processed(order.InternalId);
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Handles the account event.
        /// </summary>
        /// <param name="accountaction">The accountaction.</param>
        private void HandleAccountEvent(AccountAction accountaction) =>
            Portfolio.CashManager.Process(accountaction.AccountActionType, accountaction.CurrencyType,
                accountaction.Balance);

        /// <summary>
        /// Handles the order ticket event.
        /// </summary>
        /// <param name="orderticketevent">The orderticketevent.</param>
        private void HandleOrderTicketEvent(OrderTicketEvent orderticketevent)
        {
            //Retrieve the order
            if (!(GetOrderById(orderticketevent.OrderId) is OrderImpl order))
            {
                _log.Error($"Unable to retrieve order with id {orderticketevent.OrderId} could not proceed");
                return;
            }

            //Set new order status
            order.State = orderticketevent.OrderState;

            //Get pending order instance
            if (!OrderTracker.TryGetOrder(orderticketevent.OrderId, out PendingOrder pendingorder))
            {
                _log.Error($"Unable to retrieve pending order with id {orderticketevent.OrderId} could not proceed");
                return;
            }

            //Check if we need to apply fill
            if ((order.State == OrderState.Filled || order.State == OrderState.PartialFilled))
                pendingorder.AddFill(orderticketevent.Fill);

            //Check if we need to fire an event
            if (orderticketevent.OrderState != OrderState.None)
            {
                //Create new event
                try
                {
                    Portfolio.OnOrderTicketEvent(pendingorder, orderticketevent);
                }
                catch (Exception exc)
                {
                    Portfolio.Log(LogLevel.Error, $"Could not process orderticket event: {exc.Message}");
                    _log.Error(exc);
                }
            }
        }

        /// <summary>
        /// Cancels the order ticket.
        /// </summary>
        /// <param name="ticket">The ticket.</param>
        /// <param name="quantfund">Ticket associated quant fund</param>
        /// <returns></returns>
        private OrderTicket ProcessCancelTicket(CancelOrderTicket ticket, IQuantFund quantfund)
        {
            //Try and get current order ticket
            if (!OrderTracker.TryGetOrder(ticket.OrderId, out PendingOrder pendingorder))
            {
                ticket.SetResponse(OrderTicketResponse.Error(ticket.OrderId, OrderTicketResponseErrorCode.UnableToFindOrder));
                return ticket;
            }

            try
            {
                //Try and process cancel ticket
                if (pendingorder.OrderState.IsDone())
                {
                    _log.Error($"Order is already of state {pendingorder.OrderState} while trying to cancel this order");
                    ticket.SetResponse(OrderTicketResponse.Error(pendingorder.OrderId, OrderTicketResponseErrorCode.InvalidOrderStatus));
                }
                else if (quantfund != null && quantfund.IsBackfilling)
                    ticket.SetResponse(OrderTicketResponse.Error(pendingorder.OrderId, OrderTicketResponseErrorCode.QuantFundBackfilling));
                else
                {
                    // update the order status
                    var order = pendingorder.Order as OrderImpl;
                    order.State = OrderState.CancelPending;
                    pendingorder.UpdateOrder(order);

                    // notify the portfolio with an order event
                    HandleOrderTicketEvent(OrderTicketEvent.Cancelled(pendingorder.OrderId));

                    // send the request to be processed
                    ticket.SetResponse(OrderTicketResponse.Processed(ticket.OrderId), OrderTicketState.Processing);
                    _orderTicketQueue.Add(ticket);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc);
                ticket.SetResponse(OrderTicketResponse.Error(pendingorder.OrderId, OrderTicketResponseErrorCode.ProcessingError, exc.Message));
            }

            //return result
            return ticket;
        }

        /// <summary>
        /// Processes the submit ticket.
        /// </summary>
        /// <param name="ticket">The ticket.</param>
        /// <param name="quantfund">Associated Quant Fund</param>
        /// <returns></returns>
        private OrderTicket ProcessSubmitTicket(SubmitOrderTicket ticket, IQuantFund quantfund)
        {
            if (quantfund != null && quantfund.IsBackfilling)
                ticket.SetResponse(
                    OrderTicketResponse.Error(ticket.OrderId, OrderTicketResponseErrorCode.QuantFundBackfilling,
                        $"Quant fund {ticket.FundId} is currently backfilling, cannot process orders."));
            else
            {
                _orderTicketQueue.Add(ticket);
                ticket.SetResponse(OrderTicketResponse.Processed(ticket.OrderId), OrderTicketState.Processing);
            }

            return ticket;
        }

        /// <summary>
        /// Processes the update ticket.
        /// </summary>
        /// <param name="ticket">The ticket.</param>
        /// <param name="quantfund">Ticket associated quant fund</param>
        /// <returns></returns>
        private OrderTicket ProcessUpdateTicket(UpdateOrderTicket ticket, IQuantFund quantfund)
        {
            //Try and get current order ticket
            if (!OrderTracker.TryGetOrder(ticket.OrderId, out PendingOrder pendingorder))
            {
                ticket.SetResponse(OrderTicketResponse.Error(ticket.OrderId, OrderTicketResponseErrorCode.UnableToFindOrder));
                return ticket;
            }

            try
            {
                //Try and process cancel ticket
                if (pendingorder.OrderState.IsDone())
                {
                    _log.Error($"Order is already of state {pendingorder.OrderState} while trying to update this order");
                    ticket.SetResponse(OrderTicketResponse.Error(pendingorder.OrderId, OrderTicketResponseErrorCode.InvalidOrderStatus));
                }
                else if (quantfund != null && quantfund.IsBackfilling)
                    ticket.SetResponse(OrderTicketResponse.Error(pendingorder.OrderId, OrderTicketResponseErrorCode.QuantFundBackfilling));
                else
                {
                    // send the request to be processed
                    ticket.SetResponse(OrderTicketResponse.Processed(ticket.OrderId), OrderTicketState.Processing);
                    _orderTicketQueue.Add(ticket);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc);
                ticket.SetResponse(OrderTicketResponse.Error(pendingorder.OrderId, OrderTicketResponseErrorCode.ProcessingError, exc.Message));
            }

            //Return what we have
            return ticket;
        }

        #endregion Private Methods
    }
}