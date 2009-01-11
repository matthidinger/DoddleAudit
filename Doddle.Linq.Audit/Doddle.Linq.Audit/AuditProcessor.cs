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
            AuditThem(_context.Inserts, AuditAction.Insert);
            AuditThem(_context.Updates, AuditAction.Update);
            AuditThem(_context.Deletes, AuditAction.Delete);
        }

        public void AuditThem(IEnumerable<object> entities, AuditAction action)
        {
            foreach (object entity in entities)
            {
                Type entityType = entity.GetType();
                foreach (IAuditDefinition def in _context.AuditDefinitions)
                {
                    if (entityType == def.EntityType)
                    {
                        int pk = (int)def.PkSelector.Compile().DynamicInvoke(entity);

                        AuditRecord record = new AuditRecord();
                        record.Entity = entity;
                        record.KeySelector = def.PkSelector;
                        record.ChangeDescription = entityType.Name + " removed";
                        record.Action = action;
                        record.PrimaryTable = entityType.Name;
                        record.PrimaryTableKey = pk;

                        record.ModifiedTable = entityType.Name;
                        record.ModifiedTableKey = pk;

                        AddValuesToRecord(action, entity, record);
                        _context.QueueAudit(record);

                        continue;
                    }

                    foreach (IAuditRelationship relationship in def.Relationships)
                    {
                        if (entityType == relationship.RelationshipEntityType)
                        {
                            int fk = (int)relationship.FkSelector.Compile().DynamicInvoke(entity);
                            int pk = (int)relationship.PkSelector.Compile().DynamicInvoke(entity);

                            AuditRecord record = new AuditRecord();
                            record.Entity = entity;
                            record.KeySelector = relationship.PkSelector;
                            record.ChangeDescription = entityType.Name + " removed";
                            record.Action = action;
                            record.PrimaryTable = relationship.PrimaryEntityType.Name;
                            record.PrimaryTableKey = fk;

                            record.ModifiedTable = entityType.Name;
                            record.ModifiedTableKey = pk;

                            AddValuesToRecord(action, entity, record);
                            _context.QueueAudit(record);

                        }
                    }
                }
            }
        }

        private void AddValuesToRecord(AuditAction action, object entity, AuditRecord record)
        {
            Type entityType = entity.GetType();
            IAuditValueResolver resolver = AuditValueResolver.GetResolver(entityType);

            if (action == AuditAction.Update)
            {
                var mmi = _context.GetModifiedMembers(entity);

                foreach (MemberAudit mi in mmi)
                {
                    AuditValue values = resolver.GetAuditValue(mi.Member, mi.OriginalValue, mi.CurrentValue);
                    if (values != null)
                    {
                        record.Values.Add(values);
                    }
                }
            }
            else if (action == AuditAction.Insert)
            {
                PropertyInfo[] props = entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                foreach (PropertyInfo pi in props)
                {
                    AuditValue values = resolver.GetAuditValue(pi, null, pi.GetValue(entity, null));
                    if (values != null)
                    {
                        record.Values.Add(values);
                    }
                }
            }
            else
            {
                PropertyInfo[] props = entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                foreach (PropertyInfo pi in props)
                {
                    AuditValue values = resolver.GetAuditValue(pi, pi.GetValue(entity, null), null);
                    if (values != null)
                    {
                        record.Values.Add(values);
                    }
                }
            }
        }
    }
}
