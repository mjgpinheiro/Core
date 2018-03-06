#region License Header

/*
*
* QUANTLER.COM - Quant Fund Development Platform
* Quantler Core Trading Engine. Copyright %CurrentYear% Quantler B.V.
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
*/

#endregion License Header

using System;
using System.Composition;
using Quantler.Interfaces;
using Quantler.Modules;
using Quantler.Securities;

namespace Quantler.Local.Modules.Signal
{
    /// <summary>
    /// Makes sure we are always in the markets
    /// </summary>
    /// <seealso cref="Quantler.Modules.SignalModule" />
    [Export(typeof(IModule))]
    public class AlwaysLongSignal : SignalModule
    {
        #region Public Methods

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Initialize()
        {
            //If the position is flat, send a signal to go long (check every 1 minute)
            AddSignal("AlwaysLong", security => Position[security].IsFlat, TimeSpan.FromMinutes(1));
        }

        /// <summary>
        /// Called when a trading signal is activated.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <param name="securities">The securities.</param>
        public override void OnSignal(TradingSignal signal, Security[] securities) =>
            EnterLong(securities);

        #endregion Public Methods
    }
}