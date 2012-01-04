using System;
using System.Collections.Generic;
using System.Reflection;

namespace DoddleAudit.Tests.Fakes
{
    public class FakeAuditableContext : IAuditableContext
    {
        private readonly ContextAuditConfiguration _configuration = new ContextAuditConfiguration();
        public ContextAuditConfiguration AuditConfiguration { get { return _configuration; } }

        public int SavePendingChanges()
        {
            return 0;
        }


        private readonly IList<IEntityAuditor> _entityAuditors = new List<IEntityAuditor>();

        private readonly List<object> _pendingInserts = new List<object>();
        private readonly List<object> _pendingUpdates = new List<object>();
        private readonly List<object> _pendingDeletes = new List<object>();

        public IList<IEntityAuditor> EntityAuditors
        {
            get { return _entityAuditors; }
        }


        public void AddInsert(object entity)
        {
            _pendingInserts.Add(entity);
        }

        public void AddUpdate(object entity)
        {
            _pendingUpdates.Add(entity);
        }

        public void AddDelete(object entity)
        {
            _pendingDeletes.Add(entity);
        }

        public IEnumerable<object> PendingInserts
        {
            get { return _pendingInserts; }
        }

        public IEnumerable<object> PendingUpdates
        {
            get { return _pendingUpdates; }
        }

        public IEnumerable<object> PendingDeletes
        {
            get { return _pendingDeletes; }
        }

        public virtual void SaveAuditedEntity(AuditedEntity record)
        {

        }

        public virtual IEnumerable<ModifiedProperty> GetModifiedProperties(object entity)
        {
            throw new NotImplementedException();
        }

        public virtual PropertyInfo GetPrimaryKeyProperty(Type entityType)
        {
            //return typeof(TEntity).GetProperty("ID");
            return null;
        }

        public virtual string GetForeignKeyPropertyName(Type entityType, Type parentEntityType)
        {
            return "";
        }

 
        public Type GetEntityType(Type entityType)
        {
            return entityType;
        }
    }

}
