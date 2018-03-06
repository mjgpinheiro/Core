#region License Header

/*
*
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
*
*/

#endregion License Header

namespace Quantler.Orders
{
    /// <summary>
    /// Orderticket event from broker connection
    /// </summary>
    public class OrderTicketEvent
    {
        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="OrderTicketEvent"/> class from being created.
        /// </summary>
        private OrderTicketEvent()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets the comment.
        /// </summary>
        public string Comment { get; private set; }

        /// <summary>
        /// Associated fill, if applicable
        /// </summary>
        public Fill Fill { get; private set; }

        /// <summary>
        /// Internal order id as reference
        /// </summary>
        public long OrderId { get; private set; }

        /// <summary>
        /// Gets the state of the order.
        /// </summary>
        /// <value>
        /// The state of the order.
        /// </value>
        public OrderState OrderState { get; private set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Cancelled the specified orderid.
        /// </summary>
        /// <param name="orderid">The orderid.</param>
        /// <param name="comment">The comment.</param>
        /// <returns></returns>
        public static OrderTicketEvent Cancelled(long orderid, string comment = "") =>
            new OrderTicketEvent
            {
                Fill = null,
                OrderId = orderid,
                OrderState = OrderState.Cancelled,
                Comment = comment
            };

        /// <summary>
        /// Order has had errors
        /// </summary>
        /// <param name="orderid">The orderid.</param>
        /// <param name="comment">The comment.</param>
        /// <returns></returns>
        public static OrderTicketEvent Error(long orderid, string comment = "") =>
            new OrderTicketEvent
            {
                Fill = null,
                OrderId = orderid,
                Comment = comment,
                OrderState = OrderState.Error
            };

        /// <summary>
        /// Filled the specified orderid.
        /// </summary>
        /// <param name="orderid">The orderid.</param>
        /// <param name="fill">The fill.</param>
        /// <param name="comment">The comment.</param>
        /// <returns></returns>
        public static OrderTicketEvent Filled(long orderid, Fill fill, string comment = "") =>
            new OrderTicketEvent
            {
                Fill = fill,
                OrderId = orderid,
                OrderState = OrderState.Filled,
                Comment = comment
            };

        /// <summary>
        /// Partially filled order.
        /// </summary>
        /// <param name="orderid">The orderid.</param>
        /// <param name="fill">The fill.</param>
        /// <param name="comment">The comment.</param>
        /// <returns></returns>
        public static OrderTicketEvent PartiallyFilled(long orderid, Fill fill, string comment = "") =>
            new OrderTicketEvent
            {
                Fill = fill,
                OrderId = orderid,
                OrderState = OrderState.PartialFilled,
                Comment = comment
            };

        /// <summary>
        /// Submitted order.
        /// </summary>
        /// <param name="orderid">The orderid.</param>
        /// <param name="comment">The comment.</param>
        /// <returns></returns>
        public static OrderTicketEvent Submitted(long orderid, string comment = "") =>
            new OrderTicketEvent
            {
                Fill = null,
                OrderId = orderid,
                OrderState = OrderState.Submitted,
                Comment = comment
            };

        /// <summary>
        /// Updated the specified orderid.
        /// </summary>
        /// <param name="orderid">The orderid.</param>
        /// <param name="comment">The comment.</param>
        /// <returns></returns>
        public static OrderTicketEvent Updated(long orderid, string comment = "") =>
            new OrderTicketEvent
            {
                Fill = null,
                OrderId = orderid,
                OrderState = OrderState.Updated,
                Comment = comment
            };

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() =>
            $"{OrderState}-{OrderId}: {Comment}";

        #endregion Public Methods
    }
}