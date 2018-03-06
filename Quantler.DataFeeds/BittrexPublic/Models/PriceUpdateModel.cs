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

using System;

namespace Quantler.DataFeeds.BittrexPublic.Models
{
    /// <summary>
    /// Price updates
    /// </summary>
    public class PriceUpdateModel
    {
        #region Public Properties

        public double Ask { get; set; }
        public double BaseVolume { get; set; }
        public double Bid { get; set; }
        public DateTime Created { get; set; }
        public double High { get; set; }
        public double Last { get; set; }
        public double Low { get; set; }
        public string MarketName { get; set; }
        public int OpenBuyOrders { get; set; }
        public int OpenSellOrders { get; set; }
        public double PrevDay { get; set; }
        public DateTime TimeStamp { get; set; }
        public double Volume { get; set; }

        #endregion Public Properties
    }
}