using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Doddle.Linq.Audit
{
    public interface IAuditRelationship
    {
        LambdaExpression EntityDisplaySelector { get; set; }
        Type PrimaryEntityType { get; }
        Type RelationshipEntityType { get; }
        LambdaExpression PkSelector { get; set; }
        LambdaExpression FkSelector { get; set; }
        
    }
}
