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
using Quantler.Securities;

namespace Quantler.Indicators
{
    /// <summary>
    /// This indicator computes the Heikin-Ashi bar (HA)
    /// The Heikin-Ashi bar is calculated using the following formulas:
    /// HA_Close[0] = (Open[0] + High[0] + Low[0] + Close[0]) / 4
    /// HA_Open[0] = (HA_Open[1] + HA_Close[1]) / 2
    /// HA_High[0] = MAX(High[0], HA_Open[0], HA_Close[0])
    /// HA_Low[0] = MIN(Low[0], HA_Open[0], HA_Close[0])
    /// </summary>
    public class HeikinAshi : TradeBarIndicator
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HeikinAshi"/> class using the specified name.
        /// </summary>
        /// <param name="name">The name of this indicator</param>
        public HeikinAshi(string name)
            : base(name)
        {
            Open = new Identity(name + "_Open");
            High = new Identity(name + "_High");
            Low = new Identity(name + "_Low");
            Close = new Identity(name + "_Close");
            Volume = new Identity(name + "_Volume");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeikinAshi"/> class.
        /// </summary>
        public HeikinAshi()
            : this("HA")
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the Heikin-Ashi Close
        /// </summary>
        public IndicatorBase<IndicatorDataPoint> Close { get; }

        /// <summary>
        /// Gets the Heikin-Ashi current TradeBar
        /// </summary>
        public TradeBar CurrentBar => new TradeBar(Open.Current.Occured, Open.Current.TimeZone, TickerSymbol.NIL(""), Open, High, Low, Close, Convert.ToInt64(Volume.Current.Price));

        /// <summary>
        /// Gets the Heikin-Ashi High
        /// </summary>
        public IndicatorBase<IndicatorDataPoint> High { get; }

        /// <summary>
        /// Gets a flag indicating when this indicator is ready and fully initialized
        /// </summary>
        public override bool IsReady => Samples > 1;

        /// <summary>
        /// Gets the Heikin-Ashi Low
        /// </summary>
        public IndicatorBase<IndicatorDataPoint> Low { get; }

        /// <summary>
        /// Gets the Heikin-Ashi Open
        /// </summary>
        public IndicatorBase<IndicatorDataPoint> Open { get; }

        /// <summary>
        /// Gets the Heikin-Ashi Volume
        /// </summary>
        public IndicatorBase<IndicatorDataPoint> Volume { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Resets this indicator to its initial state
        /// </summary>
        public override void Reset()
        {
            Open.Reset();
            High.Reset();
            Low.Reset();
            Close.Reset();
            Volume.Reset();
            base.Reset();
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Computes the next value of this indicator from the given state
        /// </summary>
        /// <param name="input">The input given to the indicator</param>
        /// <returns> A new value for this indicator </returns>
        protected override decimal ComputeNextValue(TradeBar input)
        {
            if (!IsReady)
            {
                Open.Update(new IndicatorDataPoint(input.Occured, input.TimeZone, (input.Open + input.Close) / 2));
                Close.Update(new IndicatorDataPoint(input.Occured, input.TimeZone, (input.Open + input.High + input.Low + input.Close) / 4));
                High.Update(new IndicatorDataPoint(input.Occured, input.TimeZone, input.High));
                Low.Update(new IndicatorDataPoint(input.Occured, input.TimeZone, input.Low));
            }
            else
            {
                Open.Update(new IndicatorDataPoint(input.Occured, input.TimeZone, (Open + Close) / 2));
                Close.Update(new IndicatorDataPoint(input.Occured, input.TimeZone, (input.Open + input.High + input.Low + input.Close) / 4));
                High.Update(new IndicatorDataPoint(input.Occured, input.TimeZone, Math.Max(input.High, Math.Max(Open, Close))));
                Low.Update(new IndicatorDataPoint(input.Occured, input.TimeZone, Math.Min(input.Low, Math.Min(Open, Close))));
            }

            Volume.Update(new IndicatorDataPoint(input.Occured, input.TimeZone, input.Volume));

            return Close;
        }

        #endregion Protected Methods
    }
}