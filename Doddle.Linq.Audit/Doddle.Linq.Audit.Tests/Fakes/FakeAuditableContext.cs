using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Data.Linq;

namespace Doddle.Linq.Audit.Tests.Fakes
{

    public class FakeAuditableContext : IAuditableContext
    {
        public FakeAuditableContext()
        {

        }

        private readonly IList<IAuditDefinition> _auditDefinitions = new List<IAuditDefinition>();
        public IList<IAuditDefinition> AuditDefinitions
        {
            get { return _auditDefinitions; }
        }

        public IEnumerable<object> Inserts
        {
            get
            {
                List<object> products = new List<object>
                                            {
                                                new Product {ID = 1, CategoryID=1, ProductName = "Chai"}
                                            };

                return products;

            }
        }

        public IEnumerable<object> Updates
        {
            get { yield break; }
        }

        public IEnumerable<object> Deletes
        {
            get { yield break; }
        }

        public virtual void InsertAuditRecord(EntityAuditRecord record)
        {

        }

        public virtual IEnumerable<MemberAudit> GetModifiedMembers(object entity)
        {
            throw new System.NotImplementedException();
        }

        public virtual MemberInfo GetEntityPrimaryKey<TEntity>()
        {
            //return typeof(TEntity).GetProperty("ID");
            return null;
        }

        public virtual string GetEntityRelationshipKeyName<T, TR>()
        {
            return "";
        }
    }

}
