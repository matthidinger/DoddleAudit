using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace Doddle.Linq.Audit
{
    public class AuditedEntity
    {
        public AuditedEntity(object entity, IAuditDefinition auditDefinition)
        {
            AuditDefinition = auditDefinition;
            Entity = entity;
            UpdateKeys();
        }

        public AuditedEntity(object entity, IAuditAssociation auditAssociation)
        {
            AuditAssociation = auditAssociation;
            Entity = entity;
            UpdateKeys();
        }

        public AuditAction Action { get; set; }

        public object Entity { get; set; }

        public string EntityTable { get; set; }
        public EntityKey EntityTableKey { get; set; }

        public string AssociationTable { get; set; }
        public EntityKey AssociationTableKey { get; set; }

        private readonly List<AuditedEntityField> _fields = new List<AuditedEntityField>();
        public IList<AuditedEntityField> ModifiedFields { get { return _fields; } }

        internal IAuditDefinition AuditDefinition { get; set; }
        internal IAuditAssociation AuditAssociation { get; set; }

        /// <summary>
        /// New entities (inserts) will have a PK of 0 until LINQ submits changes to the DB and retrieves the real PK, so we need to update the Insert Audit record with the real PK
        /// </summary>
        internal void UpdateKeys()
        {
            if (AuditAssociation != null)
            {
                object pk = AuditAssociation.PkSelector.Compile().DynamicInvoke(Entity);
                AssociationTableKey = new EntityKey(pk);
                
                // Temporary hack to get around Entity Framework complications, including not providing access to FK properties.
                // When DeleteObject() is called it nulls out all references for some reason, making the FK Selector delegate fail
                try
                {
                    object fk = AuditAssociation.FkSelector.Compile().DynamicInvoke(Entity);
                    EntityTableKey = new EntityKey(fk);
                }
                catch
                {
                    EntityTableKey = new EntityKey(0);
                }
            }
            else
            {
                object pk = AuditDefinition.PkSelector.Compile().DynamicInvoke(Entity);
                EntityTableKey = new EntityKey(pk);
            }
        }
    }
}
