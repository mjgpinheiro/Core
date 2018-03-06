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
using NodaTime;
using Quantler.Data.Aggegrate;
using Quantler.Securities;
using System.Collections.Generic;
using System.Linq;

namespace Quantler.Data
{
    /// <summary>
    /// Data subscription information
    /// </summary>
    public class DataSubscription
    {
        /// <summary>
        /// Datatype for subscription
        /// </summary>
        public DataType DataType => Request.DataType;

        /// <summary>
        /// Gets the type.
        /// </summary>
        public Type Type => Aggregators.First().InputType;

        /// <summary>
        /// Subscription associated ticker
        /// </summary>
        public string Ticker => Request.Ticker.Name;

        /// <summary>
        /// Subscription base resolution
        /// </summary>
        public Resolution Resolution { get; }

        /// <summary>
        /// Gets or sets the date time zone.
        /// </summary>
        public TimeZone DateTimeZone { get; }

        /// <summary>
        /// Gets the exchangeModel time zone.
        /// </summary>
        public TimeZone ExchangeTimeZone { get; }

        /// <summary>
        /// Subscription associated aggregators
        /// </summary>
        public readonly HashSet<DataAggregator> Aggregators;

        /// <summary>
        /// Gets the fund identifier.
        /// </summary>
        public string FundId { get; }

        /// <summary>
        /// The associated request
        /// </summary>
        public DataSubscriptionRequest Request;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSubscription"/> class.
        /// </summary>
        /// <param name="fundid">The fundid.</param>
        /// <param name="request">The request.</param>
        /// <param name="security">The security.</param>
        /// <param name="timezone">The timezone.</param>
        /// <param name="aggregator">Initial aggregator</param>
        public DataSubscription(string fundid, DataSubscriptionRequest request, Security security, TimeZone timezone, DataAggregator aggregator)
        {
            //Set values
            Resolution = new Resolution(request.Aggregation);
            Request = request;
            Aggregators = new HashSet<DataAggregator>();
            if (aggregator != null)
                Aggregators.Add(aggregator);
            ExchangeTimeZone = security.Exchange.TimeZone;
            DateTimeZone = timezone;
            FundId = fundid;
        }
    }
}