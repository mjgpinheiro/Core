#region License Header

/*
* QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
* Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
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
* Modifications Copyright 2018 Quantler B.V.
*
*/

#endregion License Header

using Quantler.Indicators;
using Xunit;

namespace Quantler.Tests.Indicators
{
    public class MovingAverageTypeExtensionsTests
    {
        #region Public Methods

        [Fact]
        public void CreatesCorrectAveragingIndicator()
        {
            var indicator = MovingAverageType.Simple.AsIndicator(1);
            Assert.IsType<SimpleMovingAverage>(indicator);

            indicator = MovingAverageType.Exponential.AsIndicator(1);
            Assert.IsType<ExponentialMovingAverage>(indicator);

            indicator = MovingAverageType.Wilders.AsIndicator(1);
            Assert.IsType<ExponentialMovingAverage>(indicator);

            indicator = MovingAverageType.LinearWeightedMovingAverage.AsIndicator(1);
            Assert.IsType<LinearWeightedMovingAverage>(indicator);

            indicator = MovingAverageType.DoubleExponential.AsIndicator(1);
            Assert.IsType<DoubleExponentialMovingAverage>(indicator);

            indicator = MovingAverageType.TripleExponential.AsIndicator(1);
            Assert.IsType<TripleExponentialMovingAverage>(indicator);

            indicator = MovingAverageType.Triangular.AsIndicator(1);
            Assert.IsType<TriangularMovingAverage>(indicator);

            indicator = MovingAverageType.T3.AsIndicator(1);
            Assert.IsType<T3MovingAverage>(indicator);

            indicator = MovingAverageType.Kama.AsIndicator(1);
            Assert.IsType<KaufmanAdaptiveMovingAverage>(indicator);

            indicator = MovingAverageType.Hull.AsIndicator(4);
            Assert.IsType<HullMovingAverage>(indicator);

            indicator = MovingAverageType.Alma.AsIndicator(9);
            Assert.IsType<ArnaudLegouxMovingAverage>(indicator);

            string name = string.Empty;
            indicator = MovingAverageType.Simple.AsIndicator(name, 1);
            Assert.IsType<SimpleMovingAverage>(indicator);

            indicator = MovingAverageType.Exponential.AsIndicator(name, 1);
            Assert.IsType<ExponentialMovingAverage>(indicator);

            indicator = MovingAverageType.Wilders.AsIndicator(name, 1);
            Assert.IsType<ExponentialMovingAverage>(indicator);

            indicator = MovingAverageType.LinearWeightedMovingAverage.AsIndicator(name, 1);
            Assert.IsType<LinearWeightedMovingAverage>(indicator);

            indicator = MovingAverageType.DoubleExponential.AsIndicator(name, 1);
            Assert.IsType<DoubleExponentialMovingAverage>(indicator);

            indicator = MovingAverageType.TripleExponential.AsIndicator(name, 1);
            Assert.IsType<TripleExponentialMovingAverage>(indicator);

            indicator = MovingAverageType.Triangular.AsIndicator(name, 1);
            Assert.IsType<TriangularMovingAverage>(indicator);

            indicator = MovingAverageType.T3.AsIndicator(name, 1);
            Assert.IsType<T3MovingAverage>(indicator);

            indicator = MovingAverageType.Kama.AsIndicator(name, 1);
            Assert.IsType<KaufmanAdaptiveMovingAverage>(indicator);

            indicator = MovingAverageType.Hull.AsIndicator(name, 4);
            Assert.IsType<HullMovingAverage>(indicator);

            indicator = MovingAverageType.Alma.AsIndicator(name, 9);
            Assert.IsType<ArnaudLegouxMovingAverage>(indicator);
        }

        #endregion Public Methods
    }
}