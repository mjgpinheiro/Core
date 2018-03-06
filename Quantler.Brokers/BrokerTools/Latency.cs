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
using System.Diagnostics;
using System.Net.Sockets;

namespace Quantler.Brokers.BrokerTools
{
    /// <summary>
    /// Helper class for measuring latency between instance and broker
    /// </summary>
    public static class Latency
    {
        #region Public Methods

        /// <summary>
        /// Perform a latency test to a specific server and port (sets up a tcp connection and uses this time to measure latency)
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <param name="tests"></param>
        /// <returns></returns>
        public static List<int> GetTCPLatency(string server, int port, int tests = 4)
        {
            //Set return times
            List<int> times = new List<int>();

            for (int i = 0; i < tests; i++)
            {
                //Create socket connection
                using (var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    sock.Blocking = true;

                    //Start stopwatch
                    var stopwatch = new Stopwatch();

                    // Measure the Connect call only
                    stopwatch.Start();
                    try
                    {
                        sock.Connect(server, port);
                    }
                    catch
                    {
                        //No connection could be made, abort
                        break;
                    }
                    stopwatch.Stop();

                    int r = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
                    times.Add(r);
                }
            }

            //Return data
            return times;
        }

        #endregion Public Methods
    }
}