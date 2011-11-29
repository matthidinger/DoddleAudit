using System;
using System.Reflection;
using System.Collections;

namespace Doddle.Audit
{
    public class ContextAuditor
    {
        private readonly IAuditableContext _context;

        public ContextAuditor(IAuditableContext context)
        {
            _context = context;
        }

        public void AuditPendingDataModifications()
        {
            if (_context.AuditingEnabled)
            {
                AuditRows(_context.Inserts, AuditAction.Insert);
                AuditRows(_context.Updates, AuditAction.Update);
                AuditRows(_context.Deletes, AuditAction.Delete);
            }
        }

        protected void AuditRows(IEnumerable entities, AuditAction action)
        {
            if(entities == null)
                return;

            foreach (object entity in entities)
            {
                if(entity == null)
                    continue;
                

                Type entityType = _context.GetEntityType(entity.GetType());
                foreach (IAuditDefinition def in _context.AuditDefinitions)
                {
                    if (ShouldAuditEntity(entityType, def.EntityType))
                    {
                        var record = new AuditedEntity(entity, def)
                                         {
                                             Action = action, 
                                             EntityTable = _context.GetEntityType(entityType).Name
                                         };

                        AddModifiedPropertiesToRecord(action, entity, record);
                        _context.InsertAuditRecord(record);

                        continue;
                    }

                    foreach (IAuditAssociation relationship in def.Relationships)
                    {
                        if (ShouldAuditEntity(entityType, relationship.EntityType))
                        {
                            var record = new AuditedEntity(entity, relationship)
                                             {
                                                 Action = action,
                                                 EntityTable = _context.GetEntityType(relationship.AuditDefinition.EntityType).Name,
                                                 AssociationTable = _context.GetEntityType(relationship.EntityType).Name
                                             };

                            AddModifiedPropertiesToRecord(action, entity, record);
                            _context.InsertAuditRecord(record);
                        }
                    }
                }
            }
        }

        private static bool ShouldAuditEntity(Type entityType, Type auditType)
        {
            return auditType.IsAssignableFrom(entityType);
        }

        private bool ShouldAuditProperty(AuditAction action, MemberInfo pi, object entity, IAuditPropertyResolver resolver, AuditedEntityField values)
        {
            bool auditableProperty = _context.ShouldAuditProperty(pi, entity) || resolver.IsMemberCustomized(pi);
            
            if(auditableProperty)
            {
                if (action == AuditAction.Insert && _context.EmptyPropertyMode.HasFlag(EmptyPropertyMode.ExcludeEmptyOnInsert))
                {
                    if(string.IsNullOrEmpty(values.NewValue))
                        return false;
                }
                else if (action == AuditAction.Delete && _context.EmptyPropertyMode.HasFlag(EmptyPropertyMode.ExcludeEmptyOnDelete))
                {
                    if (string.IsNullOrEmpty(values.OldValue))
                        return false;
                }
            }

            return auditableProperty;
        }

        private void AddModifiedPropertiesToRecord(AuditAction action, object entity, AuditedEntity record)
        {
            var entityType = _context.GetEntityType(entity.GetType());
            var resolver = AuditPropertyResolver.GetResolver(entityType);
            if(resolver == null)
            {
                if(!_context.Resolvers.TryGetValue(entityType, out resolver))
                {
                    resolver = new AuditPropertyResolver();
                }
            }
            

            if (action == AuditAction.Update)
            {
                var mmi = _context.GetModifiedFields(entity);

                foreach (MemberAudit mi in mmi)
                {
                    var values = resolver.GetAuditValue(mi.Member, mi.OriginalValue, mi.CurrentValue);
                    if (ShouldAuditProperty(action, mi.Member, entity, resolver, values))
                    {
                        record.ModifiedFields.Add(values);
                    }
                }
            }
            else if (action == AuditAction.Insert)
            {
                var props = _context.GetEntityType(entity.GetType()).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                foreach (PropertyInfo pi in props)
                {
                    var newValue = pi.GetValue(entity, null);
                    var values = resolver.GetAuditValue(pi, null, newValue);

                    if (ShouldAuditProperty(action, pi, entity, resolver, values))
                    {
                        record.ModifiedFields.Add(values);
                    }
                }
            }
            else
            {
                var props = _context.GetEntityType(entity.GetType()).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                foreach (PropertyInfo pi in props)
                {
                    if (_context.ShouldAuditProperty(pi, entity))
                    {
                        var oldValue = pi.GetValue(entity, null);
                        var values = resolver.GetAuditValue(pi, oldValue, null);
                        if (ShouldAuditProperty(action, pi, entity, resolver, values))
                        {
                            record.ModifiedFields.Add(values);
                        }
                    }
                }
            }
        }
    }
}
