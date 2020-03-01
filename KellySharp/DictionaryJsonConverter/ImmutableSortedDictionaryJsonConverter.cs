using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KellySharp
{
    public class ImmutableSortedDictionaryJsonConverter<TKey, TValue> : JsonConverter<ImmutableSortedDictionary<TKey, TValue>?> where TKey : notnull
    {
        private readonly Converter<string, TKey> _keyParser;
        private readonly Converter<TKey, string> _keySerializer;
        private readonly JsonConverter<TValue> _valueConverter;

        public ImmutableSortedDictionaryJsonConverter(
            Converter<string, TKey> keyParser,
            Converter<TKey, string> keySerializer,
            JsonConverter<TValue> valueConverter)
        {
            _keyParser = keyParser;
            _keySerializer = keySerializer;
            _valueConverter = valueConverter;
        }

        public override ImmutableSortedDictionary<TKey, TValue>? Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {   
            if (reader.TokenType == JsonTokenType.Null)
                return null;
            
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Dictionary must be JSON object.");
            
            var result = ImmutableSortedDictionary.CreateBuilder<TKey, TValue>();
            
            while (true)
            {
                if (!reader.Read())
                    throw new JsonException("Incomplete JSON object");
                
                if (reader.TokenType == JsonTokenType.EndObject)
                    return result.ToImmutable();

                var key = _keyParser(reader.GetString());

                if (!reader.Read())
                    throw new JsonException("Incomplete JSON object");

                var value = _valueConverter.Read(ref reader, typeof(TValue), options);

                result.Add(key, value);
            }
        }

        public override void Write(
            Utf8JsonWriter writer,
            ImmutableSortedDictionary<TKey, TValue>? value,
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