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

using Quantler.Data;
using Quantler.DataFeeds.BittrexPublic;
using System;
using System.Collections.Generic;
using System.Composition;
using NLog;
using Quantler.Messaging;

namespace Quantler.Utilities.DataConverter.DataConversionModels
{
    /// <summary>
    /// Bittrex datafeed conversion model
    /// </summary>
    /// <seealso cref="BaseConversionModel" />
    [Export(typeof(DataConversion))]
    internal class BittrexDataConversionModel : BaseConversionModel
    {
        /// <summary>
        /// The log
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BittrexDataConversionModel"/> class.
        /// </summary>
        public BittrexDataConversionModel()
        {
            BittrexDataFeed = new BittrexDataFeed();
            BittrexDataFeed.Initialize(new LiveTradingMessage());
            DataFeed = BittrexDataFeed;
        }

        #endregion Public Constructors

        #region Private Properties

        /// <summary>
        /// The bittrex data feed.
        /// </summary>
        private BittrexDataFeed BittrexDataFeed { get; }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="inputLine">The line.</param>
        /// <returns></returns>
        public override IEnumerable<DataPoint> GetDataPoints(InputLine inputLine)
        {
            //check line
            var lines = inputLine.Line.Split('|');
            string data;
            if (lines.Length > 0 && long.TryParse(lines[0], out long currentutc))
            {
                DateTime occured = Time.FromUnixTime(currentutc, true);
                data = lines[1];

                //convert
                return BittrexDataFeed.ParseData(data, occured);
            }
            else
            {
                _log.Warn($"Cannot parse data from line as it is missing time");
                return new DataPoint[0];
            }
        }

        #endregion Public Methods
    }
}