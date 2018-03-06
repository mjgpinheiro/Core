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
#endregion

using FluentAssertions;
using Quantler.Data.Bars;
using Xunit;

namespace Quantler.Tests.Core.Data.Bars
{
    /// <summary>
    /// TODO: also test serialize and deserialize
    /// </summary>
    public class QuoteBarTests
    {
        [Fact(Skip = "Check, is bugged")]
        [Trait("Quantler.Core.Data.Bars", "QuoteBar")]
        public void QuoteBar_CanCreateCorrectBars()
        {
            var nbar = new QuoteBar();
            nbar.Update(10, 10, 10 , 10, 10, 10);
            nbar.Open.Should().Be(10);
            nbar.High.Should().Be(10);
            nbar.Low.Should().Be(10);
            nbar.Close.Should().Be(10);

            nbar = new QuoteBar();
            nbar.Ask = new BarImpl(11, 11, 11, 11);
            nbar.Open.Should().Be(11);
            nbar.High.Should().Be(11);
            nbar.Low.Should().Be(11);
            nbar.Close.Should().Be(11);

            nbar.Update(12, 12, 12, 1, 1, 1);
            nbar.Open.Should().Be(11);
            nbar.High.Should().Be(12);
            nbar.Low.Should().Be(11);
            nbar.Close.Should().Be(12);
        }
    }
}
