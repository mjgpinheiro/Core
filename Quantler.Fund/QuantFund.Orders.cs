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
using Quantler.Interfaces;
using Quantler.Modules;
using Quantler.Orders;
using Quantler.Securities;
using System;
using System.Linq;
using NLog;

namespace Quantler.Fund
{
    /// <summary>
    /// Order tracking and information functions
    /// </summary>
    public partial class QuantFund
    {
        #region Protected Fields

        /// <summary>
        /// The maximum amount of orders per day
        /// </summary>
        protected int MaxOrdersPerDay = Int32.MaxValue;

        #endregion Protected Fields

        #region Private Fields

        /// <summary>
        /// The current day orders amount
        /// </summary>
        private int _currentDayOrders;

        /// <summary>
        /// The maximum orders day notation
        /// </summary>
        private DateTime _maxOrdersDay = DateTime.MinValue;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Processes the order according to the order flow for a quant fund.
        /// </summary>
        /// <param name="orderticket">The orderticket.</param>
        /// <param name="useflow"></param>
        public OrderTicket ProcessTicket(OrderTicket orderticket, bool useflow = true)
        {
            //Go trough the modules flow
            var rm = Modules.FirstOrDefault(x => x is RiskManagementModule) as RiskManagementModule;
            var mm = Modules.FirstOrDefault(x => x is MoneyManagementModule) as MoneyManagementModule;
            var result = OrderTicketResponse.Success(orderticket.OrderId);

            if (orderticket is SubmitOrderTicket submitorderticket)
            {
                //Get risk management orders associated to this order
                if (rm != null && useflow)
                {
                    //Check risk management module, trading allowed
                    try
                    {
                        if (!rm.IsTradingAllowed(orderticket.Security))
                            result = OrderTicketResponse.Error(orderticket.OrderId, OrderTicketResponseErrorCode.RiskManagementNotAllowed, $"Risk management module ({rm.Name}) did not allow trading");
                    }
                    catch (Exception exc)
                    {
                        _log.Error(exc, $"Could not process user risk management method IsTradingAllowed for module with name {rm.Name}");
                        Portfolio.ExceptionHandler.HandleException(exc, FundId);
                    }

                    //Check risk management module, additional orders
                    try
                    {
                        var orders = rm.RiskManagement(submitorderticket, GetState(orderticket.Security),
                            Universe.GetWeight(orderticket.Security)).ToArray();
                        if (orders.Length > 0)
                        {
                            //Process additional orders before these orders
                            orders.ForEach(o => ProcessTicket(o, false));
                        }
                    }
                    catch (Exception exc)
                    {
                        _log.Error(exc, $"Could not process user risk management method RiskManagement for module with name {rm.Name}");
                        Portfolio.ExceptionHandler.HandleException(exc, FundId);
                    }
                }

                //Check money management
                if (mm != null && useflow)
                {
                    try
                    {
                        //Get new order quantity
                        submitorderticket.Quantity = mm.OrderQuantity(submitorderticket, GetState(orderticket.Security), Universe.GetWeight(orderticket.Security));
                    }
                    catch (Exception exc)
                    {
                        _log.Error(exc, $"Could not process user money management method OrderQuantity for module with name {mm.Name}");
                        Portfolio.ExceptionHandler.HandleException(exc, FundId);
                    }
                }

                //Check if the exchangeModel is open, else convert to a market on open order
                if (!submitorderticket.Security.Exchange.IsOpen && submitorderticket.OrderType == OrderType.Market)
                {
                    submitorderticket.OrderType = OrderType.MarketOnOpen;
                    Portfolio.Log(LogLevel.Debug, $"Converted order from market to market on open due to closed exchangeModel {orderticket.Security.Exchange.Name}", FundId);
                }
            }

            //General checks
            if (result.IsSuccess)
                result = PreCheck(orderticket);

            //Check result
            if (result.IsError)
            {
                orderticket.SetResponse(result);
                return orderticket;
            }
            else //process orderticket
                return Portfolio.OrderTicketHandler.Process(orderticket);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Perform pre-checks on the order ticket
        /// </summary>
        /// <param name="orderticket">The orderticket.</param>
        /// <returns></returns>
        private OrderTicketResponse PreCheck(OrderTicket orderticket)
        {
            //Check if we know this security
            if (orderticket.Security is UnknownSecurity)
                return OrderTicketResponse.Error(orderticket.OrderId, OrderTicketResponseErrorCode.MissingSecurity, $"Security with ticker name {orderticket.Security.Ticker} is unknown");

            //Check if we are running and thus allowed to send this order
            if (!IsRunning)
                return OrderTicketResponse.Error(orderticket.OrderId, OrderTicketResponseErrorCode.PreOrderChecksError, $"Quant fund is not in a running state, current state is {State} for quant fund {FundId}");

            //Check asset price
            if (orderticket.Security.Price == 0)
                return OrderTicketResponse.Error(orderticket.OrderId, OrderTicketResponseErrorCode.SecurityPriceZero, $"No pricing data for security with ticker {orderticket.Security.Ticker}");

            //Check if trading in this security is possible
            if (orderticket.Security.IsHalted)
                return OrderTicketResponse.Error(orderticket.OrderId, OrderTicketResponseErrorCode.NonTradableSecurity, $"Security with ticker {orderticket.Security.Ticker} trading is halted");

            //Check submit orders
            if (orderticket is SubmitOrderTicket submitOrderTicket)
            {
                //Check order size
                if (submitOrderTicket.Quantity == 0)
                    return OrderTicketResponse.Error(orderticket.OrderId, OrderTicketResponseErrorCode.OrderQuantityZero, "Order quantity of zero is not allowed");

                //Check if exchangeModel is opened for a market on close order
                if (!submitOrderTicket.Security.Exchange.IsOpen && submitOrderTicket.OrderType == OrderType.MarketOnClose)
                    return OrderTicketResponse.Error(orderticket.OrderId, OrderTicketResponseErrorCode.ExchangeNotOpen, $"Exchange {submitOrderTicket.Security.Exchange.Name} is not opened on {Portfolio.Clock.CurrentUtc} UTC");

                //Check market on close order
                if (submitOrderTicket.OrderType == OrderType.MarketOnClose)
                {
                    var nextmarketclose =
                        orderticket.Security.Exchange.NextMarketOpen(orderticket.Security.LocalTime, false);
                    //Must be submitted within a margin of at least 10 minutes
                    var bufferedlastsubmission = nextmarketclose.AddMinutes(-16);
                    if (!submitOrderTicket.Security.Exchange.IsOpen || submitOrderTicket.Security.Exchange.LocalTime > bufferedlastsubmission)
                        return OrderTicketResponse.Error(orderticket.OrderId, OrderTicketResponseErrorCode.MarketOnCloseOrderTooLate, "Market on close orders must be placed at least 16 minutes before market close");
                }

                //Check if we are backfilling
                if(IsBackfilling)
                    return OrderTicketResponse.Error(orderticket.OrderId, OrderTicketResponseErrorCode.QuantFundBackfilling, $"Cannot send orders while backfilling {Name}");
            }

            //Check if conversion price is available
            if (orderticket.Security.ConvertValue(1, CurrencyType.USD) == 0)
                return OrderTicketResponse.Error(orderticket.OrderId, OrderTicketResponseErrorCode.ConversionRateZero, $"Could not convert value to different currency for security with ticker {orderticket.Security.Ticker}");

            //Check max orders per day
            if (_maxOrdersDay < orderticket.Security.Exchange.UtcTime.Date)
            {
                _maxOrdersDay = orderticket.Security.Exchange.UtcTime.Date;
                _currentDayOrders = 1;
            }
            else if (_currentDayOrders >= MaxOrdersPerDay)
                return OrderTicketResponse.Error(orderticket.OrderId,
                    OrderTicketResponseErrorCode.ExceededMaximumOrders,
                    $"Maximum amount of orders per day of {MaxOrdersPerDay} has been reached");
            else
                _currentDayOrders++;

            //Passed all tests
            return OrderTicketResponse.Success(orderticket.OrderId);
        }

        #endregion Private Methods
    }
}