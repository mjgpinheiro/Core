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

using Quantler.Configuration;
using Quantler.Securities;
using System;

namespace Quantler.Orders.Type
{
    /// <summary>
    /// Market on open order
    /// </summary>
    /// <seealso cref="OrderImpl" />
    public class MarketOnOpenOrder : OrderImpl
    {
        #region Private Fields

        /// <summary>
        /// The on market open delayed
        /// </summary>
        private readonly TimeSpan _onMarketOpenDelayed = TimeSpan.FromMinutes(15);

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketOnOpenOrder"/> class.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="fundid">The fundid.</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="createdutc">The createdutc.</param>
        /// <param name="exchange">The exchangeModel.</param>
        /// <param name="comment">The comment.</param>
        public MarketOnOpenOrder(Security security, string fundid, decimal quantity, DateTime createdutc, string exchange, string comment = "")
            : base(security, fundid, quantity, createdutc, exchange, comment)
        {
            Type = OrderType.MarketOnOpen;
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketOnOpenOrder"/> class.
        /// </summary>
        private MarketOnOpenOrder()
        {
        }

        #endregion Private Constructors

        #region Private Properties

        /// <summary>
        /// Stop was triggered indication
        /// </summary>
        private DateTime TimedTrigger { get; set; } = DateTime.MinValue;

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Clone the current order object
        /// </summary>
        /// <returns></returns>
        public override Order Clone()
        {
            MarketOnOpenOrder newMarketOrder = new MarketOnOpenOrder();
            CopyTo(this);
            newMarketOrder.TimedTrigger = TimedTrigger;
            return newMarketOrder;
        }

        /// <summary>
        /// Determines whether this instance is triggered.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is triggered; otherwise, <c>false</c>.
        /// </returns>
        public bool IsTriggered()
        {
            //Check if datetime is filled in, set to time and date this order should be triggered
            if (TimedTrigger == DateTime.MinValue)
                TimedTrigger = Security.Exchange
                                   .NextMarketOpen(
                                       WorldClock.ConvertDateTimeToDifferentTimeZone(CreatedUtc, TimeZone.Utc,
                                           Security.Exchange.TimeZone).ToDateTimeUnspecified(),
                                       Config.PortfolioConfig.ExtendedMarketHours)
                                       .Add(_onMarketOpenDelayed);

            //Perform check if we need to send this order as and opened order
            return Security.LocalTime >= TimedTrigger;
        }

        #endregion Public Methods
    }
}