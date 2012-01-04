using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DoddleAudit
{
    /// <summary>
    /// Represents an Entity which has been audited
    /// </summary>
    [DebuggerDisplay("Entity Type: {EntityType.Name}, Key: {EntityKey}, Action: {Action}")]    
    public class AuditedEntity
    {
        private readonly List<AuditedProperty> _properties = new List<AuditedProperty>();


        public AuditedEntity(object entity, AuditAction action, Action<AuditedEntity> updateKeys)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (updateKeys == null) throw new ArgumentNullException("updateKeys");

            Entity = entity;
            Action = action;
            UpdateKeys = updateKeys;
        }

        public AuditAction Action { get; private set; }

        public object Entity { get; private set; }
        public Type EntityType { get; set; }

        // TODO: Get the Entity set name
        public string EntitySetName { get; set; }
        public EntityKey EntityKey { get; set; }

        public Type ParentEntityType { get; set; }
        public EntityKey ParentKey { get; set; }


        public IList<AuditedProperty> ModifiedProperties { get { return _properties; } }

        /// <summary>
        /// New entities (inserts) will have a PK of 0 until LINQ submits changes to the DB and retrieves the real PK, so we need to update the Insert Audit record with the real PK
        /// </summary>
        internal Action<AuditedEntity> UpdateKeys { get; private set; }
    }
}
