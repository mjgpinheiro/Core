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
    /// A stop order is an order to buy or sell a security when its price surpasses a particular point, thus ensuring a greater probability of
    /// achieving a predetermined entry or exit price, limiting the investor's loss or locking in his or her profit.
    /// Once the price surpasses the predefined entry/exit point, the stop order becomes a market order.
    /// </summary>
    /// <seealso cref="OrderImpl" />
    public class StopMarketOrder : OrderImpl
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StopMarketOrder"/> class.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="fundid">The fundid.</param>
        /// <param name="quantity">The current order quantity.</param>
        /// <param name="stopprice">The stopprice.</param>
        /// <param name="createdutc">The createdutc.</param>
        /// <param name="exchange">The exchangeModel.</param>
        /// <param name="comment">The comment.</param>
        public StopMarketOrder(Security security, string fundid, decimal quantity, decimal stopprice, DateTime createdutc, string exchange, string comment = "")
            : base(security, fundid, quantity, createdutc, exchange, comment)
        {
            StopPrice = stopprice;
            State = stopprice > 0 ? OrderState.New : OrderState.Invalid;
            Type = OrderType.StopMarket;
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StopMarketOrder"/> class.
        /// </summary>
        private StopMarketOrder()
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
            StopMarketOrder newStopMarketOrder = new StopMarketOrder();
            CopyTo(this);
            return newStopMarketOrder;
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
                if (prices.High > StopPrice)
                {
                    price = Math.Max(StopPrice, price);
                    return true;
                }
            }
            else if (Direction == Direction.Short)
            {
                if (prices.Low < StopPrice)
                {
                    price = Math.Min(StopPrice, price);
                    return true;
                }
            }
            else
                return true;

            //All else
            return false;
        }

        /// <summary>
        /// Set data based on order update
        /// </summary>
        /// <param name="ticket"></param>
        public override void Update(UpdateOrderTicket ticket)
        {
            //Regular updates
            base.Update(ticket);

            //Set new values
            if (ticket.StopPrice.HasValue)
                StopPrice = ticket.StopPrice.Value;
        }

        #endregion Public Methods
    }
}