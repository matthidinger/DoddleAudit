using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Doddle.Linq.Audit
{
    public class AuditRecord
    {
        public object Entity { get; set; }
        public LambdaExpression KeySelector { get; set; }

        public string PrimaryTable { get; set; }
        public int PrimaryTableKey { get; set; }

        public string ModifiedTable { get; set; }
        public int ModifiedTableKey { get; set; }

        public string ChangeDescription { get; set; }

        public AuditAction Action { get; set; }

        private List<AuditValue> _values = new List<AuditValue>();
        public IList<AuditValue> Values { get { return _values; } }
    }
}
