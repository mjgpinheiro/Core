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

namespace Quantler.Api.CoinMarketCap.Models
{
    /// <summary>
    /// Ticker model as retrieved from coin market
    /// </summary>
    public class TickerModel
    {
        #region Public Properties

        [JilDirective(Name = "available_supply")]
        public string AvailableSupply { get; set; }

        [JilDirective(Name = "24h_volume_usd")]
        public string DayVolumeUsd { get; set; }

        [JilDirective(Name = "id")]
        public string Id { get; set; }

        [JilDirective(Name = "last_updated")]
        public string LastUpdated { get; set; }

        [JilDirective(Name = "market_cap_usd")]
        public string MarketCapUSD { get; set; }

        [JilDirective(Name = "name")]
        public string Name { get; set; }

        [JilDirective(Name = "percent_change_1h")]
        public string PercentChange1h { get; set; }

        [JilDirective(Name = "percent_change_24h")]
        public string PercentChange24h { get; set; }

        [JilDirective(Name = "percent_change_7d")]
        public string PercentChange7d { get; set; }

        [JilDirective(Name = "price_btc")]
        public string PriceBTC { get; set; }

        [JilDirective(Name = "price_usd")]
        public string PriceUSD { get; set; }

        [JilDirective(Name = "rank")]
        public string Rank { get; set; }

        [JilDirective(Name = "symbol")]
        public string Symbol { get; set; }

        [JilDirective(Name = "total_supply")]
        public string TotalSupply { get; set; }

        #endregion Public Properties
    }
}