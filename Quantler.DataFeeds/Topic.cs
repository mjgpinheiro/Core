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

namespace Quantler.DataFeeds
{
    /// <summary>
    /// Datafeed topics
    /// </summary>
    public class Topic
    {
        #region Public Fields

        /// <summary>
        /// Backfilling request
        /// </summary>
        public const string BackfillingMessage = "|BM|";

        /// <summary>
        /// Default hearbeat topic message
        /// </summary>
        public const string HeartBeatMessage = "|HB|";

        /// <summary>
        /// History request
        /// </summary>
        public const string HistoryMessage = "|HM|";

        /// <summary>
        /// Default data topic message
        /// </summary>
        public const string DataMessage = "|DM|";

        /// <summary>
        /// Default welcome message
        /// </summary>
        public const string WelcomeMessage = "|WM|";

        #endregion Public Fields
    }
}