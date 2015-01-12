using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flai.DataStructures
{

    [Serializable]
    public class SerializableDictionary<T, Y> : IEnumerable<KeyValuePair<T, Y>>
    {
        [SerializeField]
        private List<T> _keys;

        [SerializeField]
        private List<Y> _values;

        private ReadOnlyList<T> _readOnlyKeys;
        public ReadOnlyList<T> Keys
        {
            get { return _readOnlyKeys ?? (_readOnlyKeys = new ReadOnlyList<T>(_keys)); }
        }

        private ReadOnlyList<Y> _readOnlyValues;
        public ReadOnlyList<Y> Values
        {
            get { return _readOnlyValues ?? (_readOnlyValues = new ReadOnlyList<Y>(_values)); }
        }

        public int Count
        {
            get { return _keys.Count; }
        }

        // no public constructor, must be inherited (unity serialization system cannot serialize generic types)
        protected SerializableDictionary(Dictionary<T, Y> dictionary)
            : this()
        {
            foreach (var kvp in dictionary)
            {
                _keys.Add(kvp.Key);
                _values.Add(kvp.Value);
            }
        }

        protected SerializableDictionary()
        {
            _keys = new List<T>();
            _values = new List<Y>();
        }

        public void Add(T key, Y value)
        {
            _keys.Add(key);
            _values.Add(value);
        }

        public Dictionary<T, Y> ToDictionary()
        {
            Dictionary<T, Y> dictionary = new Dictionary<T, Y>(_keys.Count);
            for (int i = 0; i < _keys.Count; i++)
            {
                dictionary.Add(_keys[i], _values[i]);
            }

            return dictionary;
        }

        public IEnumerator<KeyValuePair<T, Y>> GetEnumerator()
        {
            for (int i = 0; i < _keys.Count; i++)
            {
                yield return new KeyValuePair<T, Y>(_keys[i], _values[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
