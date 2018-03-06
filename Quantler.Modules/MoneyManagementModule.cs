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
using Quantler.Orders;
using System;
using Quantler.Securities;

namespace Quantler.Modules
{
    /// <summary>
    /// Money management module implementation
    /// </summary>
    public abstract class MoneyManagementModule : TradingModule
    {
        #region Public Methods

        /// <summary>
        /// Calculate order size for signal order
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="state"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public virtual decimal OrderQuantity(SubmitOrderTicket ticket, SecurityState state, decimal weight) =>
            throw new NotImplementedException("OrderQuantity should be implemented for a money management module to function.");

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Calculate size needed for order to get to target weight
        /// TODO: idea is OrderSize = TargetWeight(security, 0.15m) => returns the size needed to get to 15% based on allocated amount
        ///         + if cash account, we cannot short!
        /// </summary>
        /// <param name="security"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        protected decimal TargetWeight(Security security, decimal weight)
        {
            //Check if we have data
            if (security.AskPrice == 0)
                return 0;
            //Check input
            else if (weight < 0)
                return 0;
            //Check if we need to set it to 0
            else if (weight == 0)
                return -Position[security].Quantity;

            //Get current account data
            var account = Account;

            //Check value we are targeting
            decimal targetvalue = account.Equity * weight;
            decimal currentvalue = Position[security].TotalValue;
            targetvalue = Math.Abs(currentvalue - targetvalue);

            //Check order size
            Direction direction = targetvalue > currentvalue ? Direction.Long : Direction.Short;
            decimal unitvalue = currentvalue / Position[security].UnsignedQuantity;
            decimal ordersize = targetvalue / unitvalue; //Check minimum size and increment

            //Check remaining buying power
            if (account.BuyingPower - (unitvalue * ordersize) <= 0)
                return 0;

            //Check to stay within margin requirements
            decimal marginrequired = 0;
            decimal ordervalue = 0;
            decimal orderfees = 0;

            while (ordersize > 0 && (marginrequired > account.BuyingPower || ordervalue + orderfees > targetvalue))
            {
            }

            //Return target size
            return direction == Direction.Long ? ordersize : -ordersize;
        }

        #endregion Protected Methods
    }
}