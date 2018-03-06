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
using Quantler.DataFeeds.BinancePublic;
using Quantler.Messaging;
using System;
using System.Collections.Generic;
using System.Composition;

namespace Quantler.Utilities.DataConverter.DataConversionModels
{
    /// <summary>
    /// Binance data conversion model
    /// </summary>
    /// <seealso cref="BaseConversionModel" />
    [Export(typeof(DataConversion))]
    internal class BinanceDataConversionModel : BaseConversionModel
    {
        #region Private Fields

        /// <summary>
        /// The data feed
        /// </summary>
        private readonly BinanceDataFeed _binanceDataFeed;

        /// <summary>
        /// The log
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HitBTCDataConversionModel"/> class.
        /// </summary>
        public BinanceDataConversionModel()
        {
            _binanceDataFeed = new BinanceDataFeed();
            _binanceDataFeed.Initialize(new LiveTradingMessage());
            DataFeed = _binanceDataFeed;
        }

        #endregion Public Constructors

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
            if (lines.Length > 0)
            {
                //Set data
                string symbol = String.Empty;

                //Get data
                string data;
                if (lines.Length == 2)
                    data = lines[1];
                else
                {
                    symbol = lines[1];
                    data = lines[2];
                }

                //convert
                return _binanceDataFeed.ParseData(data, symbol);
            }
            else
            {
                _log.Warn($"Cannot parse data from line as it is incorrect");
                return new DataPoint[0];
            }
        }

        #endregion Public Methods
    }
}