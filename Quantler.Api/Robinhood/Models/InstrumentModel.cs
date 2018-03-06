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
using System;

namespace Quantler.Api.Robinhood.Models
{
    public class InstrumentModel
    {
        #region Public Properties

        [JilDirective("bloomberg_unique")]
        public string BloombergUnique { get; set; }

        [JilDirective("country")]
        public string Country { get; set; }

        [JilDirective("day_trade_ratio")]
        public string DayTradeRatio { get; set; }

        [JilDirective("fundamentals")]
        public string Fundamentals { get; set; }

        [JilDirective("id")]
        public string Id { get; set; }

        [JilDirective("list_date")]
        public DateTime? ListDate { get; set; }

        [JilDirective("maintenance_ratio")]
        public string MaintenanceRatio { get; set; }

        [JilDirective("margin_initial_ratio")]
        public string MarginInitialRatio { get; set; }

        [JilDirective("market")]
        public string Market { get; set; }

        [JilDirective("min_tick_size")]
        public object MinTickSize { get; set; }

        [JilDirective("name")]
        public string Name { get; set; }

        [JilDirective("quote")]
        public string Quote { get; set; }

        [JilDirective("simple_name")]
        public string SimpleName { get; set; }

        [JilDirective("splits")]
        public string Splits { get; set; }

        [JilDirective("state")]
        public string State { get; set; }

        [JilDirective("symbol")]
        public string Symbol { get; set; }

        [JilDirective("tradability")]
        public string Tradability { get; set; }

        [JilDirective("tradeable")]
        public bool Tradeable { get; set; }

        [JilDirective("type")]
        public string Type { get; set; }

        [JilDirective("url")]
        public string Url { get; set; }

        #endregion Public Properties
    }
}