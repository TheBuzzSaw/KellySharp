using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KellyTools
{
    class VulkanCodeGenerator
    {
        public const string VkXml = "vk.xml";
        public const string VkXmlUrl = "https://raw.githubusercontent.com/KhronosGroup/Vulkan-Docs/main/xml/vk.xml";

        private static string ToPascalCase(string screamingSnakeCase)
        {
            if (screamingSnakeCase is null)
            {
                return null;
            }
            else
            {
                var buffer = screamingSnakeCase.ToCharArray();
                bool doUpper = true;
                int n = 0;

                for (int i = 0; i < buffer.Length; ++i)
                {
                    if (buffer[i] == '_')
                    {
                        doUpper = true;
                    }
                    else if (doUpper)
                    {
                        buffer[n++] = char.ToUpperInvariant(buffer[i]);
                        doUpper = false;
                    }
                    else
                    {
                        buffer[n++] = char.ToLowerInvariant(buffer[i]);
                    }
                }

                return new string(buffer, 0, n);
            }
        }

        public static async Task DownloadAsync(HttpClient httpClient)
        {
            Console.WriteLine("Downloading spec...");
            var stopwatch = Stopwatch.StartNew();
            using var request = new HttpRequestMessage(HttpMethod.Get, VkXmlUrl);
            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            await using var stream = await response.Content.ReadAsStreamAsync();
            await using var fileStream = File.Create(VkXml);
            await stream.CopyToAsync(fileStream);
            Console.WriteLine("Downloaded in " + stopwatch.Elapsed);
        }

        private static void GenerateEnum(XmlReader reader, StreamWriter writer)
        {
            bool isFlags = reader.GetAttribute("type") == "bitmask";
            var enumName = reader.GetAttribute("name");
            var values = new List<KeyValuePair<string, string>>();

            while (reader.Read() && (reader.NodeType != XmlNodeType.EndElement || reader.Name != "enums"))
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "enum")
                {
                    var rawName = reader.GetAttribute("name");
                    var value = reader.GetAttribute("value") ?? ToPascalCase(reader.GetAttribute("alias")) ?? "1 << " + reader.GetAttribute("bitpos");
                    values.Add(KeyValuePair.Create(rawName, value));
                }
            }

            if (isFlags)
                writer.WriteLine("[Flags]");
            
            writer.WriteLine("public enum " + enumName);
            writer.WriteLine("{");

            for (int i = 0; i < values.Count; ++i)
            {
                var pair = values[i];
                var comma = i == values.Count - 1 ? string.Empty : ",";
                var name = ToPascalCase(pair.Key);
                writer.WriteLine($"    {name} = {pair.Value}{comma} // {pair.Key}");
            }

            writer.WriteLine("}");
            writer.WriteLine();
        }

        private static void GenerateConstants(XmlReader reader, StreamWriter writer)
        {
            writer.WriteLine("public static class Constants");
            writer.WriteLine("{");
            while (reader.Read() && (reader.NodeType != XmlNodeType.EndElement || reader.Name != "enums"))
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "enum")
                {
                    var rawName = reader.GetAttribute("name");
                    var name = ToPascalCase(rawName);
                    var value = reader.GetAttribute("value") ?? ToPascalCase(reader.GetAttribute("alias")) ?? "1 << " + reader.GetAttribute("bitpos");
                    var type = value.Contains('f') ? "float" : value.Contains("ULL") ? "ulong" : "uint";
                    writer.WriteLine($"    public const {type} {name} = {value}; // {rawName}");
                }
            }

            writer.WriteLine("}");
            writer.WriteLine();
        }

        public static void GenerateCode()
        {
            Console.WriteLine("Generating code...");
            var stopwatch = Stopwatch.StartNew();
            var builder = new StringBuilder();

            using var sourceStream = File.OpenRead(VkXml);
            using var reader = XmlReader.Create(sourceStream);
            using var destinationStream = File.Create("vk.txt");
            using var writer = new StreamWriter(destinationStream);

            writer.WriteLine("// Generated " + DateTime.UtcNow.ToString("s") + " (UTC)");

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "enums":
                            if (reader.GetAttribute("name").Contains(' '))
                                GenerateConstants(reader, writer);
                            else
                                GenerateEnum(reader, writer);
                            break;
                        default: break;
                    }
                }
            }

            Console.WriteLine("Generated code in " + stopwatch.Elapsed);
        }
    }
}