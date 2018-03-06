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

using Quantler.Data.Bars;
using Quantler.Data.Market;
using Quantler.Interfaces;
using Quantler.Securities;

namespace Quantler.Data
{
    /// <summary>
    /// Datapoint related extensions
    /// </summary>
    public static class DataPointExtensions
    {
        #region Public Methods

        /// <summary>
        /// Derive order price of execution for this datapoint
        /// </summary>
        /// <param name="datapoint">The data point.</param>
        /// <param name="security">The security</param>
        /// <param name="direction">The direction.</param>
        /// <returns></returns>
        public static Prices OrderPrice(this DataPoint datapoint, Security security, Direction direction)
        {
            var low = security.Low;
            var high = security.High;
            var open = security.Open;
            var close = security.Close;
            var current = security.Close;

            //No need to check
            if (direction == Direction.Flat)
                return new Prices(current, open, high, low, close);

            //Tick received
            if (datapoint is Tick tick)
            {
                var price = direction == Direction.Short ? tick.BidPrice : tick.AskPrice;
                if (price != 0m)
                    return new Prices(price, 0, 0, 0, 0);
            }

            //Quote bar received
            if (datapoint is QuoteBar quotebar)
            {
                var bar = direction == Direction.Short ? quotebar.Bid : quotebar.Ask;
                if (bar != null)
                    return new Prices(bar);
            }

            //Trade bar received
            if (datapoint is TradeBar tradebar)
                return new Prices(tradebar);

            //Return what we know
            return new Prices(current, open, high, low, close);
        }

        /// <summary>
        /// Derive order probable size
        /// </summary>
        /// <param name="datapoint">The datapoint.</param>
        /// <param name="direction">The direction.</param>
        /// <returns></returns>
        public static decimal OrderSize(this DataPoint datapoint, Direction direction)
        {
            //Check direction
            if (direction == Direction.Flat)
                return 0;

            //Tick received
            if (datapoint is Tick tick)
                return direction == Direction.Short ? tick.BidSize : tick.AskSize;

            //Quote bar received
            if (datapoint is QuoteBar quoteBar)
                return direction == Direction.Short ? quoteBar.LastBidSize : quoteBar.LastAskSize;

            //Trade bar received
            if (datapoint is TradeBar tradeBar)
                return tradeBar.Volume;

            //Cannot derive any order size?
            return 0;
        }

        #endregion Public Methods
    }
}