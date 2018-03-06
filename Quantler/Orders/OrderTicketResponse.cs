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

namespace Quantler.Orders
{
    /// <summary>
    /// General implementation of ticket response
    /// </summary>
    public class OrderTicketResponse
    {
        #region Private Constructors

        /// <summary>
        /// Create new ticket response
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="errorcode"></param>
        /// <param name="errormessage"></param>
        /// <param name="isprocessed"></param>
        private OrderTicketResponse(long orderid, OrderTicketResponseErrorCode errorcode, string errormessage, bool isprocessed = false)
        {
            OrderId = orderid;
            ErrorCode = errorcode;
            ErrorMessage = errormessage;
            IsProcessed = isprocessed;
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Associated error code, if applicable
        /// </summary>
        public OrderTicketResponseErrorCode ErrorCode { get; }

        /// <summary>
        /// Error messages related to error code
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// True if this response is an error response
        /// </summary>
        public bool IsError =>
            IsProcessed && ErrorCode != OrderTicketResponseErrorCode.None;

        /// <summary>
        /// True if this is a response on a processed request
        /// </summary>
        public bool IsProcessed { get; }

        /// <summary>
        /// True if processing this request succeeded
        /// </summary>
        public bool IsSuccess =>
            IsProcessed && !IsError;

        /// <summary>
        /// Associated order id
        /// </summary>
        public long OrderId { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Get error ticket response
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="error"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static OrderTicketResponse Error(long orderid, OrderTicketResponseErrorCode error, string message = "") =>
            new OrderTicketResponse(orderid, error, message, true);

        /// <summary>
        /// Get processed ticket response
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public static OrderTicketResponse Processed(long orderid) =>
            new OrderTicketResponse(orderid, OrderTicketResponseErrorCode.None, "", true);

        /// <summary>
        /// Success message, no errors
        /// </summary>
        /// <param name="orderid">The orderid.</param>
        /// <returns></returns>
        public static OrderTicketResponse Success(long orderid) =>
            new OrderTicketResponse(orderid, OrderTicketResponseErrorCode.None, String.Empty);

        /// <summary>
        /// Get unprocessed ticket response
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public static OrderTicketResponse Unprocessed(long orderid = 0) =>
            new OrderTicketResponse(orderid, OrderTicketResponseErrorCode.None, "");

        #endregion Public Methods
    }
}