using System;

namespace KellySharp
{
    [Flags]
    public enum SplitEnumeration
    {
        None = 0,
        SkipEmptyItems = 1 << 0
    }
}
