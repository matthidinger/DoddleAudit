using System.Collections.Generic;
using System.Reflection;

namespace DoddleAudit
{
    public interface IEntityAuditConfiguration
    {
        IList<IRelatationshipConfiguration> Relationships { get; }
        AuditedProperty AuditProperty(PropertyInfo property, object oldValue, object newValue);
        bool IsPropertyCustomized(PropertyInfo propertyInfo);
    }
}