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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentAssertions;
using Xunit;

namespace Quantler.Tests.Indicators
{
    /// <summary>
    ///     Test class for Quantler.Indicators.Indicator
    /// </summary>
    public class IndicatorTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "Indicator")]
        public void ComparisonFunctions()
        {
            TestComparisonOperators<int>();
            TestComparisonOperators<long>();
            TestComparisonOperators<float>();
            TestComparisonOperators<double>();
        }

        [Fact]
        [Trait("Quantler.Indicators", "Indicator")]
        public void NameSaves()
        {
            // just testing that we get the right name out
            const string name = "name";
            var target = new TestIndicator(name);
            Assert.Equal(name, target.Name);
        }

        [Fact]
        [Trait("Quantler.Indicators", "Indicator")]
        public void PassesOnDuplicateTimes()
        {
            var target = new TestIndicator();

            var time = DateTime.UtcNow;

            const decimal value1 = 1m;
            var data = new IndicatorDataPoint(time, TimeZone.Utc, value1);
            target.Update(data);
            Assert.Equal(value1, target.Current.Price);

            // this won't update because we told it to ignore duplicate
            // data based on time
            target.Update(data);
            Assert.Equal(value1, target.Current.Price);
        }

        [Fact]
        [Trait("Quantler.Indicators", "Indicator")]
        public void SortsTheSameAsDecimalAsecending()
        {
            int count = 100;
            var targets = Enumerable.Range(0, count).Select(x => new TestIndicator(x.ToString())).ToList();
            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].Update(DateTime.Today, TimeZone.Utc, i);
            }

            var expected = Enumerable.Range(0, count).Select(x => (decimal)x).OrderBy(x => x).ToList();
            var actual = targets.OrderBy(x => x).ToList();
            foreach (var pair in expected.Zip<decimal, TestIndicator, Tuple<decimal, TestIndicator>>(actual, Tuple.Create))
            {
                Assert.Equal(pair.Item1, pair.Item2.Current.Price);
            }
        }

        [Fact]
        [Trait("Quantler.Indicators", "Indicator")]
        public void SortsTheSameAsDecimalDescending()
        {
            int count = 100;
            var targets = Enumerable.Range(0, count).Select(x => new TestIndicator(x.ToString())).ToList();
            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].Update(DateTime.Today, TimeZone.Utc, i);
            }

            var expected = Enumerable.Range(0, count).Select(x => (decimal)x).OrderByDescending(x => x).ToList();
            var actual = targets.OrderByDescending(x => x).ToList();
            foreach (var pair in expected.Zip<decimal, TestIndicator, Tuple<decimal, TestIndicator>>(actual, Tuple.Create))
            {
                Assert.Equal(pair.Item1, pair.Item2.Current.Price);
            }
        }

        [Fact]
        [Trait("Quantler.Indicators", "Indicator")]
        public void ThrowsOnPastTimes()
        {
            var target = new TestIndicator();

            var time = DateTime.UtcNow;

            target.Update(new IndicatorDataPoint(time, TimeZone.Utc, 1m));

            Action act = () => target.Update(new IndicatorDataPoint(time.AddMilliseconds(-1), TimeZone.Utc, 2m));
            act.ShouldThrow<ArgumentException>()
                .Where(x => x.Message.StartsWith("This is a forward only indicator:"));
        }

        [Fact]
        [Trait("Quantler.Indicators", "Indicator")]
        public void UpdatesProperly()
        {
            // we want to make sure the initialized value is the default value
            // for a datapoint, and also verify the our indicator updates as we
            // expect it to, in this case, it should return identity
            var target = new TestIndicator();

            Assert.Equal(DateTime.MinValue, target.Current.Occured);
            Assert.Equal(0m, target.Current.Price);

            var time = DateTime.UtcNow;
            var data = new IndicatorDataPoint(time, TimeZone.Utc, 1m);

            target.Update(data);
            Assert.Equal(1m, target.Current.Price);

            target.Update(new IndicatorDataPoint(time.AddMilliseconds(1), TimeZone.Utc, 2m));
            Assert.Equal(2m, target.Current.Price);
        }

        #endregion Public Methods

        #region Private Methods

        private static MethodInfo GetOperatorMethodInfo<T>(string @operator, int argIndex)
        {
            var methodName = "op_" + @operator;
            var method =
                typeof(IndicatorBase<IndicatorDataPoint>).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .SingleOrDefault(x => x.Name == methodName && x.GetParameters()[argIndex].ParameterType == typeof(T));

            if (method == null)
                throw new Exception("Failed to find method for " + @operator + " of type " + typeof(T).Name + " at index: " + argIndex);

            return method;
        }

        private static void TestComparisonOperators<TValue>()
        {
            var indicator = new TestIndicator();
            TestOperator(indicator, default(TValue), "GreaterThan", true, false);
            TestOperator(indicator, default(TValue), "GreaterThan", false, false);
            TestOperator(indicator, default(TValue), "GreaterThanOrEqual", true, true);
            TestOperator(indicator, default(TValue), "GreaterThanOrEqual", false, true);
            TestOperator(indicator, default(TValue), "LessThan", true, false);
            TestOperator(indicator, default(TValue), "LessThan", false, false);
            TestOperator(indicator, default(TValue), "LessThanOrEqual", true, true);
            TestOperator(indicator, default(TValue), "LessThanOrEqual", false, true);
            TestOperator(indicator, default(TValue), "Equality", true, true);
            TestOperator(indicator, default(TValue), "Equality", false, true);
            TestOperator(indicator, default(TValue), "Inequality", true, false);
            TestOperator(indicator, default(TValue), "Inequality", false, false);
        }

        private static void TestOperator<TIndicator, TValue>(TIndicator indicator, TValue value, string opName, bool tvalueIsFirstParm, bool expected)
        {
            var method = GetOperatorMethodInfo<TValue>(opName, tvalueIsFirstParm ? 0 : 1);
            var ctIndicator = Expression.Constant(indicator);
            var ctValue = Expression.Constant(value);
            var call = tvalueIsFirstParm ? Expression.Call(method, ctValue, ctIndicator) : Expression.Call(method, ctIndicator, ctValue);
            var lamda = Expression.Lambda<Func<bool>>(call);
            var func = lamda.Compile();
            Assert.Equal(expected, func());
        }

        #endregion Private Methods

        #region Private Classes

        private class TestIndicator : Indicator
        {
            #region Public Constructors

            /// <summary>
            ///     Initializes a new instance of the Indicator class using the specified name.
            /// </summary>
            /// <param name="name">The name of this indicator</param>
            public TestIndicator(string name)
                : base(name)
            {
            }

            /// <summary>
            ///     Initializes a new instance of the Indicator class using the name "test"
            /// </summary>
            public TestIndicator()
                : base("test")
            {
            }

            #endregion Public Constructors

            #region Public Properties

            /// <summary>
            ///     Gets a flag indicating when this indicator is ready and fully initialized
            /// </summary>
            public override bool IsReady => true;

            #endregion Public Properties

            #region Protected Methods

            /// <summary>
            ///     Computes the next value of this indicator from the given state
            /// </summary>
            /// <param name="input">The input given to the indicator</param>
            /// <returns>A new value for this indicator</returns>
            protected override decimal ComputeNextValue(IndicatorDataPoint input)
            {
                return input;
            }

            #endregion Protected Methods
        }

        #endregion Private Classes
    }
}