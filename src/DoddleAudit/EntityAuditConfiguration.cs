using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace DoddleAudit
{
    /// <summary>
    /// Customize the auditing process for a specific entity type
    /// </summary>
    public class EntityAuditConfiguration : IEntityAuditConfiguration
    {
        private readonly Dictionary<PropertyInfo, IPropertyAuditor> _customizedProperties = new Dictionary<PropertyInfo, IPropertyAuditor>();
        private readonly IList<IRelatationshipConfiguration> _relationships = new List<IRelatationshipConfiguration>();

        protected Dictionary<PropertyInfo, IPropertyAuditor> CustomizedProperties
        {
            get { return _customizedProperties; }
        }

        IList<IRelatationshipConfiguration> IEntityAuditConfiguration.Relationships
        {
            get { return _relationships; }
        }

        AuditedProperty IEntityAuditConfiguration.AuditProperty(PropertyInfo property, object oldValue, object newValue)
        {
            var auditedProperty = _customizedProperties.ContainsKey(property) 
                ? _customizedProperties[property].AuditProperty(property, oldValue, newValue) 
                : PropertyAuditor.Default.AuditProperty(property, oldValue, newValue);

            return auditedProperty;
        }

        bool IEntityAuditConfiguration.IsPropertyCustomized(PropertyInfo property)
        {
            return _customizedProperties.ContainsKey(property);
        }

    }

    public class EntityAuditConfiguration<TEntity> : EntityAuditConfiguration
    {
        /// <summary>
        /// Customize the default behavior when auditing a specific property
        /// </summary>
        public CustomPropertyAuditor<TEntity, T> AuditProperty<T>(Expression<Func<TEntity, T>> propertySelector)
        {
            var config = new CustomPropertyAuditor<TEntity, T>();
            CustomizedProperties.Add(propertySelector.ToPropertyInfo(), config);
            return config;
        }


        /// <summary>
        /// Include an association (relationship) table to audit along with the parent table
        /// </summary>
        public RelatationshipConfiguration<TChildEntity, TEntity> AuditMany<TChildEntity>(Expression<Func<TEntity, IEnumerable<TChildEntity>>> selector)
            where TChildEntity : class
        {
            var relationship = new RelatationshipConfiguration<TChildEntity, TEntity>();
            ((IEntityAuditConfiguration) this).Relationships.Add(relationship);
            return relationship;
        }
    }
}