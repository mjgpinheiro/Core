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


using System.ComponentModel;

namespace Quantler
{
    /// <summary>
    /// Common timezones
    /// NOTE: DO NOT CHANGE ENUM NUMBERING! => DUE TO MESSAGEPACK INTEGRATION
    /// </summary>
    public enum TimeZone
    {
        /// <summary>
        /// Amsterdam, Netherlands. DST time zone.
        /// </summary>
        [Description("Europe/Amsterdam")]
        Amsterdam = 0,

        /// <summary>
        /// Anchorage, USA. DST time zone.
        /// </summary>
        [Description("America/Anchorage")]
        Anchorage = 1,

        /// <summary>
        /// Athens, Greece. DST time zone.
        /// </summary>
        [Description("Europe/Athens")]
        Athens = 2,

        /// <summary>
        /// Auckland, New Zealand. DST time zone.
        /// </summary>
        [Description("Pacific/Auckland")]
        Auckland = 3,

        /// <summary>
        /// Bangkok, Thailand.
        /// </summary>
        [Description("Asia/Bangkok")]
        Bangkok = 4,

        /// <summary>
        /// Berlin, Germany. DST time zone.
        /// </summary>
        [Description("Europe/Berlin")]
        Berlin = 5,

        /// <summary>
        /// Brisbane, Australia.
        /// </summary>
        [Description("Australia/Brisbane")]
        Brisbane = 6,

        /// <summary>
        /// Bucharest, Romania. DST time zone.
        /// </summary>
        [Description("Europe/Bucharest")]
        Bucharest = 7,

        /// <summary>
        /// Buenos Aires, Argentia.
        /// </summary>
        [Description("Argentina/Buenos_Aires")]
        BuenosAires = 8,

        /// <summary>
        /// Cairo, Egypt.
        /// </summary>
        [Description("Africa/Cairo")]
        Cairo = 9,

        /// <summary>
        /// Chagos, India.
        /// </summary>
        [Description("Indian/Chagos")]
        Chagos = 10,

        /// <summary>
        /// Chicago, USA. DST time zone.
        /// </summary>
        [Description("America/Chicago")]
        Chicago = 11,

        /// <summary>
        /// Denver, USA. DST time zone.
        /// </summary>
        [Description("America/Denver")]
        Denver = 12,

        /// <summary>
        /// Detroit, USA. DST time zone.
        /// </summary>
        [Description("America/Detroit")]
        Detroit = 13,

        /// <summary>
        /// Dublin, Ireland. DST time zone.
        /// </summary>
        [Description("Europe/Dublin")]
        Dublin = 14,

        /// <summary>
        /// Eastern Standard Time (EST) NO DST, always UTC -5 hour offset
        /// </summary>
        [Description("UTC-05")]
        EasternStandard = 15,

        /// <summary>
        /// Helsinki, Finland. DST time zone.
        /// </summary>
        [Description("Europe/Helsinki")]
        Helsinki = 16,

        /// <summary>
        /// Hong Kong, China.
        /// </summary>
        [Description("Asia/Hong_Kong")]
        HongKong = 17,

        /// <summary>
        /// Honolulu, USA. DST time zone.
        /// </summary>
        [Description("Pacific/Honolulu")]
        Honolulu = 18,

        /// <summary>
        ///Istanbul, Turkey. DST time zone.
        /// </summary>
        [Description("Europe/Istanbul")]
        Istanbul = 19,

        /// <summary>
        /// Jerusalem, Israel. DST time zone.
        /// </summary>
        [Description("Asia/Jerusalem")]
        Jerusalem = 20,

        /// <summary>
        /// Johannesburg, South Africa.
        /// </summary>
        [Description("Africa/Johannesburg")]
        Johannesburg = 21,

        /// <summary>
        /// Kiev Timezone, England. DST time zone.
        /// </summary>
        [Description("Europe/Kiev")]
        Kiev = 22,

        /// <summary>
        /// GMT+0 LONDON Timezone, England. DST time zone.
        /// </summary>
        [Description("Europe/London")]
        London = 23,

        /// <summary>
        /// Los Angeles, USA. DST time zone.
        /// </summary>
        [Description("America/Los_Angeles")]
        LosAngeles = 24,

        /// <summary>
        /// Madrid, Span. DST time zone.
        /// </summary>
        [Description("Europe/Madrid")]
        Madrid = 25,

        /// <summary>
        /// Melbourne, Australia. DST time zone.
        /// </summary>
        [Description("Australia/Melbourne")]
        Melbourne = 26,

        /// <summary>
        /// Mexico City, Mexico. DST time zone.
        /// </summary>
        [Description("America/Mexico_City")]
        MexicoCity = 27,

        /// <summary>
        /// Minsk, Belarus.
        /// </summary>
        [Description("Europe/Minsk")]
        Minsk = 28,

        /// <summary>
        /// Moscow, Russia.
        /// </summary>
        [Description("Europe/Moscow")]
        Moscow = 29,

        /// <summary>
        /// New York City, USA. DST time zone.
        /// </summary>
        [Description("America/New_York")]
        NewYork = 30,

        /// <summary>
        /// Paris, France. DST time zone.
        /// </summary>
        [Description("Europe/Paris")]
        Paris = 31,

        /// <summary>
        /// Phoenix, USA. DST time zone.
        /// </summary>
        [Description("America/Phoenix")]
        Phoenix = 32,

        /// <summary>
        /// Rome, Italy. DST time zone.
        /// </summary>
        [Description("Europe/Rome")]
        Rome = 33,

        /// <summary>
        /// Sao Paulo, Brazil. DST time zone.
        /// </summary>
        [Description("America/Sao_Paulo")]
        SaoPaulo = 34,

        /// <summary>
        /// Shanghai, China.
        /// </summary>
        [Description("Asia/Shanghai")]
        Shanghai = 35,

        /// <summary>
        /// Sydney, Australia. DST time zone.
        /// </summary>
        [Description("Australia/Sydney")]
        Sydney = 36,

        /// <summary>
        /// Tokyo, Japan.
        /// </summary>
        [Description("Asia/Tokyo")]
        Tokyo = 37,

        /// <summary>
        /// Toronto, Canada. DST time zone.
        /// </summary>
        [Description("America/Toronto")]
        Toronto = 38,

        /// <summary>
        /// Universal Coordinated time zone.
        /// </summary>
        [Description("Etc/UTC")]
        Utc = 39,

        /// <summary>
        /// Vancouver, Canada.
        /// </summary>
        [Description("America/Vancouver")]
        Vancouver = 40,

        /// <summary>
        /// Zurich, Switzerland. TDST time zone.
        /// </summary>
        [Description("Europe/Zurich")]
        Zurich = 41,

        /// <summary>
        /// Central European Time
        /// </summary>
        [Description("CET")]
        CET = 42
    }
}