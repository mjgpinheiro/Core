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
using Quantler.Data.Bars;
using Quantler.Indicators.CandlestickPatterns;
using Quantler.Interfaces;
using Quantler.Securities;
using System;
using System.Collections.Generic;

namespace Quantler.Modules
{
    /// <summary>
    /// Module indicator logic
    /// </summary>
    /// <seealso cref="Quantler.Interfaces.IModule" />
    public abstract partial class Module
    {
        #region Public Methods

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.AbandonedBaby"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="penetration">Percentage of penetration of a candle within another candle</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, AbandonedBaby> AbandonedBaby(Universe universe, Resolution resolution, decimal penetration = 0.3m,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => AbandonedBaby(security, resolution, penetration, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.AbandonedBaby"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="penetration">Percentage of penetration of a candle within another candle</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public AbandonedBaby AbandonedBaby(Security security, Resolution resolution, decimal penetration = 0.3m, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "ABANDONEDBABY", resolution);
            var pattern = new AbandonedBaby(name, penetration);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.AdvanceBlock"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public AdvanceBlock AdvanceBlock(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "ADVANCEBLOCK", resolution);
            var pattern = new AdvanceBlock(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.AdvanceBlock"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, AdvanceBlock> AdvanceBlock(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => AdvanceBlock(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.BeltHold"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public BeltHold BeltHold(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "BELTHOLD", resolution);
            var pattern = new BeltHold(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.BeltHold"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, BeltHold> BeltHold(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => BeltHold(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Breakaway"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Breakaway Breakaway(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "BREAKAWAY", resolution);
            var pattern = new Breakaway(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Breakaway"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, Breakaway> Breakaway(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => Breakaway(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.ClosingMarubozu"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public ClosingMarubozu ClosingMarubozu(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "CLOSINGMARUBOZU", resolution);
            var pattern = new ClosingMarubozu(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.ClosingMarubozu"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, ClosingMarubozu> ClosingMarubozu(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => ClosingMarubozu(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.ConcealedBabySwallow"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public ConcealedBabySwallow ConcealedBabySwallow(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "CONCEALEDBABYSWALLOW", resolution);
            var pattern = new ConcealedBabySwallow(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.ConcealedBabySwallow"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, ConcealedBabySwallow> ConcealedBabySwallow(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => ConcealedBabySwallow(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Counterattack"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Counterattack Counterattack(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "COUNTERATTACK", resolution);
            var pattern = new Counterattack(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Counterattack"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, Counterattack> Counterattack(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => Counterattack(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.DarkCloudCover"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="penetration">Percentage of penetration of a candle within another candle</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public DarkCloudCover DarkCloudCover(Security security, Resolution resolution, decimal penetration = 0.5m, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "DARKCLOUDCOVER", resolution);
            var pattern = new DarkCloudCover(name, penetration);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.DarkCloudCover"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="penetration">Percentage of penetration of a candle within another candle</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, DarkCloudCover> DarkCloudCover(Universe universe, Resolution resolution,
            decimal penetration = 0.5m, Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => DarkCloudCover(security, resolution, penetration, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Doji"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Doji Doji(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "DOJI", resolution);
            var pattern = new Doji(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Doji"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, Doji> Doji(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => Doji(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.DojiStar"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public DojiStar DojiStar(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "DOJISTAR", resolution);
            var pattern = new DojiStar(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.DojiStar"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, DojiStar> DojiStar(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => DojiStar(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.DragonflyDoji"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public DragonflyDoji DragonflyDoji(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "DRAGONFLYDOJI", resolution);
            var pattern = new DragonflyDoji(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.DragonflyDoji"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, DragonflyDoji> DragonflyDoji(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => DragonflyDoji(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Engulfing"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Engulfing Engulfing(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "ENGULFING", resolution);
            var pattern = new Engulfing(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Engulfing"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, Engulfing> Engulfing(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => Engulfing(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.EveningDojiStar"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="penetration">Percentage of penetration of a candle within another candle</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public EveningDojiStar EveningDojiStar(Security security, Resolution resolution, decimal penetration = 0.3m, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "EVENINGDOJISTAR", resolution);
            var pattern = new EveningDojiStar(name, penetration);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.EveningDojiStar"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="penetration">Percentage of penetration of a candle within another candle</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, EveningDojiStar> EveningDojiStar(Universe universe, Resolution resolution,
            decimal penetration = 0.3m, Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => EveningDojiStar(security, resolution, penetration, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.EveningStar"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="penetration">Percentage of penetration of a candle within another candle</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public EveningStar EveningStar(Security security, Resolution resolution, decimal penetration = 0.3m, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "EVENINGSTAR", resolution);
            var pattern = new EveningStar(name, penetration);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.EveningStar"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="penetration">Percentage of penetration of a candle within another candle</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, EveningStar> EveningStar(Universe universe, Resolution resolution,
            decimal penetration = 0.3m, Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => EveningStar(security, resolution, penetration, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.GapSideBySideWhite"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public GapSideBySideWhite GapSideBySideWhite(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "GAPSIDEBYSIDEWHITE", resolution);
            var pattern = new GapSideBySideWhite(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.GapSideBySideWhite"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, GapSideBySideWhite> GapSideBySideWhite(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => GapSideBySideWhite(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.GravestoneDoji"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public GravestoneDoji GravestoneDoji(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "GRAVESTONEDOJI", resolution);
            var pattern = new GravestoneDoji(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.GravestoneDoji"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, GravestoneDoji> GravestoneDoji(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => GravestoneDoji(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Hammer"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Hammer Hammer(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "HAMMER", resolution);
            var pattern = new Hammer(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Hammer"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, Hammer> Hammer(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => Hammer(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.HangingMan"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public HangingMan HangingMan(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "HANGINGMAN", resolution);
            var pattern = new HangingMan(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.HangingMan"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, HangingMan> HangingMan(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => HangingMan(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Harami"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Harami Harami(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "HARAMI", resolution);
            var pattern = new Harami(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Harami"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, Harami> Harami(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => Harami(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.HaramiCross"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public HaramiCross HaramiCross(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "HARAMICROSS", resolution);
            var pattern = new HaramiCross(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.HaramiCross"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, HaramiCross> HaramiCross(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => HaramiCross(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.HighWaveCandle"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public HighWaveCandle HighWaveCandle(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "HIGHWAVECANDLE", resolution);
            var pattern = new HighWaveCandle(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.HighWaveCandle"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, HighWaveCandle> HighWaveCandle(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => HighWaveCandle(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Hikkake"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Hikkake Hikkake(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "HIKKAKE", resolution);
            var pattern = new Hikkake(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Hikkake"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, Hikkake> Hikkake(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => Hikkake(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.HikkakeModified"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public HikkakeModified HikkakeModified(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "HIKKAKEMODIFIED", resolution);
            var pattern = new HikkakeModified(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.HikkakeModified"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, HikkakeModified> HikkakeModified(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => HikkakeModified(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.HomingPigeon"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public HomingPigeon HomingPigeon(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "HOMINGPIGEON", resolution);
            var pattern = new HomingPigeon(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.HomingPigeon"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, HomingPigeon> HomingPigeon(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => HomingPigeon(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.IdenticalThreeCrows"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public IdenticalThreeCrows IdenticalThreeCrows(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "IDENTICALTHREECROWS", resolution);
            var pattern = new IdenticalThreeCrows(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.IdenticalThreeCrows"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, IdenticalThreeCrows> IdenticalThreeCrows(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => IdenticalThreeCrows(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.InNeck"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public InNeck InNeck(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "INNECK", resolution);
            var pattern = new InNeck(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.InNeck"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, InNeck> InNeck(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => InNeck(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.InvertedHammer"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public InvertedHammer InvertedHammer(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "INVERTEDHAMMER", resolution);
            var pattern = new InvertedHammer(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.InvertedHammer"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, InvertedHammer> InvertedHammer(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => InvertedHammer(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Kicking"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Kicking Kicking(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "KICKING", resolution);
            var pattern = new Kicking(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Kicking"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, Kicking> Kicking(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => Kicking(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.KickingByLength"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public KickingByLength KickingByLength(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "KICKINGBYLENGTH", resolution);
            var pattern = new KickingByLength(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.KickingByLength"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, KickingByLength> KickingByLength(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => KickingByLength(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.LadderBottom"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public LadderBottom LadderBottom(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "LADDERBOTTOM", resolution);
            var pattern = new LadderBottom(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.LadderBottom"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, LadderBottom> LadderBottom(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => LadderBottom(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.LongLeggedDoji"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public LongLeggedDoji LongLeggedDoji(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "LONGLEGGEDDOJI", resolution);
            var pattern = new LongLeggedDoji(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.LongLeggedDoji"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, LongLeggedDoji> LongLeggedDoji(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => LongLeggedDoji(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.LongLineCandle"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public LongLineCandle LongLineCandle(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "LONGLINECANDLE", resolution);
            var pattern = new LongLineCandle(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.LongLineCandle"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, LongLineCandle> LongLineCandle(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => LongLineCandle(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Marubozu"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Marubozu Marubozu(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "MARUBOZU", resolution);
            var pattern = new Marubozu(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Marubozu"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, Marubozu> Marubozu(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => Marubozu(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.MatchingLow"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public MatchingLow MatchingLow(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "MATCHINGLOW", resolution);
            var pattern = new MatchingLow(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.MatchingLow"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, MatchingLow> MatchingLow(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => MatchingLow(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.MatHold"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="penetration">Percentage of penetration of a candle within another candle</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public MatHold MatHold(Security security, Resolution resolution, decimal penetration = 0.5m, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "MATHOLD", resolution);
            var pattern = new MatHold(name, penetration);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.MatHold"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="penetration">Percentage of penetration of a candle within another candle</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, MatHold> MatHold(Universe universe, Resolution resolution,
            decimal penetration = 0.5m, Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => MatHold(security, resolution, penetration, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.MorningDojiStar"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="penetration">Percentage of penetration of a candle within another candle</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public MorningDojiStar MorningDojiStar(Security security, Resolution resolution, decimal penetration = 0.3m, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "MORNINGDOJISTAR", resolution);
            var pattern = new MorningDojiStar(name, penetration);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.MorningDojiStar"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="penetration">Percentage of penetration of a candle within another candle</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, MorningDojiStar> MorningDojiStar(Universe universe, Resolution resolution,
            decimal penetration = 0.3m, Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => MorningDojiStar(security, resolution, penetration, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.MorningStar"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="penetration">Percentage of penetration of a candle within another candle</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public MorningStar MorningStar(Security security, Resolution resolution, decimal penetration = 0.3m, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "MORNINGSTAR", resolution);
            var pattern = new MorningStar(name, penetration);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.MorningStar"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="penetration">Percentage of penetration of a candle within another candle</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, MorningStar> MorningStar(Universe universe, Resolution resolution,
            decimal penetration = 0.3m, Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => MorningStar(security, resolution, penetration, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.OnNeck"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public OnNeck OnNeck(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "ONNECK", resolution);
            var pattern = new OnNeck(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.OnNeck"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, OnNeck> OnNeck(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => OnNeck(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Piercing"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Piercing Piercing(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "PIERCING", resolution);
            var pattern = new Piercing(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Piercing"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, Piercing> Piercing(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => Piercing(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.RickshawMan"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public RickshawMan RickshawMan(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "RICKSHAWMAN", resolution);
            var pattern = new RickshawMan(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.RickshawMan"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, RickshawMan> RickshawMan(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => RickshawMan(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.RiseFallThreeMethods"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public RiseFallThreeMethods RiseFallThreeMethods(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "RISEFALLTHREEMETHODS", resolution);
            var pattern = new RiseFallThreeMethods(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.RiseFallThreeMethods"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, RiseFallThreeMethods> RiseFallThreeMethods(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => RiseFallThreeMethods(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.SeparatingLines"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public SeparatingLines SeparatingLines(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "SEPARATINGLINES", resolution);
            var pattern = new SeparatingLines(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.SeparatingLines"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, SeparatingLines> SeparatingLines(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => SeparatingLines(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.ShootingStar"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public ShootingStar ShootingStar(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "SHOOTINGSTAR", resolution);
            var pattern = new ShootingStar(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.ShootingStar"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, ShootingStar> ShootingStar(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => ShootingStar(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.ShortLineCandle"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public ShortLineCandle ShortLineCandle(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "SHORTLINECANDLE", resolution);
            var pattern = new ShortLineCandle(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.ShortLineCandle"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, ShortLineCandle> ShortLineCandle(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => ShortLineCandle(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.SpinningTop"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public SpinningTop SpinningTop(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "SPINNINGTOP", resolution);
            var pattern = new SpinningTop(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.SpinningTop"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, SpinningTop> SpinningTop(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => SpinningTop(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.StalledPattern"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public StalledPattern StalledPattern(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "STALLEDPATTERN", resolution);
            var pattern = new StalledPattern(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.StalledPattern"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, StalledPattern> StalledPattern(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => StalledPattern(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.StickSandwich"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public StickSandwich StickSandwich(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "STICKSANDWICH", resolution);
            var pattern = new StickSandwich(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.StickSandwich"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, StickSandwich> StickSandwich(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => StickSandwich(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Takuri"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Takuri Takuri(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "TAKURI", resolution);
            var pattern = new Takuri(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Takuri"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, Takuri> Takuri(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => Takuri(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.TasukiGap"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public TasukiGap TasukiGap(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "TASUKIGAP", resolution);
            var pattern = new TasukiGap(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.TasukiGap"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, TasukiGap> TasukiGap(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => TasukiGap(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.ThreeBlackCrows"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public ThreeBlackCrows ThreeBlackCrows(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "THREEBLACKCROWS", resolution);
            var pattern = new ThreeBlackCrows(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.ThreeBlackCrows"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, ThreeBlackCrows> ThreeBlackCrows(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => ThreeBlackCrows(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.ThreeInside"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public ThreeInside ThreeInside(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "THREEINSIDE", resolution);
            var pattern = new ThreeInside(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.ThreeInside"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, ThreeInside> ThreeInside(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => ThreeInside(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.ThreeLineStrike"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public ThreeLineStrike ThreeLineStrike(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "THREELINESTRIKE", resolution);
            var pattern = new ThreeLineStrike(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.ThreeLineStrike"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, ThreeLineStrike> ThreeLineStrike(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => ThreeLineStrike(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.ThreeOutside"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public ThreeOutside ThreeOutside(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "THREEOUTSIDE", resolution);
            var pattern = new ThreeOutside(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.ThreeOutside"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, ThreeOutside> ThreeOutside(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => ThreeOutside(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.ThreeStarsInSouth"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public ThreeStarsInSouth ThreeStarsInSouth(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "THREESTARSINSOUTH", resolution);
            var pattern = new ThreeStarsInSouth(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.ThreeStarsInSouth"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, ThreeStarsInSouth> ThreeStarsInSouth(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => ThreeStarsInSouth(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.ThreeWhiteSoldiers"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public ThreeWhiteSoldiers ThreeWhiteSoldiers(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "THREEWHITESOLDIERS", resolution);
            var pattern = new ThreeWhiteSoldiers(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.ThreeWhiteSoldiers"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, ThreeWhiteSoldiers> ThreeWhiteSoldiers(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => ThreeWhiteSoldiers(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Thrusting"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Thrusting Thrusting(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "THRUSTING", resolution);
            var pattern = new Thrusting(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Thrusting"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, Thrusting> Thrusting(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => Thrusting(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Tristar"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Tristar Tristar(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "TRISTAR", resolution);
            var pattern = new Tristar(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.Tristar"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, Tristar> Tristar(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => Tristar(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.TwoCrows"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public TwoCrows TwoCrows(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "TWOCROWS", resolution);
            var pattern = new TwoCrows(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.TwoCrows"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, TwoCrows> TwoCrows(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => TwoCrows(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.UniqueThreeRiver"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public UniqueThreeRiver UniqueThreeRiver(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "UNIQUETHREERIVER", resolution);
            var pattern = new UniqueThreeRiver(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.UniqueThreeRiver"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, UniqueThreeRiver> UniqueThreeRiver(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => UniqueThreeRiver(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.UpDownGapThreeMethods"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public UpDownGapThreeMethods UpDownGapThreeMethods(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "UPDOWNGAPTHREEMETHODS", resolution);
            var pattern = new UpDownGapThreeMethods(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.UpDownGapThreeMethods"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, UpDownGapThreeMethods> UpDownGapThreeMethods(Universe universe,
            Resolution resolution, Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => UpDownGapThreeMethods(security, resolution, selector));

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.UpsideGapTwoCrows"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="security">The security whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public UpsideGapTwoCrows UpsideGapTwoCrows(Security security, Resolution resolution, Func<DataPoint, DataPointBar> selector = null)
        {
            var name = CreateIndicatorName(security, "UPSIDEGAPTWOCROWS", resolution);
            var pattern = new UpsideGapTwoCrows(name);
            RegisterIndicator(security, pattern, resolution, selector);
            return pattern;
        }

        /// <summary>
        /// Creates a new <see cref="Indicators.CandlestickPatterns.UpsideGapTwoCrows"/> pattern indicator.
        /// The indicator will be automatically updated on the given resolution.
        /// </summary>
        /// <param name="universe">The universe whose pattern we seek</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="selector">Selects a value from the BaseData to send into the indicator, if null defaults to casting the input value to a TradeBar</param>
        /// <returns>The pattern indicator for the requested security.</returns>
        public Dictionary<Security, UpsideGapTwoCrows> UpsideGapTwoCrows(Universe universe, Resolution resolution,
            Func<DataPoint, DataPointBar> selector = null) =>
            GetUniverseIndicator(universe, security => UpsideGapTwoCrows(security, resolution, selector));

        #endregion Public Methods
    }
}