using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Doddle.Linq.Audit
{
    public class EntityKey
    {
        public EntityKey(object key)
        {
            Key = key;
            KeyType = key.GetType();
        }

        public object Key { get; private set; }
        public Type KeyType { get; private set; }
    }

    public class AuditedEntity
    {
        public AuditAction Action { get; set; }

        public object Entity { get; set; }

        public string EntityTable { get; set; }
        public EntityKey EntityTableKey { get; set; }
        public LambdaExpression PrimaryKeySelector { get; set; }


        public string AssociationTable { get; set; }
        public EntityKey AssociationTableKey { get; set; }


        private readonly List<ModifiedEntityProperty> _properties = new List<ModifiedEntityProperty>();
        public IList<ModifiedEntityProperty> ModifiedProperties { get { return _properties; } }
    }
}
