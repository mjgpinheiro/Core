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

namespace Quantler.DataFeeds.HitBtcPublic.Models.v1
{
    /// <summary>
    /// Base model for market data model
    /// </summary>
    internal class MarketDataModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the market data incremental refresh.
        /// </summary>
        public MarketDataIncrementalRefreshModel MarketDataIncrementalRefresh { get; set; }

        /// <summary>
        /// Gets or sets the market data snapshot full refresh.
        /// </summary>
        public MarketDataSnapshotFullRefreshModel MarketDataSnapshotFullRefresh { get; set; }

        #endregion Public Properties
    }
}