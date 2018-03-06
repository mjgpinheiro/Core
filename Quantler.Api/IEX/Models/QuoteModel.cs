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

namespace Quantler.Api.IEX.Models
{
    /// <summary>
    /// Quote model as received from IEX api
    /// </summary>
    public class QuoteModel
    {
        #region Public Properties

        [JilDirective("avgTotalVolume")]
        public long? AvgTotalVolume { get; set; }

        [JilDirective("calculationPrice")]
        public string CalculationPrice { get; set; }

        [JilDirective("change")]
        public double? Change { get; set; }

        [JilDirective("changePercent")]
        public double? ChangePercent { get; set; }

        [JilDirective("close")]
        public double? Close { get; set; }

        [JilDirective("closeTime")]
        public long? CloseTime { get; set; }

        [JilDirective("companyName")]
        public string CompanyName { get; set; }

        [JilDirective("delayedPrice")]
        public double? DelayedPrice { get; set; }

        [JilDirective("delayedPriceTime")]
        public long? DelayedPriceTime { get; set; }

        [JilDirective("iexAskPrice")]
        public double? IexAskPrice { get; set; }

        [JilDirective("iexAskSize")]
        public long? IexAskSize { get; set; }

        [JilDirective("iexBidPrice")]
        public double? IexBidPrice { get; set; }

        [JilDirective("iexBidSize")]
        public long? IexBidSize { get; set; }

        [JilDirective("iexLastUpdated")]
        public long? IexLastUpdated { get; set; }

        [JilDirective("iexMarketPercent")]
        public double? IexMarketPercent { get; set; }

        [JilDirective("iexRealtimePrice")]
        public double? IexRealtimePrice { get; set; }

        [JilDirective("iexRealtimeSize")]
        public long? IexRealtimeSize { get; set; }

        [JilDirective("iexVolume")]
        public long? IexVolume { get; set; }

        [JilDirective("latestPrice")]
        public double? LatestPrice { get; set; }

        [JilDirective("latestSource")]
        public string LatestSource { get; set; }

        [JilDirective("latestTime")]
        public string LatestTime { get; set; }

        [JilDirective("latestUpdate")]
        public long? LatestUpdate { get; set; }

        [JilDirective("latestVolume")]
        public long? LatestVolume { get; set; }

        [JilDirective("marketCap")]
        public long? MarketCap { get; set; }

        [JilDirective("open")]
        public double? Open { get; set; }

        [JilDirective("openTime")]
        public long? OpenTime { get; set; }

        [JilDirective("peRatio")]
        public double? PeRatio { get; set; }

        [JilDirective("previousClose")]
        public double? PreviousClose { get; set; }

        [JilDirective("primaryExchange")]
        public string PrimaryExchange { get; set; }

        [JilDirective("sector")]
        public string Sector { get; set; }

        [JilDirective("symbol")]
        public string Symbol { get; set; }

        [JilDirective("week52High")]
        public double? Week52High { get; set; }

        [JilDirective("week52Low")]
        public double? Week52Low { get; set; }

        [JilDirective("ytdChange")]
        public double? YtdChange { get; set; }

        #endregion Public Properties
    }
}