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

using System.Globalization;
using System.Linq;
using Jil;

namespace Quantler.DataFeeds.BinancePublic.Models
{
    public class DepthModel
    {
        #region Public Properties

        [JilDirective("a")]
        public string[][] askprices { get; set; }

        [JilDirective("b")]
        public string[][] bidprices { get; set; }

        [JilDirective(true)]
        public PriceModel[] AskPricesModel => askprices.Select(x => new PriceModel(x)).ToArray();

        [JilDirective(true)]
        public PriceModel[] BidPricesModel => bidprices.Select(x => new PriceModel(x)).ToArray();

        [JilDirective("E")]
        public long EventTime { get; set; }

        [JilDirective("e")]
        public string EventType { get; set; }

        [JilDirective("s")]
        public string Symbol { get; set; }

        [JilDirective("u")]
        public long UpdateId { get; set; }

        #endregion Public Properties
    }
}