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

using System.Collections.Generic;

namespace Quantler.DataFeeds.HitBtcPublic.Models.v1
{
    /// <summary>
    /// contains a full snapshot of the order book.
    /// </summary>
    internal class MarketDataSnapshotFullRefreshModel
    {
        #region Public Properties

        /// <summary>
        /// Sorted arrays of price levels in the order book; full snapshot (all price levels) is provided
        /// </summary>
        public IList<PriceModel> ask { get; set; }

        /// <summary>
        /// Sorted arrays of price levels in the order book; full snapshot (all price levels) is provided
        /// </summary>
        public IList<PriceModel> bid { get; set; }

        /// <summary>
        /// Exchange status: on - trading is open; off - trading is suspended
        /// </summary>
        public string exchangeStatus { get; set; }

        /// <summary>
        /// Monotone increasing number of the snapshot, each symbol has its own sequence
        /// </summary>
        public long snapshotSeqNo { get; set; }

        /// <summary>
        /// Currency symbol traded on HitBTC exchangeModel
        /// </summary>
        public string symbol { get; set; }

        #endregion Public Properties
    }
}