using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Doddle.Linq.Audit
{
    public class AuditProcessor
    {
        private readonly IAuditableContext _context;
        public AuditProcessor(IAuditableContext context)
        {
            _context = context;
        }

        public void Process()
        {
            AuditRows(_context.Inserts, AuditAction.Insert);
            AuditRows(_context.Updates, AuditAction.Update);
            AuditRows(_context.Deletes, AuditAction.Delete);
        }

        protected void AuditRows(IEnumerable<object> entities, AuditAction action)
        {
            if(entities == null)
                return;

            foreach (object entity in entities)
            {
                Type entityType = entity.GetType();
                foreach (IAuditDefinition def in _context.AuditDefinitions)
                {
                    if (ShouldAuditEntity(entityType, def.EntityType))
                    {
                        object pk = def.PkSelector.Compile().DynamicInvoke(entity);

                        AuditedEntity record = new AuditedEntity();
                        record.Entity = entity;
                        record.PrimaryKeySelector = def.PkSelector;
                        record.Action = action;
                        record.EntityTable = entityType.Name;
                        record.EntityTableKey = new EntityKey(pk);

                        //record.AssociationTable = entityType.Name;
                        //record.AssociationTableKey = pk;

                        AddModifiedPropertiesToRecord(action, entity, record);
                        _context.InsertAuditRecord(record);

                        continue;
                    }

                    foreach (IAuditAssociation relationship in def.Relationships)
                    {
                        if (ShouldAuditEntity(entityType, relationship.EntityType))
                        {
                            object fk = relationship.FkSelector.Compile().DynamicInvoke(entity);
                            object pk = relationship.PkSelector.Compile().DynamicInvoke(entity);

                            AuditedEntity record = new AuditedEntity();
                            record.Entity = entity;
                            record.PrimaryKeySelector = relationship.PkSelector;
                            record.Action = action;
                            record.EntityTable = relationship.ParentEntityType.Name;
                            record.EntityTableKey = new EntityKey(fk);

                            record.AssociationTable = entityType.Name;
                            record.AssociationTableKey = new EntityKey(pk);

                            AddModifiedPropertiesToRecord(action, entity, record);
                            _context.InsertAuditRecord(record);

                        }
                    }
                }
            }
        }

        private static bool ShouldAuditEntity(Type entityType, Type auditType)
        {
            return entityType == auditType || entityType.BaseType == auditType;
        }

        private void AddModifiedPropertiesToRecord(AuditAction action, object entity, AuditedEntity record)
        {
            Type entityType = entity.GetType();
            IAuditPropertyResolver resolver = AuditPropertyResolver.GetResolver(entityType);

            if (action == AuditAction.Update)
            {
                var mmi = _context.GetModifiedMembers(entity);

                foreach (MemberAudit mi in mmi)
                {
                    ModifiedEntityProperty values = resolver.GetAuditValue(mi.Member, mi.OriginalValue, mi.CurrentValue);
                    if (values != null)
                    {
                        record.ModifiedProperties.Add(values);
                    }
                }
            }
            else if (action == AuditAction.Insert)
            {
                PropertyInfo[] props = entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                foreach (PropertyInfo pi in props)
                {
                    ModifiedEntityProperty values = resolver.GetAuditValue(pi, null, pi.GetValue(entity, null));
                    if (values != null)
                    {
                        record.ModifiedProperties.Add(values);
                    }
                }
            }
            else
            {
                PropertyInfo[] props = entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                foreach (PropertyInfo pi in props)
                {
                    ModifiedEntityProperty values = resolver.GetAuditValue(pi, pi.GetValue(entity, null), null);
                    if (values != null)
                    {
                        record.ModifiedProperties.Add(values);
                    }
                }
            }
        }
    }
}
