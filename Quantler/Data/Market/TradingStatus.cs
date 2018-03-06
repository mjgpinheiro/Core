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

namespace Quantler.Data.Market
{
    /// <summary>
    /// Trading status updates from exchangeModel
    /// </summary>
    [MessagePackObject]
    public class TradingStatus : DataPointImpl
    {
        #region Public Properties

        /// <summary>
        /// Reason for status update
        /// </summary>
        [Key(6)]
        public string Reason { get; set; }

        /// <summary>
        /// Status update
        /// TODO: make enum of it?
        /// TODO: implement flow for changing trading status
        /// 'H' Trading halted across all markets
        /// 'O' Trading halt released into an Order Acceptance Period 
        /// 'P' Trading paused and Order Acceptance Period on the current exchangeModel
        /// 'T' Trading on the current exchangeModel
        /// </summary>
        [Key(7)]
        public string Status { get; set; }

        #endregion Public Properties
    }
}