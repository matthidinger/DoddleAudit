namespace DoddleAudit.Sample.Web.Models
{
    public class AuditRecordProperty
    {
        public int Id { get; set; }

        public virtual int AuditRecordId { get; set; }

        public virtual AuditRecord AuditRecord { get; set; }

        public string PropertyName { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }
    }
}