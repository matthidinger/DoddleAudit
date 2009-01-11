using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Doddle.Linq.Audit
{
    public class AuditValue
    {
        public string MemberName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
