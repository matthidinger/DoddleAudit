using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace Doddle.Linq.Audit
{
    public interface IAuditAssociation
    {
        IAuditDefinition AuditDefinition { get; }
        Type EntityType { get; }

        LambdaExpression PkSelector { get; set; }
        LambdaExpression FkSelector { get; set; }
    }
}
