#region License Header
/*
*
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
*
*/
#endregion License Header

using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Quantler.Interfaces;
using Quantler.Modules;
using Quantler.Orders;
using Quantler.Securities;

namespace Quantler.Local.Modules.RiskManagement
{
    /// <summary>
    /// Trading is only allowed if there are no other active order tickets
    /// </summary>
    /// <seealso cref="Quantler.Modules.RiskManagementModule" />
    [Export(typeof(IModule))]
    public class MaxOneActiveTicket : RiskManagementModule
    {
        /// <summary>
        /// Check if we are allowed to perform trades
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        public override bool IsTradingAllowed(Security security) =>
            OrderTickets.Count(x => x.Security == security && x.State != OrderTicketState.Processed) == 0;

        /// <summary>
        /// Perform risk management checks and retrieve new orders to offset risks
        /// </summary>
        /// <param name="orderticket"></param>
        /// <param name="state"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public override IEnumerable<OrderTicket> RiskManagement(SubmitOrderTicket orderticket, SecurityState state, decimal weight) =>
            Nothing;
    }
}
