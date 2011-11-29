using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.Linq;
using System.Reflection;
using System.Transactions;
using Doddle.Audit.Helpers;

namespace Doddle.Audit.EntityFramework
{
    public abstract class AuditableDataContext : DbContext, IAuditableContext
    {
        #region Constructors

        protected AuditableDataContext()
        {
            AuditingEnabled = true;
        }

        protected AuditableDataContext(string connectionString)
            : base(connectionString)
        {
            AuditingEnabled = true;            
        }

        protected AuditableDataContext(DbCompiledModel model) : base(model)
        {
            AuditingEnabled = true;
        }

        protected AuditableDataContext(string nameOrConnectionString, DbCompiledModel model) : base(nameOrConnectionString, model)
        {
            AuditingEnabled = true;
        }

        protected AuditableDataContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection)
        {
            AuditingEnabled = true;
        }

        protected AuditableDataContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection) : base(existingConnection, model, contextOwnsConnection)
        {
            AuditingEnabled = true;
        }

        protected AuditableDataContext(ObjectContext objectContext, bool dbContextOwnsObjectContext) : base(objectContext, dbContextOwnsObjectContext)
        {
            AuditingEnabled = true;
        }

        #endregion


        /// <summary>
        /// This method defines how to save the actual audit record into the database
        /// </summary>
        /// <param name="record"></param>
        protected abstract void SaveAuditedEntity(AuditedEntity record);


        /// <summary>
        /// Define all default audits that should be applied to this database
        /// </summary>
        protected virtual void DefaultAuditDefinitions()
        {

        }


        private readonly List<IAuditDefinition> _auditDefinitions = new List<IAuditDefinition>();
        private readonly List<AuditedEntity> _queuedRecords = new List<AuditedEntity>();
        private readonly List<Func<MemberInfo, object, bool>> _propertyAuditRules = new List<Func<MemberInfo, object, bool>>();



        private bool _initialized;

        protected virtual int SaveChangesAndAudit()
        {
            using (var scope = new TransactionScope())
            {
                if (!_initialized)
                {
                    PropertyAuditRules.Add(ShouldAuditProperty);
                    DefaultAuditDefinitions();
                    _initialized = true;
                }

                var auditor = new ContextAuditor(this);
                auditor.AuditPendingDataModifications();

                base.SaveChanges();

                foreach (var record in _queuedRecords)
                {
                    if (record.Action == AuditAction.Insert)
                    {
                        record.UpdateKeys();
                    }
                    SaveAuditedEntity(record);
                }

                var result = base.SaveChanges();
                scope.Complete();
                return result;
            }
        }

        public override int SaveChanges()
        {
            return SaveChangesAndAudit();
        }

        private bool ShouldAuditProperty(MemberInfo member, object entity)
        {
            var pi = GetEntityType(entity.GetType()).GetProperty(member.Name);

            if (pi.PropertyType.IsPrimitive)
                return true;

            if (pi.PropertyType.IsNullable())
                return true;

            if (pi.PropertyType == typeof(decimal) || pi.PropertyType == typeof(string))
                return true;

            return false;
        }

        public IList<IAuditDefinition> AuditDefinitions
        {
            get { return _auditDefinitions; }
        }

        public IEnumerable<object> Inserts
        {
            get
            {
                var entries = ObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added);

                foreach (var entry in entries)
                {
                    if (entry.IsRelationship)
                    {
                        var relationshipAudit = new RelationshipChangeEntry();
                        for (int i = 0; i < entry.CurrentValues.FieldCount; i++)
                        {
                            var key = (System.Data.EntityKey) entry.CurrentValues.GetValue(i);

                            if (!key.IsTemporary)
                            {
                                // TODO: Need to support composite keys here??
                                relationshipAudit.Entities.Add(
                                    new RelationshipChangeEntity
                                        {
                                            TableName = key.EntitySetName,
                                            KeyName = key.EntityKeyValues[0].Key,
                                            KeyValue = key.EntityKeyValues[0].Value
                                        });
                            }
                        }

                        yield return relationshipAudit;
                    }
                    else
                    {
                        yield return entry.Entity;
                    }
                }
            }
        }

        public IEnumerable<object> Updates
        {
            get { return ChangeTracker.Entries().Where(e => e.State == EntityState.Modified).Select(o => o.Entity); }
        }

        public IEnumerable<object> Deletes
        {
            get { return ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted).Select(o => o.Entity); }
        }

        public void InsertAuditRecord(AuditedEntity record)
        {
            _queuedRecords.Add(record);
        }

        public IEnumerable<MemberAudit> GetModifiedFields(object entity)
        {
            var entry = ObjectContext.ObjectStateManager.GetObjectStateEntry(entity);
            EntityType type = (from meta in ObjectContext.MetadataWorkspace.GetItems<EntityType>(DataSpace.CSpace)
                               select meta)
                                          .Where(e => e.Name == GetEntityType(entity.GetType()).Name)
                                          .Single();


            var navKeys = (from np in type.NavigationProperties
                           select new
                                      {
                                          NavProp = np,
                                          Key = np.GetDependentProperties().FirstOrDefault()
                                      }).ToList();

            var modifiedProperties = entry.GetModifiedProperties()
                .Select(
                    p =>
                    new MemberAudit(GetEntityType(entity.GetType()).GetProperty(p), entry.OriginalValues[p],
                                    entry.CurrentValues[p])).ToList();

            foreach (var property in entry.GetModifiedProperties())
            {
                var fk = navKeys.Where(np => np.Key != null).FirstOrDefault(nk => nk.Key.Name == property);
                if (fk == null)
                    continue;

                // TODO: Add config property to allow this additional query
                var pi = GetEntityType(entity.GetType()).GetProperty(fk.NavProp.Name);


                EntityContainer container = ObjectContext.MetadataWorkspace.GetEntityContainer(ObjectContext.DefaultContainerName, DataSpace.CSpace);

                EntitySetBase entitySet = container.BaseEntitySets
                    .Where(item => item.ElementType.Name.Equals(pi.PropertyType.Name))
                    .FirstOrDefault();


                var originalValue = entry.OriginalValues[property];
                object old = null;
                if (originalValue != null && originalValue != DBNull.Value)
                {
                    // TODO: Get other key property from NavigationProperty
                    var oldKey = new System.Data.EntityKey(string.Format("{0}.{1}", container.Name, entitySet.Name),
                                                           "Id",
                                                           originalValue);
                    try
                    {
                        old = ObjectContext.GetObjectByKey(oldKey);
                    }
                    catch (ObjectNotFoundException)
                    {
                    }
                }


                var currentValue = entry.CurrentValues[property];
                object newObj = null;

                if (currentValue != null && currentValue != DBNull.Value)
                {
                    var newKey = new System.Data.EntityKey(string.Format("{0}.{1}", container.Name, entitySet.Name),
                                                           "Id",
                                                           currentValue);
                    try
                    {
                        newObj = ObjectContext.GetObjectByKey(newKey);
                    }
                    catch (ObjectNotFoundException)
                    {
                    }
                }


                modifiedProperties.Add(new MemberAudit(pi, old, newObj));
            }

            return modifiedProperties;
        }

        public PropertyInfo GetEntityPrimaryKey<TEntity>()
        {
            Type entityType = typeof(TEntity);
            return GetEntityPrimaryKey(entityType);
        }

        private ObjectContext ObjectContext
        {
            get { return ((IObjectContextAdapter)this).ObjectContext; }
        }


        private PropertyInfo GetEntityPrimaryKey(Type type)
        {
            EntityType entityType = (from meta in ObjectContext.MetadataWorkspace.GetItems(DataSpace.CSpace)
                                     where meta.BuiltInTypeKind == BuiltInTypeKind.EntityType
                                     select meta)
                              .OfType<EntityType>()
                              .Where(e => e.Name == type.Name).Single();

            return type.GetProperty(entityType.KeyMembers[0].Name);
        }

        public string GetEntityRelationshipKeyName<T, TR>()
        {
            Type entityType = typeof(T);
            Type relType = typeof(TR);

            EntityType type = (from meta in ObjectContext.MetadataWorkspace.GetItems<EntityType>(DataSpace.CSpace)
                               select meta)
                              .Where(e => e.Name == entityType.Name)
                              .Single();



            //foreach (IRelatedEnd relatedEnd in ((IEntityWithRelationships)entityType).RelationshipManager.GetAllRelatedEnds())
            //{
            //    foreach (IEntityWithKey relatedItem in relatedEnd)
            //    {
            //        object reference = null;
            //        entities.TryGetObjectByKey(relatedItem.EntityKey, out reference);
            //        (original as EntityObject).GetType().GetProperty(relatedEnd.TargetRoleName).SetValue(original, reference, null);
            //    }
            //}


            //var navigationProperty = type.NavigationProperties.First(n => n.DeclaringType.Name == relType.Name);
            //return navigationProperty.ToEndMember.Name;

            throw new InvalidOperationException(string.Format("Auditing associations with Entity Framework is currently unable to automatically lookup primary and foreign keys when using this overload of AuditAssocitation(). Please use the 'AuditAssocitation<{0}>(o => o.PrimaryKey, o => o.ForeignKey)' overload to manually provide the primary and foreign keys", relType));
        }

        public EmptyPropertyMode EmptyPropertyMode { get; set; }
        public bool AuditingEnabled { get; set; }

        public IList<Func<MemberInfo, object, bool>> PropertyAuditRules
        {
            get { return _propertyAuditRules; }
        }

        private IDictionary<Type, IAuditPropertyResolver> _resolvers = new Dictionary<Type, IAuditPropertyResolver>();

        public IDictionary<Type, IAuditPropertyResolver> Resolvers
        {
            get { return _resolvers; }
        }

        public Type GetEntityType(Type entityType)
        {
            return ObjectContext.GetObjectType(entityType);
        }
    }
}
