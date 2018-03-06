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

namespace Quantler.Configuration.Model
{
    public class SecurityConfig
    {
        #region Public Properties

        public string Currency { get; set; }
        public List<string> Brokers { get; set; }
        public int Digits { get; set; }
        public string Exchange { get; set; }
        public string Ticker { get; set; }
        public string Type { get; set; }
        public string Commodity { get; set; }
        public decimal Step { get; set; }
        public decimal ExpenseRatio { get; set; }

        #endregion Public Properties
    }
}