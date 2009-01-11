using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Doddle.Linq.Audit
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class AuditProviderAttribute : Attribute
    {
        readonly Type _providerType;

        // This is a positional argument
        public AuditProviderAttribute(Type providerType)
        {
            _providerType = providerType;
        }

        public Type ProviderType { get { return _providerType; } }
    }
}
