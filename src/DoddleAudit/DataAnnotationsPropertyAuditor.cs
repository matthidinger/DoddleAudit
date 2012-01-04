using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace DoddleAudit
{
    public class DataAnnotationsPropertyAuditor : PropertyAuditor
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

        public override AuditedProperty AuditProperty(PropertyInfo property, object oldValue, object newValue)
        {
            var auditedProperty = new AuditedProperty(property.Name);

            var attributes = new List<Attribute>(property.GetCustomAttributes(true).OfType<Attribute>());

            var dataTypeAttribute = attributes.OfType<DataTypeAttribute>().FirstOrDefault();

            var displayFormatAttribute = attributes.OfType<DisplayFormatAttribute>().FirstOrDefault();
            if (displayFormatAttribute == null && dataTypeAttribute != null)
            {
                displayFormatAttribute = dataTypeAttribute.DisplayFormat;
            }

            var displayAttribute = attributes.OfType<DisplayAttribute>().FirstOrDefault();
            auditedProperty.DisplayName = property.Name.SplitUpperCaseToString();
 
            if (displayAttribute != null)
            {
                auditedProperty.ShortDisplayName = displayAttribute.GetShortName();
                auditedProperty.DisplayName = displayAttribute.GetName();
            }
            else
            {
                var displayNameAttribute = attributes.OfType<DisplayNameAttribute>().FirstOrDefault();
                if (displayNameAttribute != null)
                    auditedProperty.DisplayName = displayNameAttribute.DisplayName;
            }

            auditedProperty.OldValue = GetPropertyValue(displayFormatAttribute, oldValue);
            auditedProperty.NewValue = GetPropertyValue(displayFormatAttribute, newValue);

            return auditedProperty;
        }
    }
}