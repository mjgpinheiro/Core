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

using System.Composition;
using Quantler.Interfaces;

namespace Quantler.Bootstrap.Clusters
{
    /// <summary>
    /// Non-Clustered implementation
    /// </summary>
    [Export(typeof(Cluster))]
    public class NonClustered : Cluster
    {
        #region Public Properties

        /// <summary>
        /// Returns a non clustered instance
        /// </summary>
        /// <returns></returns>
        public static NonClustered Instance => new NonClustered();

        /// <summary>
        /// Non clustered is always master
        /// </summary>
        public bool IsMaster => true;

        /// <summary>
        /// True if this task is running
        /// </summary>
        public bool IsRunning
        {
            private set;
            get;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Start task
        /// </summary>
        public void Start() => IsRunning = true;

        /// <summary>
        /// Stop task
        /// </summary>
        public void Stop() => IsRunning = false;

        #endregion Public Methods
    }
}