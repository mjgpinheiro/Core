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

using MoreLinq;
using Quantler.Data;
using Quantler.Orders;
using System.Collections.Generic;

namespace Quantler.Fund
{
    /// <summary>
    /// Partial class is used for managing quant fund module events
    /// </summary>
    public partial class QuantFund
    {
        #region Public Methods

        /// <summary>
        /// Called when data is received.
        /// </summary>
        /// <param name="data">The data.</param>
        public void OnData(DataUpdates data)
        {
            //get the data that belongs to this quant fund
            var filtereddata = data[Universe];
            var cloneddata = filtereddata.Clone();

            //Send data to modules
            if (IsRunning)
                Modules.ForEach(x => x.OnData(cloneddata));
        }

        /// <summary>
        /// Called at the end of each trading day.
        /// </summary>
        public void OnEndOfDay()
        { 
            if(IsRunning)
                Modules.ForEach(x => x.OnEndOfDay());
        }

        /// <summary>
        /// Called when a margin call has occurred.
        /// </summary>
        /// <param name="tickets">The tickets.</param>
        public void OnMarginCall(List<SubmitOrderTicket> tickets)
        {
            if(IsRunning)
                Modules.ForEach(x => x.OnMarginCall(tickets));
        }

        /// <summary>
        /// Called when an order ticket event has occurred.
        /// TODO: not implemented in the flow => so this needs to be implemented in the flow
        /// </summary>
        /// <param name="orderticketevent">The order ticket event.</param>
        public void OnOrderTicketEvent(OrderTicketEvent orderticketevent)
        {
            if(IsRunning)
                Modules.ForEach(x => x.OnOrderTicketEvent(orderticketevent));
        }

        /// <summary>
        /// Called when a quant fund is terminated.
        /// </summary>
        public void OnTermination()
        {
            //Process each on termination event handler
            bool isliquidate = true;
            Modules.ForEach(x =>
            {
                x.OnTermination(out bool liquidate);
                if (!liquidate)
                    isliquidate = false;
            });

            //Check if we need to liquidate
            if (isliquidate)
            {
                //Cancel all pending orders
                var po = PendingOrders.Cancel();
                po.ForEach(o => _log.Info($"Cancelled the following pending order on terminate: {o.OrderId} for fund with id {o.FundId}"));

                //Liquidate current quant fund
                Liquidate();
            }
        }

        #endregion Public Methods
    }
}