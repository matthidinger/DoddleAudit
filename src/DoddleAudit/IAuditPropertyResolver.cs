using System.Reflection;

namespace DoddleAudit
{
    public interface IAuditPropertyResolver
    {
        AuditedEntityField GetAuditValue(MemberInfo member, object oldValue, object newValue);
        bool IsMemberCustomized(MemberInfo member);
    }
}
