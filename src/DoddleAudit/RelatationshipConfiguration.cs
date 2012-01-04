using System;
using System.Linq.Expressions;
using System.Reflection;

namespace DoddleAudit
{
    public class RelatationshipConfiguration<TEntity, TParentEntity> : IRelatationshipConfiguration
    {
        private LambdaExpression _pkSelector;
        private LambdaExpression _fkSelector;

        public RelatationshipConfiguration()
        {
            Configuration = new EntityAuditConfiguration();
        }

        public RelatationshipConfiguration<TEntity, TParentEntity> WithForeignKey(Expression<Func<TEntity, object>> selector)
        {
            if (selector == null) throw new ArgumentNullException("selector");
            _fkSelector = selector;
            return this;
        }

        public RelatationshipConfiguration<TEntity, TParentEntity> WithPrimaryKey(Expression<Func<TEntity, object>> selector)
        {
            if (selector == null) throw new ArgumentNullException("selector");
            _pkSelector = selector;
            return this;
        }

        public void WithConfiguration<TConfig>() where TConfig : EntityAuditConfiguration<TEntity>
        {
            WithConfiguration(Activator.CreateInstance<TConfig>());
        }

        public void WithConfiguration(EntityAuditConfiguration<TEntity> config)
        {
            if (config == null) throw new ArgumentNullException("config");
            Configuration = config;
        }

        public Type ParentEntityType { get { return typeof (TParentEntity); } }
        public Type EntityType { get { return typeof(TEntity); } }


        public EntityAuditConfiguration Configuration { get; private set; }

        public PropertyInfo GetEntityPrimaryKey(IAuditableContext context)
        {
            var property = _pkSelector == null
                                 ? context.GetPrimaryKeyProperty(EntityType)
                                 : _pkSelector.ToPropertyInfo();
            return property;
        }

        public PropertyInfo GetParentEntityPrimaryKey(IAuditableContext context)
        {
            var property = _fkSelector == null
                                 ? EntityType.GetProperty(context.GetForeignKeyPropertyName(EntityType, ParentEntityType))
                                 : _fkSelector.ToPropertyInfo();
            return property;
        }
    }
}
