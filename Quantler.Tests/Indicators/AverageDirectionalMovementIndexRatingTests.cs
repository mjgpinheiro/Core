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
using System;
using FluentAssertions;
using Quantler.Data.Bars;
using Xunit;

namespace Quantler.Tests.Indicators
{
    public class AverageDirectionalMovementIndexRatingTests : CommonIndicatorTests<DataPointBar>
    {
        #region Protected Properties

        protected override Action<IndicatorBase<DataPointBar>, double> Assertion
        {
            get
            {
                return (indicator, expected) =>
                    ((double) indicator.Current.Price).Should().BeApproximately(expected, 1.0);
            }
        }

        protected override string TestColumnName => "ADXR_14";

        protected override string TestFileName => "spy_adxr.txt";

        #endregion Protected Properties

        #region Protected Methods

        protected override IndicatorBase<DataPointBar> CreateIndicator()
        {
            return new AverageDirectionalMovementIndexRating(14);
        }

        #endregion Protected Methods
    }
}