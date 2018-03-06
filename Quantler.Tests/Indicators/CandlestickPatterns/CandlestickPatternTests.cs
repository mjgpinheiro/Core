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
using Quantler.Indicators.CandlestickPatterns;
using System;
using System.Collections.Generic;
using Quantler.Data.Bars;
using Xunit;
using System.Collections;

namespace Quantler.Tests.Indicators.CandlestickPatterns
{
    public class CandlestickPatternTestData : IEnumerable<object[]>
    {
        private static readonly string[] _testFileNames =
        {
            "spy_candle_patterns.txt", "ewz_candle_patterns.txt", "eurusd_candle_patterns.txt"
        };

        public IEnumerator<object[]> GetEnumerator() => GetTestCases().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetTestCases().GetEnumerator();

        public static IEnumerable<object[]> GetTestCases()
        {
            foreach (var testFileName in _testFileNames)
            {
                yield return new object[] { new TwoCrows(), "CDL2CROWS", testFileName, "TwoCrows-" + testFileName };
                yield return new object[] { new ThreeBlackCrows(), "CDL3BLACKCROWS", testFileName, "ThreeBlackCrows-" + testFileName };
                yield return new object[] { new ThreeInside(), "CDL3INSIDE", testFileName, "ThreeInside-" + testFileName };
                yield return new object[] { new ThreeLineStrike(), "CDL3LINESTRIKE", testFileName, "ThreeLineStrike-" + testFileName };
                yield return new object[] { new ThreeOutside(), "CDL3OUTSIDE", testFileName, "ThreeOutside-" + testFileName };
                yield return new object[] { new ThreeStarsInSouth(), "CDL3STARSINSOUTH", testFileName, "ThreeStarsInSouth-" + testFileName };
                yield return new object[] { new ThreeWhiteSoldiers(), "CDL3WHITESOLDIERS", testFileName, "ThreeWhiteSoldiers-" + testFileName };
                yield return new object[] { new AbandonedBaby(), "CDLABANDONEDBABY", testFileName, "AbandonedBaby-" + testFileName };
                yield return new object[] { new AdvanceBlock(), "CDLADVANCEBLOCK", testFileName, "AdvanceBlock-" + testFileName };
                yield return new object[] { new BeltHold(), "CDLBELTHOLD", testFileName, "BeltHold-" + testFileName };
                yield return new object[] { new Breakaway(), "CDLBREAKAWAY", testFileName, "Breakaway-" + testFileName };
                yield return new object[] { new ClosingMarubozu(), "CDLCLOSINGMARUBOZU", testFileName, "ClosingMarubozu-" + testFileName };
                yield return new object[] { new ConcealedBabySwallow(), "CDLCONCEALBABYSWALL", testFileName, "ConcealedBabySwallow-" + testFileName };
                yield return new object[] { new Counterattack(), "CDLCOUNTERATTACK", testFileName, "Counterattack-" + testFileName };
                yield return new object[] { new DarkCloudCover(), "CDLDARKCLOUDCOVER", testFileName, "DarkCloudCover-" + testFileName };
                yield return new object[] { new Doji(), "CDLDOJI", testFileName, "Doji-" + testFileName };
                yield return new object[] { new DojiStar(), "CDLDOJISTAR", testFileName, "DojiStar-" + testFileName };
                yield return new object[] { new DragonflyDoji(), "CDLDRAGONFLYDOJI", testFileName, "DragonflyDoji-" + testFileName };
                yield return new object[] { new Engulfing(), "CDLENGULFING", testFileName, "Engulfing-" + testFileName };
                yield return new object[] { new EveningDojiStar(), "CDLEVENINGDOJISTAR", testFileName, "EveningDojiStar-" + testFileName };
                yield return new object[] { new EveningStar(), "CDLEVENINGSTAR", testFileName, "EveningStar-" + testFileName };
                yield return new object[] { new GapSideBySideWhite(), "CDLGAPSIDESIDEWHITE", testFileName, "GapSideBySideWhite-" + testFileName };
                yield return new object[] { new GravestoneDoji(), "CDLGRAVESTONEDOJI", testFileName, "GravestoneDoji-" + testFileName };
                yield return new object[] { new Hammer(), "CDLHAMMER", testFileName, "Hammer-" + testFileName };
                yield return new object[] { new HangingMan(), "CDLHANGINGMAN", testFileName, "HangingMan-" + testFileName };
                yield return new object[] { new Harami(), "CDLHARAMI", testFileName, "Harami-" + testFileName };
                yield return new object[] { new HaramiCross(), "CDLHARAMICROSS", testFileName, "HaramiCross-" + testFileName };

                if (testFileName.Contains("ewz"))
                {
                    // Lean uses decimals while TA-lib uses doubles, so this test only passes with the ewz test file
                    yield return new object[] { new HighWaveCandle(), "CDLHIGHWAVE", testFileName, "HighWaveCandle-" + testFileName };
                }
                yield return new object[] { new Hikkake(), "CDLHIKKAKE", testFileName, "Hikkake-" + testFileName };
                if (testFileName.Contains("spy"))
                {
                    // Lean uses decimals while TA-lib uses doubles, so this test only passes with the spy test file
                    yield return new object[] { new HikkakeModified(), "CDLHIKKAKEMOD", testFileName, "HikkakeModified-" + testFileName };
                }
                yield return new object[] { new HomingPigeon(), "CDLHOMINGPIGEON", testFileName, "HomingPigeon-" + testFileName };
                yield return new object[] { new IdenticalThreeCrows(), "CDLIDENTICAL3CROWS", testFileName, "IdenticalThreeCrows-" + testFileName };
                yield return new object[] { new InNeck(), "CDLINNECK", testFileName, "InNeck-" + testFileName };
                yield return new object[] { new InvertedHammer(), "CDLINVERTEDHAMMER", testFileName, "InvertedHammer-" + testFileName };
                yield return new object[] { new Kicking(), "CDLKICKING", testFileName, "Kicking-" + testFileName };
                yield return new object[] { new KickingByLength(), "CDLKICKINGBYLENGTH", testFileName, "KickingByLength-" + testFileName };
                yield return new object[] { new LadderBottom(), "CDLLADDERBOTTOM", testFileName, "LadderBottom-" + testFileName };
                yield return new object[] { new LongLeggedDoji(), "CDLLONGLEGGEDDOJI", testFileName, "LongLeggedDoji-" + testFileName };
                yield return new object[] { new LongLineCandle(), "CDLLONGLINE", testFileName, "LongLineCandle-" + testFileName };
                yield return new object[] { new Marubozu(), "CDLMARUBOZU", testFileName, "Marubozu-" + testFileName };
                yield return new object[] { new MatchingLow(), "CDLMATCHINGLOW", testFileName, "MatchingLow-" + testFileName };
                yield return new object[] { new MatHold(), "CDLMATHOLD", testFileName, "MatHold-" + testFileName };
                yield return new object[] { new MorningDojiStar(), "CDLMORNINGDOJISTAR", testFileName, "MorningDojiStar-" + testFileName };
                if (!testFileName.Contains("eurusd"))
                {
                    // Lean uses decimals while TA-lib uses doubles, so this test does not pass with the eurusd test file
                    yield return new object[] { new MorningStar(), "CDLMORNINGSTAR", testFileName, "MorningStar-" + testFileName };
                }
                yield return new object[] { new OnNeck(), "CDLONNECK", testFileName, "OnNeck-" + testFileName };
                yield return new object[] { new Piercing(), "CDLPIERCING", testFileName, "Piercing-" + testFileName };
                if (!testFileName.Contains("spy"))
                {
                    // Lean uses decimals while TA-lib uses doubles, so this test does not pass with the spy test file
                    yield return new object[] { new RickshawMan(), "CDLRICKSHAWMAN", testFileName, "RickshawMan-" + testFileName };
                }
                yield return new object[] { new RiseFallThreeMethods(), "CDLRISEFALL3METHODS", testFileName, "RiseFallThreeMethods-" + testFileName };
                yield return new object[] { new SeparatingLines(), "CDLSEPARATINGLINES", testFileName, "SeparatingLines-" + testFileName };
                yield return new object[] { new ShootingStar(), "CDLSHOOTINGSTAR", testFileName, "ShootingStar-" + testFileName };
                if (!testFileName.Contains("spy"))
                {
                    // Lean uses decimals while TA-lib uses doubles, so this test does not pass with the spy test file
                    yield return new object[] { new ShortLineCandle(), "CDLSHORTLINE", testFileName, "ShortLineCandle-" + testFileName };
                }
                if (!testFileName.Contains("spy"))
                {
                    // Lean uses decimals while TA-lib uses doubles, so this test does not pass with the spy test file
                    yield return new object[] { new SpinningTop(), "CDLSPINNINGTOP", testFileName, "SpinningTop-" + testFileName };
                }
                yield return new object[] { new StalledPattern(), "CDLSTALLEDPATTERN", testFileName, "StalledPattern-" + testFileName };
                if (testFileName.Contains("ewz"))
                {
                    // Lean uses decimals while TA-lib uses doubles, so this test only passes with the ewz test file
                    yield return new object[] { new StickSandwich(), "CDLSTICKSANDWICH", testFileName, "StickSandwich-" + testFileName };
                }
                yield return new object[] { new Takuri(), "CDLTAKURI", testFileName, "Takuri-" + testFileName };
                yield return new object[] { new TasukiGap(), "CDLTASUKIGAP", testFileName, "TasukiGap-" + testFileName };
                yield return new object[] { new Thrusting(), "CDLTHRUSTING", testFileName, "Thrusting-" + testFileName };
                yield return new object[] { new Tristar(), "CDLTRISTAR", testFileName, "Tristar-" + testFileName };
                yield return new object[] { new UniqueThreeRiver(), "CDLUNIQUE3RIVER", testFileName, "UniqueThreeRiver-" + testFileName };
                yield return new object[] { new UpsideGapTwoCrows(), "CDLUPSIDEGAP2CROWS", testFileName, "UpsideGapTwoCrows-" + testFileName };
                yield return new object[] { new UpDownGapThreeMethods(), "CDLXSIDEGAP3METHODS", testFileName, "UpDownGapThreeMethods-" + testFileName };
            }
        }
    }

