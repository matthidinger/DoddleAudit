using System;
using System.Linq;
using System.Reflection;

namespace DoddleAudit
{
    public class EntityAuditor<TEntity> : EntityAuditor
    {
        public EntityAuditor(PropertyInfo primaryKey)
            : base(typeof(TEntity), primaryKey)
        {
        }

        public void WithConfiguration<TConfig>() where TConfig : EntityAuditConfiguration<TEntity>
        {
            WithConfiguration(Activator.CreateInstance<TConfig>());
        }

        public void WithConfiguration(EntityAuditConfiguration<TEntity> config)
        {
            if (config == null) throw new ArgumentNullException("config");
            Configuration = config;
        }
    }



    public abstract class EntityAuditor : IEntityAuditor
    {
        private readonly PropertyInfo _primaryKey;

        protected EntityAuditor(Type entityType, PropertyInfo primaryKey)
        {
            if (entityType == null) throw new ArgumentNullException("entityType");
            if (primaryKey == null) throw new ArgumentNullException("primaryKey");
            _primaryKey = primaryKey;
            EntityType = entityType;
            Configuration = new EntityAuditConfiguration();
        }

        public virtual AuditedEntity AuditEntity(IAuditableContext context, object entity, AuditAction action)
        {
            Action<AuditedEntity> updateKeys = audited =>
                                                   {
                                                       object pk = _primaryKey.GetValue(entity, null);
                                                       audited.EntityKey = new EntityKey(pk);
                                                       audited.ParentKey = new EntityKey(pk);

                                                       var pkName = _primaryKey.Name;
                                                       var field = audited.ModifiedProperties.SingleOrDefault(f => f.PropertyName == pkName);
                                                       if (field != null)
                                                           field.NewValue = pk.ToString();
                                                   };

            var auditedEntity = new AuditedEntity(entity, action, updateKeys)
                                    {
                                        EntityType = context.GetEntityType(entity.GetType()),
                                        ParentEntityType = context.GetEntityType(entity.GetType())
                                    };

            AddModifiedPropertiesToRecord(context, auditedEntity);

            return auditedEntity;
        }


        private AuditedProperty GetPropertyValue(PropertyInfo property, object originalValue, object currentValue)
        {
            return Configuration.AuditProperty(property, originalValue, currentValue);
        }


        protected void AddModifiedPropertiesToRecord(IAuditableContext context, AuditedEntity record)
        {
            var action = record.Action;
            var entity = record.Entity;

            if (action == AuditAction.Update)
            {
                var mmi = context.GetModifiedProperties(entity);

                foreach (ModifiedProperty mi in mmi)
                {
                    var auditedProperty = GetPropertyValue(mi.Property, mi.OriginalValue, mi.CurrentValue);
                    if (ShouldAuditProperty(context, action, mi.Property, entity, auditedProperty))
                    {
                        record.ModifiedProperties.Add(auditedProperty);
                    }
                }
            }
            else
            {
                AuditInsertDelete(context, action, entity, record);
            }
        }

        private void AuditInsertDelete(IAuditableContext context, AuditAction action, object entity, AuditedEntity record)
        {
            var props = context.GetEntityType(entity.GetType())
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            Func<PropertyInfo, object> oldValue;
            Func<PropertyInfo, object> newValue;
            if(action == AuditAction.Insert)
            {
                oldValue = pi => null;
                newValue = pi => pi.GetValue(entity, null);
            }
            else
            {
                newValue = pi => null;
                oldValue = pi => pi.GetValue(entity, null);
            }

            foreach (var pi in props)
            {
                var auditedProperty = GetPropertyValue(pi, oldValue(pi), newValue(pi));
                if (ShouldAuditProperty(context, action, pi, entity, auditedProperty))
                {
                    record.ModifiedProperties.Add(auditedProperty);
                }
            }
        }


        private bool ShouldAuditProperty(IAuditableContext context, AuditAction action, PropertyInfo pi, object entity, AuditedProperty values)
        {
            bool auditableProperty = context.ShouldAuditProperty(pi, entity) || Configuration.IsPropertyCustomized(pi);

            if (auditableProperty)
            {
                if (action == AuditAction.Insert && context.AuditConfiguration.EmptyPropertyMode.HasFlag(EmptyPropertyMode.ExcludeEmptyOnInsert))
                {
                    if (string.IsNullOrEmpty(values.NewValue))
                        return false;
                }
                else if (action == AuditAction.Delete && context.AuditConfiguration.EmptyPropertyMode.HasFlag(EmptyPropertyMode.ExcludeEmptyOnDelete))
                {
                    if (string.IsNullOrEmpty(values.OldValue))
                        return false;
                }
            }

            return auditableProperty;
        }


        bool IEntityAuditor.CanAuditType(Type entityType)
        {
            return EntityType.IsAssignableFrom(entityType);
        }


        public IEntityAuditConfiguration Configuration { get; set; }

        public Type EntityType { get; private set; }
    }

}
