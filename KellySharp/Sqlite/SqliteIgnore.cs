using System;

namespace KellySharp.Sqlite
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SqliteIgnore : Attribute
    {
    }
}