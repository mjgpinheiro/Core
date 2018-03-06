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

using Newtonsoft.Json;

namespace Quantler.Configuration.Model
{
    public class ParameterConfig
    {
        #region Public Properties

        [JsonProperty("ModuleName")]
        public string ModuleName { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Value")]
        public string Value { get; set; }

        #endregion Public Properties
    }

    public class PortfolioConfig
    {
        #region Public Properties

        [JsonProperty("AccountType")]
        public string AccountType { get; set; }

        [JsonProperty("Assembly")]
        public string Assembly { get; set; }

        [JsonProperty("BaseCurrency")]
        public string BaseCurrency { get; set; }

        [JsonProperty("BrokerType")]
        public string BrokerType { get; set; }

        [JsonProperty("DisplayCurrency")]
        public string DisplayCurrency { get; set; }

        [JsonProperty("EndDateUtc")]
        public long EndDateUtc { get; set; }

        [JsonProperty("ExtendedMarketHours")]
        public bool ExtendedMarketHours { get; set; }

        [JsonProperty("Leverage")]
        public int Leverage { get; set; }

        [JsonProperty("QuantFunds")]
        public QuantFundConfig[] QuantFunds { get; set; }

        [JsonProperty("StartDateUtc")]
        public long StartDateUtc { get; set; }

        #endregion Public Properties
    }

    public class QuantFundConfig
    {
        #region Public Properties

        [JsonProperty("AllocatedFunds")]
        public long AllocatedFunds { get; set; }

        [JsonProperty("ForceTick")]
        public bool ForceTick { get; set; }

        [JsonProperty("ID")]
        public string Id { get; set; }

        [JsonProperty("Modules")]
        public string[] Modules { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Parameters")]
        public ParameterConfig[] Parameters { get; set; }

        [JsonProperty("StaticUniverse")]
        public UniverseConfig[] StaticUniverse { get; set; }

        [JsonProperty("UniverseName")]
        public string UniverseName { get; set; }

        #endregion Public Properties
    }

    public class UniverseConfig
    {
        #region Public Properties

        [JsonProperty("Ticker")]
        public string Ticker { get; set; }

        [JsonProperty("Weight")]
        public decimal Weight { get; set; }

        #endregion Public Properties
    }
}