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

using System.Collections.Concurrent;

namespace Quantler.Common
{
    /// <summary>
    /// Fixed sized queue, removes first added item when a new item is added
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Collections.Concurrent.ConcurrentQueue{T}" />
    public class FixedSizedQueue<T> : ConcurrentQueue<T>
    {
        #region Private Fields

        /// <summary>
        /// The locker object
        /// </summary>
        private readonly object _locker = new object();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedSizedQueue{T}"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        public FixedSizedQueue(int size) => Size = size;

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the current max size.
        /// </summary>
        public int Size { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Enqueues the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        public new void Enqueue(T obj)
        {
            base.Enqueue(obj);
            lock (_locker)
            {
                while (Count > Size)
                {
                    TryDequeue(out T outObj);
                }
            }
        }

        #endregion Public Methods
    }
}