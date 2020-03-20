using System;
using System.Collections.Generic;
using System.Text.Json;
using KellySharp;
using Xunit;

namespace KellyTest
{
    public class DictionaryJsonConverterTest
    {
        [Fact]
        public void EnsureStringKeysWork()
        {
            var dictionary = new Dictionary<string, Dictionary<Guid, int>>();

            for (int i = 0; i < 4; ++i)
            {
                var innerDictionary = new Dictionary<Guid, int>();

                for (int j = 0; j < 4; ++j)
                    innerDictionary[Guid.NewGuid()] = j;
                
                dictionary.Add(i.ToString(), innerDictionary);
            }

            var options = new JsonSerializerOptions();
            options.Converters.Add(DictionaryJsonConverterFactory.Default);

            // Verify that these custom tools are necessary!
            Assert.Throws<NotSupportedException>(() => JsonSerializer.Serialize(dictionary));
            
            var serialized = JsonSerializer.Serialize(dictionary, options);
            var deserialized = JsonSerializer.Deserialize<Dictionary<string, Dictionary<Guid, int>>>(serialized, options);

            Assert.Equal(dictionary, deserialized);
        }

        [Fact]
        public void SerializeAndDeserializeGuidKey()
        {
            var dictionary = new Dictionary<Guid, int>();

            for (int i = 0; i < 4; ++i)
                dictionary[Guid.NewGuid()] = i;
            
            var options = new JsonSerializerOptions();
            options.Converters.Add(DictionaryJsonConverterFactory.Default);

            // Verify that these custom tools are necessary!
            Assert.Throws<NotSupportedException>(() => JsonSerializer.Serialize(dictionary));

            var serialized = JsonSerializer.Serialize(dictionary, options);
            var deserialized = JsonSerializer.Deserialize<Dictionary<Guid, int>>(serialized, options);

            Assert.Equal(dictionary, deserialized);
        }

        [Fact]
        public void CustomParserAndSerializer()
        {
            var dictionary = new Dictionary<int, int>();

            for (int i = 0; i < 8; ++i)
                dictionary.Add(i, i);
            
            var factory = new DictionaryJsonConverterFactoryBuilder()
                .Add(s => ~int.Parse(s), i => (~i).ToString())
                .Build();
            
            var options = new JsonSerializerOptions();
            options.Converters.Add(factory);

            // Verify that these custom tools are necessary!
            Assert.Throws<NotSupportedException>(() => JsonSerializer.Serialize(dictionary));

            var serialized = JsonSerializer.Serialize(dictionary, options);
            var deserialized = JsonSerializer.Deserialize<Dictionary<int, int>>(serialized, options);

            Assert.Equal(dictionary, deserialized);
        }
    }
}