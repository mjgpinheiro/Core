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

namespace Quantler.Orders
{
    /// <summary>
    /// Order update holder
    /// </summary>
    public class OrderUpdate
    {
        #region Public Properties

        /// <summary>
        /// Alter the comment for this order
        /// </summary>
        public string Comment
        {
            get;
            set;
        }

        /// <summary>
        /// Set a new limit price, if this was a market order, the order type will change
        /// </summary>
        public decimal? LimitPrice
        {
            get;
            set;
        }

        /// <summary>
        /// Set a new order size based on its quantity
        /// </summary>
        public decimal? Quantity
        {
            get;
            set;
        }

        /// <summary>
        /// Set a new stop price for this order, if this order was a market order, the order type will change
        /// </summary>
        public decimal? StopPrice
        {
            get;
            set;
        }

        #endregion Public Properties
    }
}