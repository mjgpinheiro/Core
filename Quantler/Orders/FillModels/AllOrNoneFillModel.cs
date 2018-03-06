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

using Quantler.Broker.Model;
using Quantler.Data;
using System;

namespace Quantler.Orders.FillModels
{
    /// <summary>
    /// All or none (AON) is an instruction used on a buy or sell order that instructs the
    /// broker to fill the order completely or not at all. If there are not enough shares
    /// available to fill the order completely, the order is canceled when the market closes.
    /// An AON order is considered a duration order because the investor provides instructions
    /// to the trader about how the order must be filled, which impacts how long the order remains active.
    /// </summary>
    public class AllOrNoneFillModel : ImmediateFillBehaviour
    {
        #region Public Methods

        /// <summary>
        /// Fill order, if possible
        /// </summary>
        /// <param name="broker">Associated broker model</param>
        /// <param name="datapoint">Currently received data point</param>
        /// <param name="pendingorder">Pending order to check for filling</param>
        /// <param name="highliquidfill">If true, size of ticks are not taken in to account</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override Fill FillOrder(BrokerModel broker, DataPoint datapoint, PendingOrder pendingorder, bool highliquidfill)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}