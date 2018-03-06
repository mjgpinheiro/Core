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

namespace Quantler.Messaging
{
    /// <summary>
    /// Message as received from a queue
    /// </summary>
    public interface MessageInstance
    {
        #region Public Properties

        /// <summary>
        /// Gets the framework version.
        /// </summary>
        string FrameworkVersion { get; set; }

        /// <summary>
        /// True if this message is a result of an existing message
        /// </summary>
        bool IsResult { get; set; }

        /// <summary>
        /// Type of message
        /// </summary>
        MessageType MessageType { get; set; }

        /// <summary>
        /// Result message, if needed
        /// </summary>
        string ResultMessage { get; set; }

        /// <summary>
        /// True if the results are ok (Succeeded)
        /// </summary>
        bool ResultOk { get; set; }

        /// <summary>
        /// Date and time in utc this message was send from source
        /// </summary>
        DateTime SendUtc { get; set; }

        /// <summary>
        /// The unique id associated to this message
        /// </summary>
        string UniqueId { get; set; }

        #endregion Public Properties
    }
}