using System.Linq.Expressions;

namespace Doddle.Audit
{
    public class CustomizedAuditProperty
    {
        public LambdaExpression ValueSelector { get; set; }
        public string PropertyName { get; set; }
    }
}