using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
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

        public IEnumerable Inserts
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

        public IEnumerable Updates
        {
            get { yield break; }
        }

        public IEnumerable Deletes
        {
            get { yield break; }
        }

        public virtual void InsertAuditRecord(AuditedEntity record)
        {

        }

        public virtual IEnumerable<MemberAudit> GetModifiedFields(object entity)
        {
            throw new System.NotImplementedException();
        }

        public virtual PropertyInfo GetEntityPrimaryKey<TEntity>()
        {
            //return typeof(TEntity).GetProperty("ID");
            return null;
        }

        public virtual string GetEntityRelationshipKeyName<T, TR>()
        {
            return "";
        }

        private List<Func<MemberInfo, object, bool>> _propertyAuditRules = new List<Func<MemberInfo, object, bool>>();
        public IList<Func<MemberInfo, object, bool>> PropertyAuditRules
        {
            get { return _propertyAuditRules; }
        }
    }

}
