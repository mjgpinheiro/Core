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

using MathNet.Numerics.Statistics;
using Quantler.Trades;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quantler.Performance
{
    /// <summary>
    /// Fund related performance results
    /// TODO: implement benchmark logic
    /// TODO: we need IsDayTrading indication in the database for the catalog, this can also be saved afterwards from the account object, but it also still needs to be derived!
    /// </summary>
    public class QuantFundResult
    {
        #region Private Fields

        /// <summary>
        /// Amount of trading days to take into account
        /// </summary>
        private readonly int _tradingDaysPerYear;

        /// <summary>
        /// Associated benchmark, if applicable
        /// </summary>
        private readonly Benchmark _benchmark;

        /// <summary>
        /// If true, quantler platform fees are taken into account for accumulated fees
        /// </summary>
        private readonly bool _includeQuantlerFee;

        /// <summary>
        /// Year risk free rate
        /// </summary>
        private readonly decimal _riskFreeRate;

        /// <summary>
        /// Associated trading based result
        /// </summary>
        private readonly TradeResult _tradeResult;

        /// <summary>
        /// Current balance
        /// </summary>
        private decimal _currentBalance;

        /// <summary>
        /// Current equity (balance - fees)
        /// </summary>
        private decimal _currentEquity;

        /// <summary>
        /// Last time we updated historical values
        /// </summary>
        private DateTime _lastHistoricalUpdate = DateTime.MinValue;

        /// <summary>
        /// Placeholder for previous equity amount
        /// </summary>
        private decimal _previousEquity;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initialize new AlgorithmResult object
        /// </summary>
        /// <param name="traderesult"></param>
        /// <param name="initialcapital"></param>
        /// <param name="benchmark"></param>
        /// <param name="riskfreerate"></param>
        /// <param name="includequantlerfee"></param>
        /// <param name="tradingdaysperyear"></param>
        public QuantFundResult(TradeResult traderesult, decimal initialcapital, Benchmark benchmark, decimal riskfreerate = 0, bool includequantlerfee = true, int tradingdaysperyear = 252)
        {
            _tradeResult = traderesult;
            InitialCapital = initialcapital;
            StartDateUtc = DateTime.MinValue;
            EndDateUtc = DateTime.MinValue;
            _currentEquity = InitialCapital;
            _currentBalance = InitialCapital;
            _riskFreeRate = riskfreerate;
            _previousEquity = initialcapital;
            _includeQuantlerFee = includequantlerfee;
            _benchmark = benchmark;
            _tradingDaysPerYear = tradingdaysperyear;
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Private constructor (used for copying instance information)
        /// </summary>
        private QuantFundResult()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Measured alpha
        /// </summary>
        public decimal Alpha
        {
            private set;
            get;
        }

        /// <summary>
        /// Avg losing rate
        /// </summary>
        public decimal AvgLoseRate
        {
            private set;
            get;
        }

        /// <summary>
        /// Avg winning rate
        /// </summary>
        public decimal AvgWinRate
        {
            private set;
            get;
        }

        /// <summary>
        /// Measure Beta
        /// </summary>
        public decimal Beta
        {
            private set;
            get;
        }

        /// <summary>
        /// Compound Annual Growth Rate
        /// </summary>
        public decimal CAGR
        {
            private set;
            get;
        }

        /// <summary>
        /// Minimum amount of capital required, unlevereged
        /// </summary>
        public decimal CapitalRequired
        {
            private set;
            get;
        }

        /// <summary>
        /// Correlation between returns and benchmark
        /// </summary>
        public decimal Correlation
        {
            private set;
            get;
        }

        /// <summary>
        /// Last time we closed a trade
        /// </summary>
        public DateTime EndDateUtc
        {
            private set;
            get;
        }

        /// <summary>
        /// Current expectancy
        /// </summary>
        public decimal Expectancy =>
            WinRatio * PnLRatio - LoseRatio;

        /// <summary>
        /// Total fees paid for
        /// </summary>
        public decimal Fees =>
            FundFees + QuantlerFees + TradingFees;

        /// <summary>
        /// Fund related fees (ETFs for example)
        /// </summary>
        public decimal FundFees
        {
            private set;
            get;
        }

        /// <summary>
        /// PnL without fees
        /// </summary>
        public decimal GrossPnL =>
            _tradeResult.GrossPnL;

        /// <summary>
        /// Historical tracked benchmark (daily)
        /// </summary>
        public SortedDictionary<long, decimal> HistoricalBenchmark
        {
            private set;
            get;
        } = new SortedDictionary<long, decimal>();

        /// <summary>
        /// Historical tracked drawdown (daily)
        /// </summary>
        public SortedDictionary<long, decimal> HistoricalDrawdown
        {
            private set;
            get;
        } = new SortedDictionary<long, decimal>();

        /// <summary>
        /// Historical tracked equity (daily)
        /// </summary>
        public SortedDictionary<long, decimal> HistoricalEquity
        {
            private set;
            get;
        } = new SortedDictionary<long, decimal>();

        /// <summary>
        /// Historical tracked performance (daily)
        /// </summary>
        public SortedDictionary<long, decimal> HistoricalPerformance
        {
            private set;
            get;
        } = new SortedDictionary<long, decimal>();

        /// <summary>
        /// Initial capital
        /// </summary>
        public decimal InitialCapital
        {
            private set;
            get;
        }

        /// <summary>
        /// Last time these values were modified
        /// </summary>
        public DateTime LastModifiedUtc
        {
            private set;
            get;
        }

        /// <summary>
        /// Ratio of losing trades
        /// </summary>
        public decimal LoseRatio
        {
            private set;
            get;
        }

        /// <summary>
        /// Measured maximum intraday drawdown
        /// </summary>
        public decimal MaxDrawdown
        {
            private set;
            get;
        }

        /// <summary>
        /// Includes all expenses (Quantler, Fund costs, commissions)
        /// TODO: should be on a yearly basis not on the sum
        /// </summary>
        public decimal NetExpenseRatio =>
            Fees / GrossPnL;

        /// <summary>
        /// Current net pnl (including fees)
        /// </summary>
        public decimal NetPnL =>
            _currentBalance - InitialCapital;

        /// <summary>
        /// PnL ratio
        /// </summary>
        public decimal PnLRatio
        {
            private set;
            get;
        }

        /// <summary>
        /// Quantler related fees
        /// </summary>
        public decimal QuantlerFees
        {
            private set;
            get;
        }

        /// <summary>
        /// Return On Investment
        /// </summary>
        public decimal ROI =>
            Util.SafeDivide(NetPnL, InitialCapital);

        /// <summary>
        /// Sharpe ratio
        /// </summary>
        public decimal Sharpe
        {
            private set;
            get;
        }

        /// <summary>
        /// First time we opened a trade
        /// </summary>
        public DateTime StartDateUtc
        {
            private set;
            get;
        }

        /// <summary>
        /// Trading related fees
        /// </summary>
        public decimal TradingFees
        {
            private set;
            get;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is speculative.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is speculative; otherwise, <c>false</c>.
        /// </value>
        public bool IsSpeculative
        {
            private set;
            get;
        }

        /// <summary>
        /// Ratio of winning trades
        /// </summary>
        public decimal WinRatio
        {
            private set;
            get;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Generate a shallow copy of this instance
        /// </summary>
        /// <returns></returns>
        public QuantFundResult Copy()
        {
            return new QuantFundResult
            {
                Alpha = Alpha,
                AvgLoseRate = AvgLoseRate,
                AvgWinRate = AvgWinRate,
                Beta = Beta,
                CAGR = CAGR,
                CapitalRequired = CapitalRequired,
                Correlation = Correlation,
                EndDateUtc = EndDateUtc,
                FundFees = FundFees,
                QuantlerFees = QuantlerFees,
                TradingFees = TradingFees,
                HistoricalBenchmark = HistoricalBenchmark,
                HistoricalDrawdown = HistoricalDrawdown,
                HistoricalEquity = HistoricalEquity,
                HistoricalPerformance = HistoricalPerformance,
                InitialCapital = InitialCapital,
                LastModifiedUtc = LastModifiedUtc,
                LoseRatio = LoseRatio,
                MaxDrawdown = MaxDrawdown,
                PnLRatio = PnLRatio,
                Sharpe = Sharpe,
                StartDateUtc = StartDateUtc,
                WinRatio = WinRatio,
                _currentBalance = _currentBalance,
                _currentEquity = _currentEquity,
                _lastHistoricalUpdate = _lastHistoricalUpdate,
                _previousEquity = _previousEquity
            };
        }

        /// <summary>
        /// Update current statistic values based on new trade received
        /// </summary>
        /// <param name="trade"></param>
        public void Update(Trade trade)
        {
            //Last modified
            LastModifiedUtc = trade.ClosedUtc;

            //Add result to balance
            _currentBalance += trade.NetPnL;

            //Dates
            if (StartDateUtc == DateTime.MinValue)
                StartDateUtc = trade.OpenedUtc;
            EndDateUtc = LastModifiedUtc;

            //Fees
            TradingFees += trade.Fees;

            //Ratios
            if (_tradeResult.Trades > 0)
            {
                //Get ratios
                AvgWinRate = Util.SafeDivide(_tradeResult.GrossProfit, _tradeResult.Winners);
                AvgLoseRate = Util.SafeDivide(_tradeResult.GrossLoss, _tradeResult.Losers);
                PnLRatio = Util.SafeDivide(AvgWinRate, Math.Abs(AvgLoseRate));
                WinRatio = _tradeResult.Winners / _tradeResult.Trades;
                LoseRatio = _tradeResult.Losers / _tradeResult.Trades;

                //Get CAGR
                int years = (EndDateUtc - StartDateUtc).Days / 365;
                CAGR = years == 0 ? 0 : (decimal)Math.Pow((double)_currentEquity / (double)InitialCapital, 1 / (double)years - 1);

                //Set correlation with benchmark
                decimal Calcannualperformance(IEnumerable<decimal> values) => values.Average() * _tradingDaysPerYear;
                var histequity = HistoricalEquity.Values.Select(x => (double)x);
                var histbenchmark = HistoricalBenchmark.Values.Select(x => (double)x);
                var histperf = HistoricalPerformance.Values.Select(x => (double)x);
                Correlation = (decimal)MathNet.Numerics.Statistics.Correlation.Pearson(histequity, histbenchmark);

                //Inputs
                var annualvariance = (decimal)histperf.Variance() * _tradingDaysPerYear;
                var annualstdev = (decimal)Math.Sqrt((double)annualvariance);
                var annualperf = Calcannualperformance(HistoricalPerformance.Values);
                var annualbench = Calcannualperformance(HistoricalBenchmark.Values);
                var benchmarkvariance = (decimal)histbenchmark.Variance();

                //Sharpe ratio
                Sharpe = Util.SafeDivide((annualperf - _riskFreeRate), annualstdev);

                //Beta
                Beta = Util.SafeDivide((decimal)histperf.Covariance(histbenchmark), benchmarkvariance);

                //Alpha
                Alpha = Beta > 0 ? annualperf - (_riskFreeRate + Beta * (annualbench - _riskFreeRate)) : 0m;

                //Fund related fees
                FundFees += (trade.ClosedValue * trade.Security.Details.ExpenseRatio) * ((trade.ClosedUtc - trade.OpenedUtc).Days / 365m);
            }
        }

        /// <summary>
        /// Update current position values to keep track of equity and drawdown
        /// </summary>
        /// <param name="positions"></param>
        public void Update(IEnumerable<Position> positions)
        {
            //Check input
            if (!positions.Any())
                return;

            //Get current floating equity
            _currentEquity = _currentBalance + positions.Sum(x => x.NetProfit);

            //Update current drawdown
            decimal dd = Util.SafeDivide(_currentEquity, _currentBalance);
            MaxDrawdown = dd < MaxDrawdown ? dd : MaxDrawdown;

            //Check for capital required based on positions value
            var positionvalue = positions.Sum(x => x.TotalValue);
            CapitalRequired = positionvalue > CapitalRequired ? positionvalue : CapitalRequired;

            //Check for fund fees, Quantler fees
            //TODO: get first security and check if the utc date is a new month from the first position (start date in this instance). If so, add quantler fees to total fees (fees do not have to lower the balance as they are charged seperatly)

            //Check for historical values
            DateTime currentdate = positions.First().Security.Exchange.UtcTime.Date; //We only need date
            long currentdateux = currentdate.ToUnixTime();
            if (_lastHistoricalUpdate != currentdate)
            {
                //TODO: CurrentTime > today means we are measuring tomorrow about today. We need the prev day, but days -1 is incorrect as that would not be the last trading day we effectively measured

                //Set update
                HistoricalEquity.Add(currentdateux, _currentEquity);
                HistoricalDrawdown.Add(currentdateux, MaxDrawdown);
                HistoricalBenchmark.Add(currentdateux, _benchmark.Evaluate(currentdate));
                HistoricalPerformance.Add(currentdateux, Math.Round((_currentEquity - _previousEquity) * 100 / _previousEquity));

                //Set last update
                _lastHistoricalUpdate = currentdate;
                _previousEquity = _currentEquity;

                //Check for quantler platform fees
                if (_includeQuantlerFee && currentdate.Day == 1 && _currentEquity > 5000)
                    QuantlerFees += _currentEquity * (0.002m);
            }
        }

        #endregion Public Methods
    }
}