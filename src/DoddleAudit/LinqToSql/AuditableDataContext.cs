using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Reflection;
using System.Data.Linq.Mapping;

namespace DoddleAudit.LinqToSql
{
    public abstract class AuditableDataContext : DataContext, IAuditableContext
    {
        #region Constructors

        protected AuditableDataContext(string fileOrServerOrConnection)
            : base(fileOrServerOrConnection)
        {     
        }
        protected AuditableDataContext(string fileOrServerOrConnection, MappingSource mapping)
            : base(fileOrServerOrConnection, mapping)
        {       
        }
        protected AuditableDataContext(System.Data.IDbConnection connection)
            : base(connection)
        {        
        }
        protected AuditableDataContext(System.Data.IDbConnection connection, MappingSource mapping)
            : base(connection, mapping)
        {          
        }

        #endregion

        private readonly List<IEntityAuditor> _entityAuditors = new List<IEntityAuditor>();
        private readonly ContextAuditConfiguration _configuration = new ContextAuditConfiguration();

        public ContextAuditConfiguration AuditConfiguration { get { return _configuration; } }


        IList<IEntityAuditor> IAuditableContext.EntityAuditors
        {
            get { return _entityAuditors; }
        }

        IEnumerable<object> IAuditableContext.PendingInserts
        {
            get { return GetChangeSet().Inserts; }
        }

        IEnumerable<object> IAuditableContext.PendingUpdates
        {
            get { return GetChangeSet().Updates; }
        }

        IEnumerable<object> IAuditableContext.PendingDeletes
        {
            get { return GetChangeSet().Deletes; }
        }

        public abstract void SaveAuditedEntity(AuditedEntity record);

        IEnumerable<ModifiedProperty> IAuditableContext.GetModifiedProperties(object entity)
        {
            ITable table;

            try
            {
                table = GetTable(entity.GetType());
            }
            catch
            {
                table = GetTable(entity.GetType().BaseType);
            }

            // TODO: Test the PropertyInfo cast
            return
                table.GetModifiedMembers(entity).Select(
                    mmi =>
                    new ModifiedProperty((PropertyInfo)mmi.Member, mmi.CurrentValue, mmi.OriginalValue));
        }

        PropertyInfo IAuditableContext.GetPrimaryKeyProperty(Type entityType)
        {
            try
            {
                var pk = Mapping.GetTable(entityType).RowType.DataMembers.Single(md => md.IsPrimaryKey);
                return (PropertyInfo)pk.Member;
            }
            catch
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Auditing logic is only capable of processing tables with a single primary key. Please modify this table structure or remove the table '{0}' from automatic auditing.",
                        Mapping.GetTable(entityType).TableName));
            }
        }

        string IAuditableContext.GetForeignKeyPropertyName(Type entityType, Type parentEntityType)
        {
            return Mapping.GetTable(parentEntityType).RowType.Associations.First(ma => ma.OtherType.Type == entityType).OtherKey.First().Name;
        }

        Type IAuditableContext.GetEntityType(Type entityType)
        {
            return entityType;
        }


        int IAuditableContext.SavePendingChanges()
        {
            base.SubmitChanges(ConflictMode.ContinueOnConflict);
            return 0;
        }


        /// <summary>
        /// Sends changes that were made to retrieved objects to the underlying database, and specifies the action to be taken if the submission fails.
        /// </summary>
        /// <param name="failureMode">The action to be taken if the submission fails. Valid arguments are as follows:<see cref="F:System.Data.Linq.ConflictMode.FailOnFirstConflict"/><see cref="F:System.Data.Linq.ConflictMode.ContinueOnConflict"/></param>
        public override void SubmitChanges(ConflictMode failureMode)
        {
            AuditConfiguration.PropertyAuditRules.Add((property, entity) => property.HasAttribute(typeof(ColumnAttribute)));
            var auditor = new ContextAuditor(this);
            auditor.AuditAndSaveChanges();
        }
    }
}
