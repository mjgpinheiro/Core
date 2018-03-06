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

using Quantler.Interfaces;
using System.Composition;
using Quantler.Data;
using Quantler.Messaging;
using Quantler.Securities;

namespace Quantler.DataFeeds.Quantler
{
    /// <summary>
    /// TODO: for mining purposes, we distribute the historical data for instance via the Sia network
    /// </summary>
    /// <seealso cref="Quantler.DataFeeds.BaseFeed" />
    /// <seealso cref="Quantler.Interfaces.DataFeed" />
    [Export(typeof(DataFeed))]
    public class HistoricalSiaFeed : BaseFeed, DataFeed
    {
        public bool IsConnected { get; }
        public void AddSubscription(DataSubscriptionRequest subscriptionRequest)
        {
            throw new System.NotImplementedException();
        }

        public bool CanSubscribe(TickerSymbol ticker)
        {
            throw new System.NotImplementedException();
        }

        public void Initialize(MessageInstance initialmessage)
        {
            throw new System.NotImplementedException();
        }

        public void Reconnect()
        {
            throw new System.NotImplementedException();
        }

        public void RemoveSubscription(DataSubscriptionRequest subscriptionRequest)
        {
            throw new System.NotImplementedException();
        }

        public override string GetFeedTicker(TickerSymbol ticker)
        {
            throw new System.NotImplementedException();
        }

        public override TickerSymbol GetQuantlerTicker(string ticker)
        {
            throw new System.NotImplementedException();
        }

        public bool IsRunning { get; }
        public void Start()
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }
    }
}