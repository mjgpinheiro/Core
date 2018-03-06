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

namespace Quantler.Orders.Type
{
    /// <summary>
    /// An investor makes a market order through a broker or brokerage
    /// service to buy or sell an investment immediately at the best available current price.
    /// A market order is the default option and is likely to be executed because it does
    /// not contain restrictions on the price or the time frame in which the order can be executed.
    /// A market order is also sometimes referred to as an unrestricted order.
    /// </summary>
    public class MarketOrder : OrderImpl
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketOrder"/> class.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="fundid">The fundid.</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="createdutc">The createdutc.</param>
        /// <param name="exchange">The exchangeModel.</param>
        /// <param name="comment">The comment.</param>
        public MarketOrder(Security security, string fundid, decimal quantity, DateTime createdutc, string exchange, string comment = "")
            : base(security, fundid, quantity, createdutc, exchange, comment)
        {
            Type = OrderType.Market;
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketOrder"/> class.
        /// </summary>
        private MarketOrder()
        {
        }

        #endregion Private Constructors

        #region Public Methods

        /// <summary>
        /// Clone the current order object
        /// </summary>
        /// <returns></returns>
        public override Order Clone()
        {
            MarketOrder newMarketOrder = new MarketOrder();
            CopyTo(this);
            return newMarketOrder;
        }

        #endregion Public Methods
    }
}