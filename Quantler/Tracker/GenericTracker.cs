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
using System.Collections;
using System.Collections.Generic;

namespace Quantler.Tracker
{
    /// <summary>
    /// Used to track any type of item by both text label and index values
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericTracker<T>
    {
        #region Public Fields

        /// <summary>
        /// text label has no index
        /// </summary>
        public const int Unknown = -1;

        #endregion Public Fields

        #region Private Fields

        private readonly List<T> _tracked;
        private readonly List<string> _txt;
        private readonly Dictionary<string, int> _txtidx;
        private T _defval;
        private string _name;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// creates a tracker with given name
        /// </summary>
        /// <param name="name"></param>
        public GenericTracker(string name)
            : this(0, name, default(T))
        {
        }

        /// <summary>
        /// creates tracker with given name and default value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultvaladd"></param>
        public GenericTracker(string name, T defaultvaladd)
            : this(0, name, defaultvaladd)
        {
        }

        /// <summary>
        /// creates a tracker
        /// </summary>
        public GenericTracker()
            : this(0, string.Empty, default(T))
        {
        }

        /// <summary>
        /// creates tracker with approximate # of initial items
        /// </summary>
        /// <param name="estCount"></param>
        public GenericTracker(int estCount)
            : this(estCount, string.Empty, default(T))
        {
        }

        /// <summary>
        /// create a tracker with an approximate # of initial items and name
        /// </summary>
        /// <param name="estCount"></param>
        public GenericTracker(int estCount, string name, T defaultaddval)
        {
            _name = name;
            if (estCount != 0)
            {
                _tracked = new List<T>(estCount);
                _txtidx = new Dictionary<string, int>(estCount);
                _txt = new List<string>(estCount);
            }
            else
            {
                _tracked = new List<T>();
                _txtidx = new Dictionary<string, int>();
                _txt = new List<string>();
            }
            _defval = defaultaddval;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// gets count of items being tracked
        /// </summary>
        public virtual int Count => _tracked.Count;

        /// <summary>
        /// gets default value for a given type
        /// </summary>
        public virtual T Default
        {
            get => _defval;
            set => _defval = value;
        }

        /// <summary>
        /// name of this tracker
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// gets type of whatever is being tracked
        /// </summary>
        public virtual Type TrackedType => typeof(T);

        #endregion Public Properties

        #region Public Indexers

        /// <summary>
        /// get a tracked value from it's index
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public virtual T this[int i]
        {
            get => _tracked[i];
            set => _tracked[i] = value;
        }

        /// <summary>
        /// get a tracked value from it's text label
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public virtual T this[string txt]
        {
            get => _tracked[Getindex(txt)];
            set => _tracked[Getindex(txt)] = value;
        }

        #endregion Public Indexers

        #region Public Methods

        /// <summary>
        /// gets index of a label, adding it if it doesn't exist. initial value associated with
        /// index will be Default
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public virtual int Addindex(string txt) =>
            Addindex(txt, Default);

        /// <summary>
        /// gets index of label, adding it if it doesn't exist.
        /// </summary>
        /// <param name="txtidx">label</param>
        /// <param name="val">value to associate with label</param>
        /// <returns></returns>
        public virtual int Addindex(string txtidx, T val)
        {
            int idx = Unknown;
            if (!_txtidx.TryGetValue(txtidx, out idx))
            {
                idx = _tracked.Count;
                _txt.Add(txtidx);
                _txtidx.Add(txtidx, idx);
                _tracked.Add(val);
            }
            else
            {
                _tracked[idx] = val;
            }
            return idx;
        }

        /// <summary>
        /// clears all tracked values and labels
        /// </summary>
        public virtual void Clear()
        {
            _tracked.Clear();
            _txt.Clear();
            _txtidx.Clear();
        }

        /// <summary>
        /// get display-ready tracked value of a given index. For this to work, your tracked type
        /// MUST implement ToString() otherwise it will return as empty.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public string Display(int idx)
        {
            try
            {
                return _tracked[idx].ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// get display-ready tracked value of a given label
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public string Display(string txt)
        {
            try
            {
                int idx = Getindex(txt);
                if (idx < 0) return string.Empty;

                return _tracked[idx].ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// allows 'foreach' enumeration of each tracked element
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator() =>
            _tracked.GetEnumerator();

        /// <summary>
        /// gets index of text label or returns UNKNOWN if none found
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public int Getindex(string txt)
        {
            int idx;
            if (_txtidx.TryGetValue(txt, out idx))
                return idx;
            return Unknown;
        }

        /// <summary>
        /// gets a label given an index
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public string Getlabel(int idx) =>
            _txt[idx];

        /// <summary>
        /// reset all tracked values to their default value
        /// </summary>
        public virtual void Reset()
        {
            for (int i = 0; i < _tracked.Count; i++)
                _tracked[i] = Default;
        }

        /// <summary>
        /// reset given index to it's default value
        /// </summary>
        /// <param name="idx"></param>
        public virtual void Reset(int idx) =>
            _tracked[idx] = Default;

        /// <summary>
        /// reset given label to it's default value
        /// </summary>
        /// <param name="txt"></param>
        public virtual void Reset(string txt)
        {
            int idx = Getindex(txt);
            _tracked[idx] = Default;
        }

        /// <summary>
        /// Current items to array.
        /// </summary>
        /// <returns></returns>
        public virtual T[] ToArray() =>
            _tracked.ToArray();

        /// <summary>
        /// gets array of labels tracked
        /// </summary>
        /// <returns></returns>
        public string[] ToLabelArray() =>
            _txt.ToArray();

        /// <summary>
        /// gets value of given label
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public object Value(string txt) =>
            this[txt];

        /// <summary>
        /// gets value of give index
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public object Value(int idx) =>
            this[idx];

        /// <summary>
        /// attempts to convert tracked value to decimal given label
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public virtual decimal ValueDecimal(string txt) =>
            Convert.ToDecimal(this[txt]);

        /// <summary>
        /// attempts to convert tracked value to decimal given index
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public virtual decimal ValueDecimal(int idx) =>
            Convert.ToDecimal(this[idx]);

        #endregion Public Methods
    }
}