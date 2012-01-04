using System;
using System.Linq.Expressions;
using System.Reflection;

namespace DoddleAudit
{
    public interface IRelatationshipConfiguration
    {
        Type EntityType { get; }
        Type ParentEntityType { get; }
        EntityAuditConfiguration Configuration { get; }
        PropertyInfo GetEntityPrimaryKey(IAuditableContext context);
        PropertyInfo GetParentEntityPrimaryKey(IAuditableContext context);
    }
}
