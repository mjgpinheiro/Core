#region License Header

/*
*
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
*
*/

#endregion License Header

using System.IO;
using System.IO.Compression;

namespace Quantler.Compression
{
    /// <summary>
    /// Compression helper function
    /// </summary>
    public static class Compress
    {
        #region Public Methods

        /// <summary>
        /// Compress bytes in memory
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static byte[] CompressBytes(byte[] bytes, string name)
        {
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                dstream.Write(bytes, 0, bytes.Length);
            }
            return output.ToArray();
        }

        /// <summary>
        /// Uncompress bytes set in compressed bytes
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static byte[] UncompressBytes(byte[] bytes, string name)
        {
            MemoryStream input = new MemoryStream(bytes);
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }

        #endregion Public Methods
    }
}