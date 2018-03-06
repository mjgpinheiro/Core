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

namespace Quantler.Indicators
{
    public abstract partial class IndicatorBase<T>
    {
        #region Public Methods

        /// <summary>
        /// Returns the current Price of this instance
        /// </summary>
        /// <param name="instance">The indicator instance</param>
        /// <returns>The current Price of the indicator</returns>
        public static implicit operator decimal(IndicatorBase<T> instance)
        {
            return instance.Current;
        }

        /// <summary>
        /// Determines if the indicator's current Price is not equal to the specified Price
        /// </summary>
        public static bool operator !=(IndicatorBase<T> left, double right)
        {
            if (ReferenceEquals(left, null)) return true;
            return left.Current.Price != (decimal)right;
        }

        /// <summary>
        /// Determines if the specified Price is not equal to the indicator's current Price
        /// </summary>
        public static bool operator !=(double left, IndicatorBase<T> right)
        {
            if (ReferenceEquals(right, null)) return true;
            return (decimal)left != right.Current.Price;
        }

        /// <summary>
        /// Determines if the indicator's current Price is not equal to the specified Price
        /// </summary>
        public static bool operator !=(IndicatorBase<T> left, float right)
        {
            if (ReferenceEquals(left, null)) return true;
            return left.Current.Price != (decimal)right;
        }

        /// <summary>
        /// Determines if the specified Price is not equal to the indicator's current Price
        /// </summary>
        public static bool operator !=(float left, IndicatorBase<T> right)
        {
            if (ReferenceEquals(right, null)) return true;
            return (decimal)left != right.Current.Price;
        }

        /// <summary>
        /// Determines if the indicator's current Price is not equal to the specified Price
        /// </summary>
        public static bool operator !=(IndicatorBase<T> left, int right)
        {
            if (ReferenceEquals(left, null)) return true;
            return left.Current.Price != right;
        }

        /// <summary>
        /// Determines if the specified Price is not equal to the indicator's current Price
        /// </summary>
        public static bool operator !=(int left, IndicatorBase<T> right)
        {
            if (ReferenceEquals(right, null)) return true;
            return left != right.Current.Price;
        }

        /// <summary>
        /// Determines if the indicator's current Price is not equal to the specified Price
        /// </summary>
        public static bool operator !=(IndicatorBase<T> left, long right)
        {
            if (ReferenceEquals(left, null)) return true;
            return left.Current.Price != right;
        }

        /// <summary>
        /// Determines if the specified Price is not equal to the indicator's current Price
        /// </summary>
        public static bool operator !=(long left, IndicatorBase<T> right)
        {
            if (ReferenceEquals(right, null)) return true;
            return left != right.Current.Price;
        }

        /// <summary>
        /// Determines if the indicator's current Price is less than the specified Price
        /// </summary>
        public static bool operator <(IndicatorBase<T> left, double right)
        {
            if (ReferenceEquals(left, null)) return false;
            return left.Current.Price < (decimal)right;
        }

        /// <summary>
        /// Determines if the specified Price is less than the indicator's current Price
        /// </summary>
        public static bool operator <(double left, IndicatorBase<T> right)
        {
            if (ReferenceEquals(right, null)) return false;
            return (decimal)left < right.Current.Price;
        }

        /// <summary>
        /// Determines if the indicator's current Price is less than the specified Price
        /// </summary>
        public static bool operator <(IndicatorBase<T> left, float right)
        {
            if (ReferenceEquals(left, null)) return false;
            return left.Current.Price < (decimal)right;
        }

        /// <summary>
        /// Determines if the specified Price is less than the indicator's current Price
        /// </summary>
        public static bool operator <(float left, IndicatorBase<T> right)
        {
            if (ReferenceEquals(right, null)) return false;
            return (decimal)left < right.Current.Price;
        }

        /// <summary>
        /// Determines if the indicator's current Price is less than the specified Price
        /// </summary>
        public static bool operator <(IndicatorBase<T> left, int right)
        {
            if (ReferenceEquals(left, null)) return false;
            return left.Current.Price < right;
        }

        /// <summary>
        /// Determines if the specified Price is less than the indicator's current Price
        /// </summary>
        public static bool operator <(int left, IndicatorBase<T> right)
        {
            if (ReferenceEquals(right, null)) return false;
            return left < right.Current.Price;
        }

        /// <summary>
        /// Determines if the indicator's current Price is less than the specified Price
        /// </summary>
        public static bool operator <(IndicatorBase<T> left, long right)
        {
            if (ReferenceEquals(left, null)) return false;
            return left.Current.Price < right;
        }

