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

using Jil;
using System;

namespace Quantler.Messaging.Event
{
    /// <summary>
    /// Event message base implementation
    /// </summary>
    public abstract class EventMessage
    {
        #region Public Properties

        /// <summary>
        /// Date and time this message has occurred
        /// </summary>
        public DateTime OccuredUtc => DateTime.UtcNow;

        /// <summary>
        /// Type of message
        /// </summary>
        public virtual EventMessageType Type => EventMessageType.NIL;

        /// <summary>
        /// Unique id associated to this message
        /// </summary>
        public string UniqueId => GetUniqueId();

        /// <summary>
        /// Current framework version for backward compatability
        /// </summary>
        public string Version => Framework.CurrentVersion;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Deserialize this message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual T Deserialize<T>(string message) => JSON.Deserialize<T>(message);

        /// <summary>
        /// Check if this message equals any other message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual bool Equals(EventMessage message) => message.GetType() == GetType() && message.UniqueId == UniqueId;

        /// <summary>
        /// Serialize this message
        /// </summary>
        /// <returns></returns>
        public virtual string Serialize() => JSON.Serialize(this);

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Get unique id for this message
        /// </summary>
        /// <returns></returns>
        protected abstract string GetUniqueId();

        #endregion Protected Methods
    }
}