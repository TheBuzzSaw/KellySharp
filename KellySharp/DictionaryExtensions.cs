using System;
using System.Collections.Generic;

namespace KellySharp
{
    public static class DictionaryExtensions
    {
        public static void GetOrAdd<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key,
            TValue defaultValue = default)
            where TKey : notnull
        {
            if (!dictionary.TryGetValue(key, out var value))
                dictionary.Add(key, defaultValue);
        }

        public static void GetOrAdd<TKey, TValue>(
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
        }

        public static void AlwaysGet<TKey, TValue>(
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
        }

        public static void AlwaysGet<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key)
            where TKey : notnull
            where TValue : class, new()
        {
            AlwaysGet(dictionary, key, () => new TValue());
        }
    }
}