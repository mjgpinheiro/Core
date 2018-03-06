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

namespace Quantler.Data
{
    /// <summary>
    /// Data sources available at Quantler
    /// NOTE: DO NOT CHANGE THE ENUM NUMBERING, DUE TO THE USAGE OF MESSAGEPACK!
    /// </summary>
    public enum DataSource
    {

        /// <summary>
        /// Not Applicable (Default)
        /// </summary>
        NA = 0,

        /// <summary>
        /// IEX Data source
        /// </summary>
        IEX = 1,

        /// <summary>
        /// Bittrex Data source
        /// </summary>
        Bittrex = 2,

        /// <summary>
        /// HitBTC Data source
        /// </summary>
        HitBtc = 3,

        /// <summary>
        /// Binance Data Source
        /// </summary>
        Binance = 4,

        /// <summary>
        /// Cobinhood Data Source
        /// </summary>
        Cobinhood = 4
    }
}