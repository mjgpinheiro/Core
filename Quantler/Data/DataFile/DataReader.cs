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
using NLog;

namespace Quantler.Data.DataFile
{
    /// <summary>
    /// read tick files
    /// </summary>
    public sealed class DataReader : BinaryReader
    {
        #region Public Fields

        /// <summary>
        /// count of ticks presently read
        /// </summary>
        public int Count;

        #endregion Public Fields

        #region Private Fields

        private int _fileversion;
        private bool _haveheader;
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Create a data reader pointing to a DAT file on disk
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="filepath">The filepath.</param>
        public DataReader(string ticker, string filepath)
            : base(new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            Ticker = ticker;
            ReadHeader();
        }

        /// <summary>
        /// Create a data reader pointing to an existing stream
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="existingstream">The existingstream.</param>
        public DataReader(string ticker, Stream existingstream)
            : base(existingstream)
        {
            Ticker = ticker;
            existingstream.Position = 0;
            ReadHeader();
        }

        #endregion Public Constructors

        #region Public Events

        /// <summary>
        /// Occurs when [got data].
        /// </summary>
        public event DataReaderHandler GotData;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// file is readable, has version and real ticker symbol
        /// </summary>
        public bool IsValid => (_fileversion != 0) && (RealTicker != string.Empty) && BaseStream.CanRead;

        /// <summary>
        /// real ticker symbol for data represented in file
        /// </summary>
        public string RealTicker { get; private set; } = string.Empty;

        /// <summary>
        /// security-parsed ticker symbol
        /// </summary>
        public string Ticker { get; } = string.Empty;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// returns true if more data to process, false otherwise
        /// </summary>
        /// <returns></returns>
        public bool NextDataPoint()
        {
            //Check if we have header of file
            if (!_haveheader)
                ReadHeader();

            try
            {
                // get line type
                byte linetype = ReadByte();

                // prepare a datapoiny
                DataPoint d;

                // get the tick
                switch (linetype)
                {
                    case DataConst.EndData: return false;
                    case DataConst.StartData: return true;
                    case DataConst.FileVersion: return true;
                    case DataConst.DatapointStart:
                    {
                        //Get data point data
                        List<byte> data = new List<byte>();
                        bool ended = false;
                        while (!ended)
                        {
                            var lastbyte = ReadByte();
                            if (lastbyte == DataConst.DatapointEnd)
                                ended = true;
                            else
                                data.Add(lastbyte);
                        }

                        //Convert data
                        d = DataPointImpl.Deserialize(data.ToArray(), true);
                    }
                        ;
                        break;

                    default:
                    {
                        // weird data, try to keep reading
                        ReadByte();

                        // but don't send this data, just get next record
                        return true;
                    }
                }

                // send any data we have
                if (d != null)
                    GotData?.Invoke(this, d);

                // count it
                Count++;

                // assume there is more
                return true;
            }
            catch (EndOfStreamException)
            {
                return false;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
            catch (Exception exc)
            {
                _log.Error(exc, $"Unexpected error while reading file in datareader: {exc.Message}");
                return false;
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Read file header for DAT file information
        /// </summary>
        private void ReadHeader()
        {
            // get version id (expect to start with this)
            if (ReadByte() != DataConst.FileVersion)
                throw new BadDataFile("Expected file version in header");

            // get version
            _fileversion = ReadInt32();
            if (_fileversion != DataConst.FileCurrentVersion)
                throw new BadDataFile("version: " + _fileversion + " expected: " + DataConst.FileCurrentVersion);

            // get real symbol
            RealTicker = ReadString();

            // get end of header
            ReadByte();

            // make sure we read something
            if (RealTicker.Length <= 0)
                throw new BadDataFile("no symbol defined in data file");

            // flag header as read
            _haveheader = true;
        }

        #endregion Private Methods
    }
}