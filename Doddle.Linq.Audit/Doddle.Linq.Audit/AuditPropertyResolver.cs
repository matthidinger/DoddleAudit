using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Data.Linq.Mapping;

namespace Doddle.Linq.Audit
{
    public class AuditPropertyResolver : IAuditPropertyResolver
    {
        private static string GetPropertyValue(object input)
        {
            return (input == null) ? string.Empty : input.ToString();
        }

        public AuditedEntityField GetAuditValue(MemberInfo member, object oldValue, object newValue)
        {
            AuditedEntityField value = new AuditedEntityField();
            value.FieldName = member.Name.SplitUpperCaseToString();
            value.OldValue = GetPropertyValue(oldValue);
            value.NewValue = GetPropertyValue(newValue);
            return value;
        }

        public static IAuditPropertyResolver GetResolver<TEntity>()
        {
            return GetResolver(typeof (TEntity));
        }

        public static IAuditPropertyResolver GetResolver(Type type)
        {
            object[] attributes = type.GetCustomAttributes(typeof(AuditResolverAttribute), true);
            if (attributes.Length == 0)
                return new AuditPropertyResolver();

            AuditResolverAttribute a = attributes.GetValue(0) as AuditResolverAttribute;

            return Activator.CreateInstance(a.ResolverType) as IAuditPropertyResolver;

        }
    }

    public abstract class AuditPropertyResolver<TEntity> : IAuditPropertyResolver
    {
        protected AuditPropertyResolver()
        {
            CustomizeProperties();
        }

        private readonly AuditPropertyResolver _defaultResolver = new AuditPropertyResolver();
        private readonly Dictionary<MemberInfo, CustomizedAuditProperty> _customizedProperties = new Dictionary<MemberInfo, CustomizedAuditProperty>();

        protected abstract void CustomizeProperties();

        /// <summary>
        /// Customize a property definition to return a custom string instead of the property value
        /// </summary>
        /// <typeparam name="TR">The Type of property to customize</typeparam>
        /// <param name="newPropertyName">The new name of the property to appear in the audit log</param>
        /// <param name="propertySelector">A lambda expression that returns the property to customize</param>
        /// <param name="valueSelector">A lambda expression that returns the string that will be entered into the audit log for the customized property</param>
        public void CustomizeProperty<TR>(Expression<Func<TEntity, TR>> propertySelector, Expression<Func<TR, string>> valueSelector, string newPropertyName)
        {
            CustomizedAuditProperty prop = new CustomizedAuditProperty();
            prop.ValueSelector = valueSelector;
            prop.PropertyName = newPropertyName;

            MemberExpression exp = propertySelector.Body as MemberExpression;
            _customizedProperties.Add(exp.Member, prop);
        }

        public AuditedEntityField GetAuditValue(MemberInfo member, object oldValue, object newValue)
        {
            AuditedEntityField value;

            if (_customizedProperties.ContainsKey(member))
            {
                CustomizedAuditProperty property = _customizedProperties[member];
                Delegate func = property.ValueSelector.Compile();

                value = new AuditedEntityField();
                value.FieldName = property.PropertyName;

                if (oldValue != null)
                    value.OldValue = func.DynamicInvoke(oldValue).ToString();

                if (newValue != null)
                    value.NewValue = func.DynamicInvoke(newValue).ToString();
            }
            else
            {
                value = _defaultResolver.GetAuditValue(member, oldValue, newValue);
            }

            return value;
        }
    }
}
