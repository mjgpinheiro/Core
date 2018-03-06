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
using System.IO;
using CacheManager.Core;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json.Linq;
using NLog;
using Quantler.Machine;

namespace Quantler.Configuration
{
    internal class ConfigCache
    {
        #region Private Fields

        /// <summary>
        /// Logging instance
        /// </summary>
        private static ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Cached json objects
        /// </summary>
        private readonly ICacheManager<JContainer> _cached = CacheFactory.Build<JContainer>(x =>
            x.WithDictionaryHandle("config").WithExpiration(ExpirationMode.Sliding, TimeSpan.FromMinutes(1)));

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Get path for config file
        /// </summary>
        /// <param name="filetype"></param>
        /// <returns></returns>
        public static string GetConfigPath(ConfigFileType filetype) =>
            Instance.GetPath(Config.DefaultConfigFolder, $"{filetype.ToString()}.json");

        /// <summary>
        /// Get or add config gile from config cache
        /// </summary>
        /// <typeparam name="T">Type of config file</typeparam>
        /// <param name="file">Config file</param>
        /// <param name="reload">If true, force reload from source</param>
        /// <returns></returns>
        public T GetOrAdd<T>(ConfigFileType file, bool reload = false) => GetOrAdd<T>(file.ToString(), reload);

        /// <summary>
        /// Puts the specified data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file">The file.</param>
        /// <param name="data">The data.</param>
        public void Put<T>(ConfigFileType file, T data) =>
            _cached[file.ToString()] = JObject.FromObject(data);

        /// <summary>
        /// Get or add config file from config cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <param name="reload">Force reload if needed</param>
        /// <returns></returns>
        public T GetOrAdd<T>(string filename, bool reload = false)
        {
            //Check if this is an array or not
            if (typeof(T).IsArray)
            {
                JContainer found = _cached.Get(filename);
                JArray toreturn = null;
                if (found == null || reload)
                {
                    //Get global config (default values)
                    JArray config = LoadArrayConfig(filename);

                    //Add to cached config
                    _cached[filename] = config;
                    toreturn = config;
                }
                else
                    toreturn = found as JArray;

                //Return what we have
                return toreturn.ToObject<T>();
            }
            else
            {
                JContainer found = _cached.Get(filename);
                JObject toreturn = null;
                if (found == null || reload)
                {
                    //Get global config (default values)
                    JObject config = LoadObjectConfig(filename);

                    //Merge any user based settings
                    if (File.Exists(Instance.GetPath(Instance.AssemblyDirectory, "UserConfig", filename)))
                        config = MergeUserConfig(filename, config);

                    //Add to cached config
                    _cached[filename] = config;
                    toreturn = config;
                }
                else
                    toreturn = found as JObject;

                //Return what we have
                return toreturn.ToObject<T>();
            }

        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Load default config file from file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private JObject LoadObjectConfig(string file)
        {
            //Get content
            string content = string.Empty;

            //Check if this is a remote file or a local file
            if (Url.IsValid(file))
            {
                try
                {
                    content = file.GetStringAsync().Result;
                }
                catch (Exception exc)
                {
                    _log.Error(exc, $"Could not load config file {file} due to error");
                    throw exc;
                }
            }
            else
                content = LoadFile(Config.DefaultConfigFolder, $"{file}.json");

            //Check if we have content
            if (string.IsNullOrWhiteSpace(content))
                throw new FileNotFoundException($"Could not find and load config file: {file}");

            //Return default config
            return JObject.Parse(content);
        }

        /// <summary>
        /// Load default config file from file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private JArray LoadArrayConfig(string file)
        {
            //Get content
            string content = string.Empty;

            //Check if this is a remote file or a local file
            if (Url.IsValid(file))
            {
                try
                {
                    content = file.GetStringAsync().Result;
                }
                catch (Exception exc)
                {
                    _log.Error(exc, $"Could not load config file {file} due to error");
                    throw exc;
                }
            }
            else
                content = LoadFile(Config.DefaultConfigFolder, $"{file}.json");

            //Check if we have content
            if (string.IsNullOrWhiteSpace(content))
                throw new FileNotFoundException($"Could not find and load config file: {file}");

            //Return default config
            return JArray.Parse(content);
        }

        /// <summary>
        /// Load content of config file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        private string LoadFile(string path, string filename)
        {
            //Get path
            var filepath = Instance.GetPath(path, filename);

            //Check if file exists
            if (!File.Exists(filepath))
            {
                _log.Warn($"Could not load config file with path {filepath}");
                return string.Empty;
            }
            else
                return File.ReadAllText(Instance.GetPath(path, filename));
        }

        /// <summary>
        /// Merge user configuration file with default configuration
        /// </summary>
        /// <param name="file"></param>
        /// <param name="currentconfig"></param>
        /// <returns></returns>
        private JObject MergeUserConfig(string file, JObject currentconfig)
        {
            //Get user config
            string content = LoadFile(Config.UserConfigFolder, $"{file}.json");

            //check if we have any data
            if (string.IsNullOrWhiteSpace(content))
                return currentconfig;

            //else we need to merge
            JObject userconfig = JObject.Parse(content);

            //Merge user config
            currentconfig.Merge(userconfig, new JsonMergeSettings()
            {
                MergeArrayHandling = MergeArrayHandling.Replace,
                MergeNullValueHandling = MergeNullValueHandling.Ignore
            });

            //Return new config
            return currentconfig;
        }

        #endregion Private Methods
    }
}