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

using System;
using System.Collections.Generic;
using System.Globalization;
using Jil;

namespace Quantler.Configuration.Model
{
    public class CurrencyRatesConfig
    {
        #region Public Properties

        [JilDirective(Name = "base")]
        public string Base { get; set; }

        [JilDirective(true)]
        public DateTime Date
        {
            get => DateTime.TryParseExact(date, "yyyy-MM-dd", null, DateTimeStyles.AdjustToUniversal, out DateTime result) ? result : DateTime.MinValue;
            set => date = value.ToString("yyyy-MM-dd");
        }

        [JilDirective(Name = "date")]
        public string date { get; set; }

        [JilDirective(Name = "rates")]
        public Dictionary<string, decimal> Rates { get; set; }

        #endregion Public Properties
    }
}