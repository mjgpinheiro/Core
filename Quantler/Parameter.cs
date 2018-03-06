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

namespace Quantler
{
    /// <summary>
    /// Paramater
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class Parameter : Attribute
    {
        #region Private Fields

        private int _parametervalue;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initialize an empty parameter
        /// </summary>
        public Parameter()
        {
        }

        /// <summary>
        /// Initialze a Parameter with settings
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="inc"></param>
        /// <param name="name"></param>
        public Parameter(int min, int max, int inc, string name)
        {
            ParameterMax = Math.Abs(max);
            ParameterMin = Math.Abs(min);
            ParameterName = name;
            ParameterInc = Math.Abs(inc);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Some additional information about this parameter
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Returns the parameter ID
        /// </summary>
        public int ParameterId { get; set; }

        /// <summary>
        /// Returns the increment of this parameter
        /// </summary>
        public int ParameterInc { get; set; }

        /// <summary>
        /// Returns the max value of this parameter that can be set
        /// </summary>
        public int ParameterMax { get; set; }

        /// <summary>
        /// Returns the min value of this parameter that can be set
        /// </summary>
        public int ParameterMin { get; set; }

        /// <summary>
        /// Returns the name of this parameter
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// Returns or sets the value for this parameter
        /// </summary>
        public int ParameterValue
        {
            get => _parametervalue;
            set => _parametervalue = value > ParameterMax ? ParameterMax : value;
        }

        #endregion Public Properties
    }
}