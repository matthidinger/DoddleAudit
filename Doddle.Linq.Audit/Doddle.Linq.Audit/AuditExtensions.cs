using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq.Expressions;
using System.Linq;

namespace Doddle.Linq.Audit
{
    public static class AuditExtensions
    {
        /// <summary>
        /// Enlist an Entity for automatic Auditing.
        /// Use this overload to resolve primary keys automatically.
        /// </summary>
        /// <example>db.Audit&lt;Product&gt;();</example>
        /// <typeparam name="TEntity">Type of Entity you wish to audit</typeparam>
        public static AuditDefinition<TEntity> Audit<TEntity>(this IAuditableContext context)
        {
            AuditDefinition<TEntity> def = new AuditDefinition<TEntity>(context);
            //def.EntityDisplaySelector = entityNameSelector;
            def.PkSelector = context.GetEntityPkSelector<TEntity>();

            context.AuditDefinitions.Add(def);

            return def;
        }

        /// <summary>
        /// Enlist an Entity for automatic Auditing. 
        /// Use this overload to explicitly declare a primary key selector.
        /// </summary>
        /// <example>db.Audit&lt;Product&gt;(p => p.ProductID);</example>
        /// <typeparam name="TEntity">Type of Entity you wish to audit</typeparam>
        /// <param name="pkSelector">A lambda expression that accepts a TEntity and returns an int representing the primary key</param>
        public static AuditDefinition<TEntity> Audit<TEntity>(this IAuditableContext context, Expression<Func<TEntity, int>> pkSelector)
        {
            AuditDefinition<TEntity> def = new AuditDefinition<TEntity>(context);
            def.PkSelector = pkSelector;

            context.AuditDefinitions.Add(def);

            return def;
        }

        /// <summary>
        /// Enlist an Entity for automatic Auditing.
        /// Use this overload to resolve primary keys automatically.
        /// </summary>
        /// <example>db.Products.Audit();</example>
        /// <typeparam name="TEntity">Type of Entity you wish to audit</typeparam>
        public static AuditDefinition<TEntity> Audit<TEntity>(this Table<TEntity> table) where TEntity : class
        {
            IAuditableContext context = (IAuditableContext)table.Context;

            AuditDefinition<TEntity> def = new AuditDefinition<TEntity>(context);
            def.PkSelector = context.GetEntityPkSelector<TEntity>();

            context.AuditDefinitions.Add(def);

            return def;
        }

        /// <summary>
        /// Enlist an Entity for automatic Auditing. 
        /// Use this overload to explicitly declare a primary key selector.
        /// </summary>
        /// <example>db.Products.Audit(p => p.ProductID);</example>
        /// <typeparam name="TEntity">Type of Entity you wish to audit</typeparam>
        public static AuditDefinition<TEntity> Audit<TEntity>(this Table<TEntity> table, Expression<Func<TEntity, int>> pkSelector) where TEntity : class
        {
            IAuditableContext context = (IAuditableContext)table.Context;

            AuditDefinition<TEntity> def = new AuditDefinition<TEntity>(context);
            def.PkSelector = pkSelector;

            context.AuditDefinitions.Add(def);

            return def;
        }

        internal static LambdaExpression GetEntityPkSelector<TEntity>(this IAuditableContext context)
        {
            var pk = context.GetEntityPrimaryKey<TEntity>();
            return GetEntityPropertySelector<TEntity, int>(context, pk.Name);
        }

        internal static LambdaExpression GetEntityPropertySelector<TEntity, TProp>(this IAuditableContext context, string propertyName)
        {
            Type entityType = typeof(TEntity);

            var param = Expression.Parameter(entityType, "e");

            Expression<Func<TEntity, TProp>> selector =
                Expression.Lambda<Func<TEntity, TProp>>(Expression.Property(param, propertyName), param);

            return selector;
        }
    }
}