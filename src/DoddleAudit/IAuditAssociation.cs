using System;
using System.Linq.Expressions;

namespace DoddleAudit
{
    public interface IAuditAssociation
    {
        IAuditDefinition AuditDefinition { get; }
        Type EntityType { get; }

        LambdaExpression PkSelector { get; set; }
        LambdaExpression FkSelector { get; set; }
    }
}
