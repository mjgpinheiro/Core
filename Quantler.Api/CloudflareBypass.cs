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

using CloudFlareUtilities;
using NLog;
using System;
using System.Net;
using System.Net.Http;

namespace Quantler.Api
{
    /// <summary>
    /// Helper class for bypassing cloudflare dns protection
    /// </summary>
    public static class CloudflareBypass
    {
        #region Private Fields

        /// <summary>
        /// Logging instance
        /// </summary>
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Gets the required cookies for bypassing cloudflare.
        /// </summary>
        /// <param name="url">The base URL.</param>
        /// <param name="useragent">User-Agent needs to macth the request</param>
        public static CookieContainer GetCookies(string url, string useragent)
        {
            CookieContainer cookies = new CookieContainer();
            try
            {
                var client = new HttpClient(new ClearanceHandler(new HttpClientHandler
                {
                    UseCookies = true,
                    CookieContainer = cookies // setting the shared CookieContainer
                }));

                HttpRequestMessage msg = new HttpRequestMessage
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Get
                };

                //Add custom user agent header
                if (!string.IsNullOrWhiteSpace(useragent))
                    msg.Headers.TryAddWithoutValidation("User-Agent", useragent);

                //Send request
                HttpResponseMessage response = client.SendAsync(msg).Result;
                if (!response.IsSuccessStatusCode)
                    Log.Error($"Error during cloudflare bypass. Could not retrieve page {url}, expected page 200 status code");
            }
            catch (AggregateException ex) when (ex.InnerException is CloudFlareClearanceException)
            {
                Log.Error(ex.InnerException, ex.InnerException.Message);
            }
            
            //return result
            return cookies;
        }

        #endregion Public Methods
    }
}