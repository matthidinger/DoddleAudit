using System.Collections.Generic;
using System.Reflection;

namespace Doddle.Linq.Audit
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
    }
}