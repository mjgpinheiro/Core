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
using Quantler.Account;
using Quantler.Broker.Model;
using Quantler.Interfaces;
using Quantler.Securities;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Quantler.Orders
{
    /// <summary>
    /// Keeps track of current and historical pending orders
    /// TODO: put this in the flow
    /// TODO: is it correct if order fills are processed here
    /// </summary>
    /// <seealso cref="OrderTracker" />
    public class OrderTracker
    {
        #region Private Fields

        /// <summary>
        /// Current logging instance
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Order tracker locking object
        /// </summary>
        private readonly object _ordertrackerlocker = new object();

        /// <summary>
        /// All currently known pending orders
        /// </summary>
        private readonly ConcurrentDictionary<long, PendingOrder> _pendingorders = new ConcurrentDictionary<long, PendingOrder>();

        /// <summary>
        /// All currently known historical fills
        /// </summary>
        private readonly ConcurrentDictionary<long, Fill> _historicalfills = new ConcurrentDictionary<long, Fill>();

        /// <summary>
        /// Associated portfolio
        /// </summary>
        private readonly IPortfolio _portfolio;

        /// <summary>
        /// Current internal order id
        /// </summary>
        private long _internalOrderId;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderTracker"/> class.
        /// </summary>
        /// <param name="portfolio">The portfolio.</param>
        public OrderTracker(IPortfolio portfolio) =>
            _portfolio = portfolio;

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Returns currently active pending orders
        /// </summary>
        public PendingOrder[] PendingOrders => _pendingorders.Values.ToArray();

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Returns a new unique internal order id
        /// </summary>
        /// <returns></returns>
        public long GetNextInternalOrderId() =>
            _internalOrderId++;

        /// <summary>
        /// Process a new fill for a pending order
        /// TODO: check if this is needed, or should be done by portfolio? e.g: will this call come from the ordertickethandler or the portfoliomanager or the portfolio?
        /// </summary>
        /// <param name="pendingorder"></param>
        /// <param name="fill"></param>
        public void ProcessFill(PendingOrder pendingorder, Fill fill, BrokerModel brokermodel, BrokerAccount brokeraccount)
        {
            //Settled funds
            decimal amount = pendingorder.Security.ConvertValue(Math.Abs(fill.FillValue) - Math.Abs(fill.FillFee), brokeraccount.Currency);

            //Try and get the associated quant fund
            var quantfund = _portfolio.QuantFunds.FirstOrDefault(x => x.FundId == pendingorder.FundId);

            //True amount for direction
            var position = (quantfund != null ? quantfund.Positions : _portfolio.BrokerAccount.Positions)[pendingorder.Security];
            if (position.Direction == Direction.Flat || position.Direction == pendingorder.Order.Direction)
                amount = 0 - amount; //If we used to be flat or we are adding to a position we are subtracting cash from the cash balance
            else
                amount = Math.Abs(amount); //Else we are adding cash, since we are closing a position

            //Process fill with a settlement model used
            try
            {
                brokermodel.GetSettlementModel(pendingorder.Security).SettleFunds(brokeraccount, pendingorder.Security, fill.UtcTime, amount, quantfund);
            }
            catch (Exception exc)
            {
                _log.Error(exc, $"Could not process fill trough settlement model due to excetion. Input Fill: {fill.Security.Ticker}, Date and time: {fill.UtcTime}");
                throw exc;
            }

            //Adjust current position (account + agent)
            brokeraccount.Positions.Adjust(fill);
            var result = position.Adjust(fill);
            _log.Debug($"Processed fill {fill.UtcTime} for security {fill.Security.Ticker} with trading result {result.NetPnL}");


            //Check for day trading orders
            _historicalfills.TryAdd(fill.OrderId + fill.LocalTime.Ticks, fill);
            brokermodel.DayTradingOrdersLeft(brokeraccount, _historicalfills.Values.ToArray());

            //Cleanup if needed
            CleanUp();
        }

        /// <summary>
        /// Process a fill that is not related to a known pending order
        /// TODO: this
        /// </summary>
        /// <param name="fill"></param>
        public void ProcessFill(BrokerAccount brokeraccount, Fill fill)
        {
            //Get current position known to Quantler
            var position = brokeraccount.Positions[fill.Security];
            decimal currentsize = position.UnsignedQuantity;
            Direction currentdirection = position.IsLong ? Direction.Long : Direction.Short;

            //If we are flat, no need to intervene
            if (position.IsFlat)
                return;
            else if (fill.Direction == currentdirection) //No adjustments needed on fills that are not related to current agents
                return;

            //Change account position
            position.Adjust(fill);

            //Change position size proportionally for each quant fund
            int processed = 0;
            foreach (var quantfund in _portfolio.QuantFunds.Where(x => !x.Positions[fill.Security].IsFlat))
            {
                //Get agent position
                position = quantfund.Positions[fill.Security];
                if (position.IsFlat)
                    continue;

                //Check size and direction
                //decimal proportion = position.UnsignedSize / currentsize;

                //int newsize =
            }

            //Check for day trading orders
            _historicalfills.TryAdd(fill.OrderId + fill.LocalTime.Ticks, fill);
            _portfolio.BrokerModel.DayTradingOrdersLeft(brokeraccount, _historicalfills.Values.ToArray());
        }

        /// <summary>
        /// Check if position sizes are in line with the position size given by the broker
        /// </summary>
        /// <param name="security"></param>
        /// <param name="size"></param>
        public void ReconcilePositionSize(Security security, int size)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add order for tracking
        /// </summary>
        /// <param name="pendingorder"></param>
        public bool TryAddOrder(PendingOrder pendingorder)
        {
            lock (_ordertrackerlocker)
            {
                //Can only track it if it does not exists yet
                if (!TryGetOrder(pendingorder.OrderId, out PendingOrder found))
                {
                    if (!_pendingorders.TryAdd(pendingorder.OrderId, pendingorder))
                    {
                        _log.Error($"Could not add pending order with id {pendingorder.OrderId}");
                        return false;
                    }
                    else
                        return true;
                }
                else
                    _log.Error($"Pending order with id {pendingorder.OrderId} was already set to this tracker");
            }

            //Returm false
            return false;
        }

        /// <summary>
        /// Returns the pending order by order id, if known
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public bool TryGetOrder(long orderid, out PendingOrder pendingorder)
        {
            //Try and get the pending order
            pendingorder = PendingOrders.FirstOrDefault(x => x.OrderId == orderid);

            //Check if we found something
            return pendingorder != null;
        }

        /// <summary>
        /// Remove this pending order from the known pending orders list
        /// </summary>
        /// <param name="orderid"></param>
        public bool TryRemoveOrder(long orderid)
        {
            lock (_ordertrackerlocker)
            {
                return _pendingorders.TryRemove(orderid, out PendingOrder _);
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Remove all old pending orders if needed
        /// </summary>
        private void CleanUp()
        {
            //Get all orders that need cleanup
            PendingOrders.Where(x => x.OrderState.IsDone())
                         .ForEach(x => TryRemoveOrder(x.OrderId));

            //Get current amount of historical fills (keep track of last 300)
            int maxfillhistory = 300;
            if (_historicalfills.Count > maxfillhistory)
                _historicalfills
                    .OrderByDescending(x => x.Value.LocalTime)
                    .Index(0)
                    .Where(x => x.Key > maxfillhistory)
                    .Select(x => _historicalfills.TryRemove(x.Value.Key, out Fill xvalue));
        }

        #endregion Private Methods
    }
}