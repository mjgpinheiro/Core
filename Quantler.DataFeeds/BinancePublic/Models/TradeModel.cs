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

namespace Quantler.DataFeeds.BinancePublic.Models
{
    internal class TradeModel
    {
        #region Public Properties

        [JilDirective("a")]
        public long AggregatedTradeId { get; set; }

        [JilDirective("E")]
        public long EventTime { get; set; }

        [JilDirective("e")]
        public string EventType { get; set; }

        [JilDirective("f")]
        public long FirstId { get; set; }

        [JilDirective("m")]
        public bool IsMaker { get; set; }

        [JilDirective("l")]
        public long LastId { get; set; }

        [JilDirective("M")]
        public bool M { get; set; }

        [JilDirective("p")]
        public string Price { get; set; }

        [JilDirective("q")]
        public string Quantity { get; set; }

        [JilDirective("s")]
        public string Symbol { get; set; }

        [JilDirective("T")]
        public long TradeTime { get; set; }

        #endregion Public Properties
    }
}