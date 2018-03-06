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

namespace Quantler.Messaging
{
    /// <summary>
    /// Base implementation for messages
    /// </summary>
    public abstract class MessageBase
    {
        #region Public Properties

        /// <summary>
        /// True if this message is a result of an existing message
        /// </summary>
        public bool IsResult { get; set; }

        /// <summary>
        /// Result message, if needed
        /// </summary>
        public string ResultMessage { get; set; }

        /// <summary>
        /// True if the results are ok (Succeeded)
        /// </summary>
        public bool ResultOk { get; set; }

        /// <summary>
        /// The unique id associated to this message
        /// </summary>
        public string UniqueId { get; set; }

        #endregion Public Properties
    }
}