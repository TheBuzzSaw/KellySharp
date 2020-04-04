using System;

namespace KellySharp
{
    public static class Base64
    {
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        public static string Encode(ReadOnlySpan<byte> bytes) => Encode(bytes, Chars);
        public static string Encode(ReadOnlySpan<byte> bytes, ReadOnlySpan<char> characters)
        {
            if (characters.Length < 64)
                throw new ArgumentException("Must supply at least 64 characters.", nameof(characters));
            
            if (bytes.IsEmpty)
                return string.Empty;
            
            int blockCount = (bytes.Length + 2) / 3;
            var result = new char[blockCount * 4];
            int resultIndex = 0;

            for (int i = 2; i < bytes.Length; i += 3)
            {
                int index0 = (bytes[i - 2] & 0xfc) >> 2;
                int index1 = ((bytes[i - 2] & 0x3) << 4) | ((bytes[i - 1] & 0xf0) >> 4);
                int index2 = ((bytes[i - 1] & 0xf) << 2) | ((bytes[i] & 0xc0) >> 6);
                int index3 = bytes[i] & 0x3f;
                
                result[resultIndex++] = characters[index0];
                result[resultIndex++] = characters[index1];
                result[resultIndex++] = characters[index2];
                result[resultIndex++] = characters[index3];
            }

            int remainder = bytes.Length % 3;

            if (remainder > 0)
            {
                int n = bytes.Length - remainder;
                int index0 = (bytes[n] & 0xfc) >> 2;
                result[resultIndex++] = characters[index0];
                
                if (remainder > 1)
                {
                    int index1 = ((bytes[n] & 0x3) << 4) | ((bytes[n + 1] & 0xf0) >> 4);
                    int index2 = (bytes[n + 1] & 0xf) << 2;
                    result[resultIndex++] = characters[index1];
                    result[resultIndex++] = characters[index2];
                }
                else
                {
                    int index1 = (bytes[n] & 0x3) << 4;
                    result[resultIndex++] = characters[index1];
                }
            }

            while (resultIndex < result.Length)
                result[resultIndex++] = '=';

            return new string(result);
        }
    }
}