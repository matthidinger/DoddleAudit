using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace Doddle.Linq.Audit
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
