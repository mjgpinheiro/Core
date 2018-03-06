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

using Jil;
using System;

namespace Quantler.Api.IEX.Models
{
    /// <summary>
    /// Chart data as provided by IEX
    /// </summary>
    public class ChartModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the change.
        /// </summary>
        [JilDirective(Name = "change")]
        public double Change { get; set; }

        /// <summary>
        /// Gets or sets the change over time.
        /// </summary>
        [JilDirective(Name = "changeOverTime")]
        public double ChangeOverTime { get; set; }

        /// <summary>
        /// Gets or sets the change percent.
        /// </summary>
        [JilDirective(Name = "changePercent")]
        public double ChangePercent { get; set; }

        /// <summary>
        /// Gets or sets the close.
        /// </summary>
        [JilDirective(Name = "close")]
        public double Close { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        [JilDirective(Name = "date")]
        public string date { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        [JilDirective(true)]
        public DateTime Date => Time.TryParseDate(date, out DateTime returnvalue) ? returnvalue : throw new Exception($"Could not parse date {date}");

        /// <summary>
        /// Gets or sets the high.
        /// </summary>
        [JilDirective(Name = "high")]
        public double High { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        [JilDirective(Name = "label")]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the low.
        /// </summary>
        [JilDirective(Name = "low")]
        public double Low { get; set; }

        /// <summary>
        /// Gets or sets the open.
        /// </summary>
        [JilDirective(Name = "open")]
        public double Open { get; set; }

        /// <summary>
        /// Gets or sets the unadjusted close.
        /// </summary>
        [JilDirective(Name = "unadjustedClose")]
        public double UnadjustedClose { get; set; }

        /// <summary>
        /// Gets or sets the unadjusted volume.
        /// </summary>
        [JilDirective(Name = "unadjustedVolume")]
        public int UnadjustedVolume { get; set; }

        /// <summary>
        /// Gets or sets the volume.
        /// </summary>
        [JilDirective(Name = "volume")]
        public int Volume { get; set; }

        /// <summary>
        /// Gets or sets the vwap.
        /// </summary>
        [JilDirective(Name = "vwap")]
        public double Vwap { get; set; }

        #endregion Public Properties
    }
}