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

namespace Quantler.Securities
{
    /// <summary>
    /// Additional security information
    /// </summary>
    public class SecurityDetails
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityDetails"/> class.
        /// </summary>
        /// <param name="lotsize">The lotsize.</param>
        /// <param name="ordermaxquantity">The ordermaxquantity.</param>
        /// <param name="orderminquantuty">The orderminquantuty.</param>
        /// <param name="orderstepquantity">The orderstepquantity.</param>
        /// <param name="digits">The digits.</param>
        /// <param name="expenseratio">The expenseratio.</param>
        public SecurityDetails(int lotsize, decimal ordermaxquantity, decimal orderminquantuty, decimal orderstepquantity, 
            int digits, decimal expenseratio)
        {
            LotSize = lotsize;
            OrderMaxQuantity = ordermaxquantity;
            OrderMinQuantity = orderminquantuty;
            OrderStepQuantity = orderstepquantity;
            Digits = digits;
            ExpenseRatio = expenseratio;
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// Empty constructor for empty details
        /// </summary>
        private SecurityDetails()
        { }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Digits used for pricing
        /// </summary>
        public int Digits { get; internal set; }

        /// <summary>
        /// Size of one lot
        /// </summary>
        public int LotSize { get; internal set; }

        /// <summary>
        /// Order maximum quantity allowed
        /// TODO: this is not used?
        /// </summary>
        public decimal OrderMaxQuantity { get; internal set; }

        /// <summary>
        /// Order minimum quantity
        /// TODO: this is not used?
        /// </summary>
        public decimal OrderMinQuantity { get; internal set; }

        /// <summary>
        /// Quantity at which order quantity is incremented
        /// TODO: this is not used?
        /// </summary>
        public decimal OrderStepQuantity { get; internal set; }

        /// <summary>
        /// Gets the security expense ratio if applicable
        /// </summary>
        public decimal ExpenseRatio { get; internal set; }

        /// <summary>
        /// The minimum price increment.
        /// </summary>
        public decimal MinimumPriceIncrement { get; internal set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Empty implementation
        /// </summary>
        /// <returns></returns>
        public static SecurityDetails NIL() => new SecurityDetails();

        #endregion Public Methods
    }
}