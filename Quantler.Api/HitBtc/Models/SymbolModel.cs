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

namespace Quantler.Api.HitBtc.Models
{
    public class SymbolModel
    {
        #region Public Properties

        [JilDirective("baseCurrency")]
        public string BaseCurrency { get; set; }

        [JilDirective("feeCurrency")]
        public string FeeCurrency { get; set; }

        [JilDirective("id")]
        public string Id { get; set; }

        [JilDirective("provideLiquidityRate")]
        public string ProvideLiquidityRate { get; set; }

        [JilDirective("quantityIncrement")]
        public string QuantityIncrement { get; set; }

        [JilDirective("quoteCurrency")]
        public string QuoteCurrency { get; set; }

        [JilDirective("takeLiquidityRate")]
        public string TakeLiquidityRate { get; set; }

        [JilDirective("tickSize")]
        public string TickSize { get; set; }

        #endregion Public Properties
    }
}