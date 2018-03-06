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
using Quantler.Orders;
using System;
using System.Composition;
using System.Linq;

namespace Quantler.Bootstrap.TicketHandlers
{
    /// <summary>
    /// Order ticket handler for crypto currencies
    /// TODO: NEEDDS UNIT TESTING!
    /// </summary>
    /// <seealso cref="OrderTicketHandlerBase" />
    [Export(typeof(OrderTicketHandler))]
    public class OrderTicketHandlerCrypto : OrderTicketHandlerBase
    {
        #region Private Fields

        /// <summary>
        /// The logging instance
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        #endregion Private Fields

        #region Protected Methods

        /// <summary>
        /// Process submit order
        /// For crypto currencies: check if we have the correct amount of base currency, if not, create and additional order for getting the correct base currency amount
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        protected override OrderTicketResponse SubmitOrder(SubmitOrderTicket ticket)
        {
            //Check for associated quant fund
            var quantfund = Portfolio.QuantFunds.FirstOrDefault(x => x.FundId == ticket.FundId);

            //Check if we can get the pending order
            var cashposition = (quantfund == null ? CashManager.GetCashPositions() : CashManager.GetCashPositions(quantfund))[ticket.Security.BaseCurrency];
            decimal value = Math.Abs(ticket.Security.ConvertValue(ticket.Quantity * ticket.Security.Price, cashposition.BaseCurrency));
            if (cashposition.TotalSettledCash < value)
            {
                //We need extra cash for this in currency X
                decimal valueneeded = value - cashposition.TotalSettledCash;

                //Create and send
                var security = Portfolio.BrokerAccount.Securities[$"{cashposition.BaseCurrency}.BC"];
                var orderticket = SubmitOrderTicket.MarketOrder(ticket.FundId, security, valueneeded, $"Base currency conversion needed to execute order with id {ticket.OrderId}");
                var response = SubmitOrder(orderticket);

                //Wait for this order
                if (!response.IsError)
                    WaitForOrder(response.OrderId);
                else
                    _log.Error($"Could not process currency conversion for order with id {ticket.OrderId}, due to conversion order error {response.ErrorCode} : {response.ErrorMessage}");
            }

            //Use base implementation
            return base.SubmitOrder(ticket);
        }

        #endregion Protected Methods
    }
}