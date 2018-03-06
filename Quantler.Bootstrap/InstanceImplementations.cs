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

using Quantler.Common;
using Quantler.Configuration;
using Quantler.Messaging;
using System;

namespace Quantler.Bootstrap
{
    public class InstanceImplementations
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceImplementations"/> class.
        /// </summary>
        /// <param name="jobQueue">The job queue.</param>
        public InstanceImplementations(MessageQueue jobQueue) =>
            JobQueue = jobQueue;

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Message events for new jobs and live trading instance retrieved
        /// </summary>
        public MessageQueue JobQueue { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Get current associated implementations from config
        /// </summary>
        /// <returns></returns>
        public static InstanceImplementations FromConfig()
        {
            //get instance loader
            var loader = DynamicLoader.Instance;

            //Get instance and load
            if (!loader.TryGetInstance(Config.GlobalConfig.JobQueue, out MessageQueue messagequeue))
                throw new Exception($"Could not initialize instance JobQueue with config specified implementation of type {Config.GlobalConfig.JobQueue}");

            //Return the implementations
            return new InstanceImplementations(messagequeue);
        }

        #endregion Public Methods
    }
}