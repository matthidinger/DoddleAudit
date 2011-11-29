using System;
using System.Data.Linq;
using System.Linq.Expressions;

namespace Doddle.Audit.LinqToSql
{
    public static class LinqToSqlAuditExtensions
    {
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
            def.PkSelector = AuditExtensions.GetEntityPkProperty<TEntity>(context);

            context.AuditDefinitions.Add(def);

            return def;
        }

        /// <summary>
        /// Enlist an Entity for automatic Auditing. 
        /// Use this overload to explicitly declare a primary key selector.
        /// </summary>
        /// <example>db.Products.Audit(p => p.ProductID);</example>
        /// <typeparam name="TEntity">Type of Entity you wish to audit</typeparam>
        public static AuditDefinition<TEntity> Audit<TEntity>(this Table<TEntity> table, Expression<Func<TEntity, object>> pkSelector) where TEntity : class
        {
            IAuditableContext context = (IAuditableContext)table.Context;

            AuditDefinition<TEntity> def = new AuditDefinition<TEntity>(context);
            def.PkSelector = pkSelector;

            context.AuditDefinitions.Add(def);

            return def;
        }
    }
}