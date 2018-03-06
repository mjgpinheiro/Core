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

namespace Quantler.Orders
{
    /// <summary>
    /// State at which an order ticket can be
    /// TODO: add state cancelled and check it on processing an order ticket?
    /// </summary>
    public enum OrderTicketState
    {
        /// <summary>
        /// Order ticket has been processed
        /// </summary>
        Processed,

        /// <summary>
        /// Order ticket has not been processed yet
        /// </summary>
        Unprocessed,

        /// <summary>
        /// An error occured while processing this ticket
        /// </summary>
        Error,

        /// <summary>
        /// Ticket is being processed
        /// </summary>
        Processing
    }
}