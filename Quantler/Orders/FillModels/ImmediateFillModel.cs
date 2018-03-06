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
using Quantler.Broker.Model;
using Quantler.Data;
using Quantler.Data.Bars;
using Quantler.Orders.Type;
using Quantler.Securities;
using System;

namespace Quantler.Orders.FillModels
{
    /// <summary>
    /// Default implemented behaviour for filling pending orders
    /// Immediate: will fill asap
    /// </summary>
    public class ImmediateFillBehaviour : FillModel
    {
        #region Protected Fields

        /// <summary>
        /// Current instance logging
        /// </summary>
        protected ILogger _log = LogManager.GetCurrentClassLogger();

        #endregion Protected Fields

        #region Public Methods

        /// <summary>
        /// Fill order, if possible
        /// </summary>
        /// <param name="broker">Associated broker model</param>
        /// <param name="datapoint">Currently received data point</param>
        /// <param name="pendingorder">Pending order to check for filling</param>
        /// <param name="highliquidfill">If true, size of ticks are not taken in to account</param>
        /// <returns></returns>
        public virtual Fill FillOrder(BrokerModel broker, DataPoint datapoint, PendingOrder pendingorder, bool highliquidfill) =>
                ImmediateFill(broker, datapoint, pendingorder, highliquidfill);

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Immediate fill logic, fill the order
        /// can be reused on other fill models for filling and checking
        /// </summary>
        /// <param name="broker"></param>
        /// <param name="datapoint"></param>
        /// <param name="pendingorder"></param>
        /// <param name="highliquidfill"></param>
        /// <returns></returns>
        protected Fill ImmediateFill(BrokerModel broker, DataPoint datapoint, PendingOrder pendingorder, bool highliquidfill)
        {
            //Get order
            Order order = pendingorder.Order;
            Security security = pendingorder.Security;

            //Check market conditions
            if (order.State == OrderState.Cancelled || order.State == OrderState.Error)
                return Fill.Cancelled(order.InternalId);                                        //Order has already been cancelled or has an error
            else if (order.State == OrderState.Invalid)
                return Fill.Invalid(order.InternalId, "Invalid Order received");                //Order has already been made invalid
            else if (order.State != OrderState.Submitted)
                return Fill.Error(order.InternalId, $"Unexpected order state {order.State}");   //Something else has gone wrong
            else if (!IsExchangeOpen(pendingorder.Security, datapoint))
                return Fill.NoFill();                                                           //Exchange is not opened for trading
            else if (IsOrderExpired(order, datapoint))
                return Fill.Cancelled(order.InternalId, "Order expired");                       //Check if order has been expired

            //Check latency for processing this order
            try
            {
                if (pendingorder.CreatedUtc.Add(broker.GetLatencyModel(security).GetLatency(order)) > datapoint.Occured)
                    return Fill.NoFill();                                                           //We are not suppose to see this order yet, due to added latency
            }
            catch (Exception exc)
            {
                _log.Error(exc, $"Could not execute broker latency model function for get latency.");
                throw;
            }

            //Check possible fill price
            var prices = datapoint.OrderPrice(order.Security, order.Direction);
            decimal fillprice = prices.Current;

            //Check order type and logic
            if (order is LimitOrder limitorder)
            {
                //Check if limit order price is triggered
                if (!limitorder.IsTriggered(prices, out fillprice))
                    return Fill.NoFill();
            }
            else if (order is StopLimitOrder stoplimitorder)
            {
                //Check if stop limit order price is triggered
                if (!stoplimitorder.IsTriggered(prices, out fillprice))
                    return Fill.NoFill();
            }
            else if (order is StopMarketOrder stopmarketorder)
            {
                //Check if stop order price is triggered
                if (!stopmarketorder.IsTriggered(prices, out fillprice))
                    return Fill.NoFill();
            }
            else if (order is MarketOnCloseOrder marketoncloseorder)
            {
                //Check if market on close order is triggered
                if (!marketoncloseorder.IsTriggered())
                    return Fill.NoFill();
            }
            else if (order is MarketOnOpenOrder marketonopenorder)
            {
                //Check if market on open order is triggered
                if (!marketonopenorder.IsTriggered())
                    return Fill.NoFill();
            }

            //Check slippage model
            try
            {
                fillprice += broker.GetSlippageModel(security).GetSlippage(order);
            }
            catch (Exception exc)
            {
                _log.Error(exc, $"Could not execute broker slippage model function for get slippage.");
                throw;
            }

            //Check additional spread
            try
            {
                fillprice += broker.GetSpreadModel(security).GetAdditionalSpread(order);
            }
            catch (Exception exc)
            {
                _log.Error(exc, $"Could not execute broker spread model function for get additional spread.");
                throw;
            }

            //Check fill amount (based on liquidity)
            if (!highliquidfill)
            {
                //Check fill size possible, according to tick
                var filled = datapoint.OrderSize(order.Direction);

                //Needed to fill (in case we partially already filled some of the order)
                decimal neededfill = order.UnsignedQuantity - pendingorder.OrderFilledQuantity;

                //Check for partial fills (will not include fees)
                if (filled < neededfill)
                    return Fill.PartialFill(order, pendingorder.Security.Exchange.Name, fillprice, 0m, filled);
                else
                    //Process needed fill (will include fees)
                    try
                    {
                        return Fill.FullFill(order, pendingorder.Security.Exchange.Name, fillprice, Math.Abs(broker.GetFeeModel(pendingorder.Security).GetCommissionAndFees(pendingorder)), neededfill);
                    }
                    catch (Exception exc)
                    {
                        _log.Error(exc, $"Could not process full fill, due to broker feemodel implementation.");
                        throw;
                    }
            }
            else
                //Process full fill (will include fees)
                try
                {
                    return Fill.FullFill(order, pendingorder.Security.Exchange.Name, fillprice, Math.Abs(broker.GetFeeModel(pendingorder.Security).GetCommissionAndFees(pendingorder)), order.UnsignedQuantity);
                }
                catch (Exception exc)
                {
                    _log.Error(exc, $"Could not process full fill, due to broker feemodel implementation.");
                    throw;
                }
        }

        /// <summary>
        /// Check if market is opened
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        protected bool IsExchangeOpen(Security security, DataPoint datapoint) =>
            datapoint is DataPointBar bar ? security.Exchange.IsOpenDuringBar(bar) : security.Exchange.IsOpen;

        /// <summary>
        /// Check if order has been expired
        /// </summary>
        /// <param name="order"></param>
        /// <param name="datapoint"></param>
        /// <returns></returns>
        protected bool IsOrderExpired(Order order, DataPoint datapoint) =>
            order.TimeInForceSpec == TimeInForce.GTC ? false :
            order.TimeInForceSpec == TimeInForce.DAY ? datapoint.OccuredUtc > order.CreatedUtc.Date :
            order.TimeInForceSpec == TimeInForce.MOC ? datapoint.OccuredUtc > order.CreatedUtc.Date :   //TODO: Should be on close of market
            order.TimeInForceSpec == TimeInForce.GTD ? datapoint.OccuredUtc > order.Expiry.Value :
            true;

        #endregion Protected Methods
    }
}