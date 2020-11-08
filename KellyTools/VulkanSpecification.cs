using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

namespace KellyTools
{
    public class VulkanSpecification
    {
        public const string VkXml = "vk.xml";
        public const string VkXmlUrl = "https://raw.githubusercontent.com/KhronosGroup/Vulkan-Docs/main/xml/vk.xml";

        public static readonly ImmutableDictionary<string, string> CSharpTypeByCType =
            new Dictionary<string, string>
            {
                ["void"] = "void",
                ["char"] = "sbyte",
                ["float"] = "float",
                ["double"] = "double",
                ["uint8_t"] = "byte",
                ["uint16_t"] = "ushort",
                ["uint32_t"] = "uint",
                ["uint64_t"] = "ulong",
                ["int32_t"] = "int",
                ["int64_t"] = "long",
                ["size_t"] = nameof(UIntPtr)
            }.ToImmutableDictionary();

        private static void ParseType(
            XmlReader xmlReader,
            Dictionary<string, string> typeDefs)
        {
            if (xmlReader.AlwaysRead().NodeType == XmlNodeType.Text && xmlReader.Value.StartsWith("typedef"))
            {
                var builder = new StringBuilder(xmlReader.Value);
                var typeName = default(string);

                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        var name = xmlReader.Name;
                        var text = xmlReader.ReadText();

                        if (name == "name")
                            typeName = text;
                        
                        builder.Append(text);
                    }
                    else if (xmlReader.NodeType.IsText())
                    {
                        builder.Append(xmlReader.Value);
                    }
                    else
                    {
                        break;
                    }
                }

                var typeDef = builder.ToString();

                if (typeDef.Contains('*', StringComparison.Ordinal))
                {
                    typeDefs.Add(typeName, nameof(IntPtr));
                }
                Console.WriteLine(typeDef);
            }
            else
            {
                xmlReader.Skip();
            }
        }

        private static void ParseEnum(
            XmlReader xmlReader,
            Dictionary<string, List<KeyValuePair<string, string>>> enums,
            Dictionary<string, List<KeyValuePair<string, string>>> flagEnums)
        {
            bool isFlags = xmlReader.GetAttribute("type") == "bitmask";
            var enumName = xmlReader.GetAttribute("name");
            var values = new List<KeyValuePair<string, string>>();

            while (xmlReader.Read() && (xmlReader.NodeType != XmlNodeType.EndElement || xmlReader.Name != "enums"))
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "enum")
                {
                    var rawName = xmlReader.GetAttribute("name");
                    var value =
                        xmlReader.GetAttribute("value") ??
                        VulkanCodeGenerator.ToPascalCase(xmlReader.GetAttribute("alias")) ??
                        "1 << " + xmlReader.GetAttribute("bitpos");
                    values.Add(KeyValuePair.Create(rawName, value));
                }
            }

            if (isFlags)
                flagEnums.Add(enumName, values);
            else
                enums.Add(enumName, values);
        }

        private static void ParseConstants(
            XmlReader xmlReader,
            Dictionary<string, string> constants)
        {
            while (xmlReader.Read() && (xmlReader.NodeType != XmlNodeType.EndElement || xmlReader.Name != "enums"))
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "enum")
                {
                    var name = xmlReader.GetAttribute("name");
                    var value =
                        xmlReader.GetAttribute("value") ??
                        VulkanCodeGenerator.ToPascalCase(xmlReader.GetAttribute("alias")) ??
                        "1 << " + xmlReader.GetAttribute("bitpos");
                    constants.Add(name, value);
                }
            }
        }
        
        public static VulkanSpecification Create(XmlReader xmlReader)
        {
            Console.WriteLine("Parsing specification...");
            var stopwatch = Stopwatch.StartNew();
            var builder = new StringBuilder();

            using var destinationStream = File.Create("vk.txt");
            using var writer = new StreamWriter(destinationStream);

            var typeDefs = new Dictionary<string, string>();
            var constants = new Dictionary<string, string>();
            var enums = new Dictionary<string, List<KeyValuePair<string, string>>>();
            var flagEnums = new Dictionary<string, List<KeyValuePair<string, string>>>();

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    switch (xmlReader.Name)
                    {
                        case "type":
                            ParseType(xmlReader, typeDefs);
                            break;
                        case "enums":
                            if (xmlReader.GetAttribute("name").Contains(' '))
                                ParseConstants(xmlReader, constants);
                            else
                                ParseEnum(xmlReader, enums, flagEnums);
                            break;
                        default:
                            break;
                    }
                }
            }

            Console.WriteLine("Parsed specification in " + stopwatch.Elapsed);

            return new VulkanSpecification();
        }

        public static VulkanSpecification Create(string path)
        {
            using var xmlReader = XmlReader.Create(path);
            return Create(xmlReader);
        }
    }
}
