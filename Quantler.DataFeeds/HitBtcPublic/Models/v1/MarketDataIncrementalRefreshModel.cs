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
    /// contains incremental changes of the order book and individual trades.
    /// </summary>
    internal class MarketDataIncrementalRefreshModel
    {
        #region Public Properties

        /// <summary>
        /// An array of changes in the order book where price is a price, size is new size. size=0 means that the price level has been removed
        /// </summary>
        public IList<PriceModel> ask { get; set; }

        /// <summary>
        /// An array of changes in the order book where price is a price, size is new size. size=0 means that the price level has been removed
        /// </summary>
        public IList<PriceModel> bid { get; set; }

        /// <summary>
        /// Exchange status: on - trading is open; off - trading is suspended
        /// </summary>
        public string exchangeStatus { get; set; }

        /// <summary>
        /// Monotone increasing number of the snapshot, each symbol has its own sequence
        /// </summary>
        public long seqNo { get; set; }

        /// <summary>
        /// Currency symbol traded on HitBTC exchangeModel
        /// </summary>
        public string symbol { get; set; }

        /// <summary>
        /// UTC timestamp, in milliseconds
        /// </summary>
        public long timestamp { get; set; }

        /// <summary>
        /// An array of changes in the order book where price is a price, size is new size. size=0 means that the price level has been removed
        /// </summary>
        public IList<PriceModel> trade { get; set; }

        #endregion Public Properties
    }
}