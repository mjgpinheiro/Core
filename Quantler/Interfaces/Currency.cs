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
using Quantler.Data;

namespace Quantler.Interfaces
{
    /// <summary>
    /// Currency conversion
    /// </summary>
    public interface Currency
    {
        #region Public Properties

        /// <summary>
        /// Gets start date and time this instance is pre-loaded
        /// </summary>
        DateTime LoadedFromUtc { get; }

        /// <summary>
        /// Gets end date and time this instance is pre-loaded
        /// </summary>
        DateTime LoadedToUtc { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Converts the specified value from currency type to currency type
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        decimal Convert(decimal value, CurrencyType from, CurrencyType to);

        /// <summary>
        /// Initializes this instance
        /// </summary>
        /// <param name="clock">The clock.</param>
        /// <param name="isbacktest"></param>
        void Initialize(WorldClock clock, bool isbacktest);

        /// <summary>
        /// Update current currencies (usually used as a fallback)
        /// </summary>
        void Update();

        /// <summary>
        /// Update current currencies based on tick received
        /// </summary>
        /// <param name="updates"></param>
        void Update(DataUpdates updates);

        #endregion Public Methods
    }
}