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
using Jil;

namespace Quantler.Api.IEX.Models
{
    public class DividendsModel
    {
        #region Public Properties

        /// <summary>
        /// refers to the payment amount
        /// </summary>
        [JilDirective("amount")]
        public double Amount { get; set; }

        /// <summary>
        /// refers to the dividend declaration date
        /// </summary>
        [JilDirective("declaredDate")]
        public string declaredDate { get; set; }

        /// <summary>
        /// refers to the dividend ex-date
        /// </summary>
        [JilDirective("exDate")]
        public string exDate { get; set; }

        /// <summary>
        /// refers to the payment date
        /// </summary>
        [JilDirective("paymentDate")]
        public string paymentDate { get; set; }

        /// <summary>
        /// refers to the dividend record date
        /// </summary>
        [JilDirective("recordDate")]
        public string recordDate { get; set; }

        /// <summary>
        /// refers to the dividend declaration date
        /// </summary>
        [JilDirective(true)]
        public DateTime DeclaredDate => Time.TryParseDate(declaredDate, out DateTime returnvalue) ? returnvalue : throw new Exception($"Could not parse date {declaredDate}");

        /// <summary>
        /// refers to the dividend ex-date
        /// </summary>
        [JilDirective(true)]
        public DateTime ExDate => Time.TryParseDate(exDate, out DateTime returnvalue) ? returnvalue : throw new Exception($"Could not parse date {ExDate}");

        /// <summary>
        /// refers to the payment date
        /// </summary>
        [JilDirective(true)]
        public DateTime PaymentDate => Time.TryParseDate(paymentDate, out DateTime returnvalue) ? returnvalue : throw new Exception($"Could not parse date {paymentDate}");

        /// <summary>
        /// refers to the dividend record date
        /// </summary>
        [JilDirective(true)]
        public DateTime RecordDate => Time.TryParseDate(recordDate, out DateTime returnvalue) ? returnvalue : throw new Exception($"Could not parse date {recordDate}");

        /// <summary>
        /// refers to the dividend income type 
        /// P = Partially qualified income 
        /// Q = Qualified income 
        /// N = Unqualified income 
        /// null = N/A or unknown
        /// </summary>
        [JilDirective("qualified")]
        public string Qualified { get; set; }

        /// <summary>
        /// refers to the dividend payment type (Dividend income, Interest income, Stock dividend, Short term capital gain, Medium term capital gain, Long term capital gain, Unspecified term capital gain)
        /// </summary>
        [JilDirective("type")]
        public string Type { get; set; }

        #endregion Public Properties
    }
}