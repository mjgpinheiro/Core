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

using Flurl;
using Flurl.Http;
using Jil;
using NLog;
using System;
using System.Threading.Tasks;

namespace Quantler.Api
{
    /// <summary>
    /// Api client for processing api requests
    /// </summary>
    public abstract class ApiClient
    {
        #region Protected Fields

        /// <summary>
        /// Base url to use for requests
        /// </summary>
        protected string Endpoint;

        #endregion Protected Fields

        #region Private Fields

        /// <summary>
        /// Logging instance
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        #endregion Private Fields

        #region Private Methods

        /// <summary>
        /// Creates the safe URL path.
        /// </summary>
        /// <param name="parts">The parts.</param>
        /// <returns></returns>
        private string CreateSafeUrlPath(params string[] parts) =>
            Url.Combine(parts);

        #endregion Private Methods

        #region Protected Methods

        /// <summary>
        /// Execute request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseurl"></param>
        /// <param name="request"></param>
        /// <param name="suppresserrors"></param>
        /// <returns></returns>
        protected virtual async Task<T> ExecuteRequestAsync<T>(string baseurl, string request, bool suppresserrors = true)
        {
            try
            {
                //Get url
                var url = CreateSafeUrlPath(baseurl, request);

                //Send request and return results
                var data = await url.GetStringAsync();
                return JSON.Deserialize<T>(data);
            }
            catch (Exception exc)
            {
                if (!suppresserrors)
                    throw;
                else
                    _log.Error(exc, $"Could not execute api request {baseurl}");
            }

            //Return nothing
            return default(T);
        }

        /// <summary>
        /// Execute request
        /// </summary>
        /// <param name="baseurl"></param>
        /// <param name="request"></param>
        /// <param name="suppresserrors"></param>
        /// <returns></returns>
        protected virtual async Task<string> ExecuteRequestAsync(string baseurl, string request, bool suppresserrors = true)
        {
            try
            {
                //Get url
                var url = CreateSafeUrlPath(baseurl, request);

                //Send request and return results
                var result = await url.GetStringAsync();
                return result;
            }
            catch (Exception exc)
            {
                if (!suppresserrors)
                    throw;
                else
                    _log.Error(exc, $"Could not execute api request {baseurl}");
            }

            //Return nothing
            return string.Empty;
        }

        #endregion Protected Methods
    }
}