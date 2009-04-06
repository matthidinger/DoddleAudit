using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Reflection;
using System.Linq.Expressions;

namespace Doddle.Linq.Audit
{
    public static class ReflectionExtensions
    {
        // TODO: Remove dependency on System.Web/DataBinder
        public static T GetProperty<T>(this object item, string expresssion)
        {
            T tmp = default(T);
            tmp = (T)DataBinder.Eval(item, expresssion);
            return tmp;
        }

        public static bool HasAttribute(this Type t, Type attrType)
        {
            return t.GetCustomAttributes(attrType, true) != null;
        }

        public static bool HasAttribute(this MemberInfo mi, Type attrType)
        {
            return mi.GetCustomAttributes(attrType, false) != null;
        }

        public static PropertyInfo ToPropertyInfo(this LambdaExpression expression)
        {
            MemberExpression memberExpression;

            UnaryExpression unaryExpression = expression.Body as UnaryExpression;
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
