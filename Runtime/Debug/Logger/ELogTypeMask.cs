using System;

namespace UniUtils.Debugging
{
    /// <summary>
    /// Represents a bitmask for categorizing log types in debugging.
    /// </summary>
    [Flags]
    public enum ELogTypeMask
    {
        None     = 0,
        Log      = 1 << 0,
        Warning  = 1 << 1,
        Error    = 1 << 2,
        Assert   = 1 << 3,
        Exception = 1 << 4,
        All      = ~0,
    }
}