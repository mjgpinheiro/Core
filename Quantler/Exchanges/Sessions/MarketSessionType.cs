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

using System.Runtime.Serialization;

namespace Quantler.Exchanges.Sessions
{
    /// <summary>
    /// Market session types
    /// </summary>
    public enum MarketSessionType
    {
        /// <summary>
        /// Market is closed
        /// </summary>
        Closed = 3,

        /// <summary>
        /// Market opened, before regular trading hours
        /// </summary>
        PreMarket = 0,

        /// <summary>
        /// Market opened, regular trading time
        /// </summary>
        Market = 1,

        /// <summary>
        /// Market opened, after regular trading hours
        /// </summary>
        PostMarket = 2
    }
}