using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DoddleAudit
{
    public interface IAuditDefinition
    {
        IAuditableContext Context { get; }
        Type EntityType { get; }
        IList<IAuditAssociation> Relationships { get; }
        LambdaExpression PkSelector { get; set; }
    }
}
