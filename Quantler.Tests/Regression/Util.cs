using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quantler.Interfaces;
using System.IO;
using Quantler.Interfaces;
using Xunit;
using FluentAssertions;
using Newtonsoft.Json;
using Quantler.Bootstrap;
using Quantler.Performance;

namespace Quantler.Tests.Regression
{
    internal static class Util
    {
        /// <summary>
        /// Tests an indicator by going through the values of the indicator and checking these values from a static file
        /// </summary>
        /// <param name="indicator"></param>
        /// <param name="filename"></param>
        /// <param name="targetColumns"></param>
        /// <param name="valuecheck"></param>
        public static Result BacktestStrategy(PortfolioManager portfolio, string filename)
        {
            //Run backtest
            //LocalBacktester backtester = new LocalBacktester(portfolio, @"", 0);

            //Check and get file
            FileInfo file = new FileInfo(Directory.GetCurrentDirectory() + @"\Backtests\ResultFiles\" + filename + ".csv");
            return null;
        }
    }
}
