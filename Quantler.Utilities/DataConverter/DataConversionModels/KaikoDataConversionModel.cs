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

using NLog;
using Quantler.Data;
using Quantler.Data.Market;
using Quantler.DataFeeds;
using Quantler.DataFeeds.BinancePublic;
using Quantler.DataFeeds.BittrexPublic;
using Quantler.DataFeeds.HitBtcPublic;
using Quantler.Messaging;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Quantler.Utilities.DataConverter.DataConversionModels
{
    /// <summary>
    /// Kaiko based data conversion model
    /// </summary>
    /// <seealso cref="BaseConversionModel" />
    [Export(typeof(DataConversion))]
    internal class KaikoDataConversionModel : BaseConversionModel
    {
        #region Private Fields

        /// <summary>
        /// The aggregated lines, we need to wait for a full snapshot to be loaded
        /// </summary>
        private readonly Dictionary<string, Snapshot> _aggregatedLines = new Dictionary<string, Snapshot>();

        /// <summary>
        /// Current instance log
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Flushes the last data points.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<DataPoint> FlushLastDataPoints()
        {
            //Set return object
            List<DataPoint> datapoints = new List<DataPoint>();

            //Get all last datapoints
            foreach (var item in _aggregatedLines)
                datapoints.Add(GetLevel1Quote(item.Key, item.Value.InputLines, item.Value.Date));

            //Return data
            return datapoints;
        }

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="inputLine">The line.</param>
        /// <returns></returns>
        public override IEnumerable<DataPoint> GetDataPoints(InputLine inputLine)
        {
            //Get input
            var data = inputLine.Line.Split(',');
            var toreturn = new List<DataPoint>();

            try
            {
                //Check input
                if (data.Length < 4)
                    throw new Exception("Missing data");
                else if (Regex.Matches(data[0], @"[a-zA-Z]").Count > 0) //Header
                    return toreturn;
                else if (data.Length > 4) //Trade data
                {
                    //Prob. trade data
                    DateTime occured = Time.FromUnixTime(long.Parse(data[3]), true);
                    toreturn.Add(new Tick(DataFeed.GetQuantlerTicker(data[2]), DataFeed.DataSource)
                    {
                        TradePrice = decimal.Parse(data[4], NumberStyles.Any, new CultureInfo("en-US")),
                        Size = decimal.Parse(data[5], NumberStyles.Any, new CultureInfo("en-US")),
                        Occured = occured,
                        TimeZone = TimeZone.Utc
                    });
                    return toreturn;
                }

                //Get data
                DateTime date = Time.FromUnixTime(long.Parse(data[0]), true);
                string ticker = inputLine.Filename.Split('_')[1];

                //Get Current Snapshot
                if (!_aggregatedLines.TryGetValue(ticker, out Snapshot snapshot))
                {
                    snapshot = new Snapshot(date);
                    _aggregatedLines.Add(ticker, snapshot);
                }

                //Check if this snapshot has ended
                if (snapshot.Date < date)
                {
                    //Return the level 1 quote
                    toreturn.Add(GetLevel1Quote(ticker, snapshot.InputLines, snapshot.Date));

                    //Remove the old lines from this snapshot and add the current new one
                    snapshot.Reset(date, inputLine);
                }
                else //Add to known lines
                    snapshot.InputLines.Add(inputLine);
            }
            catch
            {
                _log.Warn($"Cannot parse data from line as it is incorrect");
            }

            //Return what we have
            return toreturn;
        }

        /// <summary>
        /// Starts this instance for conversion.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <exception cref="Exception">Could not derive datasource from filename!</exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public override void Start(Options options)
        {
            //Get files
            var filesfound = Directory.GetFiles(options.InputFolder, options.Wildcard, SearchOption.AllDirectories).Select(x => new FileInfo(x).Name);
            var datasource = filesfound.FirstOrDefault(x => x.Contains("_"))?.Split("_")[0];

            //Try and get datasource and if needed the datafeed
            if (!Enum.TryParse(typeof(DataSource), datasource, true, out object objsource))
                throw new Exception("Could not derive datasource from filename!");

            //Check if datafeed is loaded
            DataSource source = (DataSource)objsource;
            if (DataFeed == null)
            {
                switch (source)
                {
                    case DataSource.Bittrex:
                        DataFeed = new BittrexDataFeed();
                        DataFeed.Initialize(new LiveTradingMessage());
                        break;

                    case DataSource.HitBtc:
                        DataFeed = new HitBtcDataFeed();
                        DataFeed.Initialize(new LiveTradingMessage());
                        break;

                    case DataSource.Binance:
                        DataFeed = new BinanceDataFeed();
                        DataFeed.Initialize(new LiveTradingMessage());
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            //Continue with base
            base.Start(options);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Gets the level1 quote from the current input lines.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private DataPoint GetLevel1Quote(string ticker, List<InputLine> data, DateTime date)
        {
            //Creaate new orderbook
            OrderBook orderbook = new OrderBook(ticker);

            //Get lines
            foreach (var line in data)
            {
                //Get data
                var input = line.Line.Split(',');
                bool isBid = input[1] == "b";
                double price = double.Parse(input[2], NumberStyles.Any, new CultureInfo("en-US"));
                double size = double.Parse(input[3], NumberStyles.Any, new CultureInfo("en-US"));

                //Add to book
                orderbook.AddQuote(isBid, price, size);
            }

            //Return current quote
            return new Tick(DataFeed.GetQuantlerTicker(ticker), DataFeed.DataSource)
            {
                AskSize = Convert.ToDecimal(orderbook.AskSize),
                AskPrice = Convert.ToDecimal(orderbook.BestAsk),
                BidPrice = Convert.ToDecimal(orderbook.BestBid),
                BidSize = Convert.ToDecimal(orderbook.BidSize),
                Depth = 0,
                Occured = date,
                TimeZone = TimeZone.Utc
            };
        }

        #endregion Private Methods

        #region Private Classes

        /// <summary>
        /// Snapshot information from Kaiko
        /// </summary>
        private class Snapshot
        {
            #region Public Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Snapshot"/> class.
            /// </summary>
            /// <param name="date">The date.</param>
            public Snapshot(DateTime date) => Date = date;

            #endregion Public Constructors

            #region Public Properties

            /// <summary>
            /// Gets the date.
            /// </summary>
            public DateTime Date { get; private set; }

            /// <summary>
            /// Gets the input lines.
            /// </summary>
            public List<InputLine> InputLines { get; } = new List<InputLine>();

            #endregion Public Properties

            #region Public Methods

            /// <summary>
            /// Resets this instance with a new date.
            /// </summary>
            /// <param name="date">The date.</param>
            /// <param name="initialLine">The initial line.</param>
            public void Reset(DateTime date, InputLine initialLine)
            {
                Date = date;
                InputLines.Clear();
                InputLines.Add(initialLine);
            }

            #endregion Public Methods
        }

        #endregion Private Classes
    }
}