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

using NLog;
using Quantler.Broker.Model;
using Quantler.Configuration;
using Quantler.Exchanges;
using Quantler.Interfaces;
using System;
using System.Linq;

namespace Quantler.Securities
{
    /// <summary>
    /// Factory for loading security information
    /// </summary>
    public class SecurityFactory
    {
        #region Private Fields

        /// <summary>
        /// The account base currency
        /// </summary>
        private readonly CurrencyType _accountcurrency;

        /// <summary>
        /// Currently attached broker model
        /// </summary>
        private readonly BrokerModel _brokerModel;

        /// <summary>
        /// Currency conversion logic
        /// </summary>
        private readonly Currency _currency;

        /// <summary>
        /// Associated exchangeModels
        /// </summary>
        private readonly ExchangeModel[] _exchangeModels;

        /// <summary>
        /// Current instance logging
        /// </summary>
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initialize security factory, used for creating new securities
        /// </summary>
        /// <param name="brokermodel"></param>
        /// <param name="conversion"></param>
        /// <param name="exchangeModels"></param>
        /// <param name="accountcurrency"></param>
        public SecurityFactory(BrokerModel brokermodel, Currency conversion, CurrencyType accountcurrency, params ExchangeModel[] exchangeModels)
        {
            _brokerModel = brokermodel;
            _currency = conversion;
            _exchangeModels = exchangeModels;
            _accountcurrency = accountcurrency;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Creates a security object based on requested security point
        /// </summary>
        /// <param name="tickername"></param>
        /// <returns></returns>
        public Security Create(string tickername)
        {
            //Set default details
            var details = SecurityDetails.NIL();

            //Get config
            var configured = Config.SecurityConfig.FirstOrDefault(x => x.Ticker == tickername);
            if (configured == null)
                return new UnknownSecurity(tickername);

            //Get security type
            if (!Enum.TryParse(configured.Type, out SecurityType securitytype))
                securitytype = SecurityType.NIL;

            //Get Exchnage
            ExchangeModel exchangeModel = _exchangeModels.FirstOrDefault(x => x.Name == configured.Exchange);
            if (exchangeModel == null)
            {
                _log.Error(
                    $"Exchange with name {configured.Exchange} is not loaded, cannot load security {tickername}. This security is marked as unknown.");
                return new UnknownSecurity(tickername);
            }

            //Convert
            CurrencyType basecurrency = (CurrencyType)Enum.Parse(typeof(CurrencyType), configured.Currency);

            //Get commodity
            string commodity = configured.Commodity;

            //Create security
            SecurityBase toreturn;
            switch (securitytype)
            {
                case SecurityType.NIL:
                    return new UnknownSecurity(tickername);

                case SecurityType.Equity:
                    toreturn = new EquitySecurity(new TickerSymbol(tickername, commodity, basecurrency), exchangeModel, details, _currency);
                    break;

                case SecurityType.Crypto:
                    toreturn = GetCryptoCurrency(tickername, commodity, exchangeModel, details);
                    break;

                default:
                    return new UnknownSecurity(tickername);
            }

            //Check if security is known by broker model
            try
            {
                if (!_brokerModel.IsSecuritySupported(toreturn))
                {
                    _log.Warn($"Security with ticker {tickername} is not supported by broker, it is marked as unknown");
                    return new UnknownSecurity(tickername);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, $"Error while invoking broker model IsSecuritySupported for security with ticker name {tickername}");
                return new UnknownSecurity(tickername);
            }

            //Set details
            toreturn.Details = _brokerModel.GetSecurityDetails(toreturn);

            //Return what we have
            return toreturn;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Gets the crypto currency.
        /// </summary>
        /// <param name="tickername">The ticker name.</param>
        /// <param name="commodity"></param>
        /// <param name="exchangeModel">The exchangeModel.</param>
        /// <param name="details">The details.</param>
        /// <returns></returns>
        private SecurityBase GetCryptoCurrency(string tickername, string commodity, ExchangeModel exchangeModel, SecurityDetails details)
        {
            //Get all tickers associated to this crypto currency from config, for this broker model
            var configured = Config.SecurityConfig.Where(x => String.Equals(x.Ticker, tickername, StringComparison.CurrentCultureIgnoreCase) 
                                                            && String.Equals(x.Exchange, exchangeModel.Name, StringComparison.CurrentCultureIgnoreCase)
                                                            && x.Brokers.Select(b => b.ToLower()).Contains(_brokerModel.BrokerType.ToString().ToLower()))
                                                 .ToArray();

            //Check if we have items
            if(!configured.Any())
                return new UnknownSecurity(tickername);

            //Get shortest path for this currency for the account currency
            var found = configured.FirstOrDefault(x => String.Equals(x.Currency, _accountcurrency.ToString(), StringComparison.CurrentCultureIgnoreCase)) ?? configured.FirstOrDefault();

            //Return what we have
            if (found != null)
            {
                var basecurrency = (CurrencyType) Enum.Parse(typeof(CurrencyType), found.Currency);
                return new CryptoSecurity(new TickerSymbol(tickername, commodity, basecurrency), exchangeModel, details, _currency);
            }
            else
                return new UnknownSecurity(tickername);
        }

        #endregion Private Methods
    }
}