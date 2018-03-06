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

using Quantler.Broker.Model;
using Quantler.Data;

namespace Quantler.Interfaces
{
    /// <summary>
    /// Sim broker implementation, for simulated market acitivity
    /// </summary>
    /// <seealso cref="BrokerConnection" />
    public interface SimBrokerConnection : BrokerConnection
    {
        #region Public Properties

        /// <summary>
        /// Gets the broker model.
        /// </summary>
        BrokerModel BrokerModel { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Processes the market data.
        /// </summary>
        /// <param name="dataupdates">The market data.</param>
        void ProcessMarketData(DataUpdates dataupdates);

        /// <summary>
        /// Sets the current portfolio under test.
        /// </summary>
        /// <param name="portfolio">The PortfolioManager.</param>
        void SetPortfolio(IPortfolio portfolio);

        #endregion Public Methods
    }
}