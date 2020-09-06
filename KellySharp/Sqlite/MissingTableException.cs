using System;

namespace KellySharp.Sqlite
{
    public class MissingTableException : Exception
    {
        public MissingTableException(string message) : base(message)
        {}
    }
}