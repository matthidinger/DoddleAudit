using System;
using System.Reflection;

namespace DoddleAudit
{
    public class ModifiedProperty
    {
        public ModifiedProperty(PropertyInfo property, object originalValue, object currentValue)
        {
            if (property == null) throw new ArgumentNullException("property");

            Property = property;
            OriginalValue = originalValue;
            CurrentValue = currentValue;
        }

        public PropertyInfo Property { get; set; }
        public object OriginalValue { get; set; }
        public object CurrentValue { get; set; }
    }
}