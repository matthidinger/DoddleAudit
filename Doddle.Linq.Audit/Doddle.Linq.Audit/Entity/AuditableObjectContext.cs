using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Metadata.Edm;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Reflection;
using System.Data.Objects;
using System.Data.EntityClient;
using System.Linq.Expressions;

namespace Doddle.Linq.Audit.Entity
{
    public abstract class AuditableObjectContext : ObjectContext, IAuditableContext
    {
        #region Constructors

        protected AuditableObjectContext(string connectionString)
            : base(connectionString)
        { }

        protected AuditableObjectContext(string connectinString, string defaultContainerName)
            : base(connectinString, defaultContainerName)
        { }
        protected AuditableObjectContext(EntityConnection connection)
            : base(connection)
        { }
        protected AuditableObjectContext(EntityConnection connection, string defaultContainerName)
            : base(connection, defaultContainerName)
        { }

        #endregion

        /// <summary>
        /// This method defines how to insert the actual audit record into the database
        /// </summary>
        /// <param name="record"></param>
        protected abstract void InsertAuditRecordToDatabase(AuditedEntity record);


        private readonly List<IAuditDefinition> _auditDefinitions = new List<IAuditDefinition>();
        private readonly List<AuditedEntity> _queuedRecords = new List<AuditedEntity>();
        private readonly List<Func<MemberInfo, object, bool>> _propertyAuditRules = new List<Func<MemberInfo, object, bool>>();


        protected virtual void SaveChangesAndAudit()
        {
            PropertyAuditRules.Add(ShouldAuditProperty);

            AuditProcessor processor = new AuditProcessor(this);
            processor.Process();

            base.SaveChanges();

            foreach (AuditedEntity record in _queuedRecords)
            {
                if (record.Action == AuditAction.Insert)
                {
                    record.UpdateKeys();
                }
                InsertAuditRecordToDatabase(record);
            }

            base.SaveChanges();
        }

        public new void SaveChanges()
        {
            SaveChangesAndAudit();   
        }

        private bool ShouldAuditProperty(MemberInfo member, object entity)
        {
            var stateEntry = ObjectStateManager.GetObjectStateEntry(entity);
            return !stateEntry.IsRelationship;
        }

        public IList<IAuditDefinition> AuditDefinitions
        {
            get { return _auditDefinitions; }
        }

        public IEnumerable Inserts
        {
            get { return ObjectStateManager.GetObjectStateEntries(EntityState.Added).Select(o => o.Entity); }
        }

        public IEnumerable Updates
        {
            get { return ObjectStateManager.GetObjectStateEntries(EntityState.Modified).Select(o => o.Entity); }
        }

        public IEnumerable Deletes
        {
            get { return ObjectStateManager.GetObjectStateEntries(EntityState.Deleted).Select(o => o.Entity); }
        }

        public void InsertAuditRecord(AuditedEntity record)
        {
            _queuedRecords.Add(record);
        }

        public IEnumerable<MemberAudit> GetModifiedFields(object entity)
        {
            ObjectStateEntry entry = ObjectStateManager.GetObjectStateEntry(entity);
            return
                entry.GetModifiedProperties().Select(
                    p =>
                    new MemberAudit(entity.GetType().GetProperty(p), entry.OriginalValues[p], entry.CurrentValues[p]));
        }

        public PropertyInfo GetEntityPrimaryKey<TEntity>()
        {
            Type entityType = typeof(TEntity);
            return GetEntityPrimaryKey(entityType);
        }

        private PropertyInfo GetEntityPrimaryKey(Type type)
        {
            EntityType entityType = (from meta in MetadataWorkspace.GetItems(DataSpace.CSpace)
                               where meta.BuiltInTypeKind == BuiltInTypeKind.EntityType
                               select meta)
                              .OfType<EntityType>()
                              .Where(e => e.Name == type.Name).Single();

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

        public string GetEntityRelationshipKeyName<T, TR>()
        {
            Type entityType = typeof(T);
            Type relType = typeof(TR);

            EntityType type = (from meta in MetadataWorkspace.GetItems(DataSpace.CSpace)
                               where meta.BuiltInTypeKind == BuiltInTypeKind.EntityType
                               select meta)
                              .OfType<EntityType>()
                              .Where(e => e.Name == relType.Name).Single();

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

        public IList<Func<MemberInfo, object, bool>> PropertyAuditRules
        {
            get { return _propertyAuditRules; }
        }
    }
}
