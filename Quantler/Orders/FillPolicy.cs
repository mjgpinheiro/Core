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
    public enum FillPolicy
    {
        /// <summary>
        /// Fill the order completely or not at all
        /// </summary>
        AllOrNone,

        /// <summary>
        /// At the close of the market, or as near to the closing price as possible
        /// </summary>
        AtTheClose,

        /// <summary>
        /// At the very beginning of the trading day
        /// </summary>
        AtTheOpening,

        /// <summary>
        /// Transaction immediately and completely or not at all
        /// </summary>
        FOK,

        /// <summary>
        /// Default: Fill asap
        /// </summary>
        Immediate,

        /// <summary>
        /// Any portion of the order that cannot be immediately filled is cancelled.
        /// </summary>
        IOC
    }
}