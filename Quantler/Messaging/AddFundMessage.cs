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

using System.Collections.Generic;

namespace Quantler.Messaging
{
    /// <summary>
    /// Message for adding a new quant fund to an already running portfolio
    /// </summary>
    public class AddFundMessage : MessageImpl
    {
        #region Public Properties

        /// <summary>
        /// Initial allocated funds
        /// </summary>
        public decimal AllocatedFunds { get; set; }

        /// <summary>
        /// Base 64 encoded assembly, for all modules
        /// </summary>
        public string Base64Assembly { get; set; }

        /// <summary>
        /// If true, force the usage of tick data
        /// </summary>
        public bool ForceTick { get; set; }

        /// <summary>
        /// If true, force the usage of tick data
        /// </summary>
        public bool DirecStart { get; set; } = true;

        /// <summary>
        /// Associated fund id
        /// </summary>
        public string FundId { get; set; }

        /// <summary>
        /// Gets or sets the name of the fund.
        /// </summary>
        public string FundName { get; set; }

        /// <summary>
        /// Type of message
        /// </summary>
        public override MessageType MessageType { get; set; } = MessageType.AddFund;

        /// <summary>
        /// Gets or sets the module names.
        /// </summary>
        public string[] ModuleNames { get; set; }

        /// <summary>
        /// Gets or sets the fund parameters.
        /// </summary>
        public List<ModuleParameter> Parameters { get; set; }

        /// <summary>
        /// Fund associated universe [Symbol, Weight]
        /// </summary>
        public Dictionary<string, decimal> Universe { get; set; }

        /// <summary>
        /// Gets or sets the name of the universe.
        /// </summary>
        public string UniverseName { get; set; }

        #endregion Public Properties
    }

    /// <summary>
    /// Keeps module parameters associated to module
    /// </summary>
    public class ModuleParameter
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the module.
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the paramater.
        /// </summary>
        public string Value { get; set; }

        #endregion Public Properties
    }
}