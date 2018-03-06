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
using System.Collections.Generic;

namespace Quantler.Api.Bittrex.Models
{
    /// <summary>
    /// Base class for results from bittrex api
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RestResultModel<T>
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [JilDirective(Name = "message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        [JilDirective(Name = "result")]
        public IList<T> Result { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RestResultModel{T}"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        [JilDirective(Name = "success")]
        public bool Success { get; set; }

        #endregion Public Properties
    }
}