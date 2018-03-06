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

using System.Collections.Generic;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using NLog;
using Quantler.Configuration;

namespace Quantler.Common
{
    /// <summary>
    /// Used for dynamically loading an instance from an external library MEF (Managed Extensibility Framework)
    /// </summary>
    public class DynamicLoader
    {
        #region Private Fields

        /// <summary>
        /// Current instance locker object
        /// </summary>
        private static readonly object Locker = new object();

        /// <summary>
        /// Logging instance
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initialize new dynamic loader instance and load known libraries
        /// </summary>
        public DynamicLoader() =>
            LoadAssemblies();

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Get new instance of dynamic loader
        /// </summary>
        public static DynamicLoader Instance => new DynamicLoader();

        /// <summary>
        /// List of currently loaded assemblies
        /// </summary>
        public List<Assembly> LoadedAssemblies { get; } = new List<Assembly>();

        #endregion Public Properties

        #region Private Properties

        /// <summary>
        /// Configuration container
        /// </summary>
        private ContainerConfiguration Container { get; set; }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Try get an instance from a specific assembly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assembly">The assembly.</param>
        /// <param name="name">The name.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public static bool TryGetInstance<T>(Assembly assembly, string name, out T instance)
            where T : class
        {
            //Load container
            using (var container = new ContainerConfiguration().WithAssembly(assembly).CreateContainer())
            {
                //Try to get instance
                var exports = container.GetExports(typeof(T)).ToArray();
                var foundinstance = exports.FirstOrDefault(x => x.GetType().FullName == name) ?? exports.FirstOrDefault(x => x.GetType().Name == name);

                if (foundinstance != null)
                {
                    instance = foundinstance as T;
                    return true;
                }
                else
                {
                    instance = null;
                    return false;
                }
            }
        }

        /// <summary>
        /// Try get an instance from loaded assemblies
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool TryGetInstance<T>(string name, out T instance)
            where T : class
        {
            lock (Locker)
            {
                //Load configuration with associated assemblies
                using (var container = Container.CreateContainer())
                {
                    //Try to get instance
                    var exports = container.GetExports(typeof(T)).ToArray();
                    var foundinstance = exports.FirstOrDefault(x => x.GetType().FullName == name) ?? exports.FirstOrDefault(x => x.GetType().Name == name);
                    if (foundinstance != null)
                    {
                        instance = foundinstance as T;
                        return true;
                    }
                    else
                    {
                        instance = null;
                        return false;
                    }
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Get all assemblies in path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private IEnumerable<Assembly> GetAssemblies(string path) =>
                    Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories)
                             .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath);

        /// <summary>
        /// Load currently known assemblies
        /// </summary>
        private void LoadAssemblies()
        {
            lock (Locker)
            {
                //Clear current
                LoadedAssemblies.Clear();

                //Get all from current assembly directory
                var executableLocation = Assembly.GetEntryAssembly().Location;
                var path = Path.Combine(Path.GetDirectoryName(executableLocation));
                LoadedAssemblies.AddRange(GetAssemblies(path));

                //Get all from plug-ins locations
                string custompath = Config.GlobalConfig.AssembliesPath;

                //Check if path exists for reporting
                if(!string.IsNullOrWhiteSpace(custompath) && !Directory.Exists(custompath))
                    _log.Warn($"Cannot load custom assemblies path, path does not exist {custompath}");

                //Load
                if (!string.IsNullOrWhiteSpace(custompath) && Directory.Exists(custompath) && path != custompath)
                {
                    var found = GetAssemblies(custompath);
                    LoadedAssemblies.AddRange(found.Where(x => !LoadedAssemblies.Select(n => n.Location).Contains(x.Location)));
                }

                //Load container
                Container = new ContainerConfiguration().WithAssemblies(LoadedAssemblies);
            }
        }

        #endregion Private Methods
    }
}