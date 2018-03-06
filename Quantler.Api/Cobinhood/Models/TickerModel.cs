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
#endregion

using Jil;

namespace Quantler.Api.Cobinhood.Models
{
    public class TickerModel
    {
        #region Public Properties

        [JilDirective("highest_bid")]
        public string HighestBid { get; set; }

        [JilDirective("last_trade_price")]
        public string LastTradePrice { get; set; }

        [JilDirective("lowest_ask")]
        public string LowestAsk { get; set; }

        [JilDirective("24h_high")]
        public string The24HHigh { get; set; }

        [JilDirective("24h_low")]
        public string The24HLow { get; set; }

        [JilDirective("24h_open")]
        public string The24HOpen { get; set; }

        [JilDirective("24h_volume")]
        public string The24HVolume { get; set; }

        [JilDirective("timestamp")]
        public long Timestamp { get; set; }

        [JilDirective("trading_pair_id")]
        public string TradingPairId { get; set; }

        #endregion Public Properties
    }
}