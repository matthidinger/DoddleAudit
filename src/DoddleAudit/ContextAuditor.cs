using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Transactions;

namespace DoddleAudit
{
    public class ContextAuditor
    {
        private readonly IAuditableContext _context;
        private readonly List<AuditedEntity> _auditedEntities = new List<AuditedEntity>();
        private bool _initialized;

        public ContextAuditor(IAuditableContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            _context = context;
        }

        public int AuditAndSaveChanges()
        {
            using (var scope = new TransactionScope())
            {
                if (!_initialized)
                {
                    _context.AuditConfiguration.PropertyAuditRules.Add(ShouldAuditProperty);
                    _initialized = true;
                }

                if (_context.AuditConfiguration.AuditingEnabled)
                {
                    AuditRows(_context.PendingInserts, AuditAction.Insert);
                    AuditRows(_context.PendingUpdates, AuditAction.Update);
                    AuditRows(_context.PendingDeletes, AuditAction.Delete);
                }

                var result = _context.SavePendingChanges();

                foreach (var record in _auditedEntities)
                {
                    if (record.Action == AuditAction.Insert)
                    {
                        record.UpdateKeys(record);
                    }

                    _context.SaveAuditedEntity(record);
                }

                _context.SavePendingChanges();
                scope.Complete();
                return result;
            }
        }


        private static bool ShouldAuditProperty(PropertyInfo property, object entity)
        {
            if (property.PropertyType.IsPrimitive)
                return true;

            if (property.PropertyType.IsNullable())
                return true;

            if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(string))
                return true;

            return false;
        }


        protected void AuditRows(IEnumerable entities, AuditAction action)
        {
            if (entities == null)
                return;

            foreach (var entity in entities)
            {
                if (entity == null)
                    continue;

                var entityType = _context.GetEntityType(entity.GetType());
                var entityAuditor = _context.GetEntityAuditor(entityType);

                if (entityAuditor != null)
                {
                    var auditedEntity = entityAuditor.AuditEntity(_context, entity, action);
                    _auditedEntities.Add(auditedEntity);
                }
            }
        }
    }
}
