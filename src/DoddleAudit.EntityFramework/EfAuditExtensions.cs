using System;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Reflection;

namespace DoddleAudit.EntityFramework
{
    public static class AuditExtensions
    {
        ///// <summary>
        ///// Enlist an Entity for automatic Auditing.
        ///// Use this overload to resolve primary keys automatically.
        ///// </summary>
        ///// <example>db.Audit&lt;Product&gt;();</example>
        ///// <typeparam name="TEntity">Type of Entity you wish to audit</typeparam>
        //public static AuditDefinition<TEntity> Audit<TEntity>(this IAuditableContext context)
        //{
        //    AuditDefinition<TEntity> def = new AuditDefinition<TEntity>(context);
        //    def.PkSelector = GetEntityPkProperty<TEntity>(context);

        //    context.AuditDefinitions.Add(def);

        //    return def;
        //}

        ///// <summary>
        ///// Enlist an Entity for automatic Auditing. 
        ///// Use this overload to explicitly declare a primary key selector.
        ///// </summary>
        ///// <example>db.Audit&lt;Product&gt;(p => p.ProductID);</example>
        ///// <typeparam name="TEntity">Type of Entity you wish to audit</typeparam>
        ///// <param name="pkSelector">A lambda expression that accepts a TEntity and returns an object representing the primary key</param>
        //public static AuditDefinition<TEntity> Audit<TEntity>(this IAuditableContext context, Expression<Func<TEntity, object>> pkSelector)
        //{
        //    AuditDefinition<TEntity> def = new AuditDefinition<TEntity>(context);
        //    def.PkSelector = pkSelector;

        //    context.AuditDefinitions.Add(def);

        //    return def;
        //}

        /// <summary>
        /// Enlist an Entity for automatic Auditing.
        /// Use this overload to resolve primary keys automatically.
        /// </summary>
        /// <example>db.Products.Audit();</example>
        /// <typeparam name="TEntity">Type of Entity you wish to audit</typeparam>
        public static AuditDefinition<TEntity> Audit<TContext, TEntity>(this TContext dataContext, Func<TContext, IDbSet<TEntity>> table)
            where TContext : DbContext
            where TEntity : class
        {
            var context = (IAuditableContext)dataContext;
            var def = new AuditDefinition<TEntity>(context)
                          {
                              PkSelector = GetEntityPkProperty<TEntity>(context)
                          };

            context.AuditDefinitions.Add(def);

            return def;
        }

        ///// <summary>
        ///// Enlist an Entity for automatic Auditing. 
        ///// Use this overload to explicitly declare a primary key selector.
        ///// </summary>
        ///// <example>db.Products.Audit(p => p.ProductID);</example>
        ///// <typeparam name="TEntity">Type of Entity you wish to audit</typeparam>
        //public static AuditDefinition<TEntity> Audit<TEntity>(this Table<TEntity> table, Expression<Func<TEntity, object>> pkSelector) where TEntity : class
        //{
        //    IAuditableContext context = (IAuditableContext)table.Context;

        //    AuditDefinition<TEntity> def = new AuditDefinition<TEntity>(context);
        //    def.PkSelector = pkSelector;

        //    context.AuditDefinitions.Add(def);

        //    return def;
        //}

        internal static LambdaExpression GetEntityPkProperty<TEntity>(this IAuditableContext context)
        {
            var pk = context.GetEntityPrimaryKey<TEntity>();
            return GetPropertySelector<TEntity>(context, pk.Name);
        }

        internal static LambdaExpression GetPropertySelector<TEntity>(this IAuditableContext context, string propertyName)
        {
            Type entityType = typeof(TEntity);
            var param = Expression.Parameter(entityType, "e");


            Expression<Func<TEntity, object>> selector =
                Expression.Lambda<Func<TEntity, object>>(Expression.Convert(Expression.Property(param, propertyName), typeof(object)), param);

            return selector;
        }

        internal static bool ShouldAuditProperty(this IAuditableContext context, MemberInfo member, object entity)
        {
            bool auditProperty = true;
            foreach (var rule in context.PropertyAuditRules)
            {
                if (rule(member, entity) == false)
                {
                    auditProperty = false;
                }
            }

            return auditProperty;
        }

        public static void ExcludePropertyIf(this IAuditableContext context, Func<MemberInfo, object, bool> condition)
        {
            context.PropertyAuditRules.Add(condition);
        }
    }
}