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

using Quantler.Performance;
using System;
using Quantler.Messaging;

namespace Quantler.Bootstrap.ResultHandlers
{
    /// <summary>
    /// Saves results of a simualtion in intervals to a csv file
    /// TODO: implement and in flow
    /// </summary>
    /// <seealso cref="Quantler.Bootstrap.ResultHandlers.ResultHandler" />
    public class CSVResultHandler : ResultHandler
    {
        #region Public Properties

        public bool IsRunning => throw new NotImplementedException();

        #endregion Public Properties

        private Result _results;

        #region Public Methods

        public void Initialize(Result result) =>
            _results = result;

        public void Initialize(MessageInstance message, Result result)
        {
            throw new NotImplementedException();
        }

        public void OnEndOfExecution()
        {
            throw new NotImplementedException();
        }

        public void Poke(DateTime utc)
        {

        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}