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
using System.Collections.Generic;
using Quantler.Data;
using Quantler.Data.Bars;
using Quantler.Data.Corporate;
using Quantler.Data.Market;
using Quantler.Securities;

namespace Quantler.Tests.Core.Data.DataFile
{
    public class DataReaderTests
    {
        //Tests input data (expected data)
        private List<DataPoint> _inputDataPoints = new List<DataPoint>
        {
            new Tick(TickerSymbol.NIL("TST"), DataSource.Binance),
            new Tick(TickerSymbol.NIL("TST"), DataSource.Bittrex),
            new Tick(TickerSymbol.NIL("TST"), DataSource.HitBtc),
            new Tick(TickerSymbol.NIL("TST"), DataSource.IEX),
            new TradeBar(DateTime.UtcNow, TimeZone.Amsterdam, TickerSymbol.NIL("NYS"), 10, 11, 12, 13, 1000),
            new Delisting(TickerSymbol.NIL("TST"), "Delisted", DateTime.UtcNow)
        };

    }
}
