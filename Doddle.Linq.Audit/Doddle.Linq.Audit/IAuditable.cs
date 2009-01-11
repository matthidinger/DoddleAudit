using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Doddle.Linq.Audit
{
    public interface IAuditable
    {
        IList<IAuditDefinition> AuditDefinitions { get; }
        AuditDefinition<T> Audit<T>(Expression<Func<T, int>> pkSelector, Expression<Func<T, string>> entityNameSelector);
    }
}