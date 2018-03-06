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

using Quantler.Messaging.Event;
using System;
using System.Composition;
using Quantler.Interfaces;
using Quantler.Messaging;

namespace Quantler.Bootstrap.MessagingEvents
{
    /// <summary>
    /// Console based endpoint
    /// </summary>
    [Export(typeof(EventRunner))]
    public class ConsoleRunner : EventRunnerBase
    {
        #region Public Methods

        /// <summary>
        /// Initialize implementation
        /// </summary>
        public override void Initialize(MessageInstance messageInstance) =>
            //We always have subscribers (N/A)
            HasSubscribers = true;

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Send messages to front-end
        /// </summary>
        /// <param name="message"></param>
        protected override void Send(EventMessage message)
        {
            //Set string text
            string txt = $"{message.Type}: ";

            switch (message.Type)
            {
                case EventMessageType.Logging:
                    txt += $"[{(message as LoggingMessage)?.Severity}] {(message as LoggingMessage)?.Message}";
                    break;

                case EventMessageType.Exception:
                    txt += $"{(message as ExceptionMessage)?.Message}";
                    break;

                case EventMessageType.Progress:
                    txt += $"{(message as ProgressMessage)?.Percentage}%";
                    break;

                case EventMessageType.AccountInfo:
                    return;

                case EventMessageType.FundInfo:
                    return;

                case EventMessageType.Instance:
                    return;

                case EventMessageType.NIL:
                    return;

                case EventMessageType.OrderInfo:
                    return;

                case EventMessageType.PendingOrderInfo:
                    return;

                case EventMessageType.PositionInfo:
                    return;

                case EventMessageType.PerformanceInfo:
                    return;
            }

            //Console
            Console.WriteLine(txt);
        }

        #endregion Protected Methods
    }
}