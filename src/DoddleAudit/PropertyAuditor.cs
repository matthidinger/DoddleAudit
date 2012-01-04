using System.Reflection;

namespace DoddleAudit
{
    public class PropertyAuditor : IPropertyAuditor
    {
#if NET35
        public static IPropertyAuditor Default = new PropertyAuditor();
#else
        public static IPropertyAuditor Default = new DataAnnotationsPropertyAuditor();
#endif

        private static string GetPropertyValue(object input)
        {
            return (input == null) ? string.Empty : input.ToString();
        }

        public virtual AuditedProperty AuditProperty(PropertyInfo property, object oldValue, object newValue)
        {
            var value = new AuditedProperty(property.Name)
                            {
                                DisplayName = property.Name.SplitUpperCaseToString(),
                                ShortDisplayName = property.Name.SplitUpperCaseToString(),
                                OldValue = GetPropertyValue(oldValue),
                                NewValue = GetPropertyValue(newValue)
                            };

            return value;
        }
    }
}
