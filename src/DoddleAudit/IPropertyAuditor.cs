using System.Reflection;

namespace DoddleAudit
{
    public interface IPropertyAuditor
    {
        AuditedProperty AuditProperty(PropertyInfo property, object oldValue, object newValue);
    }
}
