#region License Header

/*
*
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
*
*/

#endregion License Header

using Newtonsoft.Json;

namespace Quantler.Configuration.Model
{
    /// <summary>
    /// Broker related config model
    /// </summary>
    public class BrokerConfig
    {
        #region Public Properties

        [JsonProperty("BrokerType")]
        public string BrokerType { get; set; }

        [JsonProperty("CurrencyImplementation")]
        public string CurrencyImplementation { get; set; }

        [JsonProperty("DataFeed")]
        public string DataFeed { get; set; }

        [JsonProperty("Exchanges")]
        public string[] Exchanges { get; set; }

        [JsonProperty("BrokerConnection")]
        public string BrokerConnection { get; set; }

        [JsonProperty("OrderTicketHandler")]
        public string OrderTicketHandler { get; set; }

        #endregion Public Properties
    }
}