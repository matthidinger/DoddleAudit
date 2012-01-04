using System;
using System.Diagnostics;

namespace DoddleAudit
{
    [DebuggerDisplay("Property: {DisplayName}, Old Value: {OldValue}, New Value: {NewValue}")]
    public class AuditedProperty
    {
        public AuditedProperty(string propertyName)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            PropertyName = propertyName;
            DisplayName = propertyName;
            ShortDisplayName = propertyName;
        }

        public string PropertyName { get; set; }
        public string DisplayName { get; set; }
        public string ShortDisplayName { get; set; }

        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