    public class CandlestickPatternTests
    {
        #region Private Properties

        private static Action<IndicatorBase<DataPointBar>, double> Assertion
        {
            get
            {
                return (indicator, expected) =>
                {
                    // Trace line for debugging
                    // Console.WriteLine(indicator.Current.EndTime + "\t" + expected + "\t" + indicator.Current.Value * 100);

                    Assert.Equal(expected, (double)indicator.Current.Price * 100);
                };
            }
        }

        #endregion Private Properties

        #region Public Methods

        [Theory]
        [MemberData(nameof(CandlestickPatternTestData.GetTestCases), MemberType = typeof(CandlestickPatternTestData))]
        [Trait("Quantler.Indicators", "CandlestickPattern")]
        public void ComparesAgainstExternalData(IndicatorBase<DataPointBar> indicator, string columnName, string testFileName, string testname)
        {
            TestHelper.TestIndicator(indicator, testFileName, columnName, Assertion);
        }

        [Theory]
        [MemberData(nameof(CandlestickPatternTestData.GetTestCases), MemberType = typeof(CandlestickPatternTestData))]
        [Trait("Quantler.Indicators", "CandlestickPattern")]
        public void ComparesAgainstExternalDataAfterReset(CandlestickPattern indicator, string columnName, string testFileName, string testname)
        {
            TestHelper.TestIndicator(indicator, testFileName, columnName, Assertion);
            indicator.Reset();
            TestHelper.TestIndicator(indicator, testFileName, columnName, Assertion);
        }

        [Theory]
        [MemberData(nameof(CandlestickPatternTestData.GetTestCases), MemberType = typeof(CandlestickPatternTestData))]
        [Trait("Quantler.Indicators", "CandlestickPattern")]
        public void ResetsProperly(CandlestickPattern indicator, string columnName, string testFileName, string testname)
        {
            TestHelper.TestIndicatorReset(indicator, testFileName);
        }

        #endregion Public Methods
    }
}