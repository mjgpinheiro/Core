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
using MessagePack;
using Quantler.Compression;
using Quantler.Data;
using Quantler.Data.Market;
using Quantler.Interfaces;
using Quantler.Securities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Quantler.Tests.Compression
{
    public class ArchiveTests
    {
        #region Public Properties

        /// <summary>
        /// Gets the available files.
        /// </summary>
        public IEnumerable<FileInfo> AvailableFiles => Directory
            .GetFiles(CurrentDirectory.FullName).Select(x => new FileInfo(x));

        /// <summary>
        /// Gets the current directory.
        /// </summary>
        public DirectoryInfo CurrentDirectory => new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "Compression", "TestFiles"));

        #endregion Public Properties

        #region Public Methods

        [Fact]
        [Trait("Quantler.Compression", "Archive")]
        public void Archive_AppendEntry()
        {
            //Arrange
            string archive = AvailableFiles.First(x => x.Name == "Archived.zip").FullName;
            var input = new FileInfo(Path.Combine(CurrentDirectory.FullName, "Append.zip"));
            File.Copy(archive, input.FullName, true);

            var ntick1 = new Tick(new TickerSymbol("Testing", "Testing", CurrencyType.AUD), DataSource.IEX)
            {
                TimeZone = TimeZone.Utc,
                AskSize = 12,
                AskPrice = 123.1231m,
                AskSource = DataSource.Binance,
                BidSize = 231213,
                BidPrice = 1231.123123m,
                BidSource = DataSource.Binance,
                DataType = DataType.Tick,
                Depth = 0,
                EndTime = DateTime.MaxValue,
                IsBackfilling = false,
                Occured = DateTime.MaxValue,
                Price = 12313.123123m,
                Size = 12313,
                Source = DataSource.Binance,
                TradePrice = 123123.1231m
            };

            var currentcount = Archive.GetEntries(input.FullName).Count;

            //Act
            var result = Archive.Append(input.FullName, "Testing.DAT", LZ4MessagePackSerializer.Serialize(ntick1));

            //Assert
            result.Should().BeTrue();
            Archive.GetEntries(input.FullName).Count.Should().BeGreaterThan(currentcount);
            Archive.GetEntries(input.FullName).Should().ContainKey("Testing.DAT");
        }

        [Fact]
        [Trait("Quantler.Compression", "Archive")]
        public void Archive_ArchiveDirectory()
        {
            //Arrange
            string archive = Path.Combine(CurrentDirectory.FullName, "Archived.zip");
            var destination = Path.Combine(CurrentDirectory.FullName, "ExtractAll");
            var destinationarchive = new FileInfo(Path.Combine(CurrentDirectory.FullName, "archiveddirectory.zip"));

            //Remove any left-overs from a last test run
            if (Directory.Exists(destination))
                Directory.Delete(destination, true);
            if(destinationarchive.Exists)
                File.Delete(destinationarchive.FullName);

            var allfiles = Archive.Extract(archive, destination, true);

            //Act
            var archiveddirectory = Archive.Directory(destination, CurrentDirectory.FullName, "archiveddirectory.zip");

            //Assert
            File.Exists(archiveddirectory).Should().BeTrue();
            Archive.IsValidArchive(archiveddirectory).Should().BeTrue();
            Archive.GetEntries(archiveddirectory).Count.Should().BePositive();
        }

        [Fact]
        [Trait("Quantler.Compression", "Archive")]
        public void Archive_Extract_All()
        {
            //Arrange
            string archive = Path.Combine(CurrentDirectory.FullName, "Archived.zip");
            var destination = Path.Combine(CurrentDirectory.FullName, "ExtractAll");

            //Remove any left-overs from a last test run
            if (Directory.Exists(destination))
                Directory.Delete(destination, true);

            //Act
            var result = Archive.Extract(archive, destination, true);

            //Assert
            result.Length.Should().BePositive();
            Directory.Exists(destination).Should().BeTrue();
        }

        [Fact]
        [Trait("Quantler.Compression", "Archive")]
        public void Archive_Extract_Entries()
        {
            //Arrange
            string archive = Path.Combine(CurrentDirectory.FullName, "Archived.zip");
            var entries = new[] { "BTCLRC.DAT", "BTCSNC.DAT", "USDEMGO.DAT", "BADENTRY.DAT" };
            var destination = Path.Combine(CurrentDirectory.FullName, "Entries");

            //Remove any left-overs from a last test run
            if (Directory.Exists(destination))
                Directory.Delete(destination, true);

            //Act
            var result = Archive.Extract(archive, destination, true, entries);

            //Assert
            result.Length.Should().BePositive();
            Directory.Exists(destination).Should().BeTrue();
            Directory.GetFiles(destination).Select(x => new FileInfo(x)).Should().OnlyContain(x => entries.Contains(x.Name) | x.Name == "BADENTRY.DAT");
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Compression", "Archive")]
        public void Archive_ExtractGz_Succeeded()
        {
            //Arrange

            //Act

            //Assert
        }

        [Fact]
        [Trait("Quantler.Compression", "Archive")]
        public void Archive_GetEntries()
        {
            //Arrange

            //Act
            var result = Archive.GetEntries(AvailableFiles.First(x => x.Name == "Archived.zip").FullName);

            //Assert
            result.Should().ContainKeys("BTCLRC.DAT", "BTCSNC.DAT", "USDEMGO.DAT", "USDTNT.DAT", "USDVEN.DAT", "USDXDN.DAT");
        }

        [Fact]
        [Trait("Quantler.Compression", "Archive")]
        public void Archive_IsValidArchive()
        {
            //Arrange

            //Act
            var result = Archive.IsValidArchive(AvailableFiles.First(x => x.Name == "Archived.zip").FullName);

            //Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("BTCLRC.DAT", true)]
        [InlineData("BTCSNC.DAT", true)]
        [InlineData("BTTTT.DAT", false)]
        [Trait("Quantler.Compression", "Archive")]
        public void Archive_ReadEntry(string entry, bool expected)
        {
            //Arrange
            string archive = AvailableFiles.First(x => x.Name == "Archived.zip").FullName;

            //Act
            var result = Archive.Read(archive, entry);

            //Assert
            if (expected)
            {
                result.Should().NotBeNull();
                result.Length.Should().BePositive();
            }
            else
                result.Should().BeNull();
        }

        [Fact]
        [Trait("Quantler.Compression", "Archive")]
        public void Archive_Store_Succeeded()
        {
            //Arrange

            //Copy current archive for testing
            string archive = Path.Combine(CurrentDirectory.FullName, "Store.Archived.Test.zip");

            //Check if file already exists
            if (File.Exists(archive))
                File.Delete(archive);

            var ntick1 = new Tick(new TickerSymbol("Testing", "Testing", CurrencyType.AUD), DataSource.IEX)
            {
                TimeZone = TimeZone.Utc,
                AskSize = 12,
                AskPrice = 123.1231m,
                AskSource = DataSource.Binance,
                BidSize = 231213,
                BidPrice = 1231.123123m,
                BidSource = DataSource.Binance,
                DataType = DataType.Tick,
                Depth = 0,
                EndTime = DateTime.MaxValue,
                IsBackfilling = false,
                Occured = DateTime.MaxValue,
                Price = 12313.123123m,
                Size = 12313,
                Source = DataSource.Binance,
                TradePrice = 123123.1231m
            };
            var ntick2 = new Tick(new TickerSymbol("Testing", "Testing", CurrencyType.AUD), DataSource.IEX)
            {
                TimeZone = TimeZone.Utc,
                AskSize = 12,
                AskPrice = 123.1231m,
                AskSource = DataSource.Binance,
                BidSize = 231213,
                BidPrice = 1231.123123m,
                BidSource = DataSource.Binance,
                DataType = DataType.Tick,
                Depth = 0,
                EndTime = DateTime.MaxValue,
                IsBackfilling = false,
                Occured = DateTime.MaxValue,
                Price = 12313.123123m,
                Size = 12313,
                Source = DataSource.Binance,
                TradePrice = 123123.1231m
            };

            var input = new Dictionary<string, byte[]>()
            {
                {"File1.DAT", LZ4MessagePackSerializer.Serialize(ntick1) },
                {"File2.DAT", LZ4MessagePackSerializer.Serialize(ntick2) }
            };

            //Act
            var result = Archive.Store(archive, input);

            //Assert
            result.Should().Be(2);
            File.Exists(archive).Should().BeTrue();
            Archive.IsValidArchive(archive).Should().BeTrue();
        }

        #endregion Public Methods
    }
}