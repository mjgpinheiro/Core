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
using Quantler.Trades;
using System;
using System.Collections.Generic;
using Quantler.Securities;

namespace Quantler.Performance
{
    /// <summary>
    /// Trading based statistics
    /// </summary>
    public class TradeResult
    {
        #region Private Fields

        /// <summary>
        /// Holder for current amount of consecutive losses
        /// </summary>
        private int _currentConsecLosses;

        /// <summary>
        /// Holder for current amount of consecutive wins
        /// </summary>
        private int _currentConsecWins;

        /// <summary>
        /// Holder for previous trade information
        /// </summary>
        private Trade _prevTrade;

        /// <summary>
        /// Holder for known symbols
        /// </summary>
        private List<TickerSymbol> _symbols = new List<TickerSymbol>();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Create a trade based statistics calculator
        /// </summary>
        public TradeResult()
        {
            LastTradeUtc = DateTime.MinValue;
            FirstTradeUtc = DateTime.MinValue;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Average fees applied to trades
        /// </summary>
        public decimal AverageFees
        {
            private set;
            get;
        }

        /// <summary>
        /// Average time we are in a trade
        /// </summary>
        public TimeSpan AverageTimeTrade
        {
            private set;
            get;
        }

        /// <summary>
        /// Average losing trade
        /// </summary>
        public decimal AvgLosingTrade
        {
            private set;
            get;
        }

        /// <summary>
        /// Average pnl from trade
        /// </summary>
        public decimal AvgPnlPerTrade
        {
            private set;
            get;
        }

        /// <summary>
        /// Average winning trade
        /// </summary>
        public decimal AvgWinningTrade
        {
            private set;
            get;
        }

        /// <summary>
        /// Amount of buys that ended up losing
        /// </summary>
        public int BuyLosers
        {
            private set;
            get;
        }

        /// <summary>
        /// Amount of profit or loss made by buying trades
        /// </summary>
        public decimal BuyPnL
        {
            private set;
            get;
        }

        /// <summary>
        /// Amount of buying trades that made profit
        /// </summary>
        public int BuyWins
        {
            private set;
            get;
        }

        /// <summary>
        /// Total amount of commissions paid
        /// </summary>
        public decimal Commissions
        {
            private set;
            get;
        }

        /// <summary>
        /// Consecutive losses
        /// </summary>
        public int ConsecLosses
        {
            private set;
            get;
        }

        /// <summary>
        /// Consecutive wins
        /// </summary>
        public int ConsecWins
        {
            private set;
            get;
        }

        /// <summary>
        /// First date and time a trade has occured
        /// </summary>
        public DateTime FirstTradeUtc
        {
            private set;
            get;
        }

        /// <summary>
        /// Amount of trades that turned out flat
        /// </summary>
        public int Flats
        {
            private set;
            get;
        }

        /// <summary>
        /// Total amount of loss made
        /// </summary>
        public decimal GrossLoss
        {
            private set;
            get;
        }

        /// <summary>
        /// Total gross pnl
        /// </summary>
        public decimal GrossPnL
        {
            private set;
            get;
        }

        /// <summary>
        /// Total amount of profit made
        /// </summary>
        public decimal GrossProfit
        {
            private set;
            get;
        }

        /// <summary>
        /// Total amount of interest paid
        /// </summary>
        public decimal InterestPaid
        {
            private set;
            get;
        }

        /// <summary>
        /// Date and time these results were updated
        /// </summary>
        public DateTime LastModifiedUtc
        {
            private set;
            get;
        }

        /// <summary>
        /// Date and time the last trade has occured
        /// </summary>
        public DateTime LastTradeUtc
        {
            private set;
            get;
        }

        /// <summary>
        /// Total amount of losing trades
        /// </summary>
        public int Losers
        {
            private set;
            get;
        }

        /// <summary>
        /// Maximum amount of loss registered
        /// </summary>
        public decimal MaxLoss
        {
            private set;
            get;
        }

        /// <summary>
        /// Maximum amount of profit registered
        /// </summary>
        public decimal MaxWin
        {
            private set;
            get;
        }

        /// <summary>
        /// Total net pnl
        /// </summary>
        public decimal NetPnL
        {
            private set;
            get;
        }

        /// <summary>
        /// Percentage of time we are in a position (measured between the first and the last trade)
        /// </summary>
        public decimal PercentageTimeInvested =>
            Convert.ToDecimal(TimeTraded.TotalSeconds / (LastTradeUtc - FirstTradeUtc).TotalSeconds);

        /// <summary>
        /// Calculated profit factor
        /// </summary>
        public decimal ProfitFactor
        {
            private set;
            get;
        }

        /// <summary>
        /// Short trades that ended in a loss
        /// </summary>
        public int SellLosers
        {
            private set;
            get;
        }

        /// <summary>
        /// Short pnl
        /// </summary>
        public decimal SellPnL
        {
            private set;
            get;
        }

        /// <summary>
        /// Short trades that ended in a win
        /// </summary>
        public int SellWins
        {
            private set;
            get;
        }

        /// <summary>
        /// Total amount of quantity traded
        /// </summary>
        public decimal QuantityTraded
        {
            private set;
            get;
        }

        /// <summary>
        /// Amount of symbols known and found by this result
        /// </summary>
        public int SymbolCount => _symbols.Count;

        /// <summary>
        /// Symbols included in this result
        /// </summary>
        public TickerSymbol[] TickerSymbols => _symbols.ToArray();

        /// <summary>
        /// Total amount of time in a trade
        /// </summary>
        public TimeSpan TimeTraded
        {
            private set;
            get;
        }

        /// <summary>
        /// Amount of trades occured
        /// </summary>
        public int Trades
        {
            private set;
            get;
        }

        /// <summary>
        /// Win and loss ratio
        /// </summary>
        public decimal WinLossRatio
        {
            private set;
            get;
        }

        /// <summary>
        /// Total amount of winners
        /// </summary>
        public int Winners
        {
            private set;
            get;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Create a shallow copy of this instance
        /// </summary>
        /// <returns></returns>
        public TradeResult Copy()
        {
            return new TradeResult
            {
                AverageFees = AverageFees,
                AverageTimeTrade = AverageTimeTrade,
                AvgLosingTrade = AvgLosingTrade,
                AvgPnlPerTrade = AvgPnlPerTrade,
                AvgWinningTrade = AvgWinningTrade,
                BuyLosers = BuyLosers,
                BuyPnL = BuyPnL,
                BuyWins = BuyWins,
                Commissions = Commissions,
                ConsecLosses = ConsecLosses,
                ConsecWins = ConsecWins,
                FirstTradeUtc = FirstTradeUtc,
                Flats = Flats,
                GrossLoss = GrossLoss,
                GrossPnL = GrossPnL,
                GrossProfit = GrossProfit,
                InterestPaid = InterestPaid,
                LastModifiedUtc = LastModifiedUtc,
                LastTradeUtc = LastTradeUtc,
                Losers = Losers,
                MaxLoss = MaxLoss,
                MaxWin = MaxWin,
                NetPnL = NetPnL,
                ProfitFactor = ProfitFactor,
                SellLosers = SellLosers,
                SellPnL = SellPnL,
                SellWins = SellWins,
                QuantityTraded = QuantityTraded,
                TimeTraded = TimeTraded,
                Trades = Trades,
                WinLossRatio = WinLossRatio,
                Winners = Winners,
                _currentConsecLosses = _currentConsecLosses,
                _currentConsecWins = _currentConsecWins,
                _prevTrade = _prevTrade,
                _symbols = _symbols
            };
        }

        /// <summary>
        /// Update current trade statistics based on new trade
        /// </summary>
        /// <param name="trade"></param>
        public void Update(Trade trade)
        {
            //Add to trades
            Trades++;

            //Check first and last trade
            if (FirstTradeUtc == DateTime.MinValue)
                FirstTradeUtc = trade.OpenedUtc;
            LastTradeUtc = trade.ClosedUtc;

            //Check on averages
            AvgPnlPerTrade += (trade.NetPnL - AvgPnlPerTrade) / Trades;
            if (trade.NetPnL > 0)
            {
                AvgWinningTrade += (trade.NetPnL - AvgWinningTrade) / Trades;
                Winners++;
                MaxWin = MaxWin > trade.NetPnL ? MaxWin : trade.NetPnL;
                GrossProfit += trade.NetPnL;
            }
            else if (trade.NetPnL < 0)
            {
                AvgLosingTrade += (trade.NetPnL - AvgLosingTrade) / Trades;
                Losers++;
                MaxLoss = MaxLoss > Math.Abs(trade.NetPnL) ? MaxLoss : Math.Abs(trade.NetPnL);
                GrossLoss += trade.NetPnL;
            }
            else
                Flats++;

            //Directional statistics
            if (trade.Direction == Direction.Long)
            {
                BuyPnL += trade.NetPnL;
                BuyLosers += Convert.ToInt32(trade.NetPnL < 0);
                BuyWins += Convert.ToInt32(trade.NetPnL > 0);
            }
            else if (trade.Direction == Direction.Short)
            {
                SellPnL += trade.NetPnL;
                SellLosers += Convert.ToInt32(trade.NetPnL < 0);
                SellWins += Convert.ToInt32(trade.NetPnL > 0);
            }

            //Set commissions
            Commissions += trade.Fees;

            //Consecutive statistics
            if (_prevTrade == null)
                _prevTrade = trade;
            else if (_prevTrade.NetPnL > 0 && trade.NetPnL > 0)
                _currentConsecWins++;
            else if (_prevTrade.NetPnL < 0 && trade.NetPnL < 0)
                _currentConsecLosses++;
            else
            {
                _currentConsecLosses = 0;
                _currentConsecWins = 0;
            }
            ConsecLosses = _currentConsecLosses > ConsecLosses ? _currentConsecLosses : ConsecLosses;
            ConsecWins = _currentConsecWins > ConsecWins ? _currentConsecWins : ConsecWins;

            //In trade duration
            TimeTraded += trade.Duration;

            //Gross pnl
            GrossPnL += trade.GrossPnL;

            //Interest paid
            InterestPaid += 0m; //TODO: model interest paid on margin and positions (margin trading is not supported during Beta)

            //Last modified
            LastModifiedUtc = trade.Security.Exchange.UtcTime;

            //Shares traded
            QuantityTraded += trade.Quantity;

            //Average time traded
            AverageTimeTrade += TimeSpan.FromTicks((trade.Duration.Ticks - AverageTimeTrade.Ticks) / Trades);

            //Winloss ratio
            WinLossRatio = Util.SafeDivide(Winners, Losers);

            //Profit Factor
            ProfitFactor = Util.SafeDivide(GrossProfit, Math.Abs(GrossLoss));

            //Check for symbol count
            if (!_symbols.Contains(trade.Security.Ticker))
                _symbols.Add(trade.Security.Ticker);

            //Average fees
            AverageFees += Util.SafeDivide((trade.Fees - AverageFees), Trades);
        }

        #endregion Public Methods
    }
}