        /// <summary>
        /// Determines if the specified Price is less than the indicator's current Price
        /// </summary>
        public static bool operator <(long left, IndicatorBase<T> right)
        {
            if (ReferenceEquals(right, null)) return false;
            return left < right.Current.Price;
        }

        /// <summary>
        /// Determines if the indicator's current Price is less than or equal to the specified Price
        /// </summary>
        public static bool operator <=(IndicatorBase<T> left, double right)
        {
            if (ReferenceEquals(left, null)) return false;
            return left.Current.Price <= (decimal)right;
        }

        /// <summary>
        /// Determines if the specified Price is less than or equal to the indicator's current Price
        /// </summary>
        public static bool operator <=(double left, IndicatorBase<T> right)
        {
            if (ReferenceEquals(right, null)) return false;
            return (decimal)left <= right.Current.Price;
        }

        /// <summary>
        /// Determines if the indicator's current Price is less than or equal to the specified Price
        /// </summary>
        public static bool operator <=(IndicatorBase<T> left, float right)
        {
            if (ReferenceEquals(left, null)) return false;
            return left.Current.Price <= (decimal)right;
        }

        /// <summary>
        /// Determines if the specified Price is less than or equal to the indicator's current Price
        /// </summary>
        public static bool operator <=(float left, IndicatorBase<T> right)
        {
            if (ReferenceEquals(right, null)) return false;
            return (decimal)left <= right.Current.Price;
        }

        /// <summary>
        /// Determines if the indicator's current Price is less than or equal to the specified Price
        /// </summary>
        public static bool operator <=(IndicatorBase<T> left, int right)
        {
            if (ReferenceEquals(left, null)) return false;
            return left.Current.Price <= right;
        }

        /// <summary>
        /// Determines if the specified Price is less than or equal to the indicator's current Price
        /// </summary>
        public static bool operator <=(int left, IndicatorBase<T> right)
        {
            if (ReferenceEquals(right, null)) return false;
            return left <= right.Current.Price;
        }

        /// <summary>
        /// Determines if the indicator's current Price is less than or equal to the specified Price
        /// </summary>
        public static bool operator <=(IndicatorBase<T> left, long right)
        {
            if (ReferenceEquals(left, null)) return false;
            return left.Current.Price <= right;
        }

        /// <summary>
        /// Determines if the specified Price is less than or equal to the indicator's current Price
        /// </summary>
        public static bool operator <=(long left, IndicatorBase<T> right)
        {
            if (ReferenceEquals(right, null)) return false;
            return left <= right.Current.Price;
        }

        /// <summary>
        /// Determines if the indicator's current Price is equal to the specified Price
        /// </summary>
        public static bool operator ==(IndicatorBase<T> left, double right)
        {
            if (ReferenceEquals(left, null)) return false;
            return left.Current.Price == (decimal)right;
        }

        /// <summary>
        /// Determines if the specified Price is equal to the indicator's current Price
        /// </summary>
        public static bool operator ==(double left, IndicatorBase<T> right)
        {
            if (ReferenceEquals(right, null)) return false;
            return (decimal)left == right.Current.Price;
        }

        /// <summary>
        /// Determines if the indicator's current Price is equal to the specified Price
        /// </summary>
        public static bool operator ==(IndicatorBase<T> left, float right)
        {
            if (ReferenceEquals(left, null)) return false;
            return left.Current.Price == (decimal)right;
        }

        /// <summary>
        /// Determines if the specified Price is equal to the indicator's current Price
        /// </summary>
        public static bool operator ==(float left, IndicatorBase<T> right)
        {
            if (ReferenceEquals(right, null)) return false;
            return (decimal)left == right.Current.Price;
        }

        /// <summary>
        /// Determines if the indicator's current Price is equal to the specified Price
        /// </summary>
        public static bool operator ==(IndicatorBase<T> left, int right)
        {
            if (ReferenceEquals(left, null)) return false;
            return left.Current.Price == right;
        }

        /// <summary>
        /// Determines if the specified Price is equal to the indicator's current Price
        /// </summary>
        public static bool operator ==(int left, IndicatorBase<T> right)
        {
            if (ReferenceEquals(right, null)) return false;
            return left == right.Current.Price;
        }

        /// <summary>
        /// Determines if the indicator's current Price is equal to the specified Price
        /// </summary>
        public static bool operator ==(IndicatorBase<T> left, long right)
        {
            if (ReferenceEquals(left, null)) return false;
            return left.Current.Price == right;
        }

        /// <summary>
        /// Determines if the specified Price is equal to the indicator's current Price
        /// </summary>
        public static bool operator ==(long left, IndicatorBase<T> right)
        {
            if (ReferenceEquals(right, null)) return false;
            return left == right.Current.Price;
        }

