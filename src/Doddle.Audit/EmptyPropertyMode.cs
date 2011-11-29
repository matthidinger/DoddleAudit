using System;

namespace Doddle.Audit
{
    [Flags]
    public enum EmptyPropertyMode
    {
        None = 0,
        ExcludeEmptyOnInsert = 1,
        ExcludeEmptyOnDelete = 2,
        ExcludeEmptyOnInsertOrDelete = ExcludeEmptyOnInsert | ExcludeEmptyOnDelete
    }
}