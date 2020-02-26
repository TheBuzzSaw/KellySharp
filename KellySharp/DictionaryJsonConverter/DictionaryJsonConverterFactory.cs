using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KellySharp
{
    public class DictionaryJsonConverterFactory : JsonConverterFactory
    {
        public static readonly DictionaryJsonConverterFactory Default =
            new DictionaryJsonConverterFactoryBuilder().AddDefaultParsers().Build();

        private static readonly ImmutableDictionary<Type, Type> s_converterTypes = new Dictionary<Type, Type>
        {
            [typeof(IDictionary<,>)] = typeof(IDictionaryJsonConverter<,>),
            [typeof(Dictionary<,>)] = typeof(DictionaryJsonConverter<,>),
            [typeof(SortedDictionary<,>)] = typeof(SortedDictionaryJsonConverter<,>),
            [typeof(ImmutableDictionary<,>)] = typeof(ImmutableDictionaryJsonConverter<,>),
            [typeof(ImmutableSortedDictionary<,>)] = typeof(ImmutableSortedDictionaryJsonConverter<,>)
        }.ToImmutableDictionary();

        private readonly ImmutableDictionary<Type, Delegate> _parsers;

        internal DictionaryJsonConverterFactory(ImmutableDictionary<Type, Delegate> parsers)
        {
            _parsers = parsers;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return
                typeToConvert.IsGenericType &&
                s_converterTypes.ContainsKey(typeToConvert.GetGenericTypeDefinition());
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var keyType = typeToConvert.GenericTypeArguments[0];
            var valueType = typeToConvert.GenericTypeArguments[1];

            var keyParser = _parsers[keyType];
            var converterType = s_converterTypes[typeToConvert.GetGenericTypeDefinition()];
            var result = Activator.CreateInstance(
                converterType.MakeGenericType(typeToConvert.GenericTypeArguments),
                keyParser,
                options.GetConverter(valueType)) ?? throw new NullReferenceException("Failed to create converter");
            
            return (JsonConverter)result;
        }
    }
}