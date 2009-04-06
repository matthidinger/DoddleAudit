using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Collections;

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

        protected void AuditRows(IEnumerable entities, AuditAction action)
        {
            if(entities == null)
                return;

            foreach (object entity in entities)
            {
                if(entity == null)
                    continue;
                

                Type entityType = entity.GetType();
                foreach (IAuditDefinition def in _context.AuditDefinitions)
                {
                    if (ShouldAuditEntity(entityType, def.EntityType))
                    {
                        AuditedEntity record = new AuditedEntity(entity, def);
                        record.Action = action;
                        record.EntityTable = entityType.Name;

                        AddModifiedPropertiesToRecord(action, entity, record);
                        _context.InsertAuditRecord(record);

                        continue;
                    }

                    foreach (IAuditAssociation relationship in def.Relationships)
                    {
                        if (ShouldAuditEntity(entityType, relationship.EntityType))
                        {
                            AuditedEntity record = new AuditedEntity(entity, relationship);
                            record.Action = action;
                            record.EntityTable = relationship.ParentEntityType.Name;
                            record.AssociationTable = entityType.Name;

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
                var mmi = _context.GetModifiedFields(entity);

                foreach (MemberAudit mi in mmi)
                {
                    if (_context.ShouldAuditProperty(mi.Member, entity))
                    {
                        AuditedEntityField values = resolver.GetAuditValue(mi.Member, mi.OriginalValue, mi.CurrentValue);
                        if (values != null)
                        {
                            record.ModifiedFields.Add(values);
                        }
                    }
                }
            }
            else if (action == AuditAction.Insert)
            {
                PropertyInfo[] props = entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                foreach (PropertyInfo pi in props)
                {
                    if (_context.ShouldAuditProperty(pi, entity))
                    {
                        AuditedEntityField values = resolver.GetAuditValue(pi, null, pi.GetValue(entity, null));
                        if (values != null)
                        {
                            record.ModifiedFields.Add(values);
                        }
                    }
                }
            }
            else
            {
                PropertyInfo[] props = entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                foreach (PropertyInfo pi in props)
                {
                    if (_context.ShouldAuditProperty(pi, entity))
                    {
                        AuditedEntityField values = resolver.GetAuditValue(pi, pi.GetValue(entity, null), null);
                        if (values != null)
                        {
                            record.ModifiedFields.Add(values);
                        }
                    }
                }
            }
        }
    }
}
