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

using Quantler.Data;
using Quantler.Data.Aggegrate;
using Quantler.Data.Bars;
using Quantler.Indicators;
using Quantler.Interfaces;
using Quantler.Securities;
using System;
using System.Collections.Generic;
using MoreLinq;

namespace Quantler.Modules
{
    /// <summary>
    /// Module indicator logic
    /// </summary>
    /// <seealso cref="Quantler.Interfaces.IModule" />
    public abstract partial class Module
    {
        #region Private Methods

        /// <summary>
        /// Gets the universe indicator.
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="universe">The universe.</param>
        /// <param name="createFunc">The create function.</param>
        /// <returns></returns>
        private Dictionary<Security, T> GetUniverseIndicator<T>(Universe universe, Func<Security, T> createFunc)
        {
            Dictionary<Security, T> toreturn = new Dictionary<Security, T>();
            universe.ForEach(x => toreturn.Add(x, createFunc(x)));
            return toreturn;
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Creates a new AccumulationDistribution indicator.
        /// </summary>
        /// <param name="universe">Universe for adding this indicator</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The AccumulationDistribution indicator for the requested securities over the speified period</returns>
        public Dictionary<Security, AccumulationDistribution> AD(Universe universe, Resolution resolution,
            Func<DataPoint, TradeBar> selector = null) =>
            GetUniverseIndicator(universe, security => AD(security, resolution, selector));

        /// <summary>
        /// Creates a new AccumulationDistribution indicator.
        /// </summary>
        /// <param name="security">The security whose AD we want</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The AccumulationDistribution indicator for the requested security over the speified period</returns>
        public AccumulationDistribution AD(Security security, Resolution resolution, Func<DataPoint, TradeBar> selector = null)
        {
            var name = CreateIndicatorName(security, "AD", resolution);
            var ad = new AccumulationDistribution(name);
            RegisterIndicator(security, ad, resolution, selector);
            return ad;
        }

        /// <summary>
        /// Creates a new AccumulationDistributionOscillator indicator.
        /// </summary>
        /// <param name="security">The security whose ADOSC we want</param>
        /// <param name="fastPeriod">The fast moving average period</param>
        /// <param name="slowPeriod">The slow moving average period</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The AccumulationDistributionOscillator indicator for the requested security over the speified period</returns>
        public AccumulationDistributionOscillator ADOSC(Security security, int fastPeriod, int slowPeriod, Resolution resolution, Func<DataPoint, TradeBar> selector = null)
        {
            var name = CreateIndicatorName(security, string.Format("ADOSC({0},{1})", fastPeriod, slowPeriod), resolution);
            var adOsc = new AccumulationDistributionOscillator(name, fastPeriod, slowPeriod);
            RegisterIndicator(security, adOsc, resolution, selector);
            return adOsc;
        }

        /// <summary>
        /// Creates a new AccumulationDistributionOscillator indicator.
        /// </summary>
        /// <param name="universe">The securities whose ADOSC we want</param>
        /// <param name="fastPeriod">The fast moving average period</param>
        /// <param name="slowPeriod">The slow moving average period</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The AccumulationDistributionOscillator indicator for the requested security over the speified period</returns>
        public Dictionary<Security, AccumulationDistributionOscillator> ADOSC(Universe universe, int fastPeriod, int slowPeriod,
            Resolution resolution, Func<DataPoint, TradeBar> selector = null) =>
            GetUniverseIndicator(universe, security => ADOSC(security, fastPeriod, slowPeriod, resolution, selector));

        /// <summary>
        /// Creates a new Average Directional Index indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose Average Directional Index we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="period">The period over which to compute the Average Directional Index</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The Average Directional Index indicator for the requested security.</returns>
        public AverageDirectionalIndex ADX(Security security, int period, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "ADX", resolution);
            var averageDirectionalIndex = new AverageDirectionalIndex(name, period);
            RegisterIndicator(security, averageDirectionalIndex, resolution, selector);
            return averageDirectionalIndex;
        }

        /// <summary>
        /// Creates a new Average Directional Index indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The securities whose Average Directional Index we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="period">The period over which to compute the Average Directional Index</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The Average Directional Index indicator for the requested security.</returns>
        public Dictionary<Security, AverageDirectionalIndex> ADX(Universe universe, int period,
            Resolution resolution, Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => ADX(security, period, resolution, selector));

        /// <summary>
        /// Creates a new AverageDirectionalMovementIndexRating indicator.
        /// </summary>
        /// <param name="security">The security whose ADXR we want</param>
        /// <param name="period">The period over which to compute the ADXR</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The AverageDirectionalMovementIndexRating indicator for the requested security over the specified period</returns>
        public AverageDirectionalMovementIndexRating ADXR(Security security, int period, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "ADXR" + period, resolution);
            var adxr = new AverageDirectionalMovementIndexRating(name, period);
            RegisterIndicator(security, adxr, resolution, selector);
            return adxr;
        }

        /// <summary>
        /// Creates a new AverageDirectionalMovementIndexRating indicator.
        /// </summary>
        /// <param name="universe">The securities whose ADXR we want</param>
        /// <param name="period">The period over which to compute the ADXR</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The AverageDirectionalMovementIndexRating indicator for the requested security over the specified period</returns>
        public Dictionary<Security, AverageDirectionalMovementIndexRating> ADXR(Universe universe, int period,
            Resolution resolution, Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => ADXR(security, period, resolution, selector));

        /// <summary>
        /// Creates a new ArnaudLegouxMovingAverage indicator.
        /// </summary>
        /// <param name="security">The security whose ALMA we want</param>
        /// <param name="period">int - the number of periods to calculate the ALMA</param>
        /// <param name="sigma"> int - this parameter is responsible for the shape of the curve coefficients.
        /// </param>
        /// <param name="offset">
        /// decimal - This parameter allows regulating the smoothness and high sensitivity of the
        /// Moving Average. The range for this parameter is [0, 1].
        /// </param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The ArnaudLegouxMovingAverage indicator for the requested security over the specified period</returns>
        public ArnaudLegouxMovingAverage ALMA(Security security, int period, Resolution resolution, int sigma = 6, decimal offset = 0.85m,  Func<DataPoint, decimal> selector = null)
        {
            var name = CreateIndicatorName(security, string.Format("ALMA_{0}_{1}_{2}", period, sigma, offset), resolution);
            var alma = new ArnaudLegouxMovingAverage(name, period, sigma, offset);
            RegisterIndicator(security, alma, resolution, selector);
            return alma;
        }

        /// <summary>
        /// Creates a new ArnaudLegouxMovingAverage indicator.
        /// </summary>
        /// <param name="universe">The securities whose ALMA we want</param>
        /// <param name="period">int - the number of periods to calculate the ALMA</param>
        /// <param name="sigma"> int - this parameter is responsible for the shape of the curve coefficients.
        /// </param>
        /// <param name="offset">
        /// decimal - This parameter allows regulating the smoothness and high sensitivity of the
        /// Moving Average. The range for this parameter is [0, 1].
        /// </param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The ArnaudLegouxMovingAverage indicator for the requested security over the specified period</returns>
        public Dictionary<Security, ArnaudLegouxMovingAverage> ALMA(Universe universe, int period, Resolution resolution, int sigma = 6,
            decimal offset = 0.85m,  Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => ALMA(security, period, resolution, sigma, offset, selector));

        /// <summary>
        /// Creates a new AbsolutePriceOscillator indicator.
        /// </summary>
        /// <param name="security">The security whose APO we want</param>
        /// <param name="fastPeriod">The fast moving average period</param>
        /// <param name="slowPeriod">The slow moving average period</param>
        /// <param name="movingAverageType">The type of moving average to use</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The AbsolutePriceOscillator indicator for the requested security over the specified period</returns>
        public AbsolutePriceOscillator APO(Security security, int fastPeriod, int slowPeriod, MovingAverageType movingAverageType, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            var name = CreateIndicatorName(security, string.Format("APO({0},{1})", fastPeriod, slowPeriod), resolution);
            var apo = new AbsolutePriceOscillator(name, fastPeriod, slowPeriod, movingAverageType);
            RegisterIndicator(security, apo, resolution, selector);
            return apo;
        }

        /// <summary>
        /// Creates a new AbsolutePriceOscillator indicator.
        /// </summary>
        /// <param name="universe">The securities whose APO we want</param>
        /// <param name="fastPeriod">The fast moving average period</param>
        /// <param name="slowPeriod">The slow moving average period</param>
        /// <param name="movingAverageType">The type of moving average to use</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The AbsolutePriceOscillator indicator for the requested security over the specified period</returns>
        public Dictionary<Security, AbsolutePriceOscillator> APO(Universe universe, int fastPeriod, int slowPeriod,
            MovingAverageType movingAverageType, Resolution resolution,
            Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe,
                security => APO(security, fastPeriod, slowPeriod, movingAverageType, resolution, selector));

        /// <summary>
        /// Creates a new AroonOscillator indicator which will compute the AroonUp and AroonDown (as well as the delta)
        /// </summary>
        /// <param name="security">The security whose Aroon we seek</param>
        /// <param name="period">The look back period for computing number of periods since maximum and minimum</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>An AroonOscillator configured with the specied periods</returns>
        public AroonOscillator AROON(Security security, int period, Resolution resolution, Func<DataPoint, DataPointBar> selector = null) =>
            AROON(security, period, period, resolution, selector);

        /// <summary>
        /// Creates a new AroonOscillator indicator which will compute the AroonUp and AroonDown (as well as the delta)
        /// </summary>
        /// <param name="universe">The securities whose Aroon we seek</param>
        /// <param name="period">The look back period for computing number of periods since maximum and minimum</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>An AroonOscillator configured with the specied periods</returns>
        public Dictionary<Security, AroonOscillator> AROON(Universe universe, int period, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => AROON(security, period, resolution, selector));

