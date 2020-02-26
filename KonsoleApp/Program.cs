using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using KellySharp;

namespace KonsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var dictionary = new Dictionary<Guid, string>();

            for (int i = 0; i < 8; ++i)
            {
                var c = (char)('a' + i);
                dictionary.Add(Guid.NewGuid(), new string(c, 8));
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var builder = new DictionaryJsonConverterFactoryBuilder();

            options.Converters.Add(DictionaryJsonConverterFactory.Default);

            var d = JsonSerializer.Deserialize<ImmutableSortedDictionary<Guid, int>>("{\"49fc2162-744a-4a42-b685-ea1e30ce2a2f\": 99}", options);

            var json = JsonSerializer.Serialize(Guid.NewGuid(), options);
            Console.WriteLine(json);

            var bytes = await File.ReadAllBytesAsync("input.json");
            var data = JsonSerializer.Deserialize<Dictionary<Guid, SortedDictionary<Guid, int>>>(bytes, options);
            Console.WriteLine("Parse successful!");
            Console.WriteLine(JsonSerializer.Serialize(data, options));
        }
    }
}
