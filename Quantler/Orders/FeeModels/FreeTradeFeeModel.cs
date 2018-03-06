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
using System;
using System.Composition;

namespace Quantler.Orders.FeeModels
{
    /// <summary>
    /// Robinhood applicable fees model
    /// </summary>
    [Export(typeof(FeeModel))]
    public class FreeTradeFeeModel : FeeModel
    {
        #region Public Methods

        /// <summary>
        /// Get fees for robinhood
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public decimal GetCommissionAndFees(PendingOrder order)
        {
            //FreeTrade does not charge commission, however it does have to charge regulatory trading fees
            var security = order.Security;
            if (order.Order.Direction == Direction.Short &&
                security.Exchange.TimeZone == TimeZone.NewYork)
            {
                //TAF (Trading Activity Fees) - $0.000119 p/share (sells only)
                var taffees = order.Order.UnsignedQuantity * 0.000119m;
                if (taffees > 5.95m)
                    return 5.95m;
                else
                    return Math.Round(taffees, 2, MidpointRounding.AwayFromZero);
            }
            else
                return 0m; //Free for all
        }

        /// <summary>
        /// TODO: set interest on margin for FreeTrade (margin trading is not supported yet during Beta)
        /// </summary>
        /// <param name="date"></param>
        /// <param name="fill"></param>
        /// <returns></returns>
        public decimal GetInterest(DateTime date, Fill fill) => 0m;

        #endregion Public Methods
    }
}