using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using KellySharp;

namespace KonsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var random = new Random();
            Console.WriteLine(Convert.ToBase64String(new byte[]{11}));

            for (int i = 0; i < 16; ++i)
            {
                var bytes = new byte[random.Next(32)];
                random.NextBytes(bytes);

                var a = Convert.ToBase64String(bytes);
                var b = Base64.Encode(bytes);
                var pass = a == b;
                Console.WriteLine($"{pass} -- [{a}] vs [{b}]");
            }
        }
        
        static async Task Main2(string[] args)
        {
            var semaphore = new SemaphoreSlim(1);
            // using (await AutoSemaphore.WaitAsync(semaphore));
            
            // using new Disposable<string>("Farewell", Console.WriteLine);
            // using var disposable = Disposable.Create("Goodbye", Console.WriteLine);
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

            var intConverter = (JsonConverter<int>)options.GetConverter(typeof(int));

            var types = new Type[]
            {
                typeof(int),
                typeof(long),
                typeof(decimal),
                typeof(string),
                typeof(DateTime)
            };

            foreach (var type in types)
            {
                var converter = options.GetConverter(type);

                Console.WriteLine($"{type} -- {converter}");
            }

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
