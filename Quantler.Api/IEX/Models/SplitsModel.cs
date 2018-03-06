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

namespace Quantler.Api.IEX.Models
{
    public class SplitsModel
    {
        #region Public Properties

        [JilDirective("declaredDate")]
        public string declaredDate { get; set; }

        [JilDirective("declaredDate")]
        public DateTime DeclaredDate => Time.TryParseDate(declaredDate, out DateTime returnvalue) ? returnvalue : throw new Exception($"Could not parse date {declaredDate}");

        [JilDirective("exDate")]
        public string exDate { get; set; }

        [JilDirective("exDate")]
        public DateTime ExDate => Time.TryParseDate(exDate, out DateTime returnvalue) ? returnvalue : throw new Exception($"Could not parse date {exDate}");

        [JilDirective("forFactor")]
        public long ForFactor { get; set; }

        [JilDirective("paymentDate")]
        public string paymentDate { get; set; }

        [JilDirective("paymentDate")]
        public DateTime PaymentDate => Time.TryParseDate(paymentDate, out DateTime returnvalue) ? returnvalue : throw new Exception($"Could not parse date {paymentDate}");

        [JilDirective("ratio")]
        public double Ratio { get; set; }

        [JilDirective("recordDate")]
        public string recordDate { get; set; }

        [JilDirective("recordDate")]
        public DateTime RecordDate => Time.TryParseDate(recordDate, out DateTime returnvalue) ? returnvalue : throw new Exception($"Could not parse date {recordDate}");

        [JilDirective("toFactor")]
        public long ToFactor { get; set; }

        #endregion Public Properties
    }
}