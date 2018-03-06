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

using Quantler.Orders;

namespace Quantler.Trades
{
    /// <summary>
    /// Helper method for creating trades from fills
    /// TODO: for easier testing, implement this helper
    /// TODO: add to design
    /// </summary>
    public class TradeComposite
    {
        #region Private Fields

        private TradeCompositeMatch TradeCompositeMatch;

        private TradeCompositeType TradeCompositeType;

        #endregion Private Fields

        #region Public Constructors

        public TradeComposite(TradeCompositeType type, TradeCompositeMatch match)
        {
            TradeCompositeType = type;
            TradeCompositeMatch = match;
        }

        #endregion Public Constructors

        #region Public Methods

        public Trade Create(Fill newfill, out Fill[] resultfills)
        {
            resultfills = null;
            return null;
        }

        #endregion Public Methods
    }
}