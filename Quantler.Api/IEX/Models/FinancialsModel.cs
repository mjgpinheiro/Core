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
using Jil;

namespace Quantler.Api.IEX.Models
{
    public class FinancialsModel
    {
        #region Public Properties

        [JilDirective("financials")]
        public FinancialsModelItem[] Financials { get; set; }

        [JilDirective("symbol")]
        public string Symbol { get; set; }

        #endregion Public Properties
    }

    public class FinancialsModelItem
    {
        #region Public Properties

        [JilDirective("cashChange")]
        public double CashChange { get; set; }

        [JilDirective("cashFlow")]
        public long CashFlow { get; set; }

        [JilDirective("costOfRevenue")]
        public long CostOfRevenue { get; set; }

        [JilDirective("currentAssets")]
        public long CurrentAssets { get; set; }

        [JilDirective("currentCash")]
        public long CurrentCash { get; set; }

        [JilDirective("currentDebt")]
        public long CurrentDebt { get; set; }

        [JilDirective("grossProfit")]
        public long GrossProfit { get; set; }

        [JilDirective("netIncome")]
        public long NetIncome { get; set; }

        [JilDirective("operatingExpense")]
        public long OperatingExpense { get; set; }

        [JilDirective("operatingGainsLosses")]
        public double OperatingGainsLosses { get; set; }

        [JilDirective("operatingIncome")]
        public long OperatingIncome { get; set; }

        [JilDirective("operatingRevenue")]
        public long OperatingRevenue { get; set; }

        [JilDirective("reportDate")]
        public DateTime ReportDate => Time.TryParseDate(reportDate, out DateTime returnvalue) ? returnvalue : throw new Exception($"Could not parse report date {reportDate}");

        [JilDirective("reportDate")]
        public string reportDate { get; set; }

        [JilDirective("researchAndDevelopment")]
        public long ResearchAndDevelopment { get; set; }

        [JilDirective("shareholderEquity")]
        public long ShareholderEquity { get; set; }

        [JilDirective("totalAssets")]
        public long TotalAssets { get; set; }

        [JilDirective("totalCash")]
        public long TotalCash { get; set; }

        [JilDirective("totalDebt")]
        public long TotalDebt { get; set; }

        [JilDirective("totalLiabilities")]
        public long TotalLiabilities { get; set; }

        [JilDirective("totalRevenue")]
        public long TotalRevenue { get; set; }

        #endregion Public Properties
    }
}