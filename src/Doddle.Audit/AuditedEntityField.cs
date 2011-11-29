using System;
using System.Diagnostics;
using Doddle.Audit.Helpers;

namespace Doddle.Audit
{
    [DebuggerDisplay("{DisplayName}, {OldValue}, {NewValue}")]
    public class AuditedEntityField
    {
        public AuditedEntityField(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            FieldName = name;
            DisplayName = name.SplitUpperCaseToString();
            ShortDisplayName = name.SplitUpperCaseToString();
        }

        public string FieldName { get; set; }
        public string DisplayName { get; set; }
        public string ShortDisplayName { get; set; }

        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
