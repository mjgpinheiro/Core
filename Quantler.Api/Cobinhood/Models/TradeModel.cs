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
    public class TradeModel
    {
        #region Public Properties

        [JilDirective("id")]
        public string Id { get; set; }

        [JilDirective("maker_side")]
        public string MakerSide { get; set; }

        [JilDirective("price")]
        public string Price { get; set; }

        [JilDirective("size")]
        public string Size { get; set; }

        [JilDirective("timestamp")]
        public long Timestamp { get; set; }

        #endregion Public Properties
    }
}