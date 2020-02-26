using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KellySharp
{
    public class DictionaryJsonConverterFactoryBuilder
    {
        private readonly Dictionary<Type, Delegate> _parsers = new Dictionary<Type, Delegate>();

        public DictionaryJsonConverterFactoryBuilder AddParser<T>(Converter<string, T> parser)
        {
            _parsers.Add(typeof(T), parser);
            return this;
        }

        public DictionaryJsonConverterFactoryBuilder AddDefaultParsers()
        {
            return AddParser(s => s)
                .AddParser(sbyte.Parse)
                .AddParser(short.Parse)
                .AddParser(int.Parse)
                .AddParser(long.Parse)
                .AddParser(byte.Parse)
                .AddParser(ushort.Parse)
                .AddParser(uint.Parse)
                .AddParser(ulong.Parse)
                .AddParser(BigInteger.Parse)
                .AddParser(float.Parse)
                .AddParser(double.Parse)
                .AddParser(decimal.Parse)
                .AddParser(Guid.Parse);
        }

        public DictionaryJsonConverterFactory Build()
        {
            return new DictionaryJsonConverterFactory(
                _parsers.ToImmutableDictionary());
        }
    }
}