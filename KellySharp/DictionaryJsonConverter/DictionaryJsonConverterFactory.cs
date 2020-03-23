using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KellySharp
{
    public sealed class DictionaryJsonConverterFactory : JsonConverterFactory
    {
        private static readonly ImmutableDictionary<Type, Type> s_converterTypes = new Dictionary<Type, Type>
        {
            [typeof(IDictionary<,>)] = typeof(IDictionaryJsonConverter<,>),
            [typeof(IReadOnlyDictionary<,>)] = typeof(IReadOnlyDictionaryJsonConverter<,>),
            [typeof(Dictionary<,>)] = typeof(DictionaryJsonConverter<,>),
            [typeof(SortedDictionary<,>)] = typeof(SortedDictionaryJsonConverter<,>),
            [typeof(ImmutableDictionary<,>)] = typeof(ImmutableDictionaryJsonConverter<,>),
            [typeof(ImmutableSortedDictionary<,>)] = typeof(ImmutableSortedDictionaryJsonConverter<,>)
        }.ToImmutableDictionary();

        public static readonly ImmutableArray<Type> SupportedDictionaryTypes = s_converterTypes.Keys.ToImmutableArray();
        public static readonly DictionaryJsonConverterFactory Default =
            new DictionaryJsonConverterFactoryBuilder().AddDefaults().Build();

        private readonly ImmutableDictionary<Type, Delegate> _parsers;
        private readonly ImmutableDictionary<Type, Delegate> _serializers;

        internal DictionaryJsonConverterFactory(
            ImmutableDictionary<Type, Delegate> parsers,
            ImmutableDictionary<Type, Delegate> serializers)
        {
            _parsers = parsers;
            _serializers = serializers;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return
                typeToConvert.IsGenericType &&
                s_converterTypes.ContainsKey(typeToConvert.GetGenericTypeDefinition()) &&
                _parsers.ContainsKey(typeToConvert.GenericTypeArguments[0]);
        }

        private static Converter<T, string> MakeClassSerializer<T>() where T : class
        {
            return item => item?.ToString() ?? string.Empty;
        }

        private static Converter<T, string> MakeStructSerializer<T>() where T : struct
        {
            return item => item.ToString() ?? string.Empty;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var keyType = typeToConvert.GenericTypeArguments[0];
            var valueType = typeToConvert.GenericTypeArguments[1];

            var keyParser = _parsers[keyType];

            if (!_serializers.TryGetValue(keyType, out var keySerializer))
            {
                var methodName = keyType.IsValueType ? nameof(MakeStructSerializer) : nameof(MakeClassSerializer);
                var obj = GetType()
                    .GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic)?
                    .MakeGenericMethod(keyType)
                    .Invoke(null, null) ?? throw new NullReferenceException("Failed to invoke serializer maker");
                keySerializer = (Delegate)obj;
            }

            var valueConverter = options.GetConverter(valueType);
            var converterType = s_converterTypes[typeToConvert.GetGenericTypeDefinition()];
            var result = Activator.CreateInstance(
                converterType.MakeGenericType(typeToConvert.GenericTypeArguments),
                keyParser,
                keySerializer,
                valueConverter) ?? throw new NullReferenceException("Failed to create converter");
            
            return (JsonConverter)result;
        }
    }
}