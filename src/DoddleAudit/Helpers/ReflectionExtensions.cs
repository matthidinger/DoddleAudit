using System;
using System.Reflection;
using System.Linq.Expressions;

namespace DoddleAudit
{
    public static class ReflectionExtensions
    {
        public static bool HasAttribute(this Type t, Type attrType)
        {
            return t.GetCustomAttributes(attrType, true).Length > 0;
        }

        public static bool HasAttribute(this MemberInfo mi, Type attrType)
        {
            return mi.GetCustomAttributes(attrType, false).Length > 0;
        }

        public static bool IsNullable(this Type theType)
        {
            return (theType.IsGenericType && theType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
        }

        public static PropertyInfo ToPropertyInfo(this LambdaExpression expression)
        {
            MemberExpression memberExpression;

            var unaryExpression = expression.Body as UnaryExpression;
            if(unaryExpression != null)
            {
               memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = expression.Body as MemberExpression;
            }

            return (PropertyInfo)memberExpression.Member;
        }
    }
}
