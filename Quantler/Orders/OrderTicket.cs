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

using Quantler.Securities;
using System;

namespace Quantler.Orders
{
    /// <summary>
    /// Local implementation of a order ticket
    /// </summary>
    public class OrderTicket
    {
        #region Protected Constructors

        /// <summary>
        /// Default constructor for new order tickets
        /// </summary>
        /// <param name="fundid"></param>
        /// <param name="security"></param>
        /// <param name="orderid"></param>
        protected OrderTicket(string fundid, Security security, long orderid)
        {
            FundId = fundid;
            CreatedUtc = security.Exchange.UtcTime;
            OrderId = orderid;
            Security = security;
        }

        #endregion Protected Constructors

        #region Public Properties

        /// <summary>
        /// Date and time in utc this ticket was created
        /// </summary>
        public DateTime CreatedUtc
        {
            get;
            protected set;
        }

        /// <summary>
        /// Reference order id for this ticket
        /// </summary>
        public long OrderId
        {
            get;
            protected set;
        }

        /// <summary>
        /// Response for processing this order ticket
        /// </summary>
        public OrderTicketResponse Response
        {
            get;
            protected set;
        } = OrderTicketResponse.Unprocessed();

        /// <summary>
        /// Ticket associated security
        /// </summary>
        public Security Security
        {
            get;
            protected set;
        }

        /// <summary>
        /// Associated quant fund
        /// </summary>
        public string FundId
        {
            get;
            protected set;
        }

        /// <summary>
        /// State at which this order ticket is in
        /// </summary>
        public OrderTicketState State { get; protected set; } = OrderTicketState.Unprocessed;

        /// <summary>
        /// Type of order ticket (Submit, Cancel, Update)
        /// </summary>
        public OrderTicketType Type
        {
            get;
            protected set;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Set the response for this order ticket
        /// TODO: needs to be internal and protected
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="state">The state.</param>
        public void SetResponse(OrderTicketResponse response, OrderTicketState state = OrderTicketState.Error)
        {
            Response = response;
            State = state;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() =>
            $"OrderTicket ({Type}) - QuantFundId: {FundId}, CreatedUtc: {CreatedUtc}, OrderId: {OrderId}, State: {State} ";

        #endregion Public Methods
    }
}