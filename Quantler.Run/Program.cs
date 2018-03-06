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
#endregion

using System;
using Quantler.Configuration;
using Quantler.Configuration.Model;
using NLog;
using System.Threading;
using System.Threading.Tasks;
using Quantler.Machine;
using Quantler.Bootstrap;
using Quantler.Interfaces;

namespace Quantler.Run
{
    class Program
    {
        /// <summary>
        /// Logging instance
        /// </summary>
        private static ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Main instance
        /// </summary>
        private static Bootstrap.Bootstrap Bootstrap;

        /// <summary>
        /// Program entrypoint
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //Initialize instance
            GlobalConfig config = null;
            try
            {
                config = Config.GlobalConfig;
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Could not load global configuration due to fatal error: {exc.Message}");
                return;
            }

            //Gracefully exit application
            AppDomain.CurrentDomain.ProcessExit += GracefullExit;

            //Log instance informatiom
            Thread.CurrentThread.Name = "Quantler Runner";
            Log.Trace($"Quantler Core Instance version: {Instance.QuantlerVersion}");
            Log.Trace($"Quantler Core Instance mode: {config.Mode}");
            Log.Trace($"Quantler Core Architecture: " + (Instance.Is64Bit ? "64 bit" : "32 bit"));
            Log.Trace($"Quantler Core Started: {DateTime.UtcNow} Utc");
            Log.Trace($"Quantler Core Free Memory: {Instance.MemoryAvailable} MB");

            //Set implementations for instance
            InstanceImplementations instanceimplementation;
            try
            {
                //Get implementations
                instanceimplementation = InstanceImplementations.FromConfig();
            }
            catch (Exception exc)
            {
                Log.Error($"Could not load handlers for instance due to exception: {exc.Message}");
                throw;
            }

            //Start running
            try
            {
                //Get runmode
                if (!Enum.TryParse(config.Mode, out RunMode runmode))
                {
                    Log.Error($"Could not derive run mode from entry {config.Mode}");
                    return;
                }

                //Start
                Bootstrap = new Bootstrap.Bootstrap(instanceimplementation);
                var bootstrap = Task.Run(() => Bootstrap.Run(runmode));

                //Wait for it
                bootstrap.Wait();
            }
            catch (Exception exc)
            {
                Log.Error(exc, $"Error while running instance");
                Console.WriteLine($"Press any key to exit...");
                Console.ReadKey();
            }

        }

        /// <summary>
        /// Gracefully exits the application.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private static void GracefullExit(object sender, EventArgs e)
        {
            try
            {
                Bootstrap.Exit();
            }
            catch (Exception exception)
            {
                //TODO: keep logging open before exit
                Log.Error(exception, $"Fatal exception during application exit {exception.Message}");
                Console.WriteLine($"Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}