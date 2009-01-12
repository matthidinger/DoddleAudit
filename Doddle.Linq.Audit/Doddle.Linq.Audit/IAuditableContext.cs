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
        void InsertAuditRecord(EntityAuditRecord record);
        IEnumerable<MemberAudit> GetModifiedMembers(object entity);

        MemberInfo GetEntityPrimaryKey<TEntity>();
        string GetEntityRelationshipKeyName<T, TR>();
    }

    public class MemberAudit
    {
        public MemberInfo Member { get; set; }
        public object OriginalValue { get; set; }
        public object CurrentValue { get; set; }
    }
}