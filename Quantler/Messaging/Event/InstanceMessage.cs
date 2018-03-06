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

using Quantler.Configuration;
using Quantler.Interfaces;
using Quantler.Machine;

namespace Quantler.Messaging.Event
{
    /// <summary>
    /// Currently running instance information
    /// </summary>
    /// <seealso cref="EventMessage" />
    public class InstanceMessage : EventMessage
    {
        #region Public Properties

        /// <summary>
        /// Current cpu usage percentage
        /// </summary>
        public decimal CPU { get; set; }

        /// <summary>
        /// Connection latency measured by the broker connection
        /// </summary>
        public int LatencyInMS { get; set; }

        /// <summary>
        /// Current instance location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Current percentage of memory used
        /// </summary>
        public decimal RAM { get; set; }

        /// <summary>
        /// Server name
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Message type
        /// </summary>
        public override EventMessageType Type => EventMessageType.Instance;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Generate instance information message
        /// </summary>
        /// <param name="broker"></param>
        /// <returns></returns>
        public static InstanceMessage Generate(BrokerConnection broker)
        {
            return new InstanceMessage
            {
                CPU = Instance.CpuUsedPercentage,
                LatencyInMS = broker.LatencyInMS,
                Location = Config.GlobalConfig.ServerLocation,
                RAM = Instance.PhysicalMemoryUsed,
                Server = Config.GlobalConfig.ServerName
            };
        }

        /// <summary>
        /// Always false
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override bool Equals(EventMessage message) => false;

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Get current message unique id
        /// </summary>
        /// <returns></returns>
        protected override string GetUniqueId() => Server;

        #endregion Protected Methods
    }
}