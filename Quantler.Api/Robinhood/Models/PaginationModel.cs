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

namespace Quantler.Api.Robinhood.Models
{
    /// <summary>
    /// Applicable pagination model
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginationModel<T>
    {
        #region Public Properties

        [JilDirective("next")]
        public string Next { get; set; }

        [JilDirective("previous")]
        public object Previous { get; set; }

        [JilDirective("results")]
        public T[] Results { get; set; }

        #endregion Public Properties
    }
}