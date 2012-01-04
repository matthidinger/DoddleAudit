using System;
using System.Collections.Generic;

namespace DoddleAudit.Sample.Web.Models
{
    public class AuditRecord
    {
        public int Id { get; set; }

        public byte Action { get; set; }

        public DateTime AuditDate { get; set; }

        public string ParentTable { get; set; }

        public int ParentKey { get; set; }

        public string Table { get; set; }

        public int TableKey { get; set; }

        public string UserName { get; set; }

        public virtual ICollection<AuditRecordProperty> ModifiedProperties { get; set; }
    }
}