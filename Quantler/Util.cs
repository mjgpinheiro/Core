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
using System.ComponentModel;
using System.Net.Sockets;
using System.Reflection;

namespace Quantler
{
    /// <summary>
    /// Utility class holding commonly used properties
    /// </summary>
    public class Util
    {
        #region Public Methods

        /// <summary>
        /// To check if the port is opened
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool CheckPortAvailability(int port)
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                try
                {
                    tcpClient.ConnectAsync("127.0.0.1", port).Wait();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Divide two decimals even if y is zero
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static decimal SafeDivide(decimal x, decimal y) => y == 0 ? 0 : x / y;

        /// <summary>
        /// Converts a DateTime to Quantler Date (eg July 11, 2006 = 20060711)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int ToQLDate(DateTime dt) =>
            dt.Year * 10000 + dt.Month * 100 + dt.Day;

        /// <summary>
        /// Gets the enum description.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        #endregion Public Methods
    }
}