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
using System;

namespace Quantler.Brokers.BrokerTools
{
    /// <summary>
    /// Micro caching, caching of values according to function,
    /// in case a value of a function does not fluctuate that often we can return the same value again
    /// </summary>
    public class MicroCache
    {
        #region Private Fields

        /// <summary>
        /// Currently stored items
        /// </summary>
        private ICacheManager<object> _items;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initialize new microcache
        /// </summary>
        /// <param name="evictionspan">Timespan for values to be valid</param>
        public MicroCache(TimeSpan evictionspan)
        {
            EvictionSpan = evictionspan;
            _items = CacheFactory.Build<object>(c => c.WithDictionaryHandle()
                                                                  .WithExpiration(ExpirationMode.Absolute, evictionspan));
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Set eviction timespan (time before values will be replaced)
        /// </summary>
        public TimeSpan EvictionSpan { get; private set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Get value from microcache, if not available, add to cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">Unique name in cache</param>
        /// <param name="release">Function to release current value</param>
        /// <param name="evictionspan">Custom period before item is invalidated</param>
        /// <returns></returns>
        public T GetValue<T>(string name, Func<T> release, TimeSpan? evictionspan = null)
        {
            //Try get current value
            object found = _items.Get(name);
            if (found != null)
                if (found is T)
                    return (T)found;
                else
                    throw new Exception("Requested data of unknown type in MicroCache, please ensure cache names are unique!");

            //Invoke method to retrieve requested value
            var item = release();
            if (item == null)
                return item;

            //Add or update current item
            _items.Put(name, item);
            return item;
        }

        #endregion Public Methods
    }
}