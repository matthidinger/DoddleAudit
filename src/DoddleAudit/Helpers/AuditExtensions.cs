using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DoddleAudit
{
    public static class AuditExtensions
    {
        public static IEntityAuditor GetEntityAuditor(this IAuditableContext context, Type entityType)
        {
            var entityAuditor = context.EntityAuditors.FirstOrDefault(d => d.CanAuditType(entityType));

            if (entityAuditor == null)
            {
                var relationshipConfig =
                    context.EntityAuditors.SelectMany(d => d.Configuration.Relationships).FirstOrDefault(d => d.EntityType.IsAssignableFrom(entityType));

                if (relationshipConfig != null)
                {
                    entityAuditor = new RelationshipEntityAuditor(context, relationshipConfig);
                }
            }

            return entityAuditor;
        }


        /// <summary>
        /// Enlist an Entity for automatic Auditing.
        /// Use this overload to resolve primary keys automatically.
        /// </summary>
        /// <example>db.Audit&lt;Product&gt;();</example>
        /// <typeparam name="TEntity">Type of Entity you wish to audit</typeparam>
        public static EntityAuditor<TEntity> Audit<TEntity>(this IAuditableContext context)
        {
            var def = new EntityAuditor<TEntity>(GetEntityPkProperty<TEntity>(context).ToPropertyInfo());
            context.EntityAuditors.Add(def);

            return def;
        }

        /// <summary>
        /// Enlist an Entity for automatic Auditing. 
        /// Use this overload to explicitly declare a primary key selector.
        /// </summary>
        /// <example>db.Audit&lt;Product&gt;(p => p.ProductID);</example>
        /// <typeparam name="TEntity">Type of Entity you wish to audit</typeparam>
        /// <param name="pkSelector">A lambda expression that accepts a TEntity and returns an object representing the primary key</param>
        public static EntityAuditor<TEntity> Audit<TEntity>(this IAuditableContext context, Expression<Func<TEntity, object>> pkSelector)
        {
            var def = new EntityAuditor<TEntity>(pkSelector.ToPropertyInfo());
            context.EntityAuditors.Add(def);
            return def;
        }

        internal static LambdaExpression GetEntityPkProperty<TEntity>(this IAuditableContext context)
        {
            var pk = context.GetPrimaryKeyProperty(typeof(TEntity));
            return GetPropertySelector<TEntity>(context, pk.Name);
        }

        internal static LambdaExpression GetPropertySelector(this IAuditableContext context, Type entityType, string propertyName)
        {
            var param = Expression.Parameter(entityType, "e");
            var selector = Expression.Lambda(Expression.Convert(Expression.Property(param, propertyName), typeof(object)), param);
            return selector;
        }

        internal static LambdaExpression GetPropertySelector<TEntity>(this IAuditableContext context, string propertyName)
        {
            return context.GetPropertySelector(typeof (TEntity), propertyName);
        }

        internal static bool ShouldAuditProperty(this IAuditableContext context, PropertyInfo property, object entity)
        {
            bool auditProperty = true;
            foreach (var rule in context.AuditConfiguration.PropertyAuditRules)
            {
                if (rule(property, entity) == false)
                {
                    auditProperty = false;
                }
            }

            return auditProperty;
        }

        public static void OnlyAuditPropertiesIf(this ContextAuditConfiguration configuration, Func<PropertyInfo, object, bool> condition)
        {
            configuration.PropertyAuditRules.Add(condition);
        }
    }
}