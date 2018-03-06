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

using Quantler.Messaging;
using Quantler.Performance;
using System;
using System.Composition;

namespace Quantler.Bootstrap.ResultHandlers
{
    /// <summary>
    /// Returns the results into console
    /// TODO: implement, and in flow
    /// </summary>
    /// <seealso cref="ResultHandler" />
    [Export(typeof(ResultHandler))]
    public class ConsoleResultHandler : ResultHandler
    {
        #region Public Properties

        /// <summary>
        /// True if this task is running
        /// </summary>
        public bool IsRunning => true;

        #endregion Public Properties

        #region Private Properties

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        private Result Result { get; set; }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Initializes the specified result handler.
        /// </summary>
        /// <param name="message">The initial message that starts this process</param>
        /// <param name="result">The result.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Initialize(MessageInstance message, Result result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called when [end of execution].
        /// </summary>
        public void OnEndOfExecution()
        {
            //Print to console
            Console.WriteLine($"Result handler not implemented!");
        }

        /// <summary>
        /// Pokes this instace
        /// </summary>
        /// <param name="utc">The date and time in utc.</param>
        public void Poke(DateTime utc)
        {
            //Print to console
            Console.WriteLine($"Result handler not implemented!");
        }

        public void Start()
        {
            //Print to console
            Console.WriteLine($"Result handler not implemented!");
        }

        public void Stop()
        {
            //Print to console
            Console.WriteLine($"Result handler not implemented!");
        }

        #endregion Public Methods
    }
}