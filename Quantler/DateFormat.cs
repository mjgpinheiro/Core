#region License Header

/*
* QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
* Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
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
*
* Modifications Copyright 2018 Quantler B.V.
*
*/

#endregion License Header

namespace Quantler
{
    /// <summary>
    /// Shortcut date format strings
    /// </summary>
    public static class DateFormat
    {
        #region Public Fields

        /// MySQL Format Date Representation
        public const string DB = "yyyy-MM-dd HH:mm:ss";

        /// YYYY-MM-DD Eight Character Date Representation with line seperator
        public const string EightCharacterLined = "yyyy-MM-dd";

        /// YYYYMMDD Eight Character Date Representation
        public const string EightCharacter = "yyyyMMdd";

        /// Year-Month-Date 6 Character Date Representation
        public const string SixCharacter = "yyMMdd";

        /// Daily and hourly time format
        public const string TwelveCharacter = "yyyyMMdd HH:mm";

        /// en-US format
        public const string US = "M/d/yyyy h:mm:ss tt";

        /// YYYYMM Year and Month Character Date Representation (used for futures)
        public const string YearMonth = "yyyyMM";

        /// JSON Format Date Representation
        public static string JsonFormat = "yyyy-MM-ddTHH:mm:ss";

        #endregion Public Fields
    }
}