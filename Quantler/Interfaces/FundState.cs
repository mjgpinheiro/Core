#region License Header
/*
* QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
* Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
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
* Modifications Copyright 2018 Quantler B.V.
*
*/
#endregion License Header

namespace Quantler.Interfaces
{
    /// <summary>
    /// Currently running quant fund state
    /// </summary>
    public enum FundState
    {
        /// Error while deploying quant fund
        DeployError,

        /// Running quant fund
        Running,

        /// Stopped quant fund or exited with runtime errors
        Stopped,

        /// Liquidated quant fund
        Liquidated,

        /// Quant fund has been deleted
        Deleted,

        /// Quant fund completed running
        Completed,

        /// Runtime Error Stopped Quant Fund
        RuntimeError,

        /// The quant fund is initializing
        Initializing,

        /// The quant fund is currently backfilling
        Backfilling
    }
}