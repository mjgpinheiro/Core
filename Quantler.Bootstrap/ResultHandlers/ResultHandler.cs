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
using Quantler.Interfaces;
using Quantler.Messaging;
using Quantler.Performance;

namespace Quantler.Bootstrap.ResultHandlers
{
    /// <summary>
    /// Handles results wherever they need to be send to
    /// TODO: implement into flow
    /// TODO: implement into dependencies config (for selected implementation)
    /// </summary>
    internal interface ResultHandler : QTask
    {
        #region Public Methods

        /// <summary>
        /// Initializes the specified result handler.
        /// </summary>
        /// <param name="message">The initial message that starts this process</param>
        /// <param name="result">The result.</param>
        void Initialize(MessageInstance message, Result result);

        /// <summary>
        /// Called when [end of execution].
        /// </summary>
        void OnEndOfExecution();

        /// <summary>
        /// Pokes this instace
        /// </summary>
        /// <param name="utc">The date and time in utc.</param>
        void Poke(DateTime utc);

        #endregion Public Methods
    }
}