        /// <summary>
        /// Determines if the indicator's current Price is greater than the specified Price
        /// </summary>
        public static bool operator >(IndicatorBase<T> left, double right)
        {
            if (ReferenceEquals(left, null)) return false;
            return left.Current.Price > (decimal)right;
        }

        /// <summary>
        /// Determines if the specified Price is greater than the indicator's current Price
        /// </summary>
        public static bool operator >(double left, IndicatorBase<T> right)
        {
            if (ReferenceEquals(right, null)) return false;
            return (decimal)left > right.Current.Price;
        }

        /// <summary>
        /// Determines if the indicator's current Price is greater than the specified Price
        /// </summary>
        public static bool operator >(IndicatorBase<T> left, float right)
        {
            if (ReferenceEquals(left, null)) return false;
            return left.Current.Price > (decimal)right;
        }

        /// <summary>
        /// Determines if the specified Price is greater than the indicator's current Price
        /// </summary>
        public static bool operator >(float left, IndicatorBase<T> right)
        {
            if (ReferenceEquals(right, null)) return false;
            return (decimal)left > right.Current.Price;
        }

        /// <summary>
        /// Determines if the indicator's current Price is greater than the specified Price
        /// </summary>
        public static bool operator >(IndicatorBase<T> left, int right)
        {
            if (ReferenceEquals(left, null)) return false;
            return left.Current.Price > right;
        }

        /// <summary>
        /// Determines if the specified Price is greater than the indicator's current Price
        /// </summary>
        public static bool operator >(int left, IndicatorBase<T> right)
        {
            if (ReferenceEquals(right, null)) return false;
            return left > right.Current.Price;
        }

        /// <summary>
        /// Determines if the indicator's current Price is greater than the specified Price
        /// </summary>
        public static bool operator >(IndicatorBase<T> left, long right)
        {
            if (ReferenceEquals(left, null)) return false;
            return left.Current.Price > right;
        }

        /// <summary>
        /// Determines if the specified Price is greater than the indicator's current Price
        /// </summary>
        public static bool operator >(long left, IndicatorBase<T> right)
        {
            if (ReferenceEquals(right, null)) return false;
            return left > right.Current.Price;
        }

        /// <summary>
        /// Determines if the indicator's current Price is greater than or equal to the specified Price
        /// </summary>
        public static bool operator >=(IndicatorBase<T> left, double right)
        {
            if (ReferenceEquals(left, null)) return false;
            return left.Current.Price >= (decimal)right;
        }

        /// <summary>
        /// Determines if the specified Price is greater than or equal to the indicator's current Price
        /// </summary>
        public static bool operator >=(double left, IndicatorBase<T> right)
        {
            if (ReferenceEquals(right, null)) return false;
            return (decimal)left >= right.Current.Price;
        }

        /// <summary>
        /// Determines if the indicator's current Price is greater than or equal to the specified Price
        /// </summary>
        public static bool operator >=(IndicatorBase<T> left, float right)
        {
            if (ReferenceEquals(left, null)) return false;
            return left.Current.Price >= (decimal)right;
        }

        /// <summary>
        /// Determines if the specified Price is greater than or equal to the indicator's current Price
        /// </summary>
        public static bool operator >=(float left, IndicatorBase<T> right)
        {
            if (ReferenceEquals(right, null)) return false;
            return (decimal)left >= right.Current.Price;
        }

        /// <summary>
        /// Determines if the indicator's current Price is greater than or equal to the specified Price
        /// </summary>
        public static bool operator >=(IndicatorBase<T> left, int right)
        {
            if (ReferenceEquals(left, null)) return false;
            return left.Current.Price >= right;
        }

        /// <summary>
        /// Determines if the specified Price is greater than or equal to the indicator's current Price
        /// </summary>
        public static bool operator >=(int left, IndicatorBase<T> right)
        {
            if (ReferenceEquals(right, null)) return false;
            return left >= right.Current.Price;
        }

        /// <summary>
        /// Determines if the indicator's current Price is greater than or equal to the specified Price
        /// </summary>
        public static bool operator >=(IndicatorBase<T> left, long right)
        {
            if (ReferenceEquals(left, null)) return false;
            return left.Current.Price >= right;
        }

        /// <summary>
        /// Determines if the specified Price is greater than or equal to the indicator's current Price
        /// </summary>
        public static bool operator >=(long left, IndicatorBase<T> right)
        {
            if (ReferenceEquals(right, null)) return false;
            return left >= right.Current.Price;
        }

        #endregion Public Methods
    }
}