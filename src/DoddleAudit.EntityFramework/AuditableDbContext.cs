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

namespace DoddleAudit.EntityFramework
{
    public abstract class AuditableDbContext : DbContext, IAuditableContext
    {
        private readonly List<IEntityAuditor> _entityAuditors = new List<IEntityAuditor>();

        private readonly ContextAuditConfiguration _configuration = new ContextAuditConfiguration();
        public ContextAuditConfiguration AuditConfiguration { get { return _configuration; } }

        #region Constructors

        protected AuditableDbContext()
        {
        }

        protected AuditableDbContext(string connectionString)
            : base(connectionString)
        {
        }

        protected AuditableDbContext(DbCompiledModel model)
            : base(model)
        {
        }

        protected AuditableDbContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
        }

        protected AuditableDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        protected AuditableDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
        }

        protected AuditableDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext)
            : base(objectContext, dbContextOwnsObjectContext)
        {
        }

        #endregion

        /// <summary>
        /// Enlist a DbSet for automatic Auditing
        /// </summary>
        public EntityAuditor<TEntity> Audit<TEntity>(IDbSet<TEntity> dbSet) where TEntity : class
        {
            var def = new EntityAuditor<TEntity>(this.GetEntityPkProperty<TEntity>().ToPropertyInfo());
            _entityAuditors.Add(def);
            return def;
        }


        public override int SaveChanges()
        {
            var auditor = new ContextAuditor(this);
            int result = auditor.AuditAndSaveChanges();
            return result;
        }

        int IAuditableContext.SavePendingChanges()
        {
            return base.SaveChanges();
        }

        IList<IEntityAuditor> IAuditableContext.EntityAuditors
        {
            get { return _entityAuditors; }
        }

        IEnumerable<object> IAuditableContext.PendingInserts
        {
            get
            {
                var entries = ObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added);

                foreach (var entry in entries)
                {
                    // TODO: Add support for many-to-many relationships
                    if (entry.IsRelationship)
                    {
                        var relationshipAudit = new RelationshipChangeEntry();
                        for (int i = 0; i < entry.CurrentValues.FieldCount; i++)
                        {
                            var key = (System.Data.EntityKey)entry.CurrentValues.GetValue(i);

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

        IEnumerable<object> IAuditableContext.PendingUpdates
        {
            get { return ChangeTracker.Entries().Where(e => e.State == EntityState.Modified).Select(o => o.Entity); }
        }

        IEnumerable<object> IAuditableContext.PendingDeletes
        {
            get { return ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted).Select(o => o.Entity); }
        }


        /// <summary>
        /// Use this method to define how to save the actual audit record into the database
        /// </summary>
        public abstract void SaveAuditedEntity(AuditedEntity record);
    

        public IEnumerable<ModifiedProperty> GetModifiedProperties(object entity)
        {
            var entry = ObjectContext.ObjectStateManager.GetObjectStateEntry(entity);
            EntityType type = (from meta in ObjectContext.MetadataWorkspace.GetItems<EntityType>(DataSpace.CSpace)
                               select meta).Single(e => e.Name == GetEntityType(entity.GetType()).Name);


            var navKeys = (from np in type.NavigationProperties
                           select new
                                      {
                                          NavProp = np,
                                          Key = np.GetDependentProperties().FirstOrDefault()
                                      }).ToList();

            var modifiedProperties = entry.GetModifiedProperties()
                .Select(
                    p =>
                    new ModifiedProperty(GetEntityType(entity.GetType()).GetProperty(p), entry.OriginalValues[p],
                                    entry.CurrentValues[p])).ToList();

            //foreach (var property in entry.GetModifiedProperties())
            //{
            //    var fk = navKeys.Where(np => np.Key != null).FirstOrDefault(nk => nk.Key.Name == property);
            //    if (fk == null)
            //        continue;

            //    // TODO: Add config property to allow this additional query
            //    var pi = GetEntityType(entity.GetType()).GetProperty(fk.NavProp.Name);


            //    EntityContainer container = ObjectContext.MetadataWorkspace.GetEntityContainer(ObjectContext.DefaultContainerName, DataSpace.CSpace);

            //    EntitySetBase entitySet = container.BaseEntitySets
            //        .Where(item => item.ElementType.Name.Equals(pi.PropertyType.Name))
            //        .FirstOrDefault();


            //    var originalValue = entry.OriginalValues[property];
            //    object old = null;
            //    if (originalValue != null && originalValue != DBNull.Value)
            //    {
            //        // TODO: Get other key property from NavigationProperty
            //        var oldKey = new System.Data.EntityKey(String.Format("{0}.{1}", container.Name, entitySet.Name),
            //                                               "Id",
            //                                               originalValue);
            //        try
            //        {
            //            old = ObjectContext.GetObjectByKey(oldKey);
            //        }
            //        catch (ObjectNotFoundException)
            //        {
            //        }
            //    }


            //    var currentValue = entry.CurrentValues[property];
            //    object newObj = null;

            //    if (currentValue != null && currentValue != DBNull.Value)
            //    {
            //        var newKey = new System.Data.EntityKey(String.Format("{0}.{1}", container.Name, entitySet.Name),
            //                                               "Id",
            //                                               currentValue);
            //        try
            //        {
            //            newObj = ObjectContext.GetObjectByKey(newKey);
            //        }
            //        catch (ObjectNotFoundException)
            //        {
            //        }
            //    }


            //    modifiedProperties.Add(new MemberAudit(pi, old, newObj));
            //}

            return modifiedProperties;
        }

        private ObjectContext ObjectContext
        {
            get { return ((IObjectContextAdapter)this).ObjectContext; }
        }


        public PropertyInfo GetPrimaryKeyProperty(Type type)
        {
            EntityType entityType = (from meta in ObjectContext.MetadataWorkspace.GetItems(DataSpace.CSpace)
                                     where meta.BuiltInTypeKind == BuiltInTypeKind.EntityType
                                     select meta)
                .OfType<EntityType>().Single(e => e.Name == type.Name);

            return type.GetProperty(entityType.KeyMembers[0].Name);
        }

        public string GetForeignKeyPropertyName(Type entityType, Type parentEntityType)
        {
            EntityType type = (from meta in ObjectContext.MetadataWorkspace.GetItems<EntityType>(DataSpace.CSpace)
                               select meta).Single(e => e.Name == entityType.Name);



            //foreach (IRelatedEnd relatedEnd in ((IEntityWithRelationships)entityType).RelationshipManager.GetAllRelatedEnds())
            //{
            //    foreach (IEntityWithKey relatedItem in relatedEnd)
            //    {
            //        object reference = null;
            //        entities.TryGetObjectByKey(relatedItem.EntityKey, out reference);
            //        (original as EntityObject).GetType().GetProperty(relatedEnd.TargetRoleName).SetValue(original, reference, null);
            //    }
            //}


            //var navigationProperty = type.NavigationProperties.First(n => n.DeclaringType.Name == parentEntityType.Name);
            //return navigationProperty.ToEndMember.Name;

            throw new InvalidOperationException(String.Format("Auditing associations with Entity Framework is currently unable to automatically lookup primary and foreign keys when using this overload of AuditAssocitation(). Please use the 'AuditAssocitation<{0}>(o => o.PrimaryKey, o => o.ForeignKey)' overload to manually provide the primary and foreign keys", entityType));
        }

        public Type GetEntityType(Type entityType)
        {
            return ObjectContext.GetObjectType(entityType);
        }

    }
}