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

using NetMQ;
using NetMQ.Sockets;
using Quantler.Configuration;
using Quantler.Messaging.Event;
using System;
using System.Composition;
using Quantler.Interfaces;
using Quantler.Messaging;

namespace Quantler.Bootstrap.MessagingEvents
{
    /// <summary>
    /// Implementation of an event runner for netmq
    /// </summary>
    [Export(typeof(EventRunner))]
    public class NetMQRunner : EventRunnerBase
    {
        #region Private Fields

        /// <summary>
        /// Publisher socket
        /// </summary>
        private PublisherSocket _socket;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Initialize instance
        /// </summary>
        public override void Initialize(MessageInstance messageInstance)
        {
            //We always have subscribers
            HasSubscribers = true;

            //Check if port is defined
            var foundport = Config.TryGetEnvVariable("NetMQRunnerPort", Config.GlobalConfig.NetMQRunnerPort);
            if (string.IsNullOrWhiteSpace(foundport) || !int.TryParse(foundport, out var port))
                throw new Exception($"Could not find port number in configuration for setting up outbound event messages. Please set the port number in Global config or via environment variable.");

            //Check if port is available for usage
            if (!Util.CheckPortAvailability(port))
                throw new Exception($"Port with number {port} is unavailable. Cannot start EventRunner, please check your firewall settings or if the port is already in use by another application");

            //Initialize socket
            _socket = new PublisherSocket("@tcp://*:" + port);

            //Set default manipulation logic (so we only send deltas)
            EventKeeper.SetManipulationLogic(EventMessageType.PerformanceInfo, EventKeeper.DefaultManipulationLogic(EventMessageType.PerformanceInfo));

            //Send data asap (1ms)
            MinWait = 1;
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Send frame
        /// </summary>
        /// <param name="message">Message to be send to this runner</param>
        protected override void Send(EventMessage message) =>
            _socket.SendMoreFrame(message.Type.ToString())
                  .SendFrame(message.Serialize());

        #endregion Protected Methods
    }
}