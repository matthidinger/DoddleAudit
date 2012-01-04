using System;
using System.Collections.Generic;
using System.Reflection;

namespace DoddleAudit
{
    public interface IAuditableContext
    {
        IList<IEntityAuditor> EntityAuditors { get; }

        IEnumerable<object> PendingInserts { get; }
        IEnumerable<object> PendingUpdates { get; }
        IEnumerable<object> PendingDeletes { get; }

        /// <summary>
        /// Use this method to define how to save the actual audit record into the database
        /// </summary>
        void SaveAuditedEntity(AuditedEntity record);
        
        IEnumerable<ModifiedProperty> GetModifiedProperties(object entity);

        PropertyInfo GetPrimaryKeyProperty(Type entityType);
        string GetForeignKeyPropertyName(Type entityType, Type parentEntityType);

        Type GetEntityType(Type entityType);

        ContextAuditConfiguration AuditConfiguration { get; }
        int SavePendingChanges();
    }
}