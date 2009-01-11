using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Data.Linq.Mapping;
using Doddle.Reflection;

namespace Doddle.Linq.Audit
{
    public class AuditValueResolver : IAuditValueResolver
    {
        private static string GetPropertyValue(PropertyInfo pi, object input)
        {
            object tmp = pi.GetValue(input, null);
            return (tmp == null) ? string.Empty : tmp.ToString();
        }

        private static string GetPropertyValue(object input)
        {
            return (input == null) ? string.Empty : input.ToString();
        }

        public bool IsMemberValid(MemberInfo member)
        {
            return member.HasAttribute(typeof(ColumnAttribute)) && !member.Name.EndsWith("Id");
        }

        public AuditValue GetAuditValue(MemberInfo member, object oldValue, object newValue)
        {
            if (IsMemberValid(member))
            {
                AuditValue value = new AuditValue();
                value.MemberName = member.Name.SplitUpperCaseToString();
                value.OldValue = GetPropertyValue(oldValue);
                value.NewValue = GetPropertyValue(newValue);
                return value;
            }

            return null;
        }

        public static IAuditValueResolver GetResolver(Type type)
        {
            object[] attributes = type.GetCustomAttributes(typeof(AuditProviderAttribute), true);
            if (attributes.Length == 0)
                return new AuditValueResolver();

            AuditProviderAttribute a = attributes.GetValue(0) as AuditProviderAttribute;

            return Activator.CreateInstance(a.ProviderType) as IAuditValueResolver;

        }
    }

    public abstract class AuditValueResolver<TEntity> : IAuditValueResolver
    {
        private AuditValueResolver _defaultProvider = new AuditValueResolver();
        private Dictionary<MemberInfo, CustomizedAuditProperty> _customizedProperties = new Dictionary<MemberInfo, CustomizedAuditProperty>();

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

        public AuditValue GetAuditValue(MemberInfo member, object oldValue, object newValue)
        {
            AuditValue value;

            if (_customizedProperties.ContainsKey(member))
            {
                CustomizedAuditProperty property = _customizedProperties[member];
                Delegate func = property.ValueSelector.Compile();

                value = new AuditValue();
                value.MemberName = property.PropertyName;

                if (oldValue != null)
                    value.OldValue = func.DynamicInvoke(oldValue).ToString();

                if (newValue != null)
                    value.NewValue = func.DynamicInvoke(newValue).ToString();
            }
            else
            {
                value = _defaultProvider.GetAuditValue(member, oldValue, newValue);
            }

            return value;
        }
    }
}
