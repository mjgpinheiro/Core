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

using Quantler.Configuration;
using System;
using System.Reflection;

namespace Quantler
{
    /// <summary>
    /// Framwork related functions
    /// </summary>
    public static class Framework
    {
        #region Public Fields

        /// <summary>
        /// If true, ignore version checks globally
        /// </summary>
        public static readonly bool IgnoreVersionChecks = Config.GlobalConfig.IgnoreVersionChecks;

        #endregion Public Fields

        #region Public Constructors

        /// <summary>
        /// Initializes the <see cref="Framework"/> class.
        /// </summary>
        static Framework() =>
            CurrentVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the current framework version.
        /// </summary>
        public static string CurrentVersion { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Compares two versions
        /// </summary>
        /// <returns>1 if the left version is after the right, 0 if they're the same, -1 if the left is before the right</returns>
        public static int CompareVersions(string left, string right)
        {
            //Check if we need to check for versions
            if (IgnoreVersionChecks || left == right)
                return 0;

            //Get versions
            Version leftVersion = ParseVersion(left);
            Version rightVersion = ParseVersion(right);

            //Compare
            return leftVersion.CompareTo(rightVersion);
        }

        /// <summary>
        /// Determines whether the current version is equal to the specified version.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>
        ///   <c>true</c> if [is equal version] [the specified version]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEqualVersion(string version) =>
            CompareVersions(version, CurrentVersion) == 0;

        /// <summary>
        /// Determines whether the current version is newer compared to the specified version.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>
        ///   <c>true</c> if [is newer version] [the specified version]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNewerVersion(string version) =>
            CompareVersions(version, CurrentVersion) > 0;

        /// <summary>
        /// Determines whether the current version is not the same as the specified version.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>
        ///   <c>true</c> if [is not equal version] [the specified version]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotEqualVersion(string version) =>
            !IsEqualVersion(version);

        /// <summary>
        /// Determines whether the current version is older than the specified version.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>
        ///   <c>true</c> if [is older version] [the specified version]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsOlderVersion(string version) =>
            CompareVersions(version, CurrentVersion) < 0;

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Parses the version.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        private static Version ParseVersion(string version)
        {
            //Try to get the version instance, if not possible, return a very low version instead
            if (Version.TryParse(version, out Version result))
                return result;
            else
                return new Version(0, 0, 0, 0);
        }

        #endregion Private Methods
    }
}