using System;

namespace DoddleAudit
{
    /// <summary>
    /// Specify how to audit properties which have a null or empty value
    /// </summary>
    [Flags]
    public enum EmptyPropertyMode
    {
        AlwaysAudit = 0,
        ExcludeEmptyOnInsert = 1,
        ExcludeEmptyOnDelete = 2,
        NeverAudit = ExcludeEmptyOnInsert | ExcludeEmptyOnDelete
    }
}