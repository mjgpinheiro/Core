#region License Header
/*
*
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
*
*/
#endregion License Header

using System.Composition;
using Quantler.Interfaces;
using Quantler.Modules;
using Quantler.Securities;

namespace Quantler.Local.Modules.Signal
{
    /// <summary>
    /// Makes sure we are only in the markets when the fast ema crosses over the slow ema
    /// </summary>
    /// <seealso cref="SignalModule" />
    [Export(typeof(IModule))]
    public class EMACrossOver : SignalModule
    {
        //Slow EMA
        [Parameter(100, 200, 10, "Slow")]
        public int SlowEMA { get; set; } = 150;

        //Fast EMA
        [Parameter(50, 80, 10, "Fast")]
        public int FastEMA { get; set; } = 65;

        //Timeframe in hours
        [Parameter(1, 24, 1, "Slow")]
        public int TimeFrameInHours { get; set; } = 12;

        //Crossed over signal name
        private string CrossedOver = "EMACrossedOver";

        //Crossed below signal name
        private string CrossedBelow = "EMACrossedBelow";

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Initialize()
        {
            //Get the slow and fast ema
            var emaslow = EMA(Universe, SlowEMA, Resolution.Hourly * TimeFrameInHours, Field.Average);
            var emafast = EMA(Universe, FastEMA, Resolution.Hourly * TimeFrameInHours, Field.Average);

            //If the position is not long, send a signal to go long
            AddSignal(CrossedOver, security => emaslow[security].IsReady && emafast[security] > emaslow[security] && !Position[security].IsLong, emafast);

            //If the position is not short, send a signal to go short
            AddSignal(CrossedBelow, security => emaslow[security].IsReady && emafast[security] < emaslow[security] && !Position[security].IsShort, emafast);
        }

        /// <summary>
        /// Called when a trading signal is activated.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <param name="securities">The securities.</param>
        public override void OnSignal(TradingSignal signal, Security[] securities)
        {
            //Check signals
            if(signal == CrossedOver)
                EnterLong(securities);
            else if (signal == CrossedBelow)
                EnterShort(securities);
        }
    }
}
