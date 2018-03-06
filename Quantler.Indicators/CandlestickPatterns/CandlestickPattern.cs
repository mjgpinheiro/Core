#region License Header

/*
* QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
* Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
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
*
* Modifications Copyright 2018 Quantler B.V.
*
*/

#endregion License Header

using Quantler.Data.Bars;
using System;

namespace Quantler.Indicators.CandlestickPatterns
{
    /// <summary>
    /// Abstract base class for a candlestick pattern indicator
    /// </summary>
    public abstract class CandlestickPattern : WindowIndicator<DataPointBar>
    {
        #region Protected Constructors

        /// <summary>
        /// Creates a new <see cref="CandlestickPattern"/> with the specified name
        /// </summary>
        /// <param name="name">The name of this indicator</param>
        /// <param name="period">The number of data points to hold in the window</param>
        protected CandlestickPattern(string name, int period)
            : base(name, period)
        {
        }

        #endregion Protected Constructors

        #region Protected Methods

        /// <summary>
        /// Returns the average range of the previous candles
        /// </summary>
        /// <param name="type">The type of setting to use</param>
        /// <param name="sum">The sum of the previous candles ranges</param>
        /// <param name="tradeBar">The input candle</param>
        protected static decimal GetCandleAverage(CandleSettingType type, decimal sum, DataPointBar tradeBar)
        {
            var defaultSetting = CandleSettings.Get(type);

            return defaultSetting.Factor *
                (defaultSetting.AveragePeriod != 0 ? sum / defaultSetting.AveragePeriod : GetCandleRange(type, tradeBar)) /
                (defaultSetting.RangeType == CandleRangeType.Shadows ? 2.0m : 1.0m);
        }

        /// <summary>
        /// Returns the candle color of a candle
        /// </summary>
        /// <param name="tradeBar">The input candle</param>
        protected static CandleColor GetCandleColor(DataPointBar tradeBar) =>
            tradeBar.Close >= tradeBar.Open ? CandleColor.White : CandleColor.Black;

        /// <summary>
        /// Returns true if the candle is lower than the previous one
        /// </summary>
        protected static bool GetCandleGapDown(DataPointBar tradeBar, DataPointBar previousBar) =>
            tradeBar.High < previousBar.Low;

        /// <summary>
        /// Returns true if the candle is higher than the previous one
        /// </summary>
        protected static bool GetCandleGapUp(DataPointBar tradeBar, DataPointBar previousBar) =>
            tradeBar.Low > previousBar.High;

        /// <summary>
        /// Returns the range of a candle
        /// </summary>
        /// <param name="type">The type of setting to use</param>
        /// <param name="tradeBar">The input candle</param>
        protected static decimal GetCandleRange(CandleSettingType type, DataPointBar tradeBar)
        {
            switch (CandleSettings.Get(type).RangeType)
            {
                case CandleRangeType.RealBody:
                    return GetRealBody(tradeBar);

                case CandleRangeType.HighLow:
                    return GetHighLowRange(tradeBar);

                case CandleRangeType.Shadows:
                    return GetUpperShadow(tradeBar) + GetLowerShadow(tradeBar);

                default:
                    return 0m;
            }
        }

        /// <summary>
        /// Returns the full range of the candle
        /// </summary>
        /// <param name="tradeBar">The input candle</param>
        protected static decimal GetHighLowRange(DataPointBar tradeBar) =>
            tradeBar.High - tradeBar.Low;

        /// <summary>
        /// Returns the range of the candle's lower shadow
        /// </summary>
        /// <param name="tradeBar">The input candle</param>
        protected static decimal GetLowerShadow(DataPointBar tradeBar) =>
            (tradeBar.Close >= tradeBar.Open ? tradeBar.Open : tradeBar.Close) - tradeBar.Low;

        /// <summary>
        /// Returns the distance between the close and the open of a candle
        /// </summary>
        /// <param name="tradeBar">The input candle</param>
        protected static decimal GetRealBody(DataPointBar tradeBar) =>
            Math.Abs(tradeBar.Close - tradeBar.Open);

        /// <summary>
        /// Returns true if the candle is lower than the previous one (with no body overlap)
        /// </summary>
        protected static bool GetRealBodyGapDown(DataPointBar tradeBar, DataPointBar previousBar) =>
            Math.Max(tradeBar.Open, tradeBar.Close) < Math.Min(previousBar.Open, previousBar.Close);

        /// <summary>
        /// Returns true if the candle is higher than the previous one (with no body overlap)
        /// </summary>
        protected static bool GetRealBodyGapUp(DataPointBar tradeBar, DataPointBar previousBar) =>
            Math.Min(tradeBar.Open, tradeBar.Close) > Math.Max(previousBar.Open, previousBar.Close);

        /// <summary>
        /// Returns the range of the candle's upper shadow
        /// </summary>
        /// <param name="tradeBar">The input candle</param>
        protected static decimal GetUpperShadow(DataPointBar tradeBar) =>
            tradeBar.High - (tradeBar.Close >= tradeBar.Open ? tradeBar.Close : tradeBar.Open);

        #endregion Protected Methods
    }
}