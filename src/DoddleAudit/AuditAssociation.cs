using System;
using System.Linq.Expressions;

namespace DoddleAudit
{
    public class AuditAssociation<T> : IAuditAssociation
    {
        public AuditAssociation(IAuditDefinition definition)
        {
            AuditDefinition = definition;
        }
        public IAuditDefinition AuditDefinition { get; set; }

        public Type EntityType { get { return typeof(T); } }
        public LambdaExpression PkSelector { get; set; }
        public LambdaExpression FkSelector { get; set; }
    }
}
