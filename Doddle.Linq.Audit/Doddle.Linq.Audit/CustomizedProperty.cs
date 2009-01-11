using System.Linq.Expressions;

namespace Doddle.Linq.Audit
{
    public class CustomizedProperty
    {
        public LambdaExpression ValueSelector { get; set; }
        public string PropertyName { get; set; }
    }
}