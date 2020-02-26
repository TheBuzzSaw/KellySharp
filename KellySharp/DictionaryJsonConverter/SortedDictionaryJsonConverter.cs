using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KellySharp
{
    public class SortedDictionaryJsonConverter<TKey, TValue> : JsonConverter<SortedDictionary<TKey, TValue>?> where TKey : notnull
    {
        private readonly Converter<string, TKey> _keyParser;
        private readonly JsonConverter<TValue> _valueConverter;

        public SortedDictionaryJsonConverter(
            Converter<string, TKey> keyParser,
            JsonConverter<TValue> valueConverter)
        {
            _keyParser = keyParser;
            _valueConverter = valueConverter;
        }

        public override SortedDictionary<TKey, TValue>? Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {   
            if (reader.TokenType == JsonTokenType.Null)
                return null;
            
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Dictionary must be JSON object.");
            
            var result = new SortedDictionary<TKey, TValue>();
            
            while (true)
            {
                if (!reader.Read())
                    throw new JsonException("Incomplete JSON object");
                
                if (reader.TokenType == JsonTokenType.EndObject)
                    return result;

                var key = _keyParser(reader.GetString());

                if (!reader.Read())
                    throw new JsonException("Incomplete JSON object");

                var value = _valueConverter.Read(ref reader, typeof(TValue), options);

                result.Add(key, value);
            }
        }

        public override void Write(
            Utf8JsonWriter writer,
            SortedDictionary<TKey, TValue>? value,
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
                    writer.WritePropertyName(pair.Key.ToString());
                    _valueConverter.Write(writer, pair.Value, options);
                }

                writer.WriteEndObject();
            }
        }
    }
}