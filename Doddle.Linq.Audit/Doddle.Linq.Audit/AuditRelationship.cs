using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Doddle.Linq.Audit
{
    public class AuditRelationship<T> : IAuditRelationship
    {
        public Type PrimaryEntityType { get; set; }
        public Type RelationshipEntityType { get { return typeof(T); } }
        public LambdaExpression PkSelector { get; set; }
        public LambdaExpression FkSelector { get; set; }
        public LambdaExpression EntityDisplaySelector { get; set; }
    }
}
