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

using NLog;
using Quantler.Messaging.Event;
using System;
using System.Composition;
using System.Net.Http;
using System.Text;
using Quantler.Interfaces;
using Quantler.Messaging;

namespace Quantler.Bootstrap.MessagingEvents
{
    /// <summary>
    /// Uses: https://dweet.io/
    /// Tip: use in conjunction with https://freeboard.io
    /// </summary>
    /// <seealso cref="EventRunnerBase" />
    [Export(typeof(EventRunner))]
    public class DweetRunner : EventRunnerBase
    {
        #region Private Fields

        /// <summary>
        /// The dweet URL
        /// </summary>
        private string _dweetUrl;

        /// <summary>
        /// Logging
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Initialize instance
        /// </summary>
        /// <exception cref="Exception"></exception>
        public override void Initialize(MessageInstance messageInstance)
        {
            //We always have subscribers
            HasSubscribers = true;

            //Get dweet url
            _dweetUrl = Environment.GetEnvironmentVariable("dweeturl");

            //Check url
            if (!Uri.IsWellFormedUriString(_dweetUrl, UriKind.RelativeOrAbsolute))
                throw new Exception($"Incorrect dweet url was supplied: {_dweetUrl}");

            //Slow down the sending interval
            int minsecondsinterval = 5; //Will only send updates every 5 seconds
            EventKeeper.SetCompareLogic((prev, current) =>
                        !prev.Equals(current) &&
                        (current.OccuredUtc - prev.OccuredUtc).TotalSeconds > minsecondsinterval);

            //We are not be able to send performance information as it is larger than 2.000 characters
            //You can always add a manipulator to make the message smaller (only sending the data you want to show via dweet)
            EventKeeper.SetCompareLogic(EventMessageType.PerformanceInfo, (prev, current) => false);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Send message to dweet
        /// </summary>
        /// <param name="message"></param>
        protected override void Send(EventMessage message)
        {
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.PostAsync(_dweetUrl, new StringContent(message.Serialize(), Encoding.UTF8, "application/json"));
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Could not send message to Dweet due to error, please check.");
            }
        }

        #endregion Protected Methods
    }
}