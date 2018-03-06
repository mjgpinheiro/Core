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

using Quantler.Configuration.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Quantler.Machine;

namespace Quantler.Configuration
{
    /// <summary>
    /// Configuration loader and central entry point for configuration files
    /// </summary>
    public static class Config
    {
        #region Private Fields

        /// <summary>
        /// Faster loading of existing config files
        /// </summary>
        private static readonly ConfigCache Cachedconfig = new ConfigCache();

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Load backtest configuration file
        /// </summary>
        public static SimulationConfig SimulationConfig => Cachedconfig.GetOrAdd<SimulationConfig>(ConfigFileType.Simulation);

        /// <summary>
        /// Get historical currency rates from config
        /// </summary>
        public static CurrencyRatesConfig[] CurrencyConfig
        {
            get
            {
                //Check if we need to load a non-default currency config file
                if (string.IsNullOrWhiteSpace(GlobalConfig.CurrencyRatesConfigFile))
                    return Cachedconfig.GetOrAdd<CurrencyRatesConfig[]>(ConfigFileType.CurrencyRates);
                else
                    return Cachedconfig.GetOrAdd<CurrencyRatesConfig[]>(GlobalConfig.CurrencyRatesConfigFile);
            }
        }

        /// <summary>
        /// Current default configuration files folder
        /// </summary>
        public static string DefaultConfigFolder { get; set; } = Instance.GetPath(new FileInfo(typeof(Config).GetTypeInfo().Assembly.Location).Directory.FullName, "DefaultConfig");

        /// <summary>
        /// Load global configuration for globally related settings
        /// </summary>
        public static GlobalConfig GlobalConfig => Cachedconfig.GetOrAdd<GlobalConfig>(ConfigFileType.Global);

        /// <summary>
        /// Load portfolio configuration for locally running portfolios
        /// </summary>
        public static PortfolioConfig PortfolioConfig => Cachedconfig.GetOrAdd<PortfolioConfig>(ConfigFileType.Portfolio);

        /// <summary>
        /// Get current market hours information
        /// </summary>
        public static MarketHoursConfig[] MarketHourConfig => Cachedconfig.GetOrAdd<MarketHoursConfig[]>(ConfigFileType.MarketHours);

        /// <summary>
        /// Get symbol details, for known symbols
        /// </summary>
        public static SecurityConfig[] SecurityConfig => Cachedconfig.GetOrAdd<SecurityConfig[]>(ConfigFileType.SecurityConfig);

        /// <summary>
        /// Gets the broker configuration.
        /// </summary>
        public static BrokerConfig[] BrokerConfig => Cachedconfig.GetOrAdd<BrokerConfig[]>(ConfigFileType.Broker);

        /// <summary>
        /// User configuration files folder
        /// </summary>
        public static string UserConfigFolder { get; set; } = Instance.GetPath(new FileInfo(typeof(Config).GetTypeInfo().Assembly.Location).Directory.FullName, "UserConfig");

        #endregion Public Properties

        #region Public Methods        

        /// <summary>
        /// Appends the configuration file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="I"></typeparam>
        /// <param name="data">The data.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static T AppendConfigFile<T, I>(T data, I item)
            where T : class
        {
            if (typeof(T) == typeof(CurrencyRatesConfig[]))
            {
                //get input
                IEnumerable<CurrencyRatesConfig> rates = data as CurrencyRatesConfig[];
                CurrencyRatesConfig added = item as CurrencyRatesConfig;

                //add item
                var tolist = rates.ToList();
                tolist.Add(added);
                rates = tolist;

                //save
                string newJson = JsonConvert.SerializeObject(rates);
                File.WriteAllText(ConfigCache.GetConfigPath(ConfigFileType.CurrencyRates), newJson);

                //Return what we currently have
                Cachedconfig.Put(ConfigFileType.CurrencyRates, rates);
                return CurrencyConfig as T;
            }
            else
                throw new Exception($"Append config does not have an implementation for config of type {data.GetType().FullName}");
        }

        /// <summary>
        /// Dynamically load a file for configurations not currently present
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="forcereload">Force reloading the values</param>
        /// <returns></returns>
        public static T LoadConfigFile<T>(string name, bool forcereload = false) => Cachedconfig.GetOrAdd<T>(name, forcereload);

        /// <summary>
        /// Tries to retrieve a variable which may have been supplied as an environment variable
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fallback"></param>
        /// <returns></returns>
        public static string TryGetEnvVariable(string name, string fallback = "")
        {
            //Set toreturn object
            string toreturn = Environment.GetEnvironmentVariable(name.ToLower());
            if (!string.IsNullOrWhiteSpace(toreturn))
                return toreturn;
            else
                return fallback;
        }

        #endregion Public Methods
    }
}