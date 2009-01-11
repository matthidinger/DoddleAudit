using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Doddle.Linq.Audit
{
    public class AuditDefinition<T> : IAuditDefinition
    {
        private readonly IAuditableContext _context;
        public AuditDefinition(IAuditableContext context)
        {
            _context = context;
        }
        public IAuditableContext Context
        {
            get { return _context; }
        }

        public Type EntityType
        {
            get { return typeof(T); }
        }

        public LambdaExpression PkSelector { get; set; }
        public LambdaExpression EntityDisplaySelector { get; set; }


        private readonly List<IAuditRelationship> _relationships = new List<IAuditRelationship>();
        public IList<IAuditRelationship> Relationships
        {
            get
            {
                return _relationships;
            }
        }
    }

}
