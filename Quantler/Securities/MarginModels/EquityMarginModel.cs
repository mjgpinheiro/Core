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
using Quantler.Interfaces;

namespace Quantler.Securities.MarginModels
{
    //TODO: implement equity margin model (margin based trading is not supported during beta)
    public class EquityMarginModel : MarginModel
    {
        public EquityMarginModel(decimal x, decimal y)
        {
            
        }

        public decimal GetInitialMarginRequired(Security security)
        {
            throw new NotImplementedException();
        }

        public decimal GetLeverage(Security security)
        {
            throw new NotImplementedException();
        }

        public decimal GetMaintenanceMargin(Security security)
        {
            throw new NotImplementedException();
        }

        public decimal GetMaintenanceMarginRequired(Security security)
        {
            throw new NotImplementedException();
        }

        public decimal GetRemainingMargin(IQuantFund quantfund = null)
        {
            throw new NotImplementedException();
        }
    }
}