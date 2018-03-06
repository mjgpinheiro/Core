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
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;
using Quantler.Common;
using Quantler.Data;
using Quantler.Data.Market.Filter;
using Quantler.Interfaces;
using Quantler.Messaging;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Quantler.Data.Market;
using static System.Console;

namespace Quantler.Utilities.DataCapture
{
    /// <summary>
    /// Program entry for data capture purposes
    /// </summary>
    public class Program
    {
        #region Internal Methods

        /// <summary>
        /// Main entry point
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <exception cref="Exception">Could not initialize instance of data feed " + options.Datafeed</exception>
        internal static void Main(string[] args)
        {
            //Check on action to perform
            try
            {
                //Get options in use
                var y = Parser.Default.ParseArguments<Options>(args);
                if (y.Tag != ParserResultType.Parsed)
                    return;

                Options options = ((Parsed<Options>)y).Value;

                WriteLine($"Initializing data capture instance.");

                //nlog is writer for data
                var config = new LoggingConfiguration();
                var filetarget = new FileTarget();
                config.AddTarget("log", filetarget);
                filetarget.FileName = $"{options.Destination}/capture.txt";
                filetarget.ArchiveFileName = $"{options.Destination}/archive/capture" + ".{#}.zip";
                filetarget.Layout = @"${message}";
                filetarget.MaxArchiveFiles = 0;
                filetarget.ArchiveNumbering = ArchiveNumberingMode.Date;
                filetarget.ArchiveEvery = FileArchivePeriod.Day;
                filetarget.KeepFileOpen = true;
                filetarget.OpenFileCacheTimeout = 30;
                filetarget.EnableArchiveFileCompression = true;

                //Use async writer for better performance
                var asyncwrapper = new AsyncTargetWrapper("wrapper", filetarget);
                asyncwrapper.TimeToSleepBetweenBatches = 0;
                asyncwrapper.OverflowAction = AsyncTargetWrapperOverflowAction.Grow;
                asyncwrapper.BatchSize = 50000;

                //Set new rules
                config.LoggingRules.Add(new LoggingRule("Quantler.Util.DataCapture.*", LogLevel.Trace, asyncwrapper));
                LogManager.Configuration = config;

                //Get instance
                WriteLine($"Initializing DataFeed instance {options.Datafeed}");
                if (!DynamicLoader.Instance.TryGetInstance(options.Datafeed, out DataFeed feed))
                    throw new Exception("Could not initialize instance of data feed " + options.Datafeed);
                feed.SetDependencies(null, null, new NoDataFilter());

                //Initialize feed
                ILogger Log = LogManager.GetCurrentClassLogger();
                feed.Initialize(new LiveTradingMessage());
                feed.SetDataCapture(data => Log.Info(DateTime.UtcNow.ToUnixTime(true).ToString() + "|" + Regex.Replace(data, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1"))); //Regex is used to minify any json retrieved

                //Start feed
                WriteLine($"Starting feed");
                Thread datafeedthread = new Thread(feed.Start) { IsBackground = true, Name = "DataFeed Thread" };
                datafeedthread.Start();

                //Wait for it to start and get the firehose
                WriteLine($"Waiting to be fully connected");
                Thread.Sleep(15000);
                WriteLine($"Adding firehose subscription");
                feed.AddSubscription(DataSubscriptionRequest.GetFireHoseSubscriptionRequest(feed.DataSource)); //Request firehose

                //Keep running
                while (feed.GetAvailableDataPackets().Any() || feed.IsRunning)
                {
                    //Release current available data points, so we are not filling up memory
                }

                //Closure
                WriteLine("Feed stopped");
            }
            catch (Exception exc)
            {
                WriteLine("Exception occured!");
                WriteLine(exc.Message);
            }
            finally
            {
                ReadKey();
            }
        }

        #endregion Internal Methods
    }
}