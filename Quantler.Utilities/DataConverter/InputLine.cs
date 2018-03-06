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

namespace Quantler.Utilities.DataConverter
{
    internal class InputLine
    {
        #region Public Properties

        /// <summary>
        /// Hashed line item
        /// </summary>
        public UInt64 Hash { get; set; }

        /// <summary>
        /// Line text
        /// </summary>
        public string Line { get; set; }

        /// <summary>
        /// Line number in file
        /// </summary>
        public long LineNumber { get; set; }

        /// <summary>
        /// The original filename
        /// </summary>
        public string Filename { get; set; }

        #endregion Public Properties
    }
}