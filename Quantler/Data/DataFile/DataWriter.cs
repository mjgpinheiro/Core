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

using MessagePack;
using System;
using System.IO;

namespace Quantler.Data.DataFile
{
    /// <summary>
    /// Writer for data files
    /// </summary>
    public class DataWriter : BinaryWriter
    {
        #region Public Fields

        /// <summary>
        /// DataPoints written
        /// </summary>
        public int Count;

        #endregion Public Fields

        #region Private Fields

        private DataType _datattype = DataType.Tick;
        private int _date;
        private string _file = string.Empty;
        private bool _hasheader;
        private string _path = string.Empty;
        private string _realsymbol = string.Empty;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Creates a data writer with no header, header is created from first datapoint
        /// </summary>
        public DataWriter()
        {
        }

        /// <summary>
        /// Creates a datawriter for a specific symbol on todays date. auto-creates header
        /// </summary>
        /// <param name="realsymbol"></param>
        public DataWriter(string realsymbol)
            : this(realsymbol, Util.ToQLDate(DateTime.Now))
        {
        }

        /// <summary>
        /// Creates a datawriter for specific symbol on specific date auto-creates header
        /// </summary>
        /// <param name="realsymbol"></param>
        /// <param name="date"></param>
        public DataWriter(string realsymbol, int date)
            : this(Directory.GetCurrentDirectory(), realsymbol, date, $"t1")
        {
        }

        /// <summary>
        /// Creates a datawriter with specific location, ticker symbol and date. auto-creates header
        /// </summary>
        /// <param name="path"></param>
        /// <param name="realsymbol"></param>
        /// <param name="date"></param>
        /// <param name="aggregation"></param>
        /// <param name="datatype"></param>
        public DataWriter(string path, string realsymbol, int date, string aggregation, DataType datatype = DataType.Unknown) =>
            Init(realsymbol, date, datatype, aggregation, path);

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the type of the data.
        /// </summary>
        public DataType DataType => _datattype;

        /// <summary>
        /// date represented by data
        /// </summary>
        public int Date => _date;

        /// <summary>
        /// path of this file
        /// </summary>
        public string Filepath => _file;

        /// <summary>
        /// path of this folder
        /// </summary>
        public string FolderPath
        {
            get => _path;
            set => _path = value;
        }

        /// <summary>
        /// real symbol represented by tick file
        /// </summary>
        public string RealSymbol => _realsymbol;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// write header for data file
        /// </summary>
        /// <param name="bw"></param>
        /// <param name="realsymbol"></param>
        /// <returns></returns>
        public static bool Header(DataWriter bw, string realsymbol)
        {
            //New output stream
            bw.OutStream = new FileStream(bw.Filepath, FileMode.Create, FileAccess.Write, FileShare.Read);

            // version
            bw.Write(DataConst.FileVersion);
            bw.Write(DataConst.FileCurrentVersion);

            // full symbol name
            bw.Write(realsymbol);

            // fields end
            bw.Write(DataConst.StartData);

            // flag header as created
            bw._hasheader = true;

            //Set header added succeeded
            return true;
        }

        /// <summary>
        /// Feeds this data writer with new input data
        /// </summary>
        /// <param name="datapoint"></param>
        public void Feed(DataPoint datapoint)
        {
            // make sure we have a header
            if (!_hasheader)
            {
                //Get date
                int date = Util.ToQLDate(datapoint.Occured);

                //Init header
                Init(datapoint.Ticker.Name, date, _datattype, "", _path);
            }

            //Start of content
            Write(DataConst.DatapointStart);

            //Content
            Write(datapoint.Serialize());

            //End of content
            Write(DataConst.DatapointEnd);

            //write to disk
            Flush();

            //count it
            Count++;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Initialization method
        /// </summary>
        /// <param name="realticker"></param>
        /// <param name="date"></param>
        /// <param name="datatype"></param>
        /// <param name="aggregation"></param>
        /// <param name="path"></param>
        private void Init(string realticker, int date, DataType datatype, string aggregation, string path)
        {
            // store important stuff
            _realsymbol = realticker;
            _path = path;
            _date = date;
            _datattype = datatype;

            // get filename from path and symbol
            _file = DataUtil.GetPathFileName(_path, datatype, aggregation, date, realticker);

            // if file exists, assume it has a header
            _hasheader = File.Exists(_file);

            if (!_hasheader)
            {
                //Create file and directory
                Directory.CreateDirectory(new FileInfo(_file).Directory.FullName);

                //Add header
                Header(this, realticker);
            }
            else
            {
                OutStream = new FileStream(_file, FileMode.Open, FileAccess.Write, FileShare.Read);
                OutStream.Position = OutStream.Length;
            }
        }

        #endregion Private Methods
    }
}