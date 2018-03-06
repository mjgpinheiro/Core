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

namespace Quantler.Interfaces
{
    public enum PortfolioStatus
    {
        /// <summary>
        /// Runtime error occured during execution
        /// </summary>
        RuntimeError,

        /// <summary>
        /// Portfolio is running
        /// </summary>
        Running,

        /// <summary>
        /// Portfolio is currently stopped from execution
        /// </summary>
        Stopped,

        /// <summary>
        /// Portfolio settings are invalid
        /// </summary>
        Invalid,

        /// <summary>
        /// Portfolio is currently initializing
        /// </summary>
        Initializing,

        /// <summary>
        /// Portfolio is in a liquidation state
        /// </summary>
        Liquidated,

        /// <summary>
        /// Removing current portfolio
        /// </summary>
        Deleting,

        /// <summary>
        /// Terminate current execution
        /// </summary>
        Terminating
    }
}