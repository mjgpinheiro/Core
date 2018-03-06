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

using Quantler.Data;
using Quantler.Interfaces;
using Quantler.Securities;
using System;

namespace Quantler.Orders.Type
{
    /// <summary>
    /// Creation of limit orders
    /// </summary>
    public class LimitOrder : OrderImpl
    {
        #region Public Constructors

        /// <summary>
        /// Create limit order
        /// </summary>
        /// <param name="security"></param>
        /// <param name="fundid"></param>
        /// <param name="quantity"></param>
        /// <param name="limitprice"></param>
        /// <param name="createdutc"></param>
        /// <param name="exchange"></param>
        /// <param name="comment"></param>
        public LimitOrder(Security security, string fundid, decimal quantity, decimal limitprice, DateTime createdutc,
            string exchange, string comment = "")
            : base(security, fundid, quantity, createdutc, exchange, comment)
        {
            Type = OrderType.Limit;
            LimitPrice = limitprice;
            State = limitprice > 0 ? OrderState.New : OrderState.Invalid;
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LimitOrder"/> class.
        /// </summary>
        private LimitOrder()
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
            LimitOrder newMarketOrder = new LimitOrder();
            CopyTo(this);
            return newMarketOrder;
        }

        /// <summary>
        /// Determines whether the specified prices is triggered.
        /// </summary>
        /// <param name="prices">The prices.</param>
        /// <param name="price">The price.</param>
        /// <returns>
        ///   <c>true</c> if the specified prices is triggered; otherwise, <c>false</c>.
        /// </returns>
        public bool IsTriggered(Prices prices, out decimal price)
        {
            //Current price
            price = prices.Current;

            //Check direction
            if (Direction == Direction.Long)
            {
                if (prices.Low < LimitPrice)
                {
                    // fill at the worse price this bar or the limit price, this allows far out of the money limits
                    // to be executed properly
                    price = Math.Min(prices.High, LimitPrice);
                    return true;
                }
            }
            else if (Direction == Direction.Short)
            {
                if (prices.High > LimitPrice)
                {
                    // fill at the worse price this bar or the limit price, this allows far out of the money limits
                    // to be executed properly
                    price = Math.Max(prices.Low, LimitPrice);
                    return true;
                }
            }
            else
                return true;

            //All else
            return false;
        }

        /// <inheritdoc />
        /// <summary>
        /// Update current limit order
        /// </summary>
        /// <param name="ticket"></param>
        public override void Update(UpdateOrderTicket ticket)
        {
            //Regular updates
            base.Update(ticket);

            //Set new limit price
            if (ticket.LimitPrice.HasValue)
                LimitPrice = ticket.LimitPrice.Value;
        }

        #endregion Public Methods
    }
}