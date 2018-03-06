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

using System.Collections.Concurrent;
using Quantler.Interfaces;
using System.Linq;

namespace Quantler.Securities
{
    /// <summary>
    /// Logic for determining security state based on retrieved signals
    /// </summary>
    public class SecurityStateHolder
    {
        #region Private Fields

        /// <summary>
        /// The current states
        /// </summary>
        private readonly ConcurrentDictionary<string, SecurityState> _currentstates = new ConcurrentDictionary<string, SecurityState>();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityStateHolder"/> class.
        /// </summary>
        /// <param name="security">The security.</param>
        public SecurityStateHolder(Security security) =>
            Security = security;

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the state consensus.
        /// </summary>
        public SecurityState Consensus => DeriveConsensus();

        /// <summary>
        /// Gets the associated security.
        /// </summary>
        public Security Security { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Sets the state for the module.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <param name="state">The state.</param>
        public void SetState(IModule module, SecurityState state) =>
            _currentstates[module.Id] = state;

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Derives the consensus.
        /// </summary>
        /// <returns></returns>
        private SecurityState DeriveConsensus()
        {
            //Check if we have any
            if (_currentstates.Count == 0)
                return SecurityState.NoEntry;

            //All else
            if (_currentstates.Any(x => x.Value == SecurityState.Error))               //Error state prefails above all
                return SecurityState.Error;
            else if (_currentstates.Any(x => x.Value == SecurityState.NoEntry))        //No entry before any other signal
                return SecurityState.NoEntry;
            else if (_currentstates.Any(x => x.Value == SecurityState.Liquidate))      //Liquidate before any entries or exits
                return SecurityState.Liquidate;
            else if (_currentstates.Any(x => x.Value == SecurityState.ExitLong))       //Exit long before entry
                return SecurityState.ExitLong;
            else if (_currentstates.Any(x => x.Value == SecurityState.ExitShort))      //Exit short before entry
                return SecurityState.ExitShort;
            else if (_currentstates.All(x => x.Value == SecurityState.EntryLong))      //Entry long
                return SecurityState.EntryLong;
            else if (_currentstates.All(x => x.Value == SecurityState.EntryShort))     //Entry short
                return SecurityState.EntryShort;
            else
                return SecurityState.NoEntry;                                          //All else
        }

        #endregion Private Methods
    }
}