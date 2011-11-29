using System.Reflection;

namespace Doddle.Audit
{
    public interface IAuditPropertyResolver
    {
        AuditedEntityField GetAuditValue(MemberInfo member, object oldValue, object newValue);
        bool IsMemberCustomized(MemberInfo member);
    }
}
