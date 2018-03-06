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
using NLog;
using Quantler.Broker.Model;
using Quantler.Interfaces;
using Quantler.Orders.Type;

namespace Quantler.Orders
{
    /// <summary>
    /// Factory methods for creating new orders based order tickets
    /// </summary>
    public class OrderFactory
    {
        #region Private Fields

        /// <summary>
        /// Broker model used for checking order compatibility
        /// </summary>
        private readonly BrokerModel _brokerModel;

        /// <summary>
        /// Connection used for processing orders
        /// </summary>
        private readonly IPortfolio _portfolio;

        /// <summary>
        /// Current logging instance
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderFactory"/> class.
        /// </summary>
        /// <param name="portfolio">The handler.</param>
        /// <param name="brokermodel">The brokermodel.</param>
        public OrderFactory(IPortfolio portfolio, BrokerModel brokermodel)
        {
            _portfolio = portfolio;
            _brokerModel = brokermodel;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Create pending order based on order ticket
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public PendingOrder CreateOrder(OrderTicket ticket)
        {
            //Get correct ticket information
            if (ticket.Type != OrderTicketType.Submit)
                return null;

            //Get new order ticket
            if (!(ticket is SubmitOrderTicket norderticket))
            {
                ticket.SetResponse(OrderTicketResponse.Error(ticket.OrderId, OrderTicketResponseErrorCode.ProcessingError, "Incorrect ticket type received"));
                return null;
            }

            //Check order type based on input
            Order order;
            switch (norderticket.OrderType)
            {
                case OrderType.Limit:
                    order = new LimitOrder(norderticket.Security, norderticket.FundId, norderticket.Quantity, norderticket.LimitPrice,
                    norderticket.CreatedUtc, norderticket.ExchangeName, norderticket.Comment)
                    { InternalId = _portfolio.OrderTicketHandler.GetNextInternalOrderId() };
                    break;

                case OrderType.Market:
                    order = new MarketOrder(norderticket.Security, norderticket.FundId, norderticket.Quantity,
                    norderticket.CreatedUtc, norderticket.ExchangeName, norderticket.Comment)
                    { InternalId = _portfolio.OrderTicketHandler.GetNextInternalOrderId() };
                    break;

                case OrderType.StopMarket:
                    order = new StopMarketOrder(norderticket.Security, norderticket.FundId, norderticket.Quantity,
                    norderticket.StopPrice, norderticket.CreatedUtc, norderticket.ExchangeName, norderticket.Comment)
                    { InternalId = _portfolio.OrderTicketHandler.GetNextInternalOrderId() };
                    break;

                case OrderType.StopLimit:
                    order = new StopLimitOrder(norderticket.Security, norderticket.FundId, norderticket.Quantity, norderticket.LimitPrice,
                    norderticket.StopPrice, norderticket.CreatedUtc, norderticket.ExchangeName, norderticket.Comment)
                    { InternalId = _portfolio.OrderTicketHandler.GetNextInternalOrderId() };
                    break;

                case OrderType.MarketOnOpen:
                    order = new MarketOnOpenOrder(norderticket.Security, norderticket.FundId, norderticket.Quantity, norderticket.CreatedUtc, norderticket.ExchangeName, norderticket.Comment)
                    { InternalId = _portfolio.OrderTicketHandler.GetNextInternalOrderId() };
                    break;

                case OrderType.MarketOnClose:
                    order = new MarketOnCloseOrder(norderticket.Security, norderticket.FundId, norderticket.Quantity, norderticket.CreatedUtc, norderticket.ExchangeName, norderticket.Comment)
                    { InternalId = _portfolio.OrderTicketHandler.GetNextInternalOrderId() };
                    break;

                default:
                    ticket.SetResponse(OrderTicketResponse.Error(ticket.OrderId, OrderTicketResponseErrorCode.ProcessingError, $"Unknown order type {norderticket.OrderType} supplied"));
                    return null;
            }

            //Check if order type is supported
            bool issupported;
            try
            {
                issupported = _brokerModel.IsOrderTypeSupported(order.Type);
            }
            catch (Exception exc)
            {
                _log.Error(exc, $"Could not execute brokermodel function IsOrderTypeSupported due to exception");
                throw exc;
            }

            //Create pending order
            return new PendingOrder(_portfolio, ticket.FundId, order, norderticket.Comment,
                norderticket.Security.LastTickEventUtc, issupported);
        }

        #endregion Public Methods
    }
}