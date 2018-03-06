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

using Jil;
using Quantler.Data;
using Quantler.Data.Market;
using Quantler.DataFeeds.IEXPublic;
using Quantler.Messaging;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Globalization;

namespace Quantler.Utilities.DataConverter.DataConversionModels
{
    /// <summary>
    /// IEX data conversion model
    /// TODO: test with 1.6 and see if this is also comptabble with 1.5
    /// </summary>
    /// <seealso cref="BaseConversionModel" />
    [Export(typeof(DataConversion))]
    internal class IEXDataConversionModel : BaseConversionModel
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IEXDataConversionModel"/> class.
        /// </summary>
        public IEXDataConversionModel()
        {
            DataFeed = new IEXDataFeed();
            DataFeed.Initialize(new LiveTradingMessage());
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
            //Get data in json format
            RootObject data = JSON.Deserialize<RootObject>(inputLine.Line, Jil.Options.ISO8601);

            //Go trough messages
            foreach (var message in data.Messages)
            {
                //Get current time
                if(!DateTime.TryParseExact(message.Timestamp[0], "yyyy-MM-dd HH:mm:ss.ffffff", null, DateTimeStyles.None,
                    out DateTime utc))
                if(!DateTime.TryParse(message.Timestamp[0], out utc))
                        continue;

                //Get bidprice and askprice
                decimal bidprice = message.BidPrice / 10000m;
                decimal askprice = message.AskPrice / 10000m;

                //Create tick
                DateTime nytime = utc.ConvertTo(TimeZone.Utc, TimeZone.NewYork);
                yield return new Tick(DataFeed.GetQuantlerTicker(message.Symbol), DataSource.IEX)
                {
                    AskSize = message.AskSize,
                    AskPrice = askprice,
                    BidSize = message.BidSize,
                    BidPrice = bidprice,
                    Depth = 0,
                    Occured = nytime,
                    TimeZone = TimeZone.NewYork
                };
            }
        }

        #endregion Public Methods

        #region Private Classes

        /// <summary>
        /// Trading flags
        /// </summary>
        private class Flags
        {
            #region Public Properties

            public int H { get; set; }
            public int P { get; set; }

            #endregion Public Properties
        }

        /// <summary>
        /// Message as received by IEX feed
        /// </summary>
        private class Message
        {
            #region Public Properties

            [JilDirective("askSize")]
            public long AskSize { get; set; }

            [JilDirective("timestamp")]
            public string[] Timestamp { get; set; }

            [JilDirective("symbol")]
            public string Symbol { get; set; }

            [JilDirective("messageType")]
            public string MessageType { get; set; }

            [JilDirective("messageLength")]
            public long MessageLength { get; set; }

            [JilDirective("bidPrice")]
            public long BidPrice { get; set; }

            [JilDirective("bidSize")]
            public long BidSize { get; set; }

            [JilDirective("askPrice")]
            public long AskPrice { get; set; }

            #endregion Public Properties
        }

        /// <summary>
        /// Root json object
        /// </summary>
        private class RootObject
        {
            #region Public Properties

            [JilDirective("sessionId")]
            public long SessionId { get; set; }

            [JilDirective("messageProtocolId")]
            public string MessageProtocolId { get; set; }

            [JilDirective("messages")]
            public Message[] Messages { get; set; }

            [JilDirective("messageCount")]
            public long MessageCount { get; set; }

            #endregion Public Properties
        }

        #endregion Private Classes
    }
}