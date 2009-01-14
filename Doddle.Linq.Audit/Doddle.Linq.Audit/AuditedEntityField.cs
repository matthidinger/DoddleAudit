using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Doddle.Linq.Audit
{
    public class AuditedEntityField
    {
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
