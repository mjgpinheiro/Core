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

namespace Quantler.Api.IEX.Models
{
    /// <summary>
    /// Earnings report item model
    /// </summary>
    public class EarningsItemModel
    {
        #region Public Properties

        public double actualEPS { get; set; }
        public string announceTime { get; set; }
        public double consensusEPS { get; set; }
        public string EPSReportDate { get; set; }
        public double EPSSurpriseDollar { get; set; }
        public double estimatedEPS { get; set; }
        public string fiscalEndDate { get; set; }
        public string fiscalPeriod { get; set; }
        public int numberOfEstimates { get; set; }

        #endregion Public Properties
    }

    /// <summary>
    /// Earnings report model
    /// </summary>
    public class EarningsModel
    {
        #region Public Properties

        public IList<EarningsItemModel> earnings { get; set; }
        public string symbol { get; set; }

        #endregion Public Properties
    }
}