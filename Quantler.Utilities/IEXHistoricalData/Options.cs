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
using CommandLine;

namespace Quantler.Utilities.IEXHistoricalData
{
    /// <summary>
    /// Console options associated with iex utilities
    /// </summary>
    internal class Options
    {
        #region Public Properties

        /// <summary>
        /// Date to process
        /// </summary>
        [Option('d', "date", Required = false,
            HelpText = "Date to process, for example: 20171231. If no date is provided, current date is used")]
        public int Date { get; set; } = int.Parse(DateTime.UtcNow.Date.ToString("yyyyMMdd"));

        /// <summary>
        /// IEX Download folder
        /// </summary>
        [Option('f', "folder", Required = true, HelpText = "Location where IEX downloads are put.")]
        public string IEXDownloadFolder { get; set; }

        /// <summary>
        /// IEX Download folder
        /// </summary>
        [Option('t', "version", Required = false, Default = "1.5", HelpText = "Target verison (currently TOPS 1.5 or TOPS 1.6)")]
        public string TargetVersion { get; set; }

        #endregion Public Properties
    }
}