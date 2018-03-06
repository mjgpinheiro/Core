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

using MoreLinq;
using Quantler.Account;
using Quantler.Broker.SettlementModels;
using Quantler.Broker.SpreadModels;
using Quantler.Interfaces;
using Quantler.Orders;
using Quantler.Orders.FeeModels;
using Quantler.Orders.FillModels;
using Quantler.Orders.LatencyModels;
using Quantler.Orders.SlippageModels;
using Quantler.Securities;
using Quantler.Securities.MarginModels;
using Quantler.Tracker;
using System;
using System.Linq;

namespace Quantler.Broker.Model
{
    /// <summary>
    /// Robinhood brokerage behaviour model
    /// </summary>
    public class RobinHoodBrokerModel : GenericBrokerModel
    {
        #region Private Fields

        /// <summary>
        /// Default delayed settlement model
        /// </summary>
        private readonly SettlementModel _delayedSettlementModel;

        /// <summary>
        /// The pattern day trading tracker
        /// </summary>
        private bool _patternDayTradingHit;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RobinHoodBrokerModel"/> class.
        /// </summary>
        /// <param name="accounttype">The accounttype.</param>
        /// <param name="latencymodel">The latencymodel.</param>
        /// <param name="slippagemodel">The slippagemodel.</param>
        public RobinHoodBrokerModel(
                    AccountType accounttype,
                    LatencyModel latencymodel,
                    SlippageModel slippagemodel)
            : base(accounttype, new RobinhoodFeeModel(), new ImmediateFillBehaviour(), latencymodel, new GenericMarginCallModel(),
                  new EquityMarginModel(0m, 0m), new ImmediateSettlementModel(), slippagemodel,
                  new FloatingMarketSpreadModel())
        {
            //Set delayed settlement model
            _delayedSettlementModel = new DelayedSettlementModel(3, TimeSpan.FromHours(4));

            //Set compatible currencies
            CompatibleBaseCurrencies = new[] { CurrencyType.USD };
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Type of broker
        /// </summary>
        public override BrokerType BrokerType { get; protected set; } = BrokerType.Robinhood;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Returns the amount of day trading orders left before day trading flag has been triggered
        /// http://www.finra.org/investors/day-trading-margin-requirements-know-rules
        /// http://www.sec.gov/investor/alerts/daytrading.pdf
        /// http://www.sec.gov/answers/patterndaytrader.htm
        /// TODO: once we enable margin based accounts, unit test this logic
        /// </summary>
        /// <param name="account">The brokerage account</param>
        /// <param name="historicalfills">The historical fills.</param>
        /// <returns></returns>
        public override void DayTradingOrdersLeft(BrokerAccount account, Fill[] historicalfills)
        {
            //Check if we have any fills and if we need to check this rule
            if (account.Type == AccountType.Cash)
            {
                account.DayTradingOrdersLeft = int.MaxValue; //Does not apply to cash accounts
                account.PatternDayTradingHit = false;
                return;
            }
            else if (_patternDayTradingHit) //If you are hit, you are hit. No turning back
            {
                account.DayTradingOrdersLeft = 0;
                account.PatternDayTradingHit = true;
                return;
            }
            else if (historicalfills.Length == 0)
            {
                account.DayTradingOrdersLeft = 4; //You always get 4 trades
                account.PatternDayTradingHit = false;
                return;
            }

            //Check for amount of trades in the past 5 days
            Security referenceSecurity = historicalfills.First().Security;
            DateTime currenttime = referenceSecurity.LocalTime;
            int businessdaysback = 5;
            int businessdays = 0;
            var fills = historicalfills.OrderBy(x => x.LocalTime);

            //Check for the period
            var fromdate = currenttime.DoWhile(x => x.AddDays(-1), x =>
            {
                //Only count business days
                if (referenceSecurity.Exchange.IsOpenOnDate(x))
                    businessdays++;

                //Check if we are there
                return businessdays == businessdaysback;
            });
            var fillsapplied = fills.Where(x => x.LocalTime >= fromdate) //Past 5 business days
                                    .GroupBy(x => x.Security)
                                    .Where(x => x.Count(n => n.Direction == Direction.Long) > 0 && x.Count(n => n.Direction == Direction.Short) > 0); //Where both opening and closing have occured

            //Check the fills applied for pattern day trading
            int triggered = 0;
            decimal totaltradingactivity = 0m;
            decimal daytradingactivity = 0m;
            foreach (var fillgroup in fillsapplied)
            {
                //Track position
                PositionTracker pos = new PositionTracker(account);

                //Check all subsequent positions
                fillgroup.ForEach(fill =>
                {
                    //Check if this fill applies
                    var currentdirection = pos[fill.Security].Direction;
                    pos.Adjust(fill);
                    totaltradingactivity += referenceSecurity.ConvertValue(fill.FillQuantity * fill.FillPrice, CurrencyType.USD);
                    if (currentdirection == Direction.Flat || currentdirection == fill.Direction)
                        return;
                    if (pos[fill.Security].LastModifiedUtc.Date != fill.UtcTime.Date)
                        return;

                    //Add to triggered trades
                    triggered += 1;
                    daytradingactivity += referenceSecurity.ConvertValue(fill.FillQuantity * fill.FillPrice, CurrencyType.USD);
                });
            }

            //Check if we hit the max in conjuction with our trading activity
            if (triggered >= 4 && (daytradingactivity / totaltradingactivity) > .06m)
            {
                _patternDayTradingHit = true;
                account.DayTradingOrdersLeft = 0;
                account.PatternDayTradingHit = true;
            }
            else
            {
                account.DayTradingOrdersLeft = 4 - triggered;
                account.PatternDayTradingHit = false;
            }
        }

        /// <summary>
        /// Get associated security details
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        public override SecurityDetails GetSecurityDetails(Security security)
        {
            if (security.Type == SecurityType.Equity)
                return base.GetSecurityDetails(security);
            else
            {
                _log.Error($"Security type {security.Type} is unsupported for broker of type {BrokerType}");
                return SecurityDetails.NIL();
            }
        }

        /// <summary>
        /// Get settlement model used by Traders Only
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        public override SettlementModel GetSettlementModel(Security security) =>
            AccountType == AccountType.Cash && security.Type == SecurityType.Equity ?
                    _delayedSettlementModel : SettlementModel;

        /// <summary>
        /// Check if order type as requested is supported, default yes
        /// </summary>
        /// <param name="ordertype"></param>
        /// <returns></returns>
        public override bool IsOrderTypeSupported(OrderType ordertype) =>
            ordertype == OrderType.Market ||
            ordertype == OrderType.Limit ||
            ordertype == OrderType.StopMarket ||
            ordertype == OrderType.StopLimit;

        #endregion Public Methods
    }
}