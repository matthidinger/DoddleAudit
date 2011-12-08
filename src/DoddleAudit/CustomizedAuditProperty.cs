using System.Linq.Expressions;

namespace DoddleAudit
{
    public class CustomizedAuditProperty
    {
        public LambdaExpression ValueSelector { get; set; }
        public string PropertyName { get; set; }
    }
}