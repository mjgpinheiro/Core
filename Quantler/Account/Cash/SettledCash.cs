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

namespace Quantler.Account.Cash
{
    /// <summary>
    /// Holder of settled cash
    /// </summary>
    public class SettledCash
    {
        #region Private Fields

        /// <summary>
        /// Current amount
        /// </summary>
        private decimal _amount;

        /// <summary>
        /// Can only work on it from 1 thread
        /// </summary>
        private readonly object _locker = new object();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initialize new cash information
        /// </summary>
        /// <param name="amount"></param>
        public SettledCash(decimal amount = 0) => _amount = amount;

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Current amount of cash
        /// </summary>
        public decimal Amount => _amount;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Add funds to this holder of cash
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public decimal AddFunds(decimal amount)
        {
            lock (_locker)
            {
                _amount += amount;
                return Amount;
            }
        }

        #endregion Public Methods
    }
}