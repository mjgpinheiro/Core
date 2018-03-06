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

using NLog;
using Quantler.Machine;
using System;
using System.IO;
using System.Linq;

namespace Quantler.Data.DataFile
{
    /// <summary>
    /// Data related utilities
    /// </summary>
    public static class DataUtil
    {
        #region Private Fields

        /// <summary>
        /// Current logging instance
        /// </summary>
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// create file from ticks
        /// </summary>
        /// <param name="dps"></param>
        /// <returns></returns>
        public static bool DatapointsToFile(DataPoint[] dps)
        {
            try
            {
                DataWriter dw = new DataWriter();
                foreach (DataPoint k in dps)
                    dw.Feed(k);

                Log.Debug(dw.RealSymbol + " saved " + dw.Count + " ticks to: " + dw.Filepath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating file from ticks");
                return false;
            }
            return true;
        }

        /// <summary>
        /// create file from ticks
        /// </summary>
        /// <param name="dps"></param>
        /// <param name="dw"></param>
        /// <returns></returns>
        public static bool DatapointsToFile(DataPoint[] dps, DataWriter dw)
        {
            try
            {
                foreach (DataPoint k in dps)
                    dw.Feed(k);
                Log.Debug(dw.RealSymbol + " saved " + dw.Count + " ticks to: " + dw.Filepath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating file from ticks");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get expected archive full name (including path)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="datasource"></param>
        /// <param name="date"></param>
        /// <param name="datatype"></param>
        /// <param name="aggregation"></param>
        /// <returns></returns>
        public static string GetArchiveFileName(string path, string datasource, DataType datatype, string aggregation, int date) =>
            Instance.GetPath(path, datasource, datatype.ToString(), aggregation, GetArchiveName(date));

        /// <summary>
        /// Gets the name of the path file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="datatype">The datatype.</param>
        /// <param name="aggregation">The aggregation.</param>
        /// <param name="date">The date.</param>
        /// <param name="tickersymbol">The tickersymbol.</param>
        /// <returns></returns>
        public static string GetPathFileName(string path, DataType datatype, string aggregation, int date, string tickersymbol) =>
            Instance.GetPath(path, datatype.ToString(), aggregation, SafeFilename(tickersymbol, date));

        /// <summary>
        /// Get expected archive name for data files
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string GetArchiveName(int date) =>
            date + ".zip";

        /// <summary>
        /// Check if file is writeable
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsFileWritetable(string path)
        {
            //Set filestream
            FileStream stream;

            try
            {
                if (!File.Exists(path))
                    return true;
                FileInfo file = new FileInfo(path);
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                return stream != null;
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return false;
            }
        }

        /// <summary>
        /// gets symbol safe to use as filename
        /// </summary>
        /// <param name="realticker"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string SafeFilename(string realticker, int date) =>
            string.Join(".", SafeTicker(realticker), date.ToString(), DataConst.Ext);

        /// <summary>
        /// gets symbol that is safe to use as filename
        /// </summary>
        /// <param name="realticker"></param>
        /// <returns></returns>
        public static string SafeTicker(string realticker)
        {
            char[] invalid = Path.GetInvalidPathChars();

            char[] more = "/\\*?:".ToCharArray();

            more.CopyTo(invalid, 0);

            foreach (char c in invalid)
            {
                int p = 0;
                while (p != -1)
                {
                    p = realticker.IndexOf(c);
                    if (p != -1)
                        realticker = realticker.Remove(p, 1);
                }
            }

            //Check for invalid windows folder names
            string[] invalidnames = {"CON", "AUX", "LST", "PRN", "NUL", "EOF", "INP", "OUT", ""};
            if (invalidnames.Contains(realticker.ToUpper()))
                realticker += "AX";

            return realticker;
        }

        #endregion Public Methods
    }
}