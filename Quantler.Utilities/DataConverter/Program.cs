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

using Quantler.Common;
using System;
using System.IO;
using CommandLine;
using static System.Console;

namespace Quantler.Utilities.DataConverter
{
    public class Program
    {
        #region Internal Methods

        public static void Main(string[] args)
        {
            //Check on action to perform
            try
            {
                //Get options in use
                var y = Parser.Default.ParseArguments<Options>(args);
                if (y.Tag != ParserResultType.Parsed)
                    return;

                Options options = ((Parsed<Options>) y).Value;

                //Some information before processing
                WriteLine(
                    $"Starting processing {options.InputFolder}, with wildcard {options.Wildcard}. Output folder {options.OutputFolder}");

                //Some initial checks
                if (!Directory.Exists(options.InputFolder))
                {
                    WriteLine($"Directory path {options.InputFolder} does not exists, thus no input found");
                    return;
                }
                if (!Directory.Exists(options.OutputFolder))
                {
                    WriteLine($"Directory path {options.OutputFolder} does not exists, creating it");
                    Directory.CreateDirectory(options.OutputFolder);
                }

                //Get instance
                if (!DynamicLoader.Instance.TryGetInstance(options.DataConversion, out DataConversion conversion))
                    throw new Exception("Could not initialize instance of data conversion " + options.DataConversion);

                //Process
                conversion.Start(options);
            }
            catch (Exception exc)
            {
                WriteLine("Exception occured!");
                WriteLine(exc.Message);
            }
            finally
            {
                WriteLine($"Press any key to exit...");
                ReadKey();
            }
        }

        #endregion Internal Methods
    }
}