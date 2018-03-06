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
    /// Known order types
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// Execute order based on the limit price
        /// </summary>
        Limit,

        /// <summary>
        /// Execute order based on the best market price
        /// </summary>
        Market,

        /// <summary>
        /// Order is only executed on the stop price
        /// </summary>
        StopMarket,

        /// <summary>
        /// Order is only executed on the stop price for a specific fill price
        /// </summary>
        StopLimit,

        /// <summary>
        /// The market on close order type
        /// </summary>
        MarketOnClose,

        /// <summary>
        /// The market on open order type
        /// </summary>
        MarketOnOpen
    }
}