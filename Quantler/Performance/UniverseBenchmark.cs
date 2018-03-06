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
using System.Collections.Generic;
using System.Composition;

namespace Quantler.Performance
{
    /// <summary>
    /// Default implementation for benchmarking a trading agents performance
    /// </summary>
    [Export(typeof(Benchmark))]
    public class UniverseBenchmark : Benchmark
    {
        #region Private Fields

        /// <summary>
        /// Cached benchmark values
        /// </summary>
        private readonly Dictionary<DateTime, decimal> _calcedvalues = new Dictionary<DateTime, decimal>();

        /// <summary>
        /// Function for deriving benchmark value
        /// </summary>
        private Func<DateTime, decimal> _benchmarkfunc;

        #endregion Private Fields

        #region Public Indexers

        /// <summary>
        /// Get benchmark value based on date, if available, else 0
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public decimal this[DateTime datetime] => Evaluate(datetime);

        #endregion Public Indexers

        #region Public Methods

        /// <summary>
        /// Evaluate value based on datetime moment
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public decimal Evaluate(DateTime datetime)
        {
            //Make sure we have date
            datetime = datetime.Date;

            //Check value
            if (!_calcedvalues.TryGetValue(datetime.Date, out var currentvalue))
            {
                currentvalue = _benchmarkfunc(datetime.Date);
                _calcedvalues.Add(datetime, currentvalue);
            }

            //Return value
            return currentvalue;
        }

        /// <summary>
        /// Set benchmark function
        /// </summary>
        /// <param name="benchmarkcalc"></param>
        public void OnCalc(Func<DateTime, decimal> benchmarkcalc)
        {
            if (_benchmarkfunc == null)
                _benchmarkfunc = benchmarkcalc;
        }

        #endregion Public Methods
    }
}