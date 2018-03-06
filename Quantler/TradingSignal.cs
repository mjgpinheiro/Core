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
using Quantler.Securities;
using System;
using System.Linq;

namespace Quantler
{
    /// <summary>
    /// Trading registered event
    /// </summary>
    public class TradingSignal
    {
        #region Private Fields

        /// <summary>
        /// Current instance logging
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TradingSignal"/> class.
        /// </summary>
        /// <param name="universe">The universe.</param>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        /// <param name="istimebased"></param>
        public TradingSignal(Universe universe, string name, Func<Security, bool> action, bool istimebased = false)
        {
            Universe = universe;
            Name = name;
            Action = action;
            IsTimeBased = istimebased;
        }

        #endregion Public Constructors

        #region Public Events

        /// <summary>
        /// Occurs when [event fired].
        /// </summary>
        public event TradingSignalHandler SignalFired;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Gets the action that is performed.
        /// </summary>
        public Func<Security, bool> Action { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is event based.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is event based; otherwise, <c>false</c>.
        /// </value>
        public bool IsEventBased => !IsTimeBased;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is logged.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is logged; otherwise, <c>false</c>.
        /// </value>
        public bool IsLogged { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is time based.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is time based; otherwise, <c>false</c>.
        /// </value>
        public bool IsTimeBased { get; }

        /// <summary>
        /// Gets the last time this event has occured in UTC.
        /// </summary>
        public DateTime LastOccuredUtc { get; private set; }

        /// <summary>
        /// Gets the name of this trading event.
        /// </summary>
        public string Name { get; }

        #endregion Public Properties

        #region Private Properties

        /// <summary>
        /// Gets the associated universe.
        /// </summary>
        private Universe Universe { get; }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(string left, TradingSignal right) =>
            left != right?.ToString();

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(TradingSignal left, string right) =>
            left?.ToString() != right;

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(string left, TradingSignal right) =>
            left == right?.ToString();

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(TradingSignal left, string right) =>
            left?.ToString() == right;

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public void Execute()
        {
            //Check for events found
            try
            {
                var found = Poke();
                if (found.Length > 0 && IsEnabled)
                {
                    DateTime occuredutc = Universe.Securities.First().Exchange.UtcTime;
                    if (IsLogged)
                        _log.Trace($"Executing trading signal {Name} at time {occuredutc}");

                    SignalFired?.Invoke(this, found);
                    LastOccuredUtc = occuredutc;
                }
            }
            catch (Exception exc)
            {
                //TODO: as this is a trading signal we need to notify the end user and cancel any further execution of this Quant Fund as exceptions should not occur
                _log.Error(exc, $"Error while executing trading signal {Name}");
            }
        }

        /// <summary>
        /// Pokes this instance for events that are fired.
        /// </summary>
        /// <returns></returns>
        public Security[] Poke() =>
            Universe.Select(security => new { Security = security, Triggered = Action(security) })
            .Where(n => n.Triggered)
            .Select(n => n.Security)
            .ToArray();

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() =>
            Name;

        #endregion Public Methods
    }
}