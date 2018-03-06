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

using CommandLine;

namespace Quantler.Utilities.DataConverter
{
    public class Options
    {
        #region Public Properties

        /// <summary>
        /// Cleanup any downloaded files
        /// </summary>
        [Option('c', "cleanup", Required = false, Default = false,
            HelpText = "Cleanup (remove) input files after processing is finished")]
        public bool Cleanup { get; set; }

        /// <summary>
        /// Extension
        /// </summary>
        [Option('w', "wildcard", Required = false, Default = "*.csv", HelpText = "File wildcard to search for")]
        public string Wildcard { get; set; }

        /// <summary>
        /// Conversion Implementation
        /// </summary>
        [Option('d', "dataconversion", Required = true, HelpText = "DataConversion implementation to use (for example: Quantler.Util.DataConverter.DataConversionModels.IEXDataConversionModel)")]
        public string DataConversion { get; set; }

        /// <summary>
        /// Output folder for resulting data
        /// </summary>
        [Option('i', "input", Required = true, HelpText = "Input folder for data")]
        public string InputFolder { get; set; }

        /// <summary>
        /// Output folder for resulting data
        /// </summary>
        [Option('o', "output", Required = true, HelpText = "Output folder for data")]
        public string OutputFolder { get; set; }

        #endregion Public Properties
    }
}