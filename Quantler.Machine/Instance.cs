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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Quantler.Machine
{
    /// <summary>
    /// Current instance process information
    /// </summary>
    public class Instance
    {
        #region Public Properties

        /// <summary>
        /// Get current executing directory
        /// </summary>
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetEntryAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        /// <summary>
        /// Get current cpu usage
        /// TODO: implement for .net core
        /// </summary>
        public static decimal CpuUsedPercentage => 0M;

        /// <summary>
        /// Time the current process is up and running
        /// TODO: implement for .net core
        /// </summary>
        public static TimeSpan CurrentProcessRunning => TimeSpan.MinValue;

        /// <summary>
        /// Get current host disk remaining in MegaBytes
        /// TODO: implement for .net core
        /// </summary>
        public static long DiskRemaining => long.MinValue;

        /// <summary>
        /// Get current host disk total size in MegaBytes
        /// TODO: implement for .net core
        /// </summary>
        public static long DiskTotal => long.MinValue;

        /// <summary>
        /// Get current host disk used size in MegaBytes
        /// </summary>
        public static long DiskUsed => (DiskTotal - DiskRemaining) / MB;

        /// <summary>
        /// Check if this instance is 64 bit or 32 bit based
        /// </summary>
        public static bool Is64Bit => IntPtr.Size == 4;

        /// <summary>
        /// True, if the current os is Linux
        /// </summary>
        public static bool Islinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        /// <summary>
        /// True, if the current os is Mac
        /// </summary>
        public static bool IsMac => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        /// <summary>
        /// True, if the current os is Windows
        /// </summary>
        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// <summary>
        /// Returns the amount of memory available in megabytes
        /// TODO: implement for .net core
        /// </summary>
        public static long MemoryAvailable => long.MinValue;

        /// <summary>
        /// Returns the amount of memory used in megabytes
        /// TODO: implement for .net core
        /// </summary>
        public static long MemoryUsed => long.MinValue;

        /// <summary>
        /// Get maximum memory used in megabytes
        /// TODO: implement for .net core
        /// </summary>
        public static long PeakPhysicalMemoryUsage => long.MinValue;

        /// <summary>
        /// Returns the amount of memory available in megabytes
        /// TODO: implement for .net core
        /// </summary>
        public static long PhysicalMemoryTotal => long.MinValue;

        /// <summary>
        /// Returns the amount of physical memory available in megabytes
        /// TODO: implement for .net core
        /// </summary>
        public static long PhysicalMemoryUsed => GC.GetTotalMemory(false) / MB;

        /// <summary>
        /// Get the current version used of Quantler.Core
        /// </summary>
        public static string QuantlerVersion => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

        #endregion Public Properties

        #region Private Properties

        /// <summary>
        /// Megabyte
        /// </summary>
        public static int MB => (1024 ^ 2);

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Returns the correct path, platform independent
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public static string GetPath(params object[] names) => Path.Combine(names.Select(x => x.ToString()).ToArray());

        #endregion Public Methods
    }
}