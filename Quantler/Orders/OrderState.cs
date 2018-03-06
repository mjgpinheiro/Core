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
    public enum OrderState
    {
        /// <summary>
        /// Newly created order
        /// </summary>
        New,

        /// <summary>
        /// Order has been submitted to the broker
        /// </summary>
        Submitted,

        /// <summary>
        /// Order is invalid
        /// </summary>
        Invalid,

        /// <summary>
        /// Order is updated
        /// </summary>
        Updated,

        /// <summary>
        /// Order was cancelled
        /// </summary>
        Cancelled,

        /// <summary>
        /// Order is in an error state
        /// </summary>
        Error,

        /// <summary>
        /// Fully filled order state
        /// </summary>
        Filled,

        /// <summary>
        /// Partially filled order state
        /// </summary>
        PartialFilled,

        /// <summary>
        /// No order state
        /// </summary>
        None,

        /// <summary>
        /// Waiting for cancelation confirmation
        /// </summary>
        CancelPending
    }
}