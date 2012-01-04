using System;

namespace DoddleAudit
{
    public interface IEntityAuditor
    {
        AuditedEntity AuditEntity(IAuditableContext context, object entity, AuditAction action);
        IEntityAuditConfiguration Configuration { get; }
        bool CanAuditType(Type entityType);
    }
}
