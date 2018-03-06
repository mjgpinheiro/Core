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
using System.IO;
using System.Linq;
using FluentAssertions;
using MoreLinq;
using Quantler.Configuration;
using Quantler.Configuration.Model;
using Quantler.Interfaces;
using Xunit;

namespace Quantler.Tests.Configuration
{
    public class ConfigTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Config", "Config")]
        public void Config_GlobalConfig()
        {
            //Arrange

            //Act
            var config = Config.GlobalConfig;

            //Assert
            config.DataFeed.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        [Trait("Quantler.Config", "Config")]
        public void Config_LoadBacktestConfig()
        {
            //Arrange

            //Act
            var config = Config.SimulationConfig;

            //Assert
            config.MarketDataFolder.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        [Trait("Quantler.Config", "Config")]
        public void Config_LoadBrokerConfig()
        {
            //Arrange

            //Act
            var config = Config.BrokerConfig;

            //Assert
            config.Length.Should().BePositive();
            config.ForEach(item =>
            {
                item.DataFeed.Should().NotBeNullOrWhiteSpace();
                //item.BrokerConnection.Should().NotBeNullOrWhiteSpace(); //TODO: after broker implementations, check for this setting
                item.BrokerType.Should().NotBeNullOrWhiteSpace();
                item.CurrencyImplementation.Should().NotBeNullOrWhiteSpace();
                item.OrderTicketHandler.Should().NotBeNullOrWhiteSpace();
                item.Exchanges.Length.Should().BePositive();
            });
        }

        [Fact]
        [Trait("Quantler.Config", "Config")]
        public void Config_LoadCurrencyRates()
        {
            //Arrange

            //Act
            var config = Config.CurrencyConfig;

            //Assert
            config.Length.Should().BePositive();
            config.ForEach(item =>
            {
                item.Rates.Count.Should().BePositive();
            });
        }

        [Fact]
        [Trait("Quantler.Config", "Config")]
        public void Config_MarketHoursConfig()
        {
            //Arrange

            //Act
            var config = Config.MarketHourConfig;

            //Assert
            config.Length.Should().BePositive();
            config.ForEach(c =>
            {
                c.TimeZone.Should().NotBeNullOrWhiteSpace();
                c.Type.Should().NotBeNullOrWhiteSpace();
            });
        }

        [Fact]
        [Trait("Quantler.Config", "Config")]
        public void Config_PortfolioConfig()
        {
            //Arrange

            //Act
            var config = Config.PortfolioConfig;

            //Assert
            config.Should().NotBeNull();
        }

        [Fact]
        [Trait("Quantler.Config", "Config")]
        public void Config_DefaultConfigLocation() =>
            new DirectoryInfo(Config.DefaultConfigFolder).Exists.Should().BeTrue();

        [Fact]
        [Trait("Quantler.Config", "Config")]
        public void Config_UserConfigLocation() =>
            Config.DefaultConfigFolder.Should().NotBeNullOrWhiteSpace();

        [Fact]
        [Trait("Quantler.Config", "Config")]
        public void Config_SecurityConfig()
        {
            //Arrange

            //Act
            var config = Config.SecurityConfig;

            //Assert
            config.Should().NotBeNull();
            config.Length.Should().NotBe(0);
        }

        [Fact(Skip = "TODO!")]
        [Trait("Quantler.Config", "Config")]
        public void Config_AppendConfig()
        {
            //Arrange
            var currentitems = Config.CurrencyConfig.ToArray();
            var addeditem = new CurrencyRatesConfig
            {
                Base = "EUR",
                date = DateTime.UtcNow.Date.ToString("yyyy-MM-dd"),
                Date = DateTime.UtcNow.Date,
                Rates = Enum.GetValues(typeof(CurrencyType)).Cast<CurrencyType>().ToDictionary(x => x.ToString(), x => 0.91m)
            };

            //Act
            Config.AppendConfigFile(Config.CurrencyConfig, addeditem);

            //Assert
            Config.CurrencyConfig.Length.Should().BeGreaterThan(currentitems.Length);
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Config", "Config")]
        public void Config_LoadConfig()
        {
            //Arrange

            //Act

            //Assert
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Config", "Config")]
        public void Config_LoadEnvVariable()
        {
            //Arrange

            //Act

            //Assert
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Config", "Config")]
        public void Config_MergeUserConfig()
        {
            //Arrange

            //Act

            //Assert
        }

        #endregion Public Methods
    }
}