        /// <summary>
        /// Creates a new AroonOscillator indicator which will compute the AroonUp and AroonDown (as well as the delta)
        /// </summary>
        /// <param name="security">The security whose Aroon we seek</param>
        /// <param name="upPeriod">The look back period for computing number of periods since maximum</param>
        /// <param name="downPeriod">The look back period for computing number of periods since minimum</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>An AroonOscillator configured with the specified periods</returns>
        public AroonOscillator AROON(Security security, int upPeriod, int downPeriod, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, string.Format("AROON({0},{1})", upPeriod, downPeriod), resolution);
            var aroon = new AroonOscillator(name, upPeriod, downPeriod);
            RegisterIndicator(security, aroon, resolution, selector);
            return aroon;
        }

        /// <summary>
        /// Creates a new AroonOscillator indicator which will compute the AroonUp and AroonDown (as well as the delta)
        /// </summary>
        /// <param name="universe">The securities whose Aroon we seek</param>
        /// <param name="upPeriod">The look back period for computing number of periods since maximum</param>
        /// <param name="downPeriod">The look back period for computing number of periods since minimum</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>An AroonOscillator configured with the specified periods</returns>
        public Dictionary<Security, AroonOscillator> AROON(Universe universe, int upPeriod, int downPeriod, Resolution resolution,
             Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => AROON(security, upPeriod, downPeriod, resolution, selector));

        /// <summary>
        /// Creates a new AverageTrueRange indicator for the security. The indicator will be automatically
        /// updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose ATR we want</param>
        /// <param name="period">The smoothing period used to smooth the computed TrueRange values</param>
        /// <param name="type">The type of smoothing to use</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>A new AverageTrueRange indicator with the specified smoothing type and period</returns>
        public AverageTrueRange ATR(Security security, int period, Resolution resolution, MovingAverageType type = MovingAverageType.Simple,  Func<DataPoint, DataPointBar> selector = null)
        {
            string name = CreateIndicatorName(security, "ATR" + period, resolution);
            var atr = new AverageTrueRange(name, period, type);
            RegisterIndicator(security, atr, resolution, selector);
            return atr;
        }

        /// <summary>
        /// Creates a new AverageTrueRange indicator for the security. The indicator will be automatically
        /// updated on the given resolution.
        /// </summary>
        /// <param name="universe">The securities whose ATR we want</param>
        /// <param name="period">The smoothing period used to smooth the computed TrueRange values</param>
        /// <param name="type">The type of smoothing to use</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>A new AverageTrueRange indicator with the specified smoothing type and period</returns>
        public Dictionary<Security, AverageTrueRange> ATR(Universe universe, int period, Resolution resolution,
            MovingAverageType type = MovingAverageType.Simple, 
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => ATR(security, period, resolution, type, selector));

        /// <summary>
        /// Creates a new BollingerBands indicator which will compute the MiddleBand, UpperBand, LowerBand, and StandardDeviation
        /// </summary>
        /// <param name="security">The security whose BollingerBands we seek</param>
        /// <param name="period">The period of the standard deviation and moving average (middle band)</param>
        /// <param name="k">The number of standard deviations specifying the distance between the middle band and upper or lower bands</param>
        /// <param name="movingAverageType">The type of moving average to be used</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>A BollingerBands configured with the specied period</returns>
        public BollingerBands BB(Security security, int period, decimal k, Resolution resolution, MovingAverageType movingAverageType = MovingAverageType.Simple, 
            Func<DataPoint, decimal> selector = null)
        {
            var name = CreateIndicatorName(security, string.Format("BB({0},{1})", period, k), resolution);
            var bb = new BollingerBands(name, period, k, movingAverageType);
            RegisterIndicator(security, bb, resolution, selector);
            return bb;
        }

        /// <summary>
        /// Creates a new BollingerBands indicator which will compute the MiddleBand, UpperBand, LowerBand, and StandardDeviation
        /// </summary>
        /// <param name="universe">The securities whose BollingerBands we seek</param>
        /// <param name="period">The period of the standard deviation and moving average (middle band)</param>
        /// <param name="k">The number of standard deviations specifying the distance between the middle band and upper or lower bands</param>
        /// <param name="movingAverageType">The type of moving average to be used</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>A BollingerBands configured with the specied period</returns>
        public Dictionary<Security, BollingerBands> BB(Universe universe, int period, decimal k, Resolution resolution,
            MovingAverageType movingAverageType = MovingAverageType.Simple, 
            Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => BB(security, period, k, resolution, movingAverageType, selector));

        /// <summary>
        /// Creates a new Balance Of Power indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose Balance Of Power we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The Balance Of Power indicator for the requested security.</returns>
        public BalanceOfPower BOP(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "BOP", resolution);
            var bop = new BalanceOfPower(name);
            RegisterIndicator(security, bop, resolution, selector);
            return bop;
        }

        /// <summary>
        /// Creates a new Balance Of Power indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The securities whose Balance Of Power we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The Balance Of Power indicator for the requested security.</returns>
        public Dictionary<Security, BalanceOfPower> BOP(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => BOP(security, resolution, selector));

        /// <summary>
        /// Creates a new CommodityChannelIndex indicator. The indicator will be automatically
        /// updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose CCI we want</param>
        /// <param name="period">The period over which to compute the CCI</param>
        /// <param name="movingAverageType">The type of moving average to use in computing the typical price averge</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The CommodityChannelIndex indicator for the requested security over the specified period</returns>
        public CommodityChannelIndex CCI(Security security, int period, Resolution resolution, MovingAverageType movingAverageType = MovingAverageType.Simple, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "CCI" + period, resolution);
            var cci = new CommodityChannelIndex(name, period, movingAverageType);
            RegisterIndicator(security, cci, resolution, selector);
            return cci;
        }

        /// <summary>
        /// Creates a new CommodityChannelIndex indicator. The indicator will be automatically
        /// updated on the given resolution.
        /// </summary>
        /// <param name="universe">The securities whose CCI we want</param>
        /// <param name="period">The period over which to compute the CCI</param>
        /// <param name="movingAverageType">The type of moving average to use in computing the typical price averge</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The CommodityChannelIndex indicator for the requested security over the specified period</returns>
        public Dictionary<Security, CommodityChannelIndex> CCI(Universe universe, int period, Resolution resolution,
            MovingAverageType movingAverageType = MovingAverageType.Simple, 
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => CCI(security, period, resolution, movingAverageType, selector));

        /// <summary>
        /// Creates a new ChandeMomentumOscillator indicator.
        /// </summary>
        /// <param name="security">The security whose CMO we want</param>
        /// <param name="period">The period over which to compute the CMO</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The ChandeMomentumOscillator indicator for the requested security over the specified period</returns>
        public ChandeMomentumOscillator CMO(Security security, int period, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            var name = CreateIndicatorName(security, "CMO" + period, resolution);
            var cmo = new ChandeMomentumOscillator(name, period);
            RegisterIndicator(security, cmo, resolution, selector);
            return cmo;
        }

        /// <summary>
        /// Creates a new ChandeMomentumOscillator indicator.
        /// </summary>
        /// <param name="universe">The securities whose CMO we want</param>
        /// <param name="period">The period over which to compute the CMO</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The ChandeMomentumOscillator indicator for the requested security over the specified period</returns>
        public Dictionary<Security, ChandeMomentumOscillator> CMO(Universe universe, int period,
            Resolution resolution, Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => CMO(security, period, resolution, selector));

        /// <summary>
        /// Creates a new Donchian Channel indicator which will compute the Upper Band and Lower Band.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose Donchian Channel we seek.</param>
        /// <param name="upperPeriod">The period over which to compute the upper Donchian Channel.</param>
        /// <param name="lowerPeriod">The period over which to compute the lower Donchian Channel.</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The Donchian Channel indicator for the requested security.</returns>
        public DonchianChannel DCH(Security security, int upperPeriod, int lowerPeriod, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "DCH", resolution);
            var donchianChannel = new DonchianChannel(name, upperPeriod, lowerPeriod);
            RegisterIndicator(security, donchianChannel, resolution, selector);
            return donchianChannel;
        }

        /// <summary>
        /// Creates a new Donchian Channel indicator which will compute the Upper Band and Lower Band.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The securities whose Donchian Channel we seek.</param>
        /// <param name="upperPeriod">The period over which to compute the upper Donchian Channel.</param>
        /// <param name="lowerPeriod">The period over which to compute the lower Donchian Channel.</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The Donchian Channel indicator for the requested security.</returns>
        public Dictionary<Security, DonchianChannel> DCH(Universe universe, int upperPeriod, int lowerPeriod,
            Resolution resolution, Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => DCH(security, upperPeriod, lowerPeriod, resolution, selector));

        /// <summary>
        /// Overload shorthand to create a new symmetric Donchian Channel indicator which
        /// has the upper and lower channels set to the same period length.
        /// </summary>
        /// <param name="security">The security whose Donchian Channel we seek.</param>
        /// <param name="period">The period over which to compute the Donchian Channel.</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The Donchian Channel indicator for the requested security.</returns>
        public DonchianChannel DCH(Security security, int period, Resolution resolution, Func<DataPoint, DataPointBar> selector = null) =>
            DCH(security, period, period, resolution, selector);

        /// <summary>
        /// Overload shorthand to create a new symmetric Donchian Channel indicator which
        /// has the upper and lower channels set to the same period length.
        /// </summary>
        /// <param name="universe">The securities whose Donchian Channel we seek.</param>
        /// <param name="period">The period over which to compute the Donchian Channel.</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The Donchian Channel indicator for the requested security.</returns>
        public Dictionary<Security, DonchianChannel> DCH(Universe universe, int period, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => DCH(security, period, resolution, selector));

        /// <summary>
        /// Creates a new DoubleExponentialMovingAverage indicator.
        /// </summary>
        /// <param name="security">The security whose DEMA we want</param>
        /// <param name="period">The period over which to compute the DEMA</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The DoubleExponentialMovingAverage indicator for the requested security over the specified period</returns>
        public DoubleExponentialMovingAverage DEMA(Security security, int period, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            var name = CreateIndicatorName(security, "DEMA" + period, resolution);
            var dema = new DoubleExponentialMovingAverage(name, period);
            RegisterIndicator(security, dema, resolution, selector);
            return dema;
        }

        /// <summary>
        /// Creates a new DoubleExponentialMovingAverage indicator.
        /// </summary>
        /// <param name="universe">The securities whose DEMA we want</param>
        /// <param name="period">The period over which to compute the DEMA</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The DoubleExponentialMovingAverage indicator for the requested security over the specified period</returns>
        public Dictionary<Security, DoubleExponentialMovingAverage> DEMA(Universe universe, int period,
            Resolution resolution, Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => DEMA(security, period, resolution, selector));

        /// <summary>
        /// Creates an ExponentialMovingAverage indicator for the security. The indicator will be automatically
        /// updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose EMA we want</param>
        /// <param name="period">The period of the EMA</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The ExponentialMovingAverage for the given parameters</returns>
        public ExponentialMovingAverage EMA(Security security, int period, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            string name = CreateIndicatorName(security, "EMA" + period, resolution);
            var ema = new ExponentialMovingAverage(name, period);
            RegisterIndicator(security, ema, resolution, selector);
            return ema;
        }

        /// <summary>
        /// Creates an ExponentialMovingAverage indicator for the security. The indicator will be automatically
        /// updated on the given resolution.
        /// </summary>
        /// <param name="universe">The securities whose EMA we want</param>
        /// <param name="period">The period of the EMA</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The ExponentialMovingAverage for the given parameters</returns>
        public Dictionary<Security, ExponentialMovingAverage> EMA(Universe universe, int period,
            Resolution resolution, Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => EMA(security, period, resolution, selector));

        /// <summary>
        /// Creates a new FilteredIdentity indicator for the security The indicator will be automatically
        /// updated on the security's subscription resolution
        /// </summary>
        /// <param name="security">The security whose values we want as an indicator</param>
        /// <param name="resolution">The desired resolution of the data</param>
        /// <param name="selector">Selects a value from the BaseData, if null defaults to the .Value property (x => x.Value)</param>
        /// <param name="filter">Filters the DataPoint send into the indicator, if null defaults to true (x => true) which means no filter</param>
        /// <param name="fieldName">The name of the field being selected</param>
        /// <returns>A new FilteredIdentity indicator for the specified security and selector</returns>
        public FilteredIdentity FilteredIdentity(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null, Func<DataPoint, bool> filter = null, string fieldName = null)
        {
            string name = CreateIndicatorName(security, fieldName ?? "close", resolution);
            var filteredIdentity = new FilteredIdentity(name, filter);
            RegisterIndicator(security, filteredIdentity, resolution, selector);
            return filteredIdentity;
        }

        /// <summary>
        /// Creates a new FilteredIdentity indicator for the security The indicator will be automatically
        /// updated on the security's subscription resolution
        /// </summary>
        /// <param name="universe">The securities whose values we want as an indicator</param>
        /// <param name="resolution">The desired resolution of the data</param>
        /// <param name="selector">Selects a value from the BaseData, if null defaults to the .Value property (x => x.Value)</param>
        /// <param name="filter">Filters the DataPoint send into the indicator, if null defaults to true (x => true) which means no filter</param>
        /// <param name="fieldName">The name of the field being selected</param>
        /// <returns>A new FilteredIdentity indicator for the specified security and selector</returns>
        public Dictionary<Security, FilteredIdentity> FilteredIdentity(Universe universe, Resolution resolution, Func<DataPoint, DataPointBar> selector = null, 
            Func<DataPoint, bool> filter = null, string fieldName = null) =>
            GetUniverseIndicator(universe, security => FilteredIdentity(security, resolution, selector, filter, fieldName));

        /// <summary>
        /// Creates an FractalAdaptiveMovingAverage (FRAMA) indicator for the security. The indicator will be automatically
        /// updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose FRAMA we want</param>
        /// <param name="period">The period of the FRAMA</param>
        /// <param name="longPeriod">The long period of the FRAMA</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The FRAMA for the given parameters</returns>
        public FractalAdaptiveMovingAverage FRAMA(Security security, int period, Resolution resolution, int longPeriod = 198,  Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "FRAMA" + period, resolution);
            var frama = new FractalAdaptiveMovingAverage(name, period, longPeriod);
            RegisterIndicator(security, frama, resolution, selector);
            return frama;
        }

        /// <summary>
        /// Creates an FractalAdaptiveMovingAverage (FRAMA) indicator for the security. The indicator will be automatically
        /// updated on the given resolution.
        /// </summary>
        /// <param name="universe">The securities whose FRAMA we want</param>
        /// <param name="period">The period of the FRAMA</param>
        /// <param name="longPeriod">The long period of the FRAMA</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The FRAMA for the given parameters</returns>
        public Dictionary<Security, FractalAdaptiveMovingAverage> FRAMA(Universe universe, int period, Resolution resolution,
            int longPeriod = 198,  Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => FRAMA(security, period, resolution, longPeriod, selector));

        /// <summary>
        /// Creates a new Heikin-Ashi indicator.
        /// </summary>
        /// <param name="security">The security whose Heikin-Ashi we want</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The Heikin-Ashi indicator for the requested security over the specified period</returns>
        public HeikinAshi HeikinAshi(Security security, Resolution resolution, Func<DataPoint, TradeBar> selector = null)
        {
            var name = CreateIndicatorName(security, "HA", resolution);
            var ha = new HeikinAshi(name);
            RegisterIndicator(security, ha, resolution, selector);
            return ha;
        }

        /// <summary>
        /// Creates a new Heikin-Ashi indicator.
        /// </summary>
        /// <param name="universe">The securities whose Heikin-Ashi we want</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The Heikin-Ashi indicator for the requested security over the specified period</returns>
        public Dictionary<Security, HeikinAshi> HeikinAshi(Universe universe, Resolution resolution,
            Func<DataPoint, TradeBar> selector = null) =>
            GetUniverseIndicator(universe, security => HeikinAshi(security, resolution, selector));

        /// <summary>
        /// Creates a new HullMovingAverage indicator. The Hull moving average is a series of nested weighted moving averages, is fast and smooth.
        /// </summary>
        /// <param name="security">The security whose Hull moving average we want</param>
        /// <param name="period">The period over which to compute the Hull moving average</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns></returns>
        public HullMovingAverage HMA(Security security, int period, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            var name = CreateIndicatorName(security, "HMA" + period, resolution);
            var hma = new HullMovingAverage(name, period);
            RegisterIndicator(security, hma, resolution, selector);
            return hma;
        }

        /// <summary>
        /// Creates a new HullMovingAverage indicator. The Hull moving average is a series of nested weighted moving averages, is fast and smooth.
        /// </summary>
        /// <param name="universe">The securities whose Hull moving average we want</param>
        /// <param name="period">The period over which to compute the Hull moving average</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns></returns>
        public Dictionary<Security, HullMovingAverage> HMA(Universe universe, int period, Resolution resolution,
            Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => HMA(security, period, resolution, selector));

        /// <summary>
        /// Creates a new IchimokuKinkoHyo indicator for the security. The indicator will be automatically
        /// updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose ICHIMOKU we want</param>
        /// <param name="tenkanPeriod">The period to calculate the Tenkan-sen period</param>
        /// <param name="kijunPeriod">The period to calculate the Kijun-sen period</param>
        /// <param name="senkouAPeriod">The period to calculate the Tenkan-sen period</param>
        /// <param name="senkouBPeriod">The period to calculate the Tenkan-sen period</param>
        /// <param name="senkouADelayPeriod">The period to calculate the Tenkan-sen period</param>
        /// <param name="senkouBDelayPeriod">The period to calculate the Tenkan-sen period</param>
        /// <param name="resolution">Input data resolution</param>
        /// <returns>A new IchimokuKinkoHyo indicator with the specified periods and delays</returns>
        public IchimokuKinkoHyo ICHIMOKU(Security security, int tenkanPeriod, int kijunPeriod, int senkouAPeriod, int senkouBPeriod, int senkouADelayPeriod, int senkouBDelayPeriod, Resolution resolution = null)
        {
            var name = CreateIndicatorName(security, string.Format("ICHIMOKU({0},{1})", tenkanPeriod, kijunPeriod), resolution);
            var ichimoku = new IchimokuKinkoHyo(name, tenkanPeriod, kijunPeriod, senkouAPeriod, senkouBPeriod, senkouADelayPeriod, senkouBDelayPeriod);
            RegisterIndicator(security, ichimoku, resolution);
            return ichimoku;
        }

        /// <summary>
        /// Creates a new IchimokuKinkoHyo indicator for the security. The indicator will be automatically
        /// updated on the given resolution.
        /// </summary>
        /// <param name="universe">The securities whose ICHIMOKU we want</param>
        /// <param name="tenkanPeriod">The period to calculate the Tenkan-sen period</param>
        /// <param name="kijunPeriod">The period to calculate the Kijun-sen period</param>
        /// <param name="senkouAPeriod">The period to calculate the Tenkan-sen period</param>
        /// <param name="senkouBPeriod">The period to calculate the Tenkan-sen period</param>
        /// <param name="senkouADelayPeriod">The period to calculate the Tenkan-sen period</param>
        /// <param name="senkouBDelayPeriod">The period to calculate the Tenkan-sen period</param>
        /// <param name="resolution">Input data resolution</param>
        /// <returns>A new IchimokuKinkoHyo indicator with the specified periods and delays</returns>
        public Dictionary<Security, IchimokuKinkoHyo> ICHIMOKU(Universe universe, int tenkanPeriod, int kijunPeriod,
            int senkouAPeriod, int senkouBPeriod, int senkouADelayPeriod, int senkouBDelayPeriod,
            Resolution resolution = null) =>
            GetUniverseIndicator(universe,
                security => ICHIMOKU(security, tenkanPeriod, kijunPeriod, senkouAPeriod, senkouBPeriod,
                    senkouADelayPeriod, senkouBDelayPeriod, resolution));

        /// <summary>
        /// Creates a new Identity indicator for the security The indicator will be automatically
        /// updated on the security's subscription resolution
        /// </summary>
        /// <param name="security">The security whose values we want as an indicator</param>
        /// <param name="resolution">The desired resolution of the data</param>
        /// <param name="selector">Selects a value from the BaseData, if null defaults to the .Value property (x => x.Value)</param>
        /// <param name="fieldName">The name of the field being selected</param>
        /// <returns>A new Identity indicator for the specified security and selector</returns>
        public Identity Identity(Security security, Resolution resolution, Func<DataPoint, decimal> selector = null, string fieldName = null)
        {
            string name = string.Format("{0}({1}_{2})", security, fieldName ?? "close", resolution);
            var identity = new Identity(name);
            RegisterIndicator(security, identity, CreateAggregator(security, resolution), selector);
            return identity;
        }

        /// <summary>
        /// Creates a new Identity indicator for the security The indicator will be automatically
        /// updated on the security's subscription resolution
        /// </summary>
        /// <param name="universe">The securities whose values we want as an indicator</param>
        /// <param name="resolution">The desired resolution of the data</param>
        /// <param name="selector">Selects a value from the BaseData, if null defaults to the .Value property (x => x.Value)</param>
        /// <param name="fieldName">The name of the field being selected</param>
        /// <returns>A new Identity indicator for the specified security and selector</returns>
        public Dictionary<Security, Identity> Identity(Universe universe, Resolution resolution,
            Func<DataPoint, decimal> selector = null, string fieldName = null) =>
            GetUniverseIndicator(universe, security => Identity(security, resolution, selector, fieldName));

        /// <summary>
        /// Creates a new KaufmanAdaptiveMovingAverage indicator.
        /// </summary>
        /// <param name="security">The security whose KAMA we want</param>
        /// <param name="period">The period over which to compute the KAMA</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The KaufmanAdaptiveMovingAverage indicator for the requested security over the specified period</returns>
        public KaufmanAdaptiveMovingAverage KAMA(Security security, int period, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            var name = CreateIndicatorName(security, "KAMA" + period, resolution);
            var kama = new KaufmanAdaptiveMovingAverage(name, period);
            RegisterIndicator(security, kama, resolution, selector);
            return kama;
        }

        /// <summary>
        /// Creates a new KaufmanAdaptiveMovingAverage indicator.
        /// </summary>
        /// <param name="universe">The securities whose KAMA we want</param>
        /// <param name="period">The period over which to compute the KAMA</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The KaufmanAdaptiveMovingAverage indicator for the requested security over the specified period</returns>
        public Dictionary<Security, KaufmanAdaptiveMovingAverage> KAMA(Universe universe, int period,
            Resolution resolution, Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => KAMA(security, period, resolution, selector));

        /// <summary>
        /// Creates a new Keltner Channels indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose Keltner Channel we seek</param>
        /// <param name="period">The period over which to compute the Keltner Channels</param>
        /// <param name="k">The number of multiples of the <see cref="AverageTrueRange"/> from the middle band of the Keltner Channels</param>
        /// <param name="movingAverageType">Specifies the type of moving average to be used as the middle line of the Keltner Channel</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The Keltner Channel indicator for the requested security.</returns>
        public KeltnerChannels KCH(Security security, int period, decimal k, Resolution resolution, MovingAverageType movingAverageType = MovingAverageType.Simple,  Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "KCH", resolution);
            var keltnerChannels = new KeltnerChannels(name, period, k, movingAverageType);
            RegisterIndicator(security, keltnerChannels, resolution, selector);
            return keltnerChannels;
        }

        /// <summary>
        /// Creates a new Keltner Channels indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The securities whose Keltner Channel we seek</param>
        /// <param name="period">The period over which to compute the Keltner Channels</param>
        /// <param name="k">The number of multiples of the <see cref="AverageTrueRange"/> from the middle band of the Keltner Channels</param>
        /// <param name="movingAverageType">Specifies the type of moving average to be used as the middle line of the Keltner Channel</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The Keltner Channel indicator for the requested security.</returns>
        public Dictionary<Security, KeltnerChannels> KCH(Universe universe, int period, decimal k, Resolution resolution,
            MovingAverageType movingAverageType = MovingAverageType.Simple, 
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe,
                security => KCH(security, period, k, resolution, movingAverageType, selector));

        /// <summary>
        /// Creates a new LogReturn indicator.
        /// </summary>
        /// <param name="security">The security whose log return we seek</param>
        /// <param name="period">The period of the log return.</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar.</param>
        /// <returns>log return indicator for the requested security.</returns>
        public LogReturn LOGR(Security security, int period, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            string name = CreateIndicatorName(security, "LOGR", resolution);
            var logr = new LogReturn(name, period);
            RegisterIndicator(security, logr, resolution, selector);
            return logr;
        }

        /// <summary>
        /// Creates a new LogReturn indicator.
        /// </summary>
        /// <param name="universe">The securities whose log return we seek</param>
        /// <param name="period">The period of the log return.</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar.</param>
        /// <returns>log return indicator for the requested security.</returns>
        public Dictionary<Security, LogReturn> LOGR(Universe universe, int period, Resolution resolution,
            Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => LOGR(security, period, resolution, selector));

        /// <summary>
        /// Creates and registers a new Least Squares Moving Average instance.
        /// </summary>
        /// <param name="security">The security whose LSMA we seek.</param>
        /// <param name="period">The LSMA period. Normally 14.</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar.</param>
        /// <returns>A LeastSquaredMovingAverage configured with the specified period</returns>
        public LeastSquaresMovingAverage LSMA(Security security, int period, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            var name = CreateIndicatorName(security, "LSMA" + period, resolution);
            var lsma = new LeastSquaresMovingAverage(name, period);
            RegisterIndicator(security, lsma, resolution, selector);
            return lsma;
        }

        /// <summary>
        /// Creates and registers a new Least Squares Moving Average instance.
        /// </summary>
        /// <param name="universe">The securities whose LSMA we seek.</param>
        /// <param name="period">The LSMA period. Normally 14.</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar.</param>
        /// <returns>A LeastSquaredMovingAverage configured with the specified period</returns>
        public Dictionary<Security, LeastSquaresMovingAverage> LSMA(Universe universe, int period,
            Resolution resolution, Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => LSMA(security, period, resolution, selector));

        /// <summary>
        /// Creates a new LinearWeightedMovingAverage indicator.  This indicator will linearly distribute
        /// the weights across the periods.
        /// </summary>
        /// <param name="security">The security whose LWMA we want</param>
        /// <param name="period">The period over which to compute the LWMA</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns></returns>
        public LinearWeightedMovingAverage LWMA(Security security, int period, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            string name = CreateIndicatorName(security, "LWMA" + period, resolution);
            var lwma = new LinearWeightedMovingAverage(name, period);
            RegisterIndicator(security, lwma, resolution, selector);
            return lwma;
        }

        /// <summary>
        /// Creates a new LinearWeightedMovingAverage indicator.  This indicator will linearly distribute
        /// the weights across the periods.
        /// </summary>
        /// <param name="universe">The securities whose LWMA we want</param>
        /// <param name="period">The period over which to compute the LWMA</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns></returns>
        public Dictionary<Security, LinearWeightedMovingAverage> LWMA(Universe universe, int period,
            Resolution resolution, Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => LWMA(security, period, resolution, selector));

        /// <summary>
        /// Creates a MACD indicator for the security. The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose MACD we want</param>
        /// <param name="fastPeriod">The period for the fast moving average</param>
        /// <param name="slowPeriod">The period for the slow moving average</param>
        /// <param name="signalPeriod">The period for the signal moving average</param>
        /// <param name="type">The type of moving average to use for the MACD</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The moving average convergence divergence between the fast and slow averages</returns>
        public MovingAverageConvergenceDivergence MACD(Security security, int fastPeriod, int slowPeriod, int signalPeriod, Resolution resolution, MovingAverageType type = MovingAverageType.Simple,  Func<DataPoint, decimal> selector = null)
        {
            var name = CreateIndicatorName(security, string.Format("MACD({0},{1})", fastPeriod, slowPeriod), resolution);
            var macd = new MovingAverageConvergenceDivergence(name, fastPeriod, slowPeriod, signalPeriod, type);
            RegisterIndicator(security, macd, resolution, selector);
            return macd;
        }

        /// <summary>
        /// Creates a MACD indicator for the security. The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The securities whose MACD we want</param>
        /// <param name="fastPeriod">The period for the fast moving average</param>
        /// <param name="slowPeriod">The period for the slow moving average</param>
        /// <param name="signalPeriod">The period for the signal moving average</param>
        /// <param name="type">The type of moving average to use for the MACD</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The moving average convergence divergence between the fast and slow averages</returns>
        public Dictionary<Security, MovingAverageConvergenceDivergence> MACD(Universe universe, int fastPeriod, int slowPeriod,
            int signalPeriod, Resolution resolution, MovingAverageType type = MovingAverageType.Simple, 
            Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe,
                security => MACD(security, fastPeriod, slowPeriod, signalPeriod, resolution, type, selector));

        /// <summary>
        /// Creates a new MeanAbsoluteDeviation indicator.
        /// </summary>
        /// <param name="security">The security whose MeanAbsoluteDeviation we want</param>
        /// <param name="period">The period over which to compute the MeanAbsoluteDeviation</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The MeanAbsoluteDeviation indicator for the requested security over the specified period</returns>
        public MeanAbsoluteDeviation MAD(Security security, int period, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            var name = CreateIndicatorName(security, "MAD" + period, resolution);
            var mad = new MeanAbsoluteDeviation(name, period);
            RegisterIndicator(security, mad, resolution, selector);
            return mad;
        }

        /// <summary>
        /// Creates a new MeanAbsoluteDeviation indicator.
        /// </summary>
        /// <param name="universe">The securities whose MeanAbsoluteDeviation we want</param>
        /// <param name="period">The period over which to compute the MeanAbsoluteDeviation</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The MeanAbsoluteDeviation indicator for the requested security over the specified period</returns>
        public Dictionary<Security, MeanAbsoluteDeviation> MAD(Universe universe, int period,
            Resolution resolution, Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => MAD(security, period, resolution, selector));

        /// <summary>
        /// Creates a new Maximum indicator to compute the maximum value
        /// </summary>
        /// <param name="security">The security whose max we want</param>
        /// <param name="period">The look back period over which to compute the max value</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null and the security is of type TradeBar defaults to the High property,
        /// otherwise it defaults to Value property of BaseData (x => x.Value)</param>
        /// <returns>A Maximum indicator that compute the max value and the periods since the max value</returns>
        public Maximum MAX(Security security, int period, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            var name = CreateIndicatorName(security, "MAX" + period, resolution);
            var max = new Maximum(name, period);

            RegisterIndicator(security, max, CreateAggregator(security, resolution), selector);
            return max;
        }

        /// <summary>
        /// Creates a new Maximum indicator to compute the maximum value
        /// </summary>
        /// <param name="universe">The securities whose max we want</param>
        /// <param name="period">The look back period over which to compute the max value</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null and the security is of type TradeBar defaults to the High property,
        /// otherwise it defaults to Value property of BaseData (x => x.Value)</param>
        /// <returns>A Maximum indicator that compute the max value and the periods since the max value</returns>
        public Dictionary<Security, Maximum> MAX(Universe universe, int period, Resolution resolution,
            Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => MAX(security, period, resolution, selector));

        /// <summary>
        /// Creates a new MoneyFlowIndex indicator. The indicator will be automatically
        /// updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose MFI we want</param>
        /// <param name="period">The period over which to compute the MFI</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The MoneyFlowIndex indicator for the requested security over the specified period</returns>
        public MoneyFlowIndex MFI(Security security, int period, Resolution resolution, Func<DataPoint, TradeBar> selector = null)
        {
            var name = CreateIndicatorName(security, "MFI" + period, resolution);
            var mfi = new MoneyFlowIndex(name, period);
            RegisterIndicator(security, mfi, resolution, selector);
            return mfi;
        }

        /// <summary>
        /// Creates a new MoneyFlowIndex indicator. The indicator will be automatically
        /// updated on the given resolution.
        /// </summary>
        /// <param name="universe">The securities whose MFI we want</param>
        /// <param name="period">The period over which to compute the MFI</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The MoneyFlowIndex indicator for the requested security over the specified period</returns>
        public Dictionary<Security, MoneyFlowIndex> MFI(Universe universe, int period, Resolution resolution,
            Func<DataPoint, TradeBar> selector = null) =>
            GetUniverseIndicator(universe, security => MFI(security, period, resolution, selector));

        /// <summary>
        /// Creates a new MidPoint indicator.
        /// </summary>
        /// <param name="security">The security whose MIDPOINT we want</param>
        /// <param name="period">The period over which to compute the MIDPOINT</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The MidPoint indicator for the requested security over the specified period</returns>
        public MidPoint MIDPOINT(Security security, int period, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            var name = CreateIndicatorName(security, "MIDPOINT" + period, resolution);
            var midpoint = new MidPoint(name, period);
            RegisterIndicator(security, midpoint, resolution, selector);
            return midpoint;
        }

        /// <summary>
        /// Creates a new MidPoint indicator.
        /// </summary>
        /// <param name="universe">The securities whose MIDPOINT we want</param>
        /// <param name="period">The period over which to compute the MIDPOINT</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The MidPoint indicator for the requested security over the specified period</returns>
        public Dictionary<Security, MidPoint> MIDPOINT(Universe universe, int period, Resolution resolution,
            Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => MIDPOINT(security, period, resolution, selector));

        /// <summary>
        /// Creates a new MidPrice indicator.
        /// </summary>
        /// <param name="security">The security whose MIDPRICE we want</param>
        /// <param name="period">The period over which to compute the MIDPRICE</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The MidPrice indicator for the requested security over the specified period</returns>
        public MidPrice MIDPRICE(Security security, int period, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "MIDPRICE" + period, resolution);
            var midprice = new MidPrice(name, period);
            RegisterIndicator(security, midprice, resolution, selector);
            return midprice;
        }

        /// <summary>
        /// Creates a new MidPrice indicator.
        /// </summary>
        /// <param name="universe">The securities whose MIDPRICE we want</param>
        /// <param name="period">The period over which to compute the MIDPRICE</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The MidPrice indicator for the requested security over the specified period</returns>
        public Dictionary<Security, MidPrice> MIDPRICE(Universe universe, int period, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => MIDPRICE(security, period, resolution, selector));

        /// <summary>
        /// Creates a new Minimum indicator to compute the minimum value
        /// </summary>
        /// <param name="security">The security whose min we want</param>
        /// <param name="period">The look back period over which to compute the min value</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null and the security is of type TradeBar defaults to the Low property,
        /// otherwise it defaults to Value property of BaseData (x => x.Value)</param>
        /// <returns>A Minimum indicator that compute the in value and the periods since the min value</returns>
        public Minimum MIN(Security security, int period, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            var name = CreateIndicatorName(security, "MIN" + period, resolution);
            var min = new Minimum(name, period);

            // assign a default value for the selector function
            if (selector == null)
                selector = Field.Low;

            RegisterIndicator(security, min, CreateAggregator(security, resolution), selector);
            return min;
        }

        /// <summary>
        /// Creates a new Minimum indicator to compute the minimum value
        /// </summary>
        /// <param name="universe">The securities whose min we want</param>
        /// <param name="period">The look back period over which to compute the min value</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null and the security is of type TradeBar defaults to the Low property,
        /// otherwise it defaults to Value property of BaseData (x => x.Value)</param>
        /// <returns>A Minimum indicator that compute the in value and the periods since the min value</returns>
        public Dictionary<Security, Minimum> MIN(Universe universe, int period, Resolution resolution,
            Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => MIN(security, period, resolution, selector));

        /// <summary>
        /// Creates a new Momentum indicator. This will compute the absolute n-period change in the security.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose momentum we want</param>
        /// <param name="period">The period over which to compute the momentum</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The momentum indicator for the requested security over the specified period</returns>
        public Momentum MOM(Security security, int period, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            string name = CreateIndicatorName(security, "MOM" + period, resolution);
            var momentum = new Momentum(name, period);
            RegisterIndicator(security, momentum, resolution, selector);
            return momentum;
        }

        /// <summary>
        /// Creates a new Momentum indicator. This will compute the absolute n-period change in the security.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The securities whose momentum we want</param>
        /// <param name="period">The period over which to compute the momentum</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The momentum indicator for the requested security over the specified period</returns>
        public Dictionary<Security, Momentum> MOM(Universe universe, int period, Resolution resolution,
            Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => MOM(security, period, resolution, selector));

        /// <summary>
        /// Creates a new Momersion indicator.
        /// </summary>
        /// <param name="security">The security whose Momersion we want</param>
        /// <param name="minPeriod">The minimum period over which to compute the Momersion</param>
        /// <param name="fullPeriod">The full period over which to compute the Momersion</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The Momersion indicator for the requested security over the specified period</returns>
        public MomersionIndicator MOMERSION(Security security, int minPeriod, int fullPeriod, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            var name = CreateIndicatorName(security, string.Format("MOMERSION({0},{1})", minPeriod, fullPeriod), resolution);
            var momersion = new MomersionIndicator(name, minPeriod, fullPeriod);
            RegisterIndicator(security, momersion, resolution, selector);
            return momersion;
        }

        /// <summary>
        /// Creates a new Momersion indicator.
        /// </summary>
        /// <param name="universe">The securities whose Momersion we want</param>
        /// <param name="minPeriod">The minimum period over which to compute the Momersion</param>
        /// <param name="fullPeriod">The full period over which to compute the Momersion</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The Momersion indicator for the requested security over the specified period</returns>
        public Dictionary<Security, MomersionIndicator> MOMERSION(Universe universe, int minPeriod, int fullPeriod,
            Resolution resolution, Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe,
                security => MOMERSION(security, minPeriod, fullPeriod, resolution, selector));

        /// <summary>
        /// Creates a new MomentumPercent indicator. This will compute the n-period percent change in the security.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose momentum we want</param>
        /// <param name="period">The period over which to compute the momentum</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The momentum indicator for the requested security over the specified period</returns>
        public MomentumPercent MOMP(Security security, int period, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            string name = CreateIndicatorName(security, "MOMP" + period, resolution);
            var momentum = new MomentumPercent(name, period);
            RegisterIndicator(security, momentum, resolution, selector);
            return momentum;
        }

        /// <summary>
        /// Creates a new MomentumPercent indicator. This will compute the n-period percent change in the security.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The securities whose momentum we want</param>
        /// <param name="period">The period over which to compute the momentum</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The momentum indicator for the requested security over the specified period</returns>
        public Dictionary<Security, MomentumPercent> MOMP(Universe universe, int period, Resolution resolution,
            Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => MOMP(security, period, resolution, selector));

        /// <summary>
        /// Creates a new NormalizedAverageTrueRange indicator.
        /// </summary>
        /// <param name="security">The security whose NATR we want</param>
        /// <param name="period">The period over which to compute the NATR</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The NormalizedAverageTrueRange indicator for the requested security over the specified period</returns>
        public NormalizedAverageTrueRange NATR(Security security, int period, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "NATR" + period, resolution);
            var natr = new NormalizedAverageTrueRange(name, period);
            RegisterIndicator(security, natr, resolution, selector);
            return natr;
        }

        /// <summary>
        /// Creates a new NormalizedAverageTrueRange indicator.
        /// </summary>
        /// <param name="universe">The securities whose NATR we want</param>
        /// <param name="period">The period over which to compute the NATR</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The NormalizedAverageTrueRange indicator for the requested security over the specified period</returns>
        public Dictionary<Security, NormalizedAverageTrueRange> NATR(Universe universe, int period, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => NATR(security, period, resolution, selector));

        /// <summary>
        /// Creates a new On Balance Volume indicator. This will compute the cumulative total volume
        /// based on whether the close price being higher or lower than the previous period.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose On Balance Volume we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The On Balance Volume indicator for the requested security.</returns>
        public OnBalanceVolume OBV(Security security, Resolution resolution, Func<DataPoint, TradeBar> selector = null)
        {
            var name = CreateIndicatorName(security, "OBV", resolution);
            var onBalanceVolume = new OnBalanceVolume(name);
            RegisterIndicator(security, onBalanceVolume, resolution, selector);
            return onBalanceVolume;
        }

        /// <summary>
        /// Creates a new On Balance Volume indicator. This will compute the cumulative total volume
        /// based on whether the close price being higher or lower than the previous period.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The securities whose On Balance Volume we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The On Balance Volume indicator for the requested security.</returns>
        public Dictionary<Security, OnBalanceVolume> OBV(Universe universe, Resolution resolution,
            Func<DataPoint, TradeBar> selector = null) =>
            GetUniverseIndicator(universe, security => OBV(security, resolution, selector));

        /// <summary>
        /// Creates a new PercentagePriceOscillator indicator.
        /// </summary>
        /// <param name="security">The security whose PPO we want</param>
        /// <param name="fastPeriod">The fast moving average period</param>
        /// <param name="slowPeriod">The slow moving average period</param>
        /// <param name="movingAverageType">The type of moving average to use</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The PercentagePriceOscillator indicator for the requested security over the specified period</returns>
        public PercentagePriceOscillator PPO(Security security, int fastPeriod, int slowPeriod, MovingAverageType movingAverageType, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            var name = CreateIndicatorName(security, string.Format("PPO({0},{1})", fastPeriod, slowPeriod), resolution);
            var ppo = new PercentagePriceOscillator(name, fastPeriod, slowPeriod, movingAverageType);
            RegisterIndicator(security, ppo, resolution, selector);
            return ppo;
        }

        /// <summary>
        /// Creates a new PercentagePriceOscillator indicator.
        /// </summary>
        /// <param name="universe">The securities whose PPO we want</param>
        /// <param name="fastPeriod">The fast moving average period</param>
        /// <param name="slowPeriod">The slow moving average period</param>
        /// <param name="movingAverageType">The type of moving average to use</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The PercentagePriceOscillator indicator for the requested security over the specified period</returns>
        public Dictionary<Security, PercentagePriceOscillator> PPO(Universe universe, int fastPeriod, int slowPeriod,
            MovingAverageType movingAverageType, Resolution resolution,
            Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe,
                security => PPO(security, fastPeriod, slowPeriod, movingAverageType, resolution, selector));

        /// <summary>
        /// Creates a new Parabolic SAR indicator
        /// </summary>
        /// <param name="security">The security whose PSAR we seek</param>
        /// <param name="afStart">Acceleration factor start value. Normally 0.02</param>
        /// <param name="afIncrement">Acceleration factor increment value. Normally 0.02</param>
        /// <param name="afMax">Acceleration factor max value. Normally 0.2</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>A ParabolicStopAndReverse configured with the specified periods</returns>
        public ParabolicStopAndReverse PSAR(Security security, Resolution resolution, decimal afStart = 0.02m, decimal afIncrement = 0.02m, decimal afMax = 0.2m, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, string.Format("PSAR({0},{1},{2})", afStart, afIncrement, afMax), resolution);
            var psar = new ParabolicStopAndReverse(name, afStart, afIncrement, afMax);
            RegisterIndicator(security, psar, resolution, selector);
            return psar;
        }

        /// <summary>
        /// Creates a new Parabolic SAR indicator
        /// </summary>
        /// <param name="universe">The securities whose PSAR we seek</param>
        /// <param name="afStart">Acceleration factor start value. Normally 0.02</param>
        /// <param name="afIncrement">Acceleration factor increment value. Normally 0.02</param>
        /// <param name="afMax">Acceleration factor max value. Normally 0.2</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>A ParabolicStopAndReverse configured with the specified periods</returns>
        public Dictionary<Security, ParabolicStopAndReverse> PSAR(Universe universe, Resolution resolution, decimal afStart = 0.02m, decimal afIncrement = 0.02m,
            decimal afMax = 0.2m, Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe,
                security => PSAR(security, resolution, afStart, afIncrement, afMax, selector));

        /// <summary>
        /// Creates a new RegressionChannel indicator which will compute the LinearRegression, UpperChannel and LowerChannel lines, the intercept and slope
        /// </summary>
        /// <param name="security">The security whose RegressionChannel we seek</param>
        /// <param name="period">The period of the standard deviation and least square moving average (linear regression line)</param>
        /// <param name="k">The number of standard deviations specifying the distance between the linear regression and upper or lower channel lines</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>A Regression Channel configured with the specied period and number of standard deviation</returns>
        public RegressionChannel RC(Security security, int period, decimal k, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            var name = CreateIndicatorName(security, string.Format("RC({0},{1})", period, k), resolution);
            var rc = new RegressionChannel(name, period, k);
            RegisterIndicator(security, rc, resolution, selector);
            return rc;
        }

        /// <summary>
        /// Creates a new RegressionChannel indicator which will compute the LinearRegression, UpperChannel and LowerChannel lines, the intercept and slope
        /// </summary>
        /// <param name="universe">The securities whose RegressionChannel we seek</param>
        /// <param name="period">The period of the standard deviation and least square moving average (linear regression line)</param>
        /// <param name="k">The number of standard deviations specifying the distance between the linear regression and upper or lower channel lines</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>A Regression Channel configured with the specied period and number of standard deviation</returns>
        public Dictionary<Security, RegressionChannel> RC(Universe universe, int period, decimal k,
            Resolution resolution, Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => RC(security, period, k, resolution, selector));

        /// <summary>
        /// Creates and registers a new aggregator to receive automatic updates at the specified resolution as well as configures
        /// the indicator to receive updates from the aggregator.
        /// </summary>
        /// <param name="security">The security to register against</param>
        /// <param name="indicator">The indicator to receive data from the aggregator</param>
        /// <param name="resolution">The resolution at which to send data to the indicator, null to use the same resolution as the subscription</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        public void RegisterIndicator(Security security, IndicatorBase<IndicatorDataPoint> indicator, Resolution resolution, Func<DataPoint, decimal> selector = null) =>
            RegisterIndicator(security, indicator, CreateAggregator(security, resolution), selector ?? (x => x.Price));

        /// <summary>
        /// Registers the aggregator to receive automatic updates as well as configures the indicator to receive updates
        /// from the aggregator.
        /// </summary>
        /// <param name="security">The security to register against</param>
        /// <param name="indicator">The indicator to receive data from the aggregator</param>
        /// <param name="aggregator">The aggregator to receive raw subscription data</param>
        /// <param name="selector">Selects a value from the BaseData send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        public void RegisterIndicator(Security security, IndicatorBase<IndicatorDataPoint> indicator, DataAggregator aggregator, Func<DataPoint, decimal> selector = null)
        {
            // default our selector to the Value property on BaseData
            selector = selector ?? (x => x.Price);

            // register the aggregator for automatic updates via Subscription Tracker
            aggregator = QuantFund.Portfolio.Subscription.AddSubscription(QuantFund, security, aggregator, QuantFund.IsForceTick);

            // attach to the DataConsolidated event so it updates our indicator
            aggregator.DataAggregated += (sender, aggregated) =>
            {
                var price = selector(aggregated);
                indicator.Update(new IndicatorDataPoint(aggregated.Ticker, aggregated.EndTime, aggregated.TimeZone, price));
            };
        }

        /// <summary>
        /// Registers the aggregator to receive automatic updates as well as configures the indicator to receive updates
        /// from the aggregator.
        /// </summary>
        /// <param name="security">The security to register against</param>
        /// <param name="indicator">The indicator to receive data from the aggregator</param>
        /// <param name="resolution">The resolution at which to send data to the indicator, null to use the same resolution as the subscription</param>
        public void RegisterIndicator<T>(Security security, IndicatorBase<T> indicator, Resolution resolution = null)
            where T : DataPoint =>
            RegisterIndicator(security, indicator, CreateAggregator(security, resolution));

        /// <summary>
        /// Registers the aggregator to receive automatic updates as well as configures the indicator to receive updates
        /// from the aggregator.
        /// </summary>
        /// <param name="security">The security to register against</param>
        /// <param name="indicator">The indicator to receive data from the aggregator</param>
        /// <param name="resolution">The resolution at which to send data to the indicator, null to use the same resolution as the subscription</param>
        /// <param name="selector">Selects a value from the BaseData send into the indicator, if null defaults to a cast (x => (T)x)</param>
        public void RegisterIndicator<T>(Security security, IndicatorBase<T> indicator, Resolution resolution, Func<DataPoint, T> selector)
            where T : DataPoint =>
            RegisterIndicator(security, indicator, CreateAggregator(security, resolution), selector);

        /// <summary>
        /// Registers the aggregator to receive automatic updates as well as configures the indicator to receive updates
        /// from the aggregator.
        /// </summary>
        /// <param name="security">The security to register against</param>
        /// <param name="indicator">The indicator to receive data from the aggregator</param>
        /// <param name="aggregator">The aggregator to receive raw subscription data</param>
        /// <param name="selector">Selects a value from the BaseData send into the indicator, if null defaults to a cast (x => (T)x)</param>
        public void RegisterIndicator<T>(Security security, IndicatorBase<T> indicator, DataAggregator aggregator, Func<DataPoint, T> selector = null)
            where T : DataPoint
        {
            // assign default using cast
            selector = selector ?? (x => (T)x);

            // register the aggregator for automatic updates via Subscription Tracker
            aggregator = QuantFund.Portfolio.Subscription.AddSubscription(QuantFund, security, aggregator, QuantFund.IsForceTick);

            // check the output type of the aggregator and verify we can assign it to T
            var type = typeof(T);
            if (!type.IsAssignableFrom(aggregator.OutputType))
            {
                throw new ArgumentException(string.Format("Type mismatch found between aggregator and indicator for security: {0}." +
                                                          "aggregator outputs type {1} but indicator expects input type {2}",
                    security, aggregator.OutputType.Name, type.Name)
                );
            }

            // attach to the DataConsolidated event so it updates our indicator
            aggregator.DataAggregated += (sender, aggregated) =>
            {
                var value = selector(aggregated);
                indicator.Update(value);
            };
        }

        /// <summary>
        /// Creates a new RateOfChange indicator. This will compute the n-period rate of change in the security.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose RateOfChange we want</param>
        /// <param name="period">The period over which to compute the RateOfChange</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The RateOfChange indicator for the requested security over the specified period</returns>
        public RateOfChange ROC(Security security, int period, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            string name = CreateIndicatorName(security, "ROC" + period, resolution);
            var rateofchange = new RateOfChange(name, period);
            RegisterIndicator(security, rateofchange, resolution, selector);
            return rateofchange;
        }

        /// <summary>
        /// Creates a new RateOfChange indicator. This will compute the n-period rate of change in the security.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The securities whose RateOfChange we want</param>
        /// <param name="period">The period over which to compute the RateOfChange</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The RateOfChange indicator for the requested security over the specified period</returns>
        public Dictionary<Security, RateOfChange> ROC(Universe universe, int period, Resolution resolution,
            Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => ROC(security, period, resolution, selector));

        /// <summary>
        /// Creates a new RateOfChangePercent indicator. This will compute the n-period percentage rate of change in the security.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose RateOfChangePercent we want</param>
        /// <param name="period">The period over which to compute the RateOfChangePercent</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The RateOfChangePercent indicator for the requested security over the specified period</returns>
        public RateOfChangePercent ROCP(Security security, int period, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            string name = CreateIndicatorName(security, "ROCP" + period, resolution);
            var rateofchangepercent = new RateOfChangePercent(name, period);
            RegisterIndicator(security, rateofchangepercent, resolution, selector);
            return rateofchangepercent;
        }

        /// <summary>
        /// Creates a new RateOfChangePercent indicator. This will compute the n-period percentage rate of change in the security.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The securities whose RateOfChangePercent we want</param>
        /// <param name="period">The period over which to compute the RateOfChangePercent</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The RateOfChangePercent indicator for the requested security over the specified period</returns>
        public Dictionary<Security, RateOfChangePercent> ROCP(Universe universe, int period,
            Resolution resolution, Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => ROCP(security, period, resolution, selector));

        /// <summary>
        /// Creates a new RateOfChangeRatio indicator.
        /// </summary>
        /// <param name="security">The security whose ROCR we want</param>
        /// <param name="period">The period over which to compute the ROCR</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The RateOfChangeRatio indicator for the requested security over the specified period</returns>
        public RateOfChangeRatio ROCR(Security security, int period, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            var name = CreateIndicatorName(security, "ROCR" + period, resolution);
            var rocr = new RateOfChangeRatio(name, period);
            RegisterIndicator(security, rocr, resolution, selector);
            return rocr;
        }

        /// <summary>
        /// Creates a new RateOfChangeRatio indicator.
        /// </summary>
        /// <param name="universe">The securities whose ROCR we want</param>
        /// <param name="period">The period over which to compute the ROCR</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The RateOfChangeRatio indicator for the requested security over the specified period</returns>
        public Dictionary<Security, RateOfChangeRatio> ROCR(Universe universe, int period, Resolution resolution,
            Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => ROCR(security, period, resolution, selector));

        /// <summary>
        /// Creates a new RelativeStrengthIndex indicator. This will produce an oscillator that ranges from 0 to 100 based
        /// on the ratio of average gains to average losses over the specified period.
        /// </summary>
        /// <param name="security">The security whose RSI we want</param>
        /// <param name="period">The period over which to compute the RSI</param>
        /// <param name="movingAverageType">The type of moving average to use in computing the average gain/loss values</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The RelativeStrengthIndex indicator for the requested security over the specified period</returns>
        public RelativeStrengthIndex RSI(Security security, int period, Resolution resolution, MovingAverageType movingAverageType = MovingAverageType.Simple, Func<DataPoint, decimal> selector = null)
        {
            var name = CreateIndicatorName(security, "RSI" + period, resolution);
            var rsi = new RelativeStrengthIndex(name, period, movingAverageType);
            RegisterIndicator(security, rsi, resolution, selector);
            return rsi;
        }

        /// <summary>
        /// Creates a new RelativeStrengthIndex indicator. This will produce an oscillator that ranges from 0 to 100 based
        /// on the ratio of average gains to average losses over the specified period.
        /// </summary>
        /// <param name="universe">The securities whose RSI we want</param>
        /// <param name="period">The period over which to compute the RSI</param>
        /// <param name="movingAverageType">The type of moving average to use in computing the average gain/loss values</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The RelativeStrengthIndex indicator for the requested security over the specified period</returns>
        public Dictionary<Security, RelativeStrengthIndex> RSI(Universe universe, int period, Resolution resolution,
            MovingAverageType movingAverageType = MovingAverageType.Simple, 
            Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => RSI(security, period, resolution, movingAverageType, selector));

        /// <summary>
        /// Creates an SimpleMovingAverage indicator for the security. The indicator will be automatically
        /// updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose SMA we want</param>
        /// <param name="period">The period of the SMA</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The SimpleMovingAverage for the given parameters</returns>
        public SimpleMovingAverage SMA(Security security, int period, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            string name = CreateIndicatorName(security, "SMA" + period, resolution);
            var sma = new SimpleMovingAverage(name, period);
            RegisterIndicator(security, sma, resolution, selector);
            return sma;
        }

        /// <summary>
        /// Creates an SimpleMovingAverage indicator for the security. The indicator will be automatically
        /// updated on the given resolution.
        /// </summary>
        /// <param name="universe">The securities whose SMA we want</param>
        /// <param name="period">The period of the SMA</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The SimpleMovingAverage for the given parameters</returns>
        public Dictionary<Security, SimpleMovingAverage> SMA(Universe universe, int period,
            Resolution resolution, Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => SMA(security, period, resolution, selector));

        /// <summary>
        /// Creates a new StandardDeviation indicator. This will return the population standard deviation of samples over the specified period.
        /// </summary>
        /// <param name="security">The security whose STD we want</param>
        /// <param name="period">The period over which to compute the STD</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The StandardDeviation indicator for the requested security over the speified period</returns>
        public StandardDeviation STD(Security security, int period, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            var name = CreateIndicatorName(security, "STD" + period, resolution);
            var std = new StandardDeviation(name, period);
            RegisterIndicator(security, std, resolution, selector);
            return std;
        }

        /// <summary>
        /// Creates a new StandardDeviation indicator. This will return the population standard deviation of samples over the specified period.
        /// </summary>
        /// <param name="universe">The securities whose STD we want</param>
        /// <param name="period">The period over which to compute the STD</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The StandardDeviation indicator for the requested security over the speified period</returns>
        public Dictionary<Security, StandardDeviation> STD(Universe universe, int period, Resolution resolution,
            Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => STD(security, period, resolution, selector));

        /// <summary>
        /// Creates a new Stochastic indicator.
        /// </summary>
        /// <param name="security">The security whose stochastic we seek</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="period">The period of the stochastic. Normally 14</param>
        /// <param name="kPeriod">The sum period of the stochastic. Normally 14</param>
        /// <param name="dPeriod">The sum period of the stochastic. Normally 3</param>
        /// <returns>Stochastic indicator for the requested security.</returns>
        public Stochastic STO(Security security, int period, int kPeriod, int dPeriod, Resolution resolution = null)
        {
            string name = CreateIndicatorName(security, "STO", resolution);
            var stoch = new Stochastic(name, period, kPeriod, dPeriod);
            RegisterIndicator(security, stoch, resolution);
            return stoch;
        }

        /// <summary>
        /// Creates a new Stochastic indicator.
        /// </summary>
        /// <param name="universe">The securities whose stochastic we seek</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="period">The period of the stochastic. Normally 14</param>
        /// <param name="kPeriod">The sum period of the stochastic. Normally 14</param>
        /// <param name="dPeriod">The sum period of the stochastic. Normally 3</param>
        /// <returns>Stochastic indicator for the requested security.</returns>
        public Dictionary<Security, Stochastic> STO(Universe universe, int period, int kPeriod, int dPeriod,
            Resolution resolution = null) =>
            GetUniverseIndicator(universe, security => STO(security, period, kPeriod, dPeriod, resolution));

        /// <summary>
        /// Overload short hand to create a new Stochastic indicator; defaulting to the 3 period for dStoch
        /// </summary>
        /// <param name="security">The security whose stochastic we seek</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="period">The period of the stochastic. Normally 14</param>
        /// <returns>Stochastic indicator for the requested security.</returns>
        public Stochastic STO(Security security, int period, Resolution resolution = null) =>
            STO(security, period, period, 3, resolution);

        /// <summary>
        /// Overload short hand to create a new Stochastic indicator; defaulting to the 3 period for dStoch
        /// </summary>
        /// <param name="universe">The securities whose stochastic we seek</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="period">The period of the stochastic. Normally 14</param>
        /// <returns>Stochastic indicator for the requested security.</returns>
        public Dictionary<Security, Stochastic> STO(Universe universe, int period, Resolution resolution = null) =>
            GetUniverseIndicator(universe, security => STO(security, period, resolution));

        /// <summary>
        /// Creates a new Sum indicator.
        /// </summary>
        /// <param name="security">The security whose Sum we want</param>
        /// <param name="period">The period over which to compute the Sum</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The Sum indicator for the requested security over the specified period</returns>
        public Sum SUM(Security security, int period, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            var name = CreateIndicatorName(security, "SUM" + period, resolution);
            var sum = new Sum(name, period);
            RegisterIndicator(security, sum, resolution, selector);
            return sum;
        }

        /// <summary>
        /// Creates a new Sum indicator.
        /// </summary>
        /// <param name="universe">The securities whose Sum we want</param>
        /// <param name="period">The period over which to compute the Sum</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The Sum indicator for the requested security over the specified period</returns>
        public Dictionary<Security, Sum> SUM(Universe universe, int period, Resolution resolution,
            Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => SUM(security, period, resolution, selector));

        /// <summary>
        /// Creates Swiss Army Knife transformation for the security. The indicator will be automatically
        /// updated on the given resolution.
        /// </summary>
        /// <param name="security">The security to use for calculations</param>
        /// <param name="period">The period of the calculation</param>
        /// <param name="delta">The delta scale of the BandStop or BandPass</param>
        /// <param name="tool">The tool os the Swiss Army Knife</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">elects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The calculation using the given tool</returns>
        public SwissArmyKnife SWISS(Security security, int period, double delta, SwissArmyKnifeTool tool, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            string name = CreateIndicatorName(security, "SWISS" + period, resolution);
            var swiss = new SwissArmyKnife(name, period, delta, tool);
            RegisterIndicator(security, swiss, resolution, selector);
            return swiss;
        }

        /// <summary>
        /// Creates Swiss Army Knife transformation for the security. The indicator will be automatically
        /// updated on the given resolution.
        /// </summary>
        /// <param name="universe">The securities to use for calculations</param>
        /// <param name="period">The period of the calculation</param>
        /// <param name="delta">The delta scale of the BandStop or BandPass</param>
        /// <param name="tool">The tool os the Swiss Army Knife</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">elects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The calculation using the given tool</returns>
        public Dictionary<Security, SwissArmyKnife> SWISS(Universe universe, int period, double delta,
            SwissArmyKnifeTool tool, Resolution resolution, Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => SWISS(security, period, delta, tool, resolution, selector));

        /// <summary>
        /// Creates a new T3MovingAverage indicator.
        /// </summary>
        /// <param name="security">The security whose T3 we want</param>
        /// <param name="period">The period over which to compute the T3</param>
        /// <param name="volumeFactor">The volume factor to be used for the T3 (value must be in the [0,1] range, defaults to 0.7)</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The T3MovingAverage indicator for the requested security over the specified period</returns>
        public T3MovingAverage T3(Security security, int period, Resolution resolution, decimal volumeFactor = 0.7m, Func<DataPoint, decimal> selector = null)
        {
            var name = CreateIndicatorName(security, string.Format("T3({0},{1})", period, volumeFactor), resolution);
            var t3 = new T3MovingAverage(name, period, volumeFactor);
            RegisterIndicator(security, t3, resolution, selector);
            return t3;
        }

        /// <summary>
        /// Creates a new T3MovingAverage indicator.
        /// </summary>
        /// <param name="universe">The securities whose T3 we want</param>
        /// <param name="period">The period over which to compute the T3</param>
        /// <param name="volumeFactor">The volume factor to be used for the T3 (value must be in the [0,1] range, defaults to 0.7)</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The T3MovingAverage indicator for the requested security over the specified period</returns>
        public Dictionary<Security, T3MovingAverage> T3(Universe universe, int period, Resolution resolution,
            decimal volumeFactor = 0.7m, Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => T3(security, period, resolution, volumeFactor, selector));

        /// <summary>
        /// Creates a new TripleExponentialMovingAverage indicator.
        /// </summary>
        /// <param name="security">The security whose TEMA we want</param>
        /// <param name="period">The period over which to compute the TEMA</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The TripleExponentialMovingAverage indicator for the requested security over the specified period</returns>
        public TripleExponentialMovingAverage TEMA(Security security, int period, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            var name = CreateIndicatorName(security, "TEMA" + period, resolution);
            var tema = new TripleExponentialMovingAverage(name, period);
            RegisterIndicator(security, tema, resolution, selector);
            return tema;
        }

        /// <summary>
        /// Creates a new TripleExponentialMovingAverage indicator.
        /// </summary>
        /// <param name="universe">The securities whose TEMA we want</param>
        /// <param name="period">The period over which to compute the TEMA</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The TripleExponentialMovingAverage indicator for the requested security over the specified period</returns>
        public Dictionary<Security, TripleExponentialMovingAverage> TEMA(Universe universe, int period,
            Resolution resolution, Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => TEMA(security, period, resolution, selector));

        /// <summary>
        /// Creates a new TrueRange indicator.
        /// </summary>
        /// <param name="security">The security whose TR we want</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The TrueRange indicator for the requested security.</returns>
        public TrueRange TR(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "TR", resolution);
            var tr = new TrueRange(name);
            RegisterIndicator(security, tr, resolution, selector);
            return tr;
        }

        /// <summary>
        /// Creates a new TrueRange indicator.
        /// </summary>
        /// <param name="universe">The securities whose TR we want</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The TrueRange indicator for the requested security.</returns>
        public Dictionary<Security, TrueRange> TR(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => TR(security, resolution, selector));

        /// <summary>
        /// Creates a new TriangularMovingAverage indicator.
        /// </summary>
        /// <param name="security">The security whose TRIMA we want</param>
        /// <param name="period">The period over which to compute the TRIMA</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The TriangularMovingAverage indicator for the requested security over the specified period</returns>
        public TriangularMovingAverage TRIMA(Security security, int period, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            var name = CreateIndicatorName(security, "TRIMA" + period, resolution);
            var trima = new TriangularMovingAverage(name, period);
            RegisterIndicator(security, trima, resolution, selector);
            return trima;
        }

        /// <summary>
        /// Creates a new TriangularMovingAverage indicator.
        /// </summary>
        /// <param name="universe">The securities whose TRIMA we want</param>
        /// <param name="period">The period over which to compute the TRIMA</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The TriangularMovingAverage indicator for the requested security over the specified period</returns>
        public Dictionary<Security, TriangularMovingAverage> TRIMA(Universe universe, int period,
            Resolution resolution, Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => TRIMA(security, period, resolution, selector));

        /// <summary>
        /// Creates a new Trix indicator.
        /// </summary>
        /// <param name="security">The security whose TRIX we want</param>
        /// <param name="period">The period over which to compute the TRIX</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The Trix indicator for the requested security over the specified period</returns>
        public Trix TRIX(Security security, int period, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            var name = CreateIndicatorName(security, "TRIX" + period, resolution);
            var trix = new Trix(name, period);
            RegisterIndicator(security, trix, resolution, selector);
            return trix;
        }

        /// <summary>
        /// Creates a new Trix indicator.
        /// </summary>
        /// <param name="universe">The securities whose TRIX we want</param>
        /// <param name="period">The period over which to compute the TRIX</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The Trix indicator for the requested security over the specified period</returns>
        public Dictionary<Security, Trix> TRIX(Universe universe, int period, Resolution resolution,
            Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => TRIX(security, period, resolution, selector));

        /// <summary>
        /// Creates a new UltimateOscillator indicator.
        /// </summary>
        /// <param name="security">The security whose ULTOSC we want</param>
        /// <param name="period1">The first period over which to compute the ULTOSC</param>
        /// <param name="period2">The second period over which to compute the ULTOSC</param>
        /// <param name="period3">The third period over which to compute the ULTOSC</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The UltimateOscillator indicator for the requested security over the specified period</returns>
        public UltimateOscillator ULTOSC(Security security, int period1, int period2, int period3, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, string.Format("ULTOSC({0},{1},{2})", period1, period2, period3), resolution);
            var ultosc = new UltimateOscillator(name, period1, period2, period3);
            RegisterIndicator(security, ultosc, resolution, selector);
            return ultosc;
        }

        /// <summary>
        /// Creates a new UltimateOscillator indicator.
        /// </summary>
        /// <param name="universe">The securities whose ULTOSC we want</param>
        /// <param name="period1">The first period over which to compute the ULTOSC</param>
        /// <param name="period2">The second period over which to compute the ULTOSC</param>
        /// <param name="period3">The third period over which to compute the ULTOSC</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The UltimateOscillator indicator for the requested security over the specified period</returns>
        public Dictionary<Security, UltimateOscillator> ULTOSC(Universe universe, int period1, int period2, int period3,
            Resolution resolution, Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe,
                security => ULTOSC(security, period1, period2, period3, resolution, selector));

        /// <summary>
        /// Creates a new Variance indicator. This will return the population variance of samples over the specified period.
        /// </summary>
        /// <param name="security">The security whose VAR we want</param>
        /// <param name="period">The period over which to compute the VAR</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The Variance indicator for the requested security over the speified period</returns>
        public Variance VAR(Security security, int period, Resolution resolution, Func<DataPoint, decimal> selector = null)
        {
            var name = CreateIndicatorName(security, "VAR" + period, resolution);
            var variance = new Variance(name, period);
            RegisterIndicator(security, variance, resolution, selector);
            return variance;
        }

        /// <summary>
        /// Creates a new Variance indicator. This will return the population variance of samples over the specified period.
        /// </summary>
        /// <param name="universe">The security whose VAR we want</param>
        /// <param name="period">The period over which to compute the VAR</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The Variance indicator for the requested security over the speified period</returns>
        public Dictionary<Security, Variance> VAR(Universe universe, int period, Resolution resolution,
            Func<DataPoint, decimal> selector = null) =>
            GetUniverseIndicator(universe, security => VAR(security, period, resolution, selector));

        /// <summary>
        /// Creates an VolumeWeightedAveragePrice (VWAP) indicator for the security. The indicator will be automatically
        /// updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose VWAP we want</param>
        /// <param name="period">The period of the VWAP</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The VolumeWeightedAveragePrice for the given parameters</returns>
        public VolumeWeightedAveragePriceIndicator VWAP(Security security, int period, Resolution resolution, Func<DataPoint, TradeBar> selector = null)
        {
            var name = CreateIndicatorName(security, "VWAP" + period, resolution);
            var vwap = new VolumeWeightedAveragePriceIndicator(name, period);
            RegisterIndicator(security, vwap, resolution, selector);
            return vwap;
        }

        /// <summary>
        /// Creates an VolumeWeightedAveragePrice (VWAP) indicator for the security. The indicator will be automatically
        /// updated on the given resolution.
        /// </summary>
        /// <param name="universe">The securities whose VWAP we want</param>
        /// <param name="period">The period of the VWAP</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The VolumeWeightedAveragePrice for the given parameters</returns>
        public Dictionary<Security, VolumeWeightedAveragePriceIndicator> VWAP(Universe universe, int period,
            Resolution resolution, Func<DataPoint, TradeBar> selector = null) =>
            GetUniverseIndicator(universe, security => VWAP(security, period, resolution, selector));

        /// <summary>
        /// Creates a new Williams %R indicator. This will compute the percentage change of
        /// the current closing price in relation to the high and low of the past N periods.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose Williams %R we want</param>
        /// <param name="period">The period over which to compute the Williams %R</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The rateofchangepercent indicator for the requested security over the specified period</returns>
        public WilliamsPercentR WILR(Security security, int period, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            string name = CreateIndicatorName(security, "WILR" + period, resolution);
            var williamspercentr = new WilliamsPercentR(name, period);
            RegisterIndicator(security, williamspercentr, resolution, selector);
            return williamspercentr;
        }

        /// <summary>
        /// Creates a new Williams %R indicator. This will compute the percentage change of
        /// the current closing price in relation to the high and low of the past N periods.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The securities whose Williams %R we want</param>
        /// <param name="period">The period over which to compute the Williams %R</param>
        /// <param name="resolution">Input data resolution</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to the Value property of BaseData (x => x.Value)</param>
        /// <returns>The rateofchangepercent indicator for the requested security over the specified period</returns>
        public Dictionary<Security, WilliamsPercentR> WILR(Universe universe, int period, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => WILR(security, period, resolution, selector));

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Creates the name of the indicator.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="indicatorname">The indicatorname.</param>
        /// <param name="resolution">The resolution.</param>
        /// <returns></returns>
        private string CreateIndicatorName(Security security, string indicatorname, Resolution resolution) =>
            $"{security.Ticker}.{indicatorname}.{resolution?.ToString()}";

        /// <summary>
        /// Creates the aggregator.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="resolution">The resolution.</param>
        /// <returns></returns>
        private DataAggregator CreateAggregator(Security security, Resolution resolution)
        {
            //Return object
            DataAggregator toreturn;

            //Create aggregator
            if (!resolution.IsTick && resolution.TimeSpan.HasValue)
                toreturn = new TickQuoteBarAggregator(resolution.TimeSpan.Value);
            else
                toreturn = new TickAggregator(Convert.ToInt32(resolution.Ticks));

            //Check result
            if(toreturn == null)
                throw new Exception("Could not create aggregator, unknown what type of aggregator is needed");

            //Return based on the subscription tracker
            return QuantFund.Portfolio.Subscription.AddSubscription(QuantFund, security, toreturn, QuantFund.IsForceTick);
        }

        #endregion Private Methods
    }
}