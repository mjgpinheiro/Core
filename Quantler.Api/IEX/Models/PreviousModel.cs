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

using System;
using Jil;

namespace Quantler.Api.IEX.Models
{
    public class PreviousModel
    {
        [JilDirective("change")]
        public double Change { get; set; }

        [JilDirective("changePercent")]
        public double ChangePercent { get; set; }

        [JilDirective("close")]
        public double Close { get; set; }

        [JilDirective(true)]
        public DateTime Date => Time.TryParseDate(date, out DateTime returnvalue) ? returnvalue : throw new Exception($"Could not parse date {date}");

        [JilDirective("date")]
        public string date { get; set; }

        [JilDirective("high")]
        public double High { get; set; }

        [JilDirective("low")]
        public double Low { get; set; }

        [JilDirective("open")]
        public double Open { get; set; }

        [JilDirective("symbol")]
        public string Symbol { get; set; }

        [JilDirective("unadjustedVolume")]
        public long UnadjustedVolume { get; set; }

        [JilDirective("volume")]
        public long Volume { get; set; }

        [JilDirective("vwap")]
        public double Vwap { get; set; }
    }
}
