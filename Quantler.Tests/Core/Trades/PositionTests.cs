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

using Xunit;

namespace Quantler.Tests.Core.Trades
{
    /// <summary>
    /// TODO: set unit tests
    /// </summary>
    public class PositionTests
    {
        #region Public Methods

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Trades", "Position")]
        //Test to see if we get the expected trade if we are long with one fill and go short from another single fill (we flip the position)
        public void Position_Adjust_FullInLong_Flip_Short()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Trades", "Position")]
        //Test to see if we get the expected trade if we are full in long and close this position with a full out short
        public void Position_Adjust_FullInLong_FullOutShort()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Trades", "Position")]
        //Test to see if we get the expected trade if we are long with one fill and receive multiple trades with partial closing fills
        public void Position_Adjust_FullInLong_MultiOutShort()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Trades", "Position")]
        //Test to see if
        public void Position_Adjust_FullInShort_Flip_Long()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Trades", "Position")]
        //Test to see if we get the expected trade if we are full in short and close this position with a full out long
        public void Position_Adjust_FullInShort_FullOutLong()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Trades", "Position")]
        //Test to see if we get the expected trade if we are long with one fill and receive multiple trades with partial closing fills
        public void Position_Adjust_FullInShort_MultiOutLong()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Trades", "Position")]
        //Test to see if we get the expected trade if we are use multiple fills to create a position and close it with one fill
        public void Position_Adjust_MultiInLong_FullOutShort()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Trades", "Position")]
        //Test to see if
        public void Position_Adjust_MultiInLong_PartialsOutShort()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Trades", "Position")]
        //Test to see if we get the expected trade if we are use multiple fills to create a position and close it with one fill
        public void Position_Adjust_MultiInShort_FullOutLong()
        {
        }

        [Fact(Skip = "TODO")]
        [Trait("Quantler.Core.Trades", "Position")]
        //Test to see if
        public void Position_Adjust_MultiInShort_PartialsOutLong()
        {
        }

        #endregion Public Methods
    }
}