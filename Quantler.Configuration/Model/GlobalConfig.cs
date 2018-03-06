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
    public class GlobalConfig
    {
        #region Public Properties

        [JsonProperty("ActionsScheduler")]
        public string ActionsScheduler { get; set; }

        [JsonProperty("ApiKey")]
        public string ApiKey { get; set; }

        [JsonProperty("ApiSecret")]
        public string ApiSecret { get; set; }

        [JsonProperty("AssembliesPath")]
        public string AssembliesPath { get; set; }

        [JsonProperty("BacktestTimeMaxSeconds")]
        public int BacktestTimeMaxSeconds { get; set; }

        [JsonProperty("Benchmark")]
        public string Benchmark { get; set; }

        [JsonProperty("BrokerConnection")]
        public string BrokerConnection { get; set; }

        [JsonProperty("Cluster")]
        public string Cluster { get; set; }

        [JsonProperty("Currency")]
        public string Currency { get; set; }

        [JsonProperty("CurrencyRatesConfigFile")]
        public string CurrencyRatesConfigFile { get; set; }

        [JsonProperty("DataFeed")]
        public string DataFeed { get; set; }

        [JsonProperty("DataFeedEndpoint")]
        public string DataFeedEndpoint { get; set; }

        [JsonProperty("DataFeedWaitTimeInMS")]
        public int DataFeedWaitTimeInMS { get; set; }

        [JsonProperty("DataFilter")]
        public string DataFilter { get; set; }

        [JsonProperty("EventRunner")]
        public string EventRunner { get; set; }

        [JsonProperty("ExceptionHandler")]
        public string ExceptionHandler { get; set; }

        [JsonProperty("HistEnd")]
        public string HistEnd { get; set; }

        [JsonProperty("HistEndpoint")]
        public string HistEndpoint { get; set; }

        [JsonProperty("HistStart")]
        public string HistStart { get; set; }

        [JsonProperty("IgnoreVersionChecks")]
        public bool IgnoreVersionChecks { get; set; }

        [JsonProperty("JobQueue")]
        public string JobQueue { get; set; }

        [JsonProperty("MaxOrdersPerDay")]
        public int MaxOrdersPerDay { get; set; }

        [JsonProperty("MessageQueue")]
        public string MessageQueue { get; set; }

        [JsonProperty("Mode")]
        public string Mode { get; set; }

        [JsonProperty("NetMQRunnerPort")]
        public string NetMQRunnerPort { get; set; }

        [JsonProperty("OrderTicketHandler")]
        public string OrderTicketHandler { get; set; }

        [JsonProperty("SchedulerLoggingEnabled")]
        public bool SchedulerLoggingEnabled { get; set; }

        [JsonProperty("ServerLocation")]
        public string ServerLocation { get; set; }

        [JsonProperty("ServerName")]
        public string ServerName { get; set; }

        #endregion Public Properties
    }
}