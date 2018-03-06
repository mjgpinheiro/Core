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

using System;
using System.Composition;
using Jil;
using NetMQ;
using NetMQ.Sockets;
using NLog;
using Quantler.Messaging;

namespace Quantler.Bootstrap.MessageQueues
{
    /// <summary>
    /// TODO: implement logic for netmq based commands/requests queue (used for local implementations, where needed)
    /// pullsocket implementation (divide and conquer)
    /// </summary>
    [Export(typeof(MessageQueue))]
    public class NetMQQueue : MessageQueue
    {
        #region Public Properties

        public bool IsRunning => false;

        #endregion Public Properties

        private PullSocket _pullSocket;

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        #region Public Methods

        public void Acknowledge(MessageInstance message)
        {
            return;
            throw new NotImplementedException();
        }

        public void Complete(MessageInstance message, MessageResult result)
        {
            return;
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            return;
            throw new NotImplementedException();
        }

        public void Start()
        {
            return;
            throw new NotImplementedException();
        }

        public void Stop()
        {
            return;
            throw new NotImplementedException();
        }

        public bool TryGetNextMessage(out MessageInstance item)
        {
            item = null;
            return false;
            throw new NotImplementedException();
            //try and get the next message instance from the push socket
            //item = null;
            //try
            //{
            //    if (_pullSocket.TryReceiveFrameBytes(out byte[] frame))
            //    {

            //        return true;
            //    }
            //    else
            //        return false;
            //}
            //catch (Exception exc)
            //{
            //    _log.Error(exc, $"Exception while processing message from queue");
            //    return false;
            //}
        }

        #endregion Public Methods
    }
}