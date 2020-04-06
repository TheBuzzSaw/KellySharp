using System;
using System.Collections.Generic;

namespace KellySharp
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key,
            TValue defaultValue = default)
            where TKey : notnull
        {
            if (!dictionary.TryGetValue(key, out var value))
            {
                value = defaultValue;
                dictionary.Add(key, value);
            }

            return value;
        }

        public static TValue GetOrAdd<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key,
            Func<TValue> valueFactory)
            where TKey : notnull
        {
            if (!dictionary.TryGetValue(key, out var value))
            {
                value = valueFactory.Invoke();
                dictionary.Add(key, value);
            }

            return value;
        }

        public static TValue AlwaysGet<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key,
            Func<TValue> valueFactory)
            where TKey : notnull
            where TValue : class
        {
            if (!dictionary.TryGetValue(key, out var value))
            {
                value = valueFactory();
                dictionary.Add(key, value);
            }
            else if (value is null)
            {
                value = valueFactory.Invoke();
                dictionary[key] = value;
            }

            return value;
        }

        public static TValue AlwaysGet<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key)
            where TKey : notnull
            where TValue : class, new()
        {
            return AlwaysGet(dictionary, key, () => new TValue());
        }
    }
}