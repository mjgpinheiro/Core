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
using System.Reflection;

namespace Quantler.Interfaces
{
    /// <summary>
    /// Handler keeps watch on trading agent behavior
    /// Its duty is to make sure that malfunctioning agents cannot affect other agents in a portfolio
    /// </summary>
    public interface ExceptionHandler
    {
        #region Public Properties

        /// <summary>
        /// Current amount of unknown exceptions occured
        /// </summary>
        int CurrentUnknownExceptions { get; }

        /// <summary>
        /// Maximum number of portfolio level (unkown) exceptions
        /// </summary>
        int MaxUnknownExceptions { get; }

        /// <summary>
        /// Stop any further execution of this portfolio due to severe exceptions
        /// </summary>
        bool StopExecution { get; }

        /// <summary>
        /// Gets the runtime error.
        /// </summary>
        Exception RuntimeError { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Add assembly to watch for exceptions
        /// </summary>
        /// <param name="fundid"></param>
        /// <param name="assembly"></param>
        void AddAssembly(string fundid, Assembly assembly);

        /// <summary>
        /// Handle an exception that has occured
        /// </summary>
        /// <param name="exc"></param>
        void HandleException(Exception exc);

        /// <summary>
        /// Handle an exception that has occured for a specific quant fund
        /// </summary>
        /// <param name="exc">The exc.</param>
        /// <param name="fundid">The fundid.</param>
        void HandleException(Exception exc, string fundid);

        /// <summary>
        /// Remove an assembly from being watched
        /// </summary>
        /// <param name="assembly"></param>
        void RemoveAssembly(Assembly assembly);

        /// <summary>
        /// Remove an assembly from being watched based on its agentid
        /// </summary>
        /// <param name="fundid"></param>
        void RemoveAssembly(string fundid);

        #endregion Public Methods
    }
}