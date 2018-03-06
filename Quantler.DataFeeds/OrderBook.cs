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

using System.Collections.Generic;
using System.Linq;

namespace Quantler.DataFeeds
{
    /// <summary>
    /// Keeps track of the current order book, only checks for level 1 quotes (best bid and best ask)
    /// </summary>
    public class OrderBook
    {
        #region Public Properties

        /// <summary>
        /// Gets the best ask quantity.
        /// </summary>
        public double AskSize => Asks.Count > 0 ? Asks.First().Value : 0;

        /// <summary>
        /// Gets the current best ask.
        /// </summary>
        public double BestAsk => Asks.Count > 0 ? Asks.First().Key : 0;

        /// <summary>
        /// Gets the current best bid.
        /// </summary>
        public double BestBid => Bids.Count > 0 ? Bids.First().Key : 0;

        /// <summary>
        /// Gets the best bid quantity.
        /// </summary>
        public double BidSize => Bids.Count > 0 ? Bids.First().Value : 0;

        /// <summary>
        /// Gets the associated ticker
        /// </summary>
        public string Ticker { get; }

        #endregion Public Properties

        #region Private Fields

        /// <summary>
        /// Locker instance
        /// </summary>
        private readonly object _locker = new object();

        #endregion Private Fields

        #region Private Properties

        /// <summary>
        /// Gets the asks order book.
        /// </summary>
        private SortedList<double, double> Asks { get; } = new SortedList<double, double>();

        /// <summary>
        /// Gets the bids order book.
        /// </summary>
        private SortedList<double, double> Bids { get; } = new SortedList<double, double>(new DescendedDoubleComparer());

        #endregion Private Properties

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderBook"/> class.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        public OrderBook(string ticker) =>
            Ticker = ticker;

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Adds the quote.
        /// </summary>
        /// <param name="isBid">if set to <c>true</c> [is bid].</param>
        /// <param name="price">The price.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public bool AddQuote(bool isBid, double price, double size)
        {
            //Get items
            var items = isBid ? Bids : Asks;
            if(size == 0)
                return RemoveQuote(isBid, price, size);
            else if (!items.ContainsKey(price))
                lock (_locker)
                {
                    items.Add(price, size);
                }
            else
                return UpdateQuote(isBid, price, size);

            //Check result is update on level 0 quotes?
            return items.IndexOfKey(price) == 0;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            lock (_locker)
            {
                Asks.Clear();
                Bids.Clear();
            }
        }

        /// <summary>
        /// Removes the quote.
        /// </summary>
        /// <param name="isBid">if set to <c>true</c> [is bid].</param>
        /// <param name="price">The price.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public bool RemoveQuote(bool isBid, double price, double size)
        {
            //Get items
            var items = isBid ? Bids : Asks;
            var toreturn = items.IndexOfKey(price) == 0;
            lock (_locker)
            {
                if (items.ContainsKey(price))
                    items.Remove(price);
            }

            //Check result is update on level 1 quotes?
            return toreturn && ((BidSize > 0 && isBid) || (AskSize > 0 && !isBid));
        }

        /// <summary>
        /// Sets the best book values, removing all other known values.
        /// </summary>
        /// <param name="isBid">if set to <c>true</c> [is bid].</param>
        /// <param name="price">The price.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public bool SetBestBook(bool isBid, double price, double size)
        {
            //Reset orderbook best values
            var list = isBid ? Bids : Asks;

            lock (_locker)
            {
                if (isBid)
                    list.Where(x => x.Key > price).ToArray().Select(x => list.Remove(x.Key)).ToArray();
                else
                    list.Where(x => x.Key < price).ToArray().Select(x => list.Remove(x.Key)).ToArray();
            }

            //Return results
            return AddQuote(isBid, price, size);
        }

        /// <summary>
        /// Updates the quote.
        /// </summary>
        /// <param name="isBid">if set to <c>true</c> [is bid].</param>
        /// <param name="price">The price.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public bool UpdateQuote(bool isBid, double price, double size)
        {
            //Get items
            var items = isBid ? Bids : Asks;
            lock (_locker)
            {
                if (items.ContainsKey(price))
                    items[price] = size;
            }

            //Check result is update on level 1 quotes?
            return items.IndexOfKey(price) == 0 && ((BidSize > 0 && isBid) || (AskSize > 0 && !isBid));
        }

        #endregion Public Methods
    }

    internal class DescendedDoubleComparer : IComparer<double>
    {
        #region Public Methods

        public int Compare(double x, double y)
        {
            // use the default compare to do the original comparison for doubles
            int ascendingResult = Comparer<double>.Default.Compare(x, y);

            // turn the result around
            return 0 - ascendingResult;
        }

        #endregion Public Methods
    }
}