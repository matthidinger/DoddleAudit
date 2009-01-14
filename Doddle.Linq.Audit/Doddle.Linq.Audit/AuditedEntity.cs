using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Doddle.Linq.Audit
{
    public class AuditedEntity
    {
        public AuditAction Action { get; set; }

        public object Entity { get; set; }

        public string EntityTable { get; set; }
        public EntityKey EntityTableKey { get; set; }
        public LambdaExpression PrimaryKeySelector { get; set; }

        public string AssociationTable { get; set; }
        public EntityKey AssociationTableKey { get; set; }

        private readonly List<AuditedEntityField> _fields = new List<AuditedEntityField>();
        public IList<AuditedEntityField> ModifiedFields { get { return _fields; } }
    }
}
