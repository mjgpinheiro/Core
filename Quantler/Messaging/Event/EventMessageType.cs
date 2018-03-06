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

namespace Quantler.Messaging.Event
{
    /// <summary>
    /// Known event message types (outbound)
    /// </summary>
    public enum EventMessageType
    {
        /// <summary>
        /// Account information update
        /// </summary>
        AccountInfo,

        /// <summary>
        /// Exception occurance in a fund
        /// </summary>
        Exception,

        /// <summary>
        /// Fund information
        /// </summary>
        FundInfo,

        /// <summary>
        /// Instance information
        /// </summary>
        Instance,

        /// <summary>
        /// Logging message
        /// </summary>
        Logging,

        /// <summary>
        /// Order info update
        /// </summary>
        OrderInfo,

        /// <summary>
        /// Pending order update info
        /// </summary>
        PendingOrderInfo,

        /// <summary>
        /// Performance update info
        /// </summary>
        PerformanceInfo,

        /// <summary>
        /// Current position info
        /// </summary>
        PositionInfo,

        /// <summary>
        /// Progress update (for backtesting)
        /// </summary>
        Progress,

        /// <summary>
        /// Unknown
        /// </summary>
        NIL
    }
}