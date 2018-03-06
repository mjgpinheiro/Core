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

using Quantler.Interfaces;
using System;
using System.Composition;
using System.Reflection;

namespace Quantler.Bootstrap.ExceptionHandlers
{
    /// <summary>
    /// Default implementation of exception handling
    /// TODO: implement default exceptionhandler
    /// </summary>
    /// <seealso cref="Quantler.Interfaces.ExceptionHandler" />
    [Export(typeof(ExceptionHandler))]
    public class DefaultExceptionHandler : ExceptionHandler
    {
        #region Public Properties

        /// <summary>
        /// Current amount of unknown exceptions occured
        /// </summary>
        public int CurrentUnknownExceptions => 0;

        /// <summary>
        /// Maximum number of portfolio level (unkown) exceptions
        /// </summary>
        public int MaxUnknownExceptions => 1;

        /// <summary>
        /// Gets the runtime error.
        /// </summary>
        public Exception RuntimeError => null;

        /// <summary>
        /// Stop any further execution of this portfolio due to severe exceptions
        /// </summary>
        public bool StopExecution => false;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Add assembly to watch for exceptions
        /// </summary>
        /// <param name="fundid"></param>
        /// <param name="assembly"></param>
        public void AddAssembly(string fundid, Assembly assembly)
        {
            return;
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle an exception that has occured
        /// </summary>
        /// <param name="exc"></param>
        public void HandleException(Exception exc)
        {
            return;
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle an exception that has occured for a specific quant fund
        /// </summary>
        /// <param name="exc">The exc.</param>
        /// <param name="fundid">The fundid.</param>
        public void HandleException(Exception exc, string fundid)
        {
            return;
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove an assembly from being watched
        /// </summary>
        /// <param name="assembly"></param>
        public void RemoveAssembly(Assembly assembly)
        {
            return;
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove an assembly from being watched based on its agentid
        /// </summary>
        /// <param name="fundid"></param>
        public void RemoveAssembly(string fundid)
        {
            return;
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}