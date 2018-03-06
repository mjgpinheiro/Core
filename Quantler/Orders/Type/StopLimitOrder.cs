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
    /// Stop order which on the hit of the stopprice will convert to a limit order
    /// </summary>
    public class StopLimitOrder : OrderImpl
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StopLimitOrder"/> class.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="fundid">The fundid.</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="limitprice">The limitprice.</param>
        /// <param name="stopprice">The stopprice.</param>
        /// <param name="createdutc">The createdutc.</param>
        /// <param name="exchange">The exchangeModel.</param>
        /// <param name="comment">The comment.</param>
        public StopLimitOrder(Security security, string fundid, decimal quantity, decimal limitprice, decimal stopprice,
            DateTime createdutc, string exchange, string comment = "")
            : base(security, fundid, quantity, createdutc, exchange, comment)
        {
            LimitPrice = limitprice;
            StopPrice = stopprice;
            State = StopPrice > 0 && limitprice > 0 ? OrderState.New : OrderState.Invalid;
            Type = OrderType.StopLimit;
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StopLimitOrder"/> class.
        /// </summary>
        private StopLimitOrder()
        {
        }

        #endregion Private Constructors

        #region Private Properties

        /// <summary>
        /// Stop was triggered indication
        /// </summary>
        private bool Triggered { get; set; }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Clone the current order object
        /// </summary>
        /// <returns></returns>
        public override Order Clone()
        {
            StopLimitOrder newMarketOrder = new StopLimitOrder();
            CopyTo(this);
            newMarketOrder.Triggered = Triggered;
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
                if (prices.High > StopPrice || Triggered)
                {
                    Triggered = true;

                    if (price < LimitPrice)
                    {
                        // Note > Can't use minimum price, because no way to be sure minimum wasn't before the stop triggered.
                        price = LimitPrice;
                        return true;
                    }
                }
            }
            else if (Direction == Direction.Short)
            {
                if (prices.Low < StopPrice || Triggered)
                {
                    Triggered = true;

                    if (price > LimitPrice)
                    {
                        // Note > Can't use minimum price, because no way to be sure minimum wasn't before the stop triggered.
                        price = LimitPrice;
                        return true;
                    }
                }
            }
            else
                return true;

            //All else
            return false;
        }

        /// <summary>
        /// Update current stop limit order
        /// </summary>
        /// <param name="ticket"></param>
        public override void Update(UpdateOrderTicket ticket)
        {
            //Regular updates
            base.Update(ticket);

            //Set new values
            if (ticket.StopPrice.HasValue)
                StopPrice = ticket.StopPrice.Value;
            if (ticket.LimitPrice.HasValue)
                LimitPrice = ticket.LimitPrice.Value;
        }

        #endregion Public Methods
    }
}