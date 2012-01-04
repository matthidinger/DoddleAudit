using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Metadata.Edm;
using System.Linq;
using System.Reflection;
using System.Data.Objects;
using System.Data.EntityClient;

namespace DoddleAudit.EntityFramework
{
    public abstract class AuditableObjectContext : ObjectContext, IAuditableContext
    {
        private readonly List<IEntityAuditor> _entityAuditors = new List<IEntityAuditor>();

        private readonly ContextAuditConfiguration _configuration = new ContextAuditConfiguration();
        public ContextAuditConfiguration AuditConfiguration { get { return _configuration; } }


        #region Constructors

        protected AuditableObjectContext(string connectionString)
            : base(connectionString)
        {          
        }

        protected AuditableObjectContext(string connectinString, string defaultContainerName)
            : base(connectinString, defaultContainerName)
        {
        }
        protected AuditableObjectContext(EntityConnection connection)
            : base(connection)
        {

        }
        protected AuditableObjectContext(EntityConnection connection, string defaultContainerName)
            : base(connection, defaultContainerName)
        {
        }

        #endregion

        /// <summary>
        /// Enlist a DbSet for automatic Auditing
        /// </summary>
        public EntityAuditor<TEntity> Audit<TEntity>(ObjectSet<TEntity> objectSet) where TEntity : class
        {
            var def = new EntityAuditor<TEntity>(this.GetEntityPkProperty<TEntity>().ToPropertyInfo());
            _entityAuditors.Add(def);
            return def;
        }

        int IAuditableContext.SavePendingChanges()
        {
            return base.SaveChanges();
        }

        public abstract void SaveAuditedEntity(AuditedEntity record);


        public new void SaveChanges()
        {
            var auditor = new ContextAuditor(this);
            AuditConfiguration.OnlyAuditPropertiesIf(ShouldAuditProperty);
            auditor.AuditAndSaveChanges();  
        }

        private bool ShouldAuditProperty(PropertyInfo property, object entity)
        {
            var stateEntry = ObjectStateManager.GetObjectStateEntry(entity);
            return !stateEntry.IsRelationship;
        }

        IList<IEntityAuditor> IAuditableContext.EntityAuditors
        {
            get { return _entityAuditors; }
        }

        IEnumerable<object> IAuditableContext.PendingInserts
        {
            get { return ObjectStateManager.GetObjectStateEntries(EntityState.Added).Select(o => o.Entity); }
        }

        IEnumerable<object> IAuditableContext.PendingUpdates
        {
            get { return ObjectStateManager.GetObjectStateEntries(EntityState.Modified).Select(o => o.Entity); }
        }

        IEnumerable<object> IAuditableContext.PendingDeletes
        {
            get { return ObjectStateManager.GetObjectStateEntries(EntityState.Deleted).Select(o => o.Entity); }
        }


        IEnumerable<ModifiedProperty> IAuditableContext.GetModifiedProperties(object entity)
        {
            ObjectStateEntry entry = ObjectStateManager.GetObjectStateEntry(entity);
            return
                entry.GetModifiedProperties().Select(
                    p =>
                    new ModifiedProperty(entity.GetType().GetProperty(p), entry.OriginalValues[p], entry.CurrentValues[p]));
        }


        PropertyInfo IAuditableContext.GetPrimaryKeyProperty(Type type)
        {
            EntityType entityType = (from meta in MetadataWorkspace.GetItems(DataSpace.CSpace)
                                     where meta.BuiltInTypeKind == BuiltInTypeKind.EntityType
                                     select meta)
                .OfType<EntityType>().Single(e => e.Name == type.Name);

            return type.GetProperty(entityType.KeyMembers[0].Name);
        }

        public string GetEntitySetName(string entityTypeName)
        {
            var container = MetadataWorkspace.GetEntityContainer(DefaultContainerName, DataSpace.CSpace);
            string entitySetName = (from meta in container.BaseEntitySets
                                    where meta.ElementType.Name == entityTypeName
                                    select meta.Name).FirstOrDefault();
            return entitySetName;
        }

        string IAuditableContext.GetForeignKeyPropertyName(Type entityType, Type parentEntityType)
        {
            EntityType type = (from meta in MetadataWorkspace.GetItems(DataSpace.CSpace)
                               where meta.BuiltInTypeKind == BuiltInTypeKind.EntityType
                               select meta)
                .OfType<EntityType>().Single(e => e.Name == parentEntityType.Name);

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

            throw new InvalidOperationException(string.Format("Auditing associations with Entity Framework is currently unable to automatically lookup primary and foreign keys when using this overload of AuditAssocitation(). Please use the 'AuditAssocitation<{0}>(o => o.PrimaryKey, o => o.ForeignKey)' overload to manually provide the primary and foreign keys", parentEntityType));
        }

        Type IAuditableContext.GetEntityType(Type entityType)
        {
            return entityType;
        }
    }
}
