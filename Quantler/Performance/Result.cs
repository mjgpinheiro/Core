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

using Quantler.Securities;
using Quantler.Trades;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Quantler.Performance
{
    /// <summary>
    /// Implementation for results calculation
    /// </summary>
    public class Result
    {
        #region Private Fields

        /// <summary>
        /// Saved algorithm based results
        /// </summary>
        private readonly QuantFundResult _quantFundResult;

        /// <summary>
        /// Saved trading based results
        /// </summary>
        private readonly TradeResult _tradeResults;

        /// <summary>
        /// Saved security based results
        /// </summary>
        private readonly Dictionary<Security, TradeResult> _securityBasedResults = new Dictionary<Security, TradeResult>();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initialize results holder
        /// </summary>
        /// <param name="initialcapital"></param>
        /// <param name="benchmark"></param>
        public Result(decimal initialcapital, Benchmark benchmark)
        {
            _tradeResults = new TradeResult();
            _quantFundResult = new QuantFundResult(_tradeResults, initialcapital, benchmark);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Get algorithm based results
        /// </summary>
        public QuantFundResult QuantFund => _quantFundResult;

        /// <summary>
        /// Get summary of performance
        /// </summary>
        public Dictionary<string, string> Summary => GetSummary();

        /// <summary>
        /// Get results based on trading performance
        /// </summary>
        public TradeResult Trades => _tradeResults;

        #endregion Public Properties

        #region Public Indexers

        /// <summary>
        /// Get current results related to a specific security
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        public TradeResult this[Security security] => _securityBasedResults.ContainsKey(security) ? _securityBasedResults[security] : new TradeResult();

        /// <summary>
        /// Gets the <see cref="TradeResult"/> with the specified ticker.
        /// </summary>
        /// <param name="ticker">The ticker.</param>
        public TradeResult this[TickerSymbol ticker] => _securityBasedResults.Keys.Count(x => x.Ticker == ticker) > 0
            ? _securityBasedResults.First(x => x.Key.Ticker == ticker).Value
            : new TradeResult();

        #endregion Public Indexers

        #region Public Methods

        /// <summary>
        /// Update current results based on new trade received
        /// TODO: protect from misuse (internal?)
        /// </summary>
        /// <param name="trade"></param>
        public void Update(Trade trade)
        {
            //Update algorithm based
            _quantFundResult.Update(trade);

            //Update trade results
            _tradeResults.Update(trade);

            //Update security based
            if (!_securityBasedResults.TryGetValue(trade.Security, out TradeResult securityresult))
            {
                securityresult = new TradeResult();
                _securityBasedResults.Add(trade.Security, securityresult);
            }
            securityresult.Update(trade);
        }

        /// <summary>
        /// Update results where position values are needed
        /// TODO: protect from misuse (internal?)
        /// </summary>
        /// <param name="positions"></param>
        public void Update(IEnumerable<Position> positions) => _quantFundResult.Update(positions);

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Processes the current results and forms a summery report
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetSummary() =>
            new Dictionary<string, string>()
            {
                {"Initial Capital", Rounded(QuantFund.InitialCapital) },
                {"Trades", _tradeResults.Trades.ToString(CultureInfo.InvariantCulture) },
                {"Avg.Win", Rounded(_tradeResults.AvgWinningTrade) },
                {"Avg.Loss", Rounded(_tradeResults.AvgLosingTrade) },
                {"CAGR", Rounded(QuantFund.CAGR) },
                {"Max Drawdown", Rounded(QuantFund.MaxDrawdown) },
                {"Net PnL", Rounded(QuantFund.NetPnL) },
                {"ROI", Rounded(QuantFund.ROI) },
                {"Sharpe Ratio", Rounded(QuantFund.Sharpe) },
                {"Loss Ratio", Rounded(QuantFund.LoseRatio) },
                {"Win Ratio", Rounded(QuantFund.WinRatio) },
                {"Fees", Rounded(QuantFund.Fees) }
            };

        /// <summary>
        /// Perfectly rounded stats values
        /// </summary>
        /// <param name="value"></param>
        /// <param name="places"></param>
        /// <returns></returns>
        private string Rounded(decimal value, int places = 2) =>
            Math.Round(value, places).ToString(CultureInfo.InvariantCulture);

        #endregion Private Methods
    }
}