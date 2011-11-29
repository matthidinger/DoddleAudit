using System;
using System.Collections.Generic;
using System.Reflection;

namespace Doddle.Audit.Tests.Fakes
{
    public class FakeAuditableContext : IAuditableContext
    {
        public FakeAuditableContext()
        {
            AuditingEnabled = true;
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
                var products = new List<object>
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

        public EmptyPropertyMode EmptyPropertyMode { get; set; }


        private List<Func<MemberInfo, object, bool>> _propertyAuditRules = new List<Func<MemberInfo, object, bool>>();

        public IList<Func<MemberInfo, object, bool>> PropertyAuditRules
        {
            get { return _propertyAuditRules; }
        }

        private IDictionary<Type, IAuditPropertyResolver> _resolvers = new Dictionary<Type, IAuditPropertyResolver>();

        public IDictionary<Type, IAuditPropertyResolver> Resolvers
        {
            get { return _resolvers; }
        }

        public bool AuditingEnabled { get; set; }

        public Type GetEntityType(Type entityType)
        {
            return entityType;
        }
    }

}
