using System;
using System.Linq.Expressions;
using System.Reflection;

namespace DoddleAudit
{
    public class CustomPropertyAuditor<TEntity, TProp> : IPropertyAuditor
    {
        private Expression<Func<TProp, string>> _propertySelector;
        private string _propertyName;

        public CustomPropertyAuditor<TEntity, TProp> WithPropertyName(string propertyName)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");
            _propertyName = propertyName;

            return this;
        }

        public CustomPropertyAuditor<TEntity, TProp> GetValueFrom(Expression<Func<TProp, string>> valueSelector)
        {
            if (valueSelector == null) throw new ArgumentNullException("valueSelector");
            _propertySelector = valueSelector;
            return this;
        }

        AuditedProperty IPropertyAuditor.AuditProperty(PropertyInfo property, object oldValue, object newValue)
        {
            var auditedProperty = new AuditedProperty(_propertyName ?? property.Name);
            var func = _propertySelector.Compile();

            if (oldValue != null)
                auditedProperty.OldValue = func.DynamicInvoke(oldValue).ToString();

            if (newValue != null)
                auditedProperty.NewValue = func.DynamicInvoke(newValue).ToString();

            return auditedProperty;
        }
    }
}