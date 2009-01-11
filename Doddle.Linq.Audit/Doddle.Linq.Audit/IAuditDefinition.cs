using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Doddle.Linq.Audit
{
    public interface IAuditDefinition
    {
        IAuditableContext Context { get; }
        Type EntityType { get; }
        IList<IAuditRelationship> Relationships { get; }
        LambdaExpression PkSelector { get; set; }
        LambdaExpression EntityDisplaySelector { get; set; }
    }
}
