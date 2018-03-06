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
using System;

namespace Quantler
{
    /// <summary>
    /// Provides static properties to be used as selectors with the indicator system
    /// </summary>
    public static class Field
    {
        #region Public Properties

        /// <summary>
        /// Defines an average price that is equal to (O + H + L + C) / 4
        /// </summary>
        public static Func<DataPoint, decimal> Average => BaseDataBarPropertyOrValue(x => (x.Open + x.High + x.Low + x.Close) / 4m);

        /// <summary>
        /// Gets a selector that selects the Close value
        /// </summary>
        public static Func<DataPoint, decimal> Close => x => x.Price;

        /// <summary>
        /// Gets a selector that selects the High value
        /// </summary>
        public static Func<DataPoint, decimal> High => BaseDataBarPropertyOrValue(x => x.High);

        /// <summary>
        /// Gets a selector that selects the Low value
        /// </summary>
        public static Func<DataPoint, decimal> Low => BaseDataBarPropertyOrValue(x => x.Low);

        /// <summary>
        /// Defines an average price that is equal to (H + L) / 2
        /// </summary>
        public static Func<DataPoint, decimal> Median => BaseDataBarPropertyOrValue(x => (x.High + x.Low) / 2m);

        /// <summary>
        /// Gets a selector that selects the Open value
        /// </summary>
        public static Func<DataPoint, decimal> Open => BaseDataBarPropertyOrValue(x => x.Open);

        /// <summary>
        /// Defines an average price that is equal to (2*O + H + L + 3*C)/7
        /// </summary>
        public static Func<DataPoint, decimal> SevenBar => BaseDataBarPropertyOrValue(x => (2 * x.Open + x.High + x.Low + 3 * x.Close) / 7m);

        /// <summary>
        /// Defines an average price that is equal to (H + L + C) / 3
        /// </summary>
        public static Func<DataPoint, decimal> Typical => BaseDataBarPropertyOrValue(x => (x.High + x.Low + x.Close) / 3m);

        /// <summary>
        /// Gets a selector that selectors the Volume value
        /// </summary>
        public static Func<DataPoint, decimal> Volume => BaseDataBarPropertyOrValue(x => (x as TradeBar)?.Volume ?? 0m, x => 0m);

        /// <summary>
        /// Defines an average price that is equal to (H + L + 2*C) / 4
        /// </summary>
        public static Func<DataPoint, decimal> Weighted => BaseDataBarPropertyOrValue(x => (x.High + x.Low + 2 * x.Close) / 4m);

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// Bases the data bar property or value.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="defaultSelector">The default selector.</param>
        /// <returns></returns>
        private static Func<DataPoint, decimal> BaseDataBarPropertyOrValue(Func<DataPointBar, decimal> selector, Func<DataPoint, decimal> defaultSelector = null)
        {
            return x =>
            {
                if (x is DataPointBar bar)
                    return selector(bar);

                defaultSelector = defaultSelector ?? (data => data.Price);
                return defaultSelector(x);
            };
        }

        #endregion Private Methods
    }
}