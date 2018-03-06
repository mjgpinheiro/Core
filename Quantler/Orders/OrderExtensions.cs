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

using System;
using System.Collections.Generic;

namespace Quantler.Orders
{
    /// <summary>
    /// Extensions for pending orders
    /// </summary>
    public static class OrderExtensions
    {
        #region Public Methods

        /// <summary>
        /// Cancel all orders in the current IEnumerable
        /// </summary>
        /// <param name="source"></param>
        public static IEnumerable<CancelOrderTicket> Cancel(this IEnumerable<PendingOrder> source)
        {
            foreach (var pendingorder in source)
            {
                //Set as return
                yield return pendingorder.Cancel() as CancelOrderTicket;
            }
        }

        /// <summary>
        /// Update all orders in the current IEnumerable
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="updateAction">The update action.</param>
        /// <returns></returns>
        public static IEnumerable<UpdateOrderTicket> Update(this IEnumerable<PendingOrder> source, Action<OrderUpdate> updateAction)
        {
            //Go trough each pending order and execute action
            foreach (var pendingorder in source)
            {
                //Set as return
                yield return pendingorder.Update(updateAction) as UpdateOrderTicket;
            }
        }

        /// <summary>
        /// Determines whether this order state is done and therefore a final state.
        /// </summary>
        /// <param name="orderstate">The orderstate.</param>
        /// <returns>
        ///   <c>true</c> if the specified orderstate is done; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDone(this OrderState orderstate) =>
            orderstate == OrderState.Filled ||
            orderstate == OrderState.Error ||
            orderstate == OrderState.Cancelled;

        #endregion Public Methods
    }
}