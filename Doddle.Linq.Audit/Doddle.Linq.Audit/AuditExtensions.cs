using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq.Expressions;
using System.Linq;

namespace Doddle.Linq.Audit
{
    public static class AuditExtensions
    {
        public static AuditDefinition<T> Audit<T>(this IAuditableContext context)
        {
            AuditDefinition<T> def = new AuditDefinition<T>(context);
            //def.EntityDisplaySelector = entityNameSelector;
            def.PkSelector = GetEntityPkSelector<T>(context);

            context.AuditDefinitions.Add(def);

            return def;
        }

        public static LambdaExpression GetEntityPkSelector<TEntity>(IAuditableContext context)
        {
            var pk = context.GetEntityPrimaryKey<TEntity>();
            return GetEntityPropertySelector<TEntity, int>(context, pk.Name);
        }

        public static LambdaExpression GetEntityPropertySelector<TEntity, TReturn>(IAuditableContext context, string propertyName)
        {
            Type entityType = typeof(TEntity);

            var param = Expression.Parameter(entityType, "e");

            Expression<Func<TEntity, TReturn>> selector =
                Expression.Lambda<Func<TEntity, TReturn>>(Expression.Property(param, propertyName), param);

            return selector;
        }

        public static AuditDefinition<T> Audit<T>(this Table<T> table) where T : class
        {
            IAuditableContext context = (IAuditableContext)table.Context;

            AuditDefinition<T> def = new AuditDefinition<T>(context);
            def.PkSelector = GetEntityPkSelector<T>(context);

            context.AuditDefinitions.Add(def);

            return def;
        }

        public static AuditDefinition<T> Audit<T>(this IAuditableContext context, Expression<Func<T, int>> pkSelector, Expression<Func<T, string>> entityNameSelector)
        {
            AuditDefinition<T> def = new AuditDefinition<T>(context);
            def.EntityDisplaySelector = entityNameSelector;
            def.PkSelector = pkSelector;

            context.AuditDefinitions.Add(def);

            return def;
        }

        public static AuditDefinition<T> Audit<T>(this Table<T> table, Expression<Func<T, int>> pkSelector, Expression<Func<T, string>> entityNameSelector) where T : class
        {
            IAuditableContext context = (IAuditableContext)table.Context;

            AuditDefinition<T> def = new AuditDefinition<T>(context);
            def.EntityDisplaySelector = entityNameSelector;
            def.PkSelector = pkSelector;

            context.AuditDefinitions.Add(def);

            return def;
        }


        public static AuditDefinition<T> Relationship<T, TR>(this AuditDefinition<T> definition, Expression<Func<TR, int>> pkSelector, Expression<Func<TR, int>> fkSelector, Expression<Func<TR, string>> entityNameSelector)
        {

            var relationship = new AuditRelationship<TR>();
            relationship.EntityDisplaySelector = entityNameSelector;
            relationship.PrimaryEntityType = typeof(T);
            relationship.PkSelector = pkSelector;
            relationship.FkSelector = fkSelector;
            definition.Relationships.Add(relationship);

            return definition;
        }

        public static AuditDefinition<T> Relationship<T, TR>(this AuditDefinition<T> definition, Expression<Func<T, IEnumerable<TR>>> relatedEntity) where TR : class
        {
            var relationship = new AuditRelationship<TR>();


            //relationship.EntityDisplaySelector = entityNameSelector;
            relationship.PrimaryEntityType = typeof(T);
            relationship.PkSelector = GetEntityPkSelector<TR>(definition.Context);

            relationship.FkSelector = GetEntityPropertySelector<TR, int?>(definition.Context,
                                                                    definition.Context.GetEntityRelationshipKeyName<T, TR>());
            
            definition.Relationships.Add(relationship);

            return definition;
        }
    }
}