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

using Quantler.Interfaces;
using Quantler.Securities;
using System;

namespace Quantler.Orders
{
    /// <summary>
    /// Order fill
    /// </summary>
    public class Fill
    {
        #region Protected Constructors

        /// <summary>
        /// Only for internal usage
        /// </summary>
        protected Fill() { }

        #endregion Protected Constructors

        #region Public Properties

        /// <summary>
        /// Direction of the fill
        /// </summary>
        public Direction Direction { get; protected set; }

        /// <summary>
        /// Exchange name at which this fill took place
        /// </summary>
        public string ExchangeName { get; protected set; }

        /// <summary>
        /// Fees associated with this fill (in security base currency)
        /// </summary>
        public decimal FillFee { get; protected set; }

        /// <summary>
        /// Price at which it is filled
        /// </summary>
        public decimal FillPrice { get; protected set; }

        /// <summary>
        /// Currency of the fill
        /// </summary>
        public CurrencyType FillPriceCurrency { get; protected set; }

        /// <summary>
        /// Quantity that has been filled
        /// </summary>
        public decimal FillQuantity { get; protected set; }

        /// <summary>
        /// Get value that was filled in security base currency
        /// </summary>
        public decimal FillValue => FillQuantity * FillPrice;

        /// <summary>
        /// Local time at which this fill took place
        /// </summary>
        public DateTime LocalTime { get; protected set; }

        /// <summary>
        /// Any messages associated with this fill
        /// </summary>
        public string Message { get; protected set; }

        /// <summary>
        /// Associated order id causing this fill
        /// </summary>
        public long OrderId { get; protected set; }

        /// <summary>
        /// Associated security
        /// </summary>
        public Security Security { get; protected set; }

        /// <summary>
        /// New order status, if applicable
        /// </summary>
        public OrderState Status { get; protected set; }

        /// <summary>
        /// Utc date and time fill occured
        /// </summary>
        public DateTime UtcTime { get; protected set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Adjusted fill (in case we have a position and a fill closed another fill partially)
        /// </summary>
        /// <param name="fill"></param>
        /// <param name="price"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static Fill AdjustedFill(Fill fill, decimal price, decimal quantity) =>
            FullFill(fill.Direction, fill.ExchangeName, fill.FillFee, price, quantity, fill.FillPriceCurrency, fill.OrderId, fill.Security, fill.Status, fill.Message);

        /// <summary>
        /// Invalid order
        /// </summary>
        /// <param name="orderid">The orderid.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static Fill Cancelled(long orderid, string message = "") =>
            new Fill { OrderId = orderid, Status = OrderState.Cancelled, Message = message };

        /// <summary>
        /// Error during fill of order
        /// </summary>
        /// <param name="orderid">The orderid.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static Fill Error(long orderid, string message) =>
            new Fill { OrderId = orderid, Status = OrderState.Error, Message = message };

        /// <summary>
        /// New full order fill
        /// </summary>
        /// <param name="po">The po.</param>
        /// <param name="exchangename">The exchangename.</param>
        /// <param name="price">The price.</param>
        /// <param name="fees">The fees.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="state">The state.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static Fill FullFill(Order po, string exchangename, decimal price, decimal fees, decimal quantity, OrderState state = OrderState.Filled, string message = "") =>
            new Fill
            {
                Direction = po.Direction,
                ExchangeName = exchangename,
                FillFee = fees,
                FillPrice = price,
                FillQuantity = quantity,
                LocalTime = po.Security.LocalTime,
                Security = po.Security,
                Message = message,
                OrderId = po.InternalId,
                Status = state,
                FillPriceCurrency = po.Security.BaseCurrency,
                UtcTime = po.Security.Exchange.UtcTime
            };

        /// <summary>
        /// New full order fill
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="exchangename">The exchangename.</param>
        /// <param name="fillfee">The fillfee.</param>
        /// <param name="fillprice">The fillprice.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="fillpricecurrency">The fillpricecurrency.</param>
        /// <param name="orderid">The orderid.</param>
        /// <param name="security">The security.</param>
        /// <param name="state">The state.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static Fill FullFill(Direction direction, string exchangename, decimal fillfee, decimal fillprice, decimal quantity, CurrencyType fillpricecurrency, long orderid, Security security, OrderState state = OrderState.Filled, string message = "")
        {
            return new Fill
            {
                Direction = direction,
                ExchangeName = exchangename,
                FillFee = fillfee,
                FillPrice = fillprice,
                FillPriceCurrency = fillpricecurrency,
                OrderId = orderid,
                Security = security,
                Status = state,
                Message = message,
                FillQuantity = quantity,
                LocalTime = security.Exchange.LocalTime,
                UtcTime = security.Exchange.UtcTime
            };
        }

        /// <summary>
        /// Invalid order
        /// </summary>
        /// <param name="orderid">The orderid.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static Fill Invalid(long orderid, string message) =>
            new Fill { OrderId = orderid, Status = OrderState.Invalid, Message = message };

        /// <summary>
        /// Create a no filled fill
        /// </summary>
        /// <returns></returns>
        public static Fill NoFill() =>
            new Fill { Status = OrderState.Submitted };

        /// <summary>
        /// Partially filled order
        /// </summary>
        /// <param name="po">The po.</param>
        /// <param name="exchangename">The exchangename.</param>
        /// <param name="price">The price.</param>
        /// <param name="fees">The fees.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static Fill PartialFill(Order po, string exchangename, decimal price, decimal fees, decimal quantity, string message = "") =>
            FullFill(po, exchangename, price, fees, quantity, OrderState.PartialFilled, message);

        #endregion Public Methods
    }
}