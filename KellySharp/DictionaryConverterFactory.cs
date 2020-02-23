using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KellySharp
{
    public class DictionaryConverterFactory : JsonConverterFactory
    {
        public static readonly ImmutableDictionary<Type, Delegate> DefaultParsers = new Dictionary<Type, Delegate>
        {
            [typeof(string)] = new Converter<string, string>(s => s),
            [typeof(sbyte)] = new Converter<string, sbyte>(sbyte.Parse),
            [typeof(short)] = new Converter<string, short>(short.Parse),
            [typeof(int)] = new Converter<string, int>(int.Parse),
            [typeof(long)] = new Converter<string, long>(long.Parse),
            [typeof(byte)] = new Converter<string, byte>(byte.Parse),
            [typeof(ushort)] = new Converter<string, ushort>(ushort.Parse),
            [typeof(uint)] = new Converter<string, uint>(uint.Parse),
            [typeof(ulong)] = new Converter<string, ulong>(ulong.Parse),
            [typeof(BigInteger)] = new Converter<string, BigInteger>(BigInteger.Parse),
            [typeof(float)] = new Converter<string, float>(float.Parse),
            [typeof(double)] = new Converter<string, double>(double.Parse),
            [typeof(decimal)] = new Converter<string, decimal>(decimal.Parse),
            [typeof(Guid)] = new Converter<string, Guid>(Guid.Parse)
        }.ToImmutableDictionary();

        private static readonly Type[] s_allowedTypes = new[]
        {
            typeof(IDictionary<,>),
            typeof(Dictionary<,>),
            typeof(SortedDictionary<,>),
            typeof(ImmutableDictionary<,>),
            typeof(ImmutableSortedDictionary<,>)
        };

        private readonly ImmutableDictionary<Type, Delegate> _parsers;

        public DictionaryConverterFactory() : this(DefaultParsers)
        {
        }

        public DictionaryConverterFactory(ImmutableDictionary<Type, Delegate> parsers)
        {
            _parsers = parsers;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return
                typeToConvert.IsGenericType &&
                Array.Exists(s_allowedTypes, t => t == typeToConvert.GetGenericTypeDefinition());
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var keyType = typeToConvert.GenericTypeArguments[0];
            var valueType = typeToConvert.GenericTypeArguments[1];

            var keyConverter = _parsers[keyType];
            var result = Activator.CreateInstance(
                typeof(IDictionaryJsonConverter<,>).MakeGenericType(typeToConvert.GenericTypeArguments),
                keyConverter,
                options.GetConverter(valueType)) ?? throw new NullReferenceException("Failed to create converter");
            
            return (JsonConverter)result;
        }
    }
}