using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using Doddle.Audit.Helpers;

namespace Doddle.Audit
{
    public class DataAnnotationsAuditPropertyResolver : AuditPropertyResolver
    {
        private static string GetPropertyValue(DisplayFormatAttribute format, object input)
        {
            if (input == null)
            {
                if (format != null && format.NullDisplayText != null)
                    return format.NullDisplayText;

                return null;
            }

            if (format != null && format.DataFormatString != null)
            {
                return string.Format(format.DataFormatString, input);
            }

            return input.ToString();
        }

        public override AuditedEntityField GetAuditValue(MemberInfo member, object oldValue, object newValue)
        {
            var field = new AuditedEntityField(member.Name);

            var attributes = new List<Attribute>(member.GetCustomAttributes(true).OfType<Attribute>());

            var dataTypeAttribute = attributes.OfType<DataTypeAttribute>().FirstOrDefault();

            var displayFormatAttribute = attributes.OfType<DisplayFormatAttribute>().FirstOrDefault();
            if (displayFormatAttribute == null && dataTypeAttribute != null)
            {
                displayFormatAttribute = dataTypeAttribute.DisplayFormat;
            }

            var displayAttribute = attributes.OfType<DisplayAttribute>().FirstOrDefault();
            string str = null;
            if (displayAttribute != null)
            {
                field.ShortDisplayName = displayAttribute.GetShortName();
                str = displayAttribute.GetName();
            }
            if (str != null)
            {
                field.DisplayName = str;
            }
            else
            {
                var displayNameAttribute = attributes.OfType<DisplayNameAttribute>().FirstOrDefault();
                if (displayNameAttribute != null)
                    field.DisplayName = displayNameAttribute.DisplayName;
            }

            field.OldValue = GetPropertyValue(displayFormatAttribute, oldValue);
            field.NewValue = GetPropertyValue(displayFormatAttribute, newValue);

            return field;
        }
    }

    public class AuditPropertyResolver : IAuditPropertyResolver
    {
        private static string GetPropertyValue(object input)
        {
            return (input == null) ? string.Empty : input.ToString();
        }

        public virtual AuditedEntityField GetAuditValue(MemberInfo member, object oldValue, object newValue)
        {
            var value = new AuditedEntityField(member.Name)
                            {
                                OldValue = GetPropertyValue(oldValue),
                                NewValue = GetPropertyValue(newValue)
                            };

            return value;
        }

        public virtual bool IsMemberCustomized(MemberInfo member)
        {
            return false;
        }

        public static IAuditPropertyResolver GetResolver<TEntity>()
        {
            return GetResolver(typeof(TEntity));
        }

        public static IAuditPropertyResolver GetResolver(Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(AuditResolverAttribute), true);
            if (attributes.Length == 0)
                return null;

            var a = attributes.GetValue(0) as AuditResolverAttribute;

            return Activator.CreateInstance(a.ResolverType) as IAuditPropertyResolver;

        }
    }

    public abstract class AuditPropertyResolver<TEntity> : IAuditPropertyResolver
    {
        protected AuditPropertyResolver()
        {
            CustomizeProperties();
        }

        private readonly AuditPropertyResolver _defaultResolver = new DataAnnotationsAuditPropertyResolver();
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
            var prop = new CustomizedAuditProperty {ValueSelector = valueSelector, PropertyName = newPropertyName};
            var exp = propertySelector.ToPropertyInfo();
            _customizedProperties.Add(exp, prop);
        }

        public AuditedEntityField GetAuditValue(MemberInfo member, object oldValue, object newValue)
        {
            AuditedEntityField value;

            if (_customizedProperties.ContainsKey(member))
            {
                CustomizedAuditProperty property = _customizedProperties[member];
                Delegate func = property.ValueSelector.Compile();

                value = new AuditedEntityField(property.PropertyName);

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

        public bool IsMemberCustomized(MemberInfo member)
        {
            return _customizedProperties.ContainsKey(member);
        }
    }
}
