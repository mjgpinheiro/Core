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

using CacheManager.Core;
using Quantler.Messaging.Event;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quantler.Bootstrap.MessagingEvents
{
    /// <summary>
    /// Custom logic for allowing messages and manipulating messages before sending them
    /// </summary>
    public class EventKeeper
    {
        #region Private Fields

        /// <summary>
        /// Custom compare logic
        /// </summary>
        private readonly Dictionary<EventMessageType, Func<EventMessage, EventMessage, bool>> _comparers = new Dictionary<EventMessageType, Func<EventMessage, EventMessage, bool>>();

        /// <summary>
        /// Custom manipulation logic
        /// </summary>
        private readonly Dictionary<EventMessageType, Func<EventMessage, EventMessage>> _manipulators = new Dictionary<EventMessageType, Func<EventMessage, EventMessage>>();

        /// <summary>
        /// Previous message cached
        /// </summary>
        private readonly ICacheManager<EventMessage> _prevMessages;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initialize event keeper with custom cache
        /// </summary>
        /// <param name="cache">Custom defined cache</param>
        public EventKeeper(ICacheManager<EventMessage> cache) => _prevMessages = cache;

        /// <summary>
        /// Initialize event keeper
        /// </summary>
        /// <param name="evictionseconds">Eviction of previous messages in seconds</param>
        public EventKeeper(int evictionseconds = 10)
        {
            _prevMessages = CacheFactory.Build<EventMessage>(settings =>
                                                settings.WithDictionaryHandle()
                                                .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(evictionseconds)));
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Contains default manipulation logic for send messages and changing content (for example removing old values to lower the message size)
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public Func<EventMessage, EventMessage> DefaultManipulationLogic(EventMessageType type)
        {
            switch (type)
            {
                case EventMessageType.PerformanceInfo:
                    return x =>
                    {
                        //Only send deltas
                        if (x is PerformanceInfoMessage item)
                        {
                            //Get previous
                            if (_prevMessages.Get(x.UniqueId) is PerformanceInfoMessage prev)
                            {
                                long lastmessagedt = prev.QuantFundResult.HistoricalEquity.Max(n => n.Key);
                                //TODO: make sure only deltas are send (historical values) (so we do not send all performance based information)
                            }

                            //Return results
                            return item;
                        }
                        else
                            return x;
                    };
                default:
                    return x => x;
            }
        }

        /// <summary>
        /// Check if this message has changes (so only deltas are send)
        /// </summary>
        /// <param name="message">Message to check if contains changes</param>
        /// <returns></returns>
        public bool HasChanged(EventMessage message)
        {
            //Get previous if known
            var prevmessage = _prevMessages.Get(message.UniqueId);
            if (prevmessage == null)
            {
                _prevMessages.Add(message.UniqueId, message);
                return true;
            }

            //Check for manipulation
            if (_manipulators.ContainsKey(message.Type))
                message = _manipulators[message.Type](message);

            //Check comparer's
            bool toreturn;
            toreturn = !_comparers.TryGetValue(message.Type, out Func<EventMessage, EventMessage, bool> comparer) ? comparer(prevmessage, message) : message.Equals(prevmessage);

            //Add to cache (if changes has occurred)
            if (toreturn)
                _prevMessages.Put(message.UniqueId, message);

            //Return result
            return toreturn;
        }

        /// <summary>
        /// Add custom comparer for event type
        /// Where (EventMessage, EventMessage) => (PrevMessage, CurrentMessage)
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="comparer">The comparer.</param>
        public void SetCompareLogic(EventMessageType type, Func<EventMessage, EventMessage, bool> comparer) => _comparers[type] = comparer;

        /// <summary>
        /// Add custom comparer for all event types
        /// Where (EventMessage, EventMessage) => (PrevMessage, CurrentMessage)
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public void SetCompareLogic(Func<EventMessage, EventMessage, bool> comparer)
        {
            SetCompareLogic(EventMessageType.AccountInfo, comparer);
            SetCompareLogic(EventMessageType.Exception, comparer);
            SetCompareLogic(EventMessageType.FundInfo, comparer);
            SetCompareLogic(EventMessageType.Instance, comparer);
            SetCompareLogic(EventMessageType.Logging, comparer);
            SetCompareLogic(EventMessageType.NIL, comparer);
            SetCompareLogic(EventMessageType.OrderInfo, comparer);
            SetCompareLogic(EventMessageType.PendingOrderInfo, comparer);
            SetCompareLogic(EventMessageType.PerformanceInfo, comparer);
            SetCompareLogic(EventMessageType.PositionInfo, comparer);
            SetCompareLogic(EventMessageType.Progress, comparer);
        }

        /// <summary>
        /// Add custom manipulation logic
        /// Where (EventMessage, EventMessage) =&gt; (CurrentMessage, NewMessage)
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="logic">The logic.</param>
        public void SetManipulationLogic(EventMessageType type, Func<EventMessage, EventMessage> logic) => _manipulators[type] = logic;

        #endregion Public Methods
    }
}