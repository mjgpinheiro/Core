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

namespace Quantler.Data.DataFile
{
    /// <summary>
    /// Constants for working with data files
    /// </summary>
    public class DataConst
    {
        #region Public Fields

        /// <summary>
        /// Datapoint instance end
        /// </summary>
        public const byte DatapointEnd = 4;

        /// <summary>
        /// Datapoint instance start
        /// </summary>
        public const byte DatapointStart = 3;

        /// <summary>
        /// Extension used, including dot
        /// </summary>
        public const string DotExt = "." + Ext;

        /// <summary>
        /// The end data
        /// </summary>
        public const byte EndData = 2;

        /// <summary>
        /// Extension used
        /// </summary>
        public const string Ext = "DAT";

        /// <summary>
        /// The current file version
        /// </summary>
        public const int FileCurrentVersion = 1;

        /// <summary>
        /// The file version part
        /// </summary>
        public const byte FileVersion = 0;

        /// <summary>
        /// The start data
        /// </summary>
        public const byte StartData = 1;

        /// <summary>
        /// The wildcard extension
        /// </summary>
        public const string WildcardExt = "*" + DotExt;

        #endregion Public Fields
    }
}