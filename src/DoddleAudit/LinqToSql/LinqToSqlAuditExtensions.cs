using System;
using System.Data.Linq;
using System.Linq.Expressions;

namespace DoddleAudit.LinqToSql
{
    public static class LinqToSqlAuditExtensions
    {
        /// <summary>
        /// Enlist an Entity for automatic Auditing.
        /// Use this overload to resolve primary keys automatically.
        /// </summary>
        /// <example>db.Products.Audit();</example>
        /// <typeparam name="TEntity">Type of Entity you wish to audit</typeparam>
        public static EntityAuditor<TEntity> Audit<TEntity>(this Table<TEntity> table) where TEntity : class
        {
            var context = (IAuditableContext)table.Context;
            var def = new EntityAuditor<TEntity>(context.GetEntityPkProperty<TEntity>().ToPropertyInfo());
            context.EntityAuditors.Add(def);
            return def;
        }

        /// <summary>
        /// Enlist an Entity for automatic Auditing. 
        /// Use this overload to explicitly declare a primary key selector.
        /// </summary>
        /// <example>db.Products.Audit(p => p.ProductID);</example>
        /// <typeparam name="TEntity">Type of Entity you wish to audit</typeparam>
        public static EntityAuditor<TEntity> Audit<TEntity>(this Table<TEntity> table, Expression<Func<TEntity, object>> pkSelector) where TEntity : class
        {
            var context = (IAuditableContext)table.Context;
            var def = new EntityAuditor<TEntity>(pkSelector.ToPropertyInfo());
            context.EntityAuditors.Add(def);
            return def;
        }
    }
}