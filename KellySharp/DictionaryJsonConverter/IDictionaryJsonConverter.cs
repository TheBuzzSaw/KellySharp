using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KellySharp
{
    public class IDictionaryJsonConverter<TKey, TValue> :
        JsonConverter<IDictionary<TKey, TValue>?> where TKey : notnull
    {
        private readonly Converter<string, TKey> _keyParser;
        private readonly Converter<TKey, string> _keySerializer;
        private readonly JsonConverter<TValue> _valueConverter;

        public IDictionaryJsonConverter(
            Converter<string, TKey> keyParser,
            Converter<TKey, string> keySerializer,
            JsonConverter<TValue> valueConverter)
        {
            _keyParser = keyParser;
            _keySerializer = keySerializer;
            _valueConverter = valueConverter;
        }

        public override IDictionary<TKey, TValue>? Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            return DictionaryJsonConverter<TKey, TValue>.Read(
                ref reader, _keyParser, _valueConverter, options);
        }

        public override void Write(
            Utf8JsonWriter writer,
            IDictionary<TKey, TValue>? value,
            JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStartObject();

                foreach (var pair in value)
                {
                    writer.WritePropertyName(_keySerializer(pair.Key));
                    _valueConverter.Write(writer, pair.Value, options);
                }

                writer.WriteEndObject();
            }
        }
    }
}