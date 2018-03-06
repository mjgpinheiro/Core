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
using System.Collections.Generic;

namespace Quantler.Orders
{
    /// <summary>
    /// Quantler Order
    /// </summary>
    public interface Order
    {
        #region Public Properties

        /// <summary>
        /// broker based order id
        /// </summary>
        List<string> BrokerId { get; }

        /// <summary>
        /// order comment
        /// </summary>
        string Comment { get; }

        /// <summary>
        /// Get the order created date and time object
        /// </summary>
        DateTime CreatedUtc { get; }

        /// <summary>
        /// Direction of the order either Buy or Sell
        /// </summary>
        Direction Direction { get; }

        /// <summary>
        /// destination for order
        /// </summary>
        string ExchangeName { get; }

        /// <summary>
        /// Order expire date and time, if applicable
        /// </summary>
        DateTime? Expiry { get; }

        /// <summary>
        /// Fill policy used when processing this order
        /// </summary>
        FillPolicy FillPolicy { get; }

        /// <summary>
        /// owner/originator of this order
        /// </summary>
        string FundId { get; }

        /// <summary>
        /// Internal order id
        /// </summary>
        long InternalId { get; }

        /// <summary>
        /// price of order. (0 for market)
        /// </summary>
        decimal LimitPrice { get; }

        /// <summary>
        /// Order quantity
        /// </summary>
        decimal Quantity { get; }

        /// <summary>
        /// security/contract information for order
        /// </summary>
        Security Security { get; }

        /// <summary>
        /// current order state
        /// </summary>
        OrderState State { get; }

        /// <summary>
        /// stop price if applicable
        /// </summary>
        decimal StopPrice { get; }

        /// <summary>
        /// order time in force specification
        /// </summary>
        TimeInForce TimeInForceSpec { get; }

        /// <summary>
        /// Get Type of order
        /// </summary>
        OrderType Type { get; }

        /// <summary>
        /// unsigned quantity of order
        /// </summary>
        decimal UnsignedQuantity { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Clone the current order
        /// </summary>
        /// <returns></returns>
        Order Clone();

        #endregion Public Methods
    }
}