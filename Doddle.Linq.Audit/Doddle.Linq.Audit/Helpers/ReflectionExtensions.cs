using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Reflection;

namespace Doddle.Reflection
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
    }
}
