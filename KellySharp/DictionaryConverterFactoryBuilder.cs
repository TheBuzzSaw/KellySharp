using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KellySharp
{
    public class DictionaryConverterFactoryBuilder
    {
        private readonly Dictionary<Type, Delegate> _parsers = new Dictionary<Type, Delegate>();

        public DictionaryConverterFactoryBuilder AddParser<T>(Converter<string, T> parser)
        {
            _parsers.Add(typeof(T), parser);
            return this;
        }

        public DictionaryConverterFactoryBuilder AddDefaultParsers()
        {
            foreach (var pair in DictionaryConverterFactory.DefaultParsers)
                _parsers.Add(pair.Key, pair.Value);
            
            return this;
        }

        public DictionaryConverterFactory Build()
        {
            return new DictionaryConverterFactory(_parsers.ToImmutableDictionary());
        }
    }
}