using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Doddle.Linq.Audit
{
    public class AuditAssociation<T> : IAuditAssociation
    {
        public Type ParentEntityType { get; set; }
        public Type EntityType { get { return typeof(T); } }
        public LambdaExpression PkSelector { get; set; }
        public LambdaExpression FkSelector { get; set; }
    }
}
