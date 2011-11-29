using System;
using System.Linq.Expressions;

namespace Doddle.Audit
{
    public interface IAuditAssociation
    {
        IAuditDefinition AuditDefinition { get; }
        Type EntityType { get; }

        LambdaExpression PkSelector { get; set; }
        LambdaExpression FkSelector { get; set; }
    }
}
