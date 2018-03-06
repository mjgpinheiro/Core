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

using Flurl.Http;
using Quantler.Api.IEX;
using Quantler.Api.IEX.Models;
using Quantler.Compression;
using System.Linq;
using static System.Console;

namespace Quantler.Utilities.IEXHistoricalData
{
    /// <summary>
    /// Downloads a historical data archive from IEX
    /// </summary>
    internal static class IEXHistDownloader
    {
        #region Private Properties

        /// <summary>
        /// Gets or sets the iex API.
        /// </summary>
        private static IEXApi IEXApi { get; set; }

        #endregion Private Properties

        #region Internal Methods

        /// <summary>
        /// Process request
        /// </summary>
        /// <param name="options"></param>
        internal static void ProcessRequest(Options options)
        {
            //Get from api endpoint
            IEXApi = new IEXApi();
            var data = IEXApi.GetHISTAsync(options.Date).Result;
            HISTModel iexhist;

            //Check if we have a response
            if (data == null || data.Count(x => x.Feed == "TOPS") == 0)
            {
                WriteLine("Error, exiting iex hist downloader, no response from api.");
                return;
            }
            else
                iexhist = data.FirstOrDefault(x => x.Feed == "TOPS" && x.Version == options.TargetVersion);

            //Check if we have a link
            if (iexhist == null)
            {
                WriteLine("ERROR: did not retrieve a link from IEX API. Exiting...");
                return;
            }

            //Download file and save it
            WriteLine($"Downloading file {iexhist.Link}");
            var downloadedfile = iexhist.Link.DownloadFileAsync(options.IEXDownloadFolder).Result;

            //Unpack
            WriteLine("Done downloading file: " + downloadedfile);
            WriteLine($"Extracting file");
            Archive.ExtractGz(downloadedfile, options.IEXDownloadFolder);

            WriteLine("Done, exiting...");
        }

        #endregion Internal Methods
    }
}