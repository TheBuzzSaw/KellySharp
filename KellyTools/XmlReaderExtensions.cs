using System;
using System.Xml;

namespace KellyTools
{
    public static class XmlReaderExtensions
    {
        public static XmlReader AlwaysRead(this XmlReader xmlReader)
        {
            if (!xmlReader.Read())
                throw new InvalidOperationException();
            
            return xmlReader;
        }

        public static string ReadText(this XmlReader xmlReader)
        {
            var result = default(string);

            if (xmlReader.Read())
            {
                if (xmlReader.NodeType.IsText())
                    result = xmlReader.Value;
                
                xmlReader.Skip();
            }
            
            return result;
        }

        public static bool IsText(this XmlNodeType xmlNodeType)
        {
            return
                xmlNodeType == XmlNodeType.Text ||
                xmlNodeType == XmlNodeType.Whitespace ||
                xmlNodeType == XmlNodeType.SignificantWhitespace;
        }
    }
}
