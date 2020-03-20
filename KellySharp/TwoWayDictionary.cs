using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KellySharp
{
    public class TwoWayDictionary<TKey, TValue> : IDictionary<TKey, TValue>
        where TKey : notnull
        where TValue : notnull
    {
        private readonly IDictionary<TKey, TValue> _main;
        private readonly IDictionary<TValue, TKey> _reverse;

        public TwoWayDictionary<TValue, TKey> Reverse { get; }

        public ICollection<TKey> Keys => _main.Keys;
        public ICollection<TValue> Values => _main.Values;
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => _main.IsReadOnly;
        
        public int Count => _main.Count;

        public TValue this[TKey key]
        {
            get => _main[key];
            set
            {
                bool hadOldValue = _main.Remove(key, out var oldValue);
                
                if (hadOldValue)
                    _reverse.Remove(oldValue);
                
                bool hadOldKey = _reverse.Remove(value, out var oldKey);
                
                if (hadOldKey)
                    _main.Remove(oldKey);

                try
                {
                    Add(key, value);
                }
                catch
                {
                    if (hadOldKey)
                        Add(oldKey, value);
                    
                    if (hadOldValue)
                        Add(key, oldValue);
                    
                    throw;
                }
            }
        }

        public TwoWayDictionary()
        {
            _main = new Dictionary<TKey, TValue>();
            _reverse = new Dictionary<TValue, TKey>();
            Reverse = new TwoWayDictionary<TValue, TKey>(this);
        }

        public TwoWayDictionary(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            _main = new Dictionary<TKey, TValue>(pairs);
            _reverse = new Dictionary<TValue, TKey>(_main.Select(pair => KeyValuePair.Create(pair.Value, pair.Key)));
            Reverse = new TwoWayDictionary<TValue, TKey>(this);
        }

        private TwoWayDictionary(TwoWayDictionary<TValue, TKey> reverse)
        {
            Reverse = reverse;
            _main = reverse._reverse;
            _reverse = reverse._main;
        }

        public void Add(TKey key, TValue value)
        {
            _main.Add(key, value);

            try
            {
                _reverse.Add(value, key);
            }
            catch
            {
                _main.Remove(key);
                throw;
            }
        }

        public bool ContainsKey(TKey key) => _main.ContainsKey(key);
        public bool ContainsValue(TValue value) => _reverse.ContainsKey(value);

        public bool Remove(TKey key)
        {
            if (_main.Remove(key, out var value))
            {
                _reverse.Remove(value);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryGetValue(TKey key, out TValue value) => _main.TryGetValue(key, out value);

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            _main.Add(item);

            try
            {
                var pair = KeyValuePair.Create(item.Value, item.Key);
                _reverse.Add(pair);
            }
            catch
            {
                _main.Remove(item);
                throw;
            }
        }

        public void Clear()
        {
            _main.Clear();
            _reverse.Clear();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) =>
            _main.Contains(item);

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
            _main.CopyTo(array, arrayIndex);

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (_main.Remove(item))
            {
                var pair = KeyValuePair.Create(item.Value, item.Key);
                _reverse.Remove(pair);
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _main.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => (_main as IEnumerable).GetEnumerator();
    }
}
