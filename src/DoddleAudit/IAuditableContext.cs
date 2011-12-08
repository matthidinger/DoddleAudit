using System;
using System.Collections.Generic;
using System.Reflection;

namespace DoddleAudit
{
    public interface IAuditableContext
    {
        IList<IAuditDefinition> AuditDefinitions { get; }

        IEnumerable<object> Inserts { get; }
        IEnumerable<object> Updates { get; }
        IEnumerable<object> Deletes { get; }
        
        void InsertAuditRecord(AuditedEntity record);
        
        IEnumerable<MemberAudit> GetModifiedFields(object entity);
        PropertyInfo GetEntityPrimaryKey<TEntity>();
        string GetEntityRelationshipKeyName<T, TR>();

        EmptyPropertyMode EmptyPropertyMode { get; set; }

        IList<Func<MemberInfo, object, bool>> PropertyAuditRules { get; }
        IDictionary<Type, IAuditPropertyResolver> Resolvers { get; }
        bool AuditingEnabled { get; set; }
        Type GetEntityType(Type entityType);
    }
}