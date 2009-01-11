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

        private readonly List<AuditRecord> _queuedRecords = new List<AuditRecord>();
        private readonly List<IAuditDefinition> _auditDefinitions = new List<IAuditDefinition>();


        /// <summary>
        /// This method defines how to insert the actual audit record into the database
        /// </summary>
        /// <param name="record"></param>
        protected abstract void InsertAuditRecord(AuditRecord record);

        /// <summary>
        /// Define all default audits that should be applied to this database
        /// </summary>
        //protected abstract void DefaultAuditDefinitions();

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

        public void QueueAudit(AuditRecord record)
        {
            _queuedRecords.Add(record);
        }

        public IEnumerable<MemberAudit> GetModifiedMembers(object entity)
        {
            ITable table = GetTable(entity.GetType());
            return
                table.GetModifiedMembers(entity).Select(
                    mmi =>
                    new MemberAudit
                        {Member = mmi.Member, CurrentValue = mmi.CurrentValue, OriginalValue = mmi.OriginalValue});
        }

        public MemberInfo GetEntityPrimaryKey<TEntity>()
        {
            Type entityType = typeof (TEntity);
            var pk = Mapping.GetTable(typeof (TEntity)).RowType.DataMembers.SingleOrDefault(md => md.IsPrimaryKey);
            return pk.Member;
        }

        public string GetEntityRelationshipKeyName<T, TR>()
        {
            Type entityType = typeof(T);
            Type relType = typeof(TR);

            return Mapping.GetTable(entityType).RowType.Associations.First(ma => ma.OtherType.Type == relType).OtherKey.First().Name;
        }


        protected virtual void DefaultAuditDefinitions()
        {
            
        }
        public override void SubmitChanges(ConflictMode failureMode)
        {
            DefaultAuditDefinitions();
            

            AuditProcessor processor = new AuditProcessor(this);
            processor.Process();

            base.SubmitChanges(failureMode);

            foreach (AuditRecord record in _queuedRecords)
            {
                // New entities (inserts) will have a PK of 0 until LINQ submits changes to the DB and retrieves the real PK,
                // so we need to update the Insert Audit record with the real PK
                if (record.Action == AuditAction.Insert)
                {
                    int pk = (int)record.KeySelector.Compile().DynamicInvoke(record.Entity);

                    if (record.ModifiedTableKey == 0)
                        record.ModifiedTableKey = pk;

                    if (record.PrimaryTableKey == 0)
                        record.PrimaryTableKey = pk;
                }

                InsertAuditRecord(record);
            }

            // Submit all audits to the db, continue processing even if Auditing records fail
            base.SubmitChanges(ConflictMode.ContinueOnConflict);
        }

        

    }
}
