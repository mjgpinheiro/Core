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

using Quantler.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using Quantler.Account;
using Quantler.Interfaces;
using Quantler.Trades;

namespace Quantler.Securities.MarginModels
{
    /// <summary>
    /// Generic implementation of margin calls
    /// </summary>
    public class GenericMarginCallModel : MarginCallModel
    {
        #region Public Methods

        /// <summary>
        /// Check if margin calls are applicable and reveive any order tickets associated to this margin call
        /// </summary>
        /// <param name="quantFunds"></param>
        /// <param name="account"></param>
        /// <param name="marginwarning"></param>
        /// <returns></returns>
        public OrderTicket[] CheckMarginCall(IQuantFund[] quantFunds, BrokerAccount account, out bool marginwarning)
        {
            //Initials
            marginwarning = false;
            List<OrderTicket> toreturn = new List<OrderTicket>();

            //Check margin in use
            if (account.MarginInUse == 0)
                return toreturn.ToArray();

            //Check margin level warning
            if (account.MarginLevel <= account.MarginCallLevel)
                marginwarning = true;

            //Generate margin call orders
            decimal marginneeded = account.Equity * .2M;

            //No margin left == margin call
            if (account.FreeMargin <= marginneeded)
            {
                //Set level of margin we need to thrive for
                decimal marginprocessed = Math.Abs(account.FreeMargin - marginneeded);

                //Go trough all positions
                foreach (var position in quantFunds.SelectMany(x => x.Positions)
                                                .Where(x => !x.IsFlat)
                                                .OrderBy(x => x.NetProfit))
                {
                    //Check for order
                    var quantfund = quantFunds.First(x => x.Positions[position.Security].Quantity == position.Quantity);
                    var order = GetMarginCallOrder(quantfund.FundId, position, ref marginprocessed);
                    if (order != null && order.Quantity != 0)
                        toreturn.Add(order);

                    //Check processed margin
                    if (marginprocessed <= 0)
                        break;
                }
            }

            //Return orders
            return toreturn.ToArray();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Get margin call order
        /// </summary>
        /// <param name="fundid"></param>
        /// <param name="position"></param>
        /// <param name="marginprocessed"></param>
        /// <returns></returns>
        private SubmitOrderTicket GetMarginCallOrder(string fundid, Position position, ref decimal marginprocessed)
        {
            //Calculate units needed to process enough margin
            var marginneeded = (marginprocessed - position.MarginInUse) > 0 ? position.MarginInUse : marginprocessed - position.MarginInUse;

            //Calculate order size
            decimal quantity = (int)Math.Round((position.Quantity / position.MarginInUse) * marginneeded, MidpointRounding.AwayFromZero);

            //Set as processed margin
            marginprocessed -= (quantity / position.Quantity) * marginneeded;

            //Return new order
            return new SubmitOrderTicket(fundid, position.Security, quantity, comment: "Margin call order");
        }

        #endregion Private Methods
    }
}