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
using System.Collections.Generic;
using System.IO;
using Quantler.Machine;
using Quantler.Securities;

namespace Quantler.Data.DataFile
{
    /// <summary>
    /// Archive data as it arrives. Once archived, ticks can be replayed, viewed or analyzed
    /// </summary>
    public class DataArchiver
    {
        #region Private Fields

        /// <summary>
        /// Date based
        /// </summary>
        private readonly Dictionary<TickerSymbol, DateTime> _datedict = new Dictionary<TickerSymbol, DateTime>();

        /// <summary>
        /// File reference, for each ticker
        /// </summary>
        private readonly Dictionary<TickerSymbol, DataWriter> _filedict = new Dictionary<TickerSymbol, DataWriter>();

        /// <summary>
        /// Current path
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// The aggregation set
        /// </summary>
        private readonly string _aggregation;

        /// <summary>
        /// If true, we have stopped archiving
        /// </summary>
        private bool _stopped;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataArchiver"/> class.
        /// </summary>
        public DataArchiver()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataArchiver"/> class.
        /// </summary>
        /// <param name="folderpath">The folderpath.</param>
        /// <param name="aggregation">The aggregation.</param>
        public DataArchiver(string folderpath, string aggregation)
        {
            _path = folderpath;
            _aggregation = aggregation;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Add new data point
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public bool NewDataPoint(DataPoint d)
        {
            //Check if we stopped processing
            if (_stopped) return false;
            DataWriter tw;

            // prepare last date of data
            DateTime lastdate;

            // get last date
            bool havedate = _datedict.TryGetValue(d.Ticker, out lastdate);

            // if we don't have date, use present date
            if (!havedate)
            {
                lastdate = d.Occured.Date;
                _datedict.Add(d.Ticker, d.Occured.Date);
            }

            // see if we need a new day
            bool samedate = lastdate == d.Occured.Date;

            // see if we have stream already
            bool havestream = _filedict.TryGetValue(d.Ticker, out tw);

            // if no changes, just save tick
            if (samedate && havestream)
            {
                try
                {
                    tw.Feed(d);
                    return true;
                }
                catch (IOException) { return false; }
            }
            try
            {
                // ensure file is writable
                int date = Util.ToQLDate(d.Occured);
                string fn = Instance.GetPath(_path, DataUtil.SafeFilename(d.Ticker.Name, date));
                if (DataUtil.IsFileWritetable(fn))
                {
                    // open new stream
                    tw = new DataWriter(_path, d.Ticker.Name, date, _aggregation, d.DataType);

                    // save tick
                    tw.Feed(d);

                    // save stream
                    if (!havestream)
                        _filedict.Add(d.Ticker, tw);
                    else
                        _filedict[d.Ticker] = tw;

                    // save date if changed
                    if (!samedate)
                        _datedict[d.Ticker] = d.Occured.Date;
                }
            }
            catch (IOException) { return false; }
            catch (Exception) { return false; }

            return false;
        }

        /// <summary>
        /// Stop data archiver from saving datapoints to disk
        /// </summary>
        public void Stop()
        {
            try
            {
                foreach (var file in _filedict.Keys)
                    _filedict[file].Dispose();
                _stopped = true;
            }
            catch
            {
                // ignored
            }
        }

        #endregion Public Methods
    }
}