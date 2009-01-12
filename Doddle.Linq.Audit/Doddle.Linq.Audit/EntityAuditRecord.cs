using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Doddle.Linq.Audit
{
    public class EntityAuditRecord
    {
        public AuditAction Action { get; set; }

        public object Entity { get; set; }
        public LambdaExpression KeySelector { get; set; }

        public string EntityTable { get; set; }
        public int EntityTableKey { get; set; }

        public string AssociationTable { get; set; }
        public int? AssociationTableKey { get; set; }


        private readonly List<ModifiedEntityProperty> _properties = new List<ModifiedEntityProperty>();
        public IList<ModifiedEntityProperty> ModifiedProperties { get { return _properties; } }
    }
}
