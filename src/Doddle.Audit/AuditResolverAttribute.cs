using System;

namespace Doddle.Audit
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class AuditResolverAttribute : Attribute
    {
        readonly Type _resolverType;

        /// <summary>
        /// The type of class that will be used to resolve properties when this Entity is audited. The Resolver must derive from AuditPropertyResolver
        /// </summary>
        public AuditResolverAttribute(Type resolverType)
        {
            _resolverType = resolverType;
        }

        /// <summary>
        /// The type of class that will be used to resolve properties when this Entity is audited. The Resolver must derive from AuditPropertyResolver
        /// </summary>
        public Type ResolverType { get { return _resolverType; } }
    }
}
