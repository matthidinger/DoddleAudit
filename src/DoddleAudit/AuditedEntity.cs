using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DoddleAudit.Helpers;

namespace DoddleAudit
{
    [DebuggerDisplay("Entity: {EntityTable}, Key: {EntityTableKey}, Action: {Action}")]
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
                
                object fk = AuditAssociation.FkSelector.Compile().DynamicInvoke(Entity);
                EntityTableKey = new EntityKey(fk);

            }
            else
            {
                object pk = AuditDefinition.PkSelector.Compile().DynamicInvoke(Entity);

                var pkName = AuditDefinition.PkSelector.ToPropertyInfo().Name;
                var field = ModifiedFields.SingleOrDefault(f => f.FieldName == pkName);
                if (field != null)
                    field.NewValue = pk.ToString();

                EntityTableKey = new EntityKey(pk);
            }
        }
    }
}
