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

using MoreLinq;
using Quantler.Compression;
using Quantler.Data;
using Quantler.Data.Aggegrate;
using Quantler.Data.DataFile;
using Quantler.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Quantler.Utilities.DataConverter.DataConversionModels
{
    /// <summary>
    /// Conversion model for converting text data to Quantler DAT files
    /// </summary>
    /// <seealso cref="DataConversion" />
    internal abstract class BaseConversionModel : DataConversion
    {
        #region Protected Fields

        /// <summary>
        /// The data feed
        /// </summary>
        protected DataFeed DataFeed;

        #endregion Protected Fields

        #region Private Fields

        /// <summary>
        /// The input lines
        /// </summary>
        private readonly ConcurrentQueue<InputLine> _inputLines = new ConcurrentQueue<InputLine>();

        /// <summary>
        /// The minute aggregations (QuoteBars)
        /// </summary>
        private readonly Dictionary<string, TickAggregator> _minBarAggregators = new Dictionary<string, TickAggregator>();

        /// <summary>
        /// The minute data writers
        /// </summary>
        private readonly Dictionary<string, DataWriter> _minBarWriters = new Dictionary<string, DataWriter>();

        /// <summary>
        /// The tick writers
        /// </summary>
        private readonly Dictionary<string, DataWriter> _tickWriters = new Dictionary<string, DataWriter>();

        /// <summary>
        /// The end of file indication
        /// </summary>
        private bool _endOfFile;

        /// <summary>
        /// The options
        /// </summary>
        private Options _options;

        /// <summary>
        /// The current date
        /// </summary>
        private int _date;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Flushes the last data points.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<DataPoint> FlushLastDataPoints() =>
            new List<DataPoint>();

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="inputLine">The line.</param>
        /// <returns></returns>
        public abstract IEnumerable<DataPoint> GetDataPoints(InputLine inputLine);

        /// <summary>
        /// Starts this instance for conversion.
        /// </summary>
        /// <param name="options">The options.</param>
        public virtual void Start(Options options)
        {
            //Set input
            _options = options;

            try
            {
                //Start reader (reads lines on one thread and processes these lines on another)
                var files = GetFiles(options.InputFolder, options.Wildcard);
                Task.Run(() => ReadFiles(files));

                //Go trough lines
                InputLine line;
                long currentline = 0;
                long maxlines = 0;

                while (!_endOfFile | _inputLines.TryDequeue(out line))
                {
                    //Check if we have something to do
                    if (_inputLines.Count == 0 || line == null)
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    //Check line count
                    if (_endOfFile && maxlines == 0)
                    {
                        Console.WriteLine($"Parsing files....");
                        maxlines = _inputLines.Count;
                    }
                    else
                        currentline++;

                    //check progress
                    if (_endOfFile && currentline % (maxlines / 200) == 0)
                    {
                        Console.WriteLine(
                            $"Current progress: {(currentline / Convert.ToDecimal(maxlines)) * 100:##.##}%");
                    }

                    //Process line
                    Process(GetDataPoints(line));
                }

                //Flush converter (in case we have any left-overs in memory)
                Process(FlushLastDataPoints());

                //Flush all writers
                _minBarWriters.Values.ForEach(x => x.Flush());
                _tickWriters.Values.ForEach(x => x.Flush());

                //Close all writers
                _minBarWriters.Values.ForEach(x => x.Dispose());
                _tickWriters.Values.ForEach(x => x.Dispose());

                //Save in resulting archive (1 archive per day, per aggregation for all data. So this makes 3 archives per day in total).
                foreach (var folder in Directory.GetDirectories(options.OutputFolder).Select(x => new DirectoryInfo(x)))
                    Console.WriteLine($"Created new archive {Archive.Directory(folder.FullName, options.OutputFolder, $"{_date}.{folder.Name}.zip", false)}");

                //Check cleanup
                if (options.Cleanup)
                {
                    Console.WriteLine($"Performing cleanup of files");
                    files.ForEach(x => x.Delete());
                }

                //Cleanup non archived files
                Directory.GetDirectories(options.OutputFolder).ForEach(x => Directory.Delete(x, true));

                //Done
                Console.WriteLine("Done!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Errors while processing files: {e.Message}");
                Console.ReadKey();
                throw;
            }
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Reads the files and sends every line for parsing
        /// </summary>
        /// <param name="files">Files to read</param>
        protected virtual void ReadFiles(FileInfo[] files)
        {
            try
            {
                //Console log
                Console.WriteLine($"Found the following files as input: ");
                files.ForEach(x => Console.WriteLine($"\t \t {x.Name}"));
                Console.WriteLine($"Loading files...");

                //Get reader
                if (files.Length == 1)
                {
                    int linenumber = 1;

                    using (var progress = new ProgressBar())
                    using (var stream = new FileStream(files[0].FullName, FileMode.Open))
                    using (var reader = new StreamReader(stream))
                    {
                        //Loop through lines
                        while (!reader.EndOfStream)
                        {
                            //Set input
                            _inputLines.Enqueue(new InputLine
                            {
                                Filename = files[0].Name,
                                Hash = 0,
                                Line = reader.ReadLine(),
                                LineNumber = linenumber++
                            });

                            //Set progress
                            progress.Report((double)reader.BaseStream.Position / reader.BaseStream.Length);
                        }

                        //Set to end of file
                        _endOfFile = true;
                    }

                    //Done
                    return;
                }

                //Merge multiple files
                Dictionary<UInt64, InputLine> hashedlines = new Dictionary<UInt64, InputLine>();
                foreach (var file in files)
                {
                    //For merging files we use the unix timestamp which should be present in the file
                    long unixtimestamp = 0;
                    using (var stream = new FileStream(file.FullName, FileMode.Open))
                    using (var reader = new StreamReader(stream))
                    {
                        while (!reader.EndOfStream)
                        {
                            //get line
                            string line = reader.ReadLine();
                            string hasline = line;

                            //Check for split
                            if (line.Contains('|'))
                            {
                                var lines = line.Split('|');
                                unixtimestamp = long.Parse(string.Join("", lines[0].Length > 13 ? lines[0].Take(13) : lines[0]));
                                hasline = lines[1];
                            }
                            else if (line.Contains(','))
                            {
                                var lines = line.Split(',');
                                unixtimestamp = long.Parse(lines[0]);
                            }
                            else
                                unixtimestamp++; //Fall back

                            //Check line
                            UInt64 hash = CalculateHash(hasline);
                            if (!hashedlines.ContainsKey(hash))
                                hashedlines.Add(hash, new InputLine
                                {
                                    Hash = hash,
                                    Line = line,
                                    LineNumber = unixtimestamp
                                });
                        }
                    }
                }

                //Add data for processing
                using (var progress = new ProgressBar())
                {
                    long position = 0;
                    long progressnotify = hashedlines.Count / 50;
                    hashedlines.Values.OrderBy(x => x.LineNumber)
                        .ForEach(x =>
                        {
                            _inputLines.Enqueue(x);
                            position++;

                            if (position % progressnotify == 0)
                                progress.Report(position / hashedlines.Count);
                        });
                }

                //End of processing
                _endOfFile = true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Errors while reading files: {e.Message}");
                Console.ReadKey();
                throw;
            }
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Calculates the hash.
        /// </summary>
        /// <param name="read">String to hash.</param>
        /// <returns></returns>
        private UInt64 CalculateHash(string read)
        {
            UInt64 hashedValue = 3074457345618258791ul;
            foreach (char t in read)
            {
                hashedValue += t;
                hashedValue *= 3074457345618258799ul;
            }
            return hashedValue;
        }

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="filepattern">The filepattern.</param>
        /// <returns></returns>
        private FileInfo[] GetFiles(string folder, string filepattern) =>
            Directory.EnumerateFiles(folder, filepattern, SearchOption.AllDirectories)
                .Select(x => new FileInfo(x))
                .ToArray();

        /// <summary>
        /// Processes the specified data points.
        /// </summary>
        /// <param name="dataPoints">The data points.</param>
        private void Process(IEnumerable<DataPoint> dataPoints)
        {
            //Convert, aggregate and save
            foreach (var datapoint in dataPoints.OrderBy(x => x.Occured))
            {
                //Set meta-data
                string ticker = DataFeed.GetFeedTicker(datapoint.Ticker);
                if(_date == 0)
                    _date = int.Parse(datapoint.Occured.ToString("yyyyMMdd"));

                //Check for aggregation
                if (!_minBarAggregators.TryGetValue(ticker, out TickAggregator tickAggregator))
                {
                    _minBarWriters[ticker] = new DataWriter(Machine.Instance.GetPath(_options.OutputFolder), ticker, _date, "1m", DataType.TradeBar);
                    _minBarAggregators[ticker] = new TickAggregator(TimeSpan.FromMinutes(1));
                    _minBarAggregators[ticker].DataAggregated += (sender, data) => _minBarWriters[ticker].Feed(data);
                    tickAggregator = _minBarAggregators[ticker];
                }

                //Check for tick writer
                if (!_tickWriters.TryGetValue(ticker, out DataWriter tickwriter))
                {
                    tickwriter = new DataWriter(Machine.Instance.GetPath(_options.OutputFolder), ticker, _date, "1t", DataType.Tick);
                    _tickWriters[ticker] = tickwriter;
                }

                //Add data
                tickAggregator.Feed(datapoint);
                tickwriter.Feed(datapoint);
            }
        }

        #endregion Private Methods
    }
}