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

using MessagePack;
using System;
using Quantler.Securities;

namespace Quantler.Data.Corporate
{
    /// <summary>
    /// Delisting of asset notification
    /// </summary>
    [MessagePackObject]
    public class Delisting : DataPointImpl
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Delisting"/> class.
        /// </summary>
        public Delisting()
        {
            DataType = DataType.Delisting;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Delisting"/> class.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="ticker"></param>
        /// <param name="processed"></param>
        public Delisting(TickerSymbol ticker, string state, DateTime processed)
            : this()
        {
            Ticker = ticker;
            State = state;
            Occured = processed;
            TimeZone = TimeZone.Utc;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Current state of delisting process
        /// </summary>
        [Key(6)]
        public string State { get; set; }

        #endregion Public Properties
    }
}