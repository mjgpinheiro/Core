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

using Quantler.Interfaces;

namespace Quantler.Messaging
{
    /// <summary>
    /// Receives messages ready for processing by a Quant Fund instance
    /// </summary>
    public interface MessageQueue : QTask
    {
        #region Public Methods

        /// <summary>
        /// Acknowledge current message
        /// </summary>
        /// <param name="message"></param>
        void Acknowledge(MessageInstance message);

        /// <summary>
        /// Set result of processing this message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="result"></param>
        void Complete(MessageInstance message, MessageResult result);

        /// <summary>
        /// Initialize message queue instance
        /// </summary>
        void Initialize();

        /// <summary>
        /// Try to get the next message in the queue
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool TryGetNextMessage(out MessageInstance item);

        #endregion Public Methods
    }
}