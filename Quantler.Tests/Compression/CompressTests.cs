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

using FluentAssertions;
using Quantler.Compression;
using System.Text;
using Xunit;

namespace Quantler.Tests.Compression
{
    /// <summary>
    /// Compression unit tests
    /// </summary>
    public class CompressTests
    {
        #region Public Properties

        /// <summary>
        /// Gets the input.
        /// </summary>
        public string Input => "hIQmIdtXuAzwRxEIqZSahIQmIdtXuAzwRxEIqZSahIQmIdtXuAzwRxEIqZSahIQmIdtXuAzwRxEIqZSahIQmIdtXuAzwRxEIqZSahIQmIdtXuAzwRxEIqZSahIQmIdtXuAzwRxEIqZSahIQmIdtXuAzwRxEIqZSa";

        #endregion Public Properties

        #region Public Methods

        [Fact]
        [Trait("Quantler.Compression", "Compress")]
        //Check compression
        public void QuantlerCompression_Compress_Succeeded()
        {
            //Arrange
            var inputdata = Encoding.ASCII.GetBytes(Input);

            //Act
            var result = Compress.CompressBytes(inputdata, "testing");

            //Assert
            result.Should().NotBeNull();
            result.Length.Should().BePositive();
            result.Length.Should().BeLessThan(inputdata.Length);
        }

        [Fact]
        [Trait("Quantler.Compression", "Compress")]
        //Check uncompress
        public void QuantlerCompression_Uncompress_Succeeded()
        {
            //Arrange
            var inputdata = Encoding.ASCII.GetBytes(Input);
            var inputcompressed = Compress.CompressBytes(inputdata, "testing");

            //Act
            var decompressed = Compress.UncompressBytes(inputcompressed, "testing");
            var result = Encoding.ASCII.GetString(decompressed);

            //Assert
            decompressed.Should().NotBeNull();
            result.Should().NotBeNullOrWhiteSpace();
            result.Should().Be(Input);
        }

        #endregion Public Methods
    }
}