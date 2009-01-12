using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Reflection;
using System.Data.Linq.Mapping;
using System.Linq.Expressions;

namespace Doddle.Linq.Audit.LinqToSql
{
    public abstract class AuditableDataContext : DataContext, IAuditableContext
    {
        #region Constructors

        protected AuditableDataContext(string fileOrServerOrConnection)
            : base(fileOrServerOrConnection)
        { }
        protected AuditableDataContext(string fileOrServerOrConnection, MappingSource mapping)
            : base(fileOrServerOrConnection, mapping)
        { }
        protected AuditableDataContext(System.Data.IDbConnection connection)
            : base(connection)
        { }
        protected AuditableDataContext(System.Data.IDbConnection connection, MappingSource mapping)
            : base(connection, mapping)
        { }

        #endregion

        private readonly List<EntityAuditRecord> _queuedRecords = new List<EntityAuditRecord>();
        private readonly List<IAuditDefinition> _auditDefinitions = new List<IAuditDefinition>();


        /// <summary>
        /// This method defines how to insert the actual audit record into the database
        /// </summary>
        /// <param name="record"></param>
        protected abstract void InsertAuditRecordToDatabase(EntityAuditRecord record);


        public IList<IAuditDefinition> AuditDefinitions
        {
            get { return _auditDefinitions; }
        }

        public IEnumerable<object> Inserts
        {
            get { return GetChangeSet().Inserts; }
        }

        public IEnumerable<object> Updates
        {
            get { return GetChangeSet().Updates; }
        }

        public IEnumerable<object> Deletes
        {
            get { return GetChangeSet().Deletes;  }
        }

        public void InsertAuditRecord(EntityAuditRecord record)
        {
            _queuedRecords.Add(record);
        }

        public IEnumerable<MemberAudit> GetModifiedMembers(object entity)
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

            return
                table.GetModifiedMembers(entity).Select(
                    mmi =>
                    new MemberAudit
                        {Member = mmi.Member, CurrentValue = mmi.CurrentValue, OriginalValue = mmi.OriginalValue});
        }

        public MemberInfo GetEntityPrimaryKey<TEntity>()
        {
            Type entityType = typeof (TEntity);

            try
            {
                var pk = Mapping.GetTable(typeof(TEntity)).RowType.DataMembers.Single(md => md.IsPrimaryKey);
                return pk.Member;
            }
            catch (Exception)
            {
                throw new Exception(string.Format("Auditing logic is only capable of processing tables with a single primary key. Please modify this table structure or remove the table '{0}' from automatic auditing.", Mapping.GetTable(entityType).TableName));
           }
        }

        public string GetEntityRelationshipKeyName<T, TR>()
        {
            Type entityType = typeof(T);
            Type relType = typeof(TR);

            return Mapping.GetTable(entityType).RowType.Associations.First(ma => ma.OtherType.Type == relType).OtherKey.First().Name;
        }


        /// <summary>
        /// Define all default audits that should be applied to this database
        /// </summary>
        protected virtual void DefaultAuditDefinitions()
        {
            
        }

        public override void SubmitChanges(ConflictMode failureMode)
        {
            DefaultAuditDefinitions();
            

            AuditProcessor processor = new AuditProcessor(this);
            processor.Process();

            base.SubmitChanges(failureMode);

            foreach (EntityAuditRecord record in _queuedRecords)
            {
                // New entities (inserts) will have a PK of 0 until LINQ submits changes to the DB and retrieves the real PK,
                // so we need to update the Insert Audit record with the real PK
                if (record.Action == AuditAction.Insert)
                {
                    int pk = (int)record.KeySelector.Compile().DynamicInvoke(record.Entity);
                    if (record.EntityTableKey == 0)
                        record.EntityTableKey = pk;
                }

                InsertAuditRecordToDatabase(record);
            }

            // Submit all audits to the db, continue processing even if Auditing records fail
            base.SubmitChanges(ConflictMode.ContinueOnConflict);
        }
    }
}
