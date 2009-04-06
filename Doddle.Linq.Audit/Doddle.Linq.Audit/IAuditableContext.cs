using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace Doddle.Linq.Audit
{
    public interface IAuditableContext
    {
        IList<IAuditDefinition> AuditDefinitions { get; }

        IEnumerable Inserts { get; }
        IEnumerable Updates { get; }
        IEnumerable Deletes { get; }
        
        void InsertAuditRecord(AuditedEntity record);
        
        IEnumerable<MemberAudit> GetModifiedFields(object entity);
        PropertyInfo GetEntityPrimaryKey<TEntity>();
        string GetEntityRelationshipKeyName<T, TR>();

        IList<Func<MemberInfo, object, bool>> PropertyAuditRules { get; }
    }
}