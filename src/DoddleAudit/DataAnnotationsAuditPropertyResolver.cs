using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace DoddleAudit
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
}