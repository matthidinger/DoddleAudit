using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace Doddle.Linq.Audit
{
    public class AuditDefinition<TEntity> : IAuditDefinition
    {
        private readonly IAuditableContext _context;
        public AuditDefinition(IAuditableContext context)
        {
            _context = context;
        }
        public IAuditableContext Context
        {
            get { return _context; }
        }

        public Type EntityType
        {
            get { return typeof(TEntity); }
        }

        public LambdaExpression PkSelector { get; set; }

        private readonly List<IAuditAssociation> _relationships = new List<IAuditAssociation>();
        public IList<IAuditAssociation> Relationships
        {
            get
            {
                return _relationships;
            }
        }


        /// <summary>
        /// Include an association (relationship) table to audit along with the parent table. 
        /// Use this overload to explicity define the primary and foreign key relationship of the association.
        /// </summary>
        /// <typeparam name="TEntity">Type of Entity that the parent table represents</typeparam>
        /// <typeparam name="TAssociation">Type of associted Entity to audit with the parent</typeparam>
        /// <param name="pkSelector">A lambda expression that returns the primary key property of the Associated table</param>
        /// <param name="fkSelector">A lambda expression that returns the foreign key property, which refers to the primary key of the parent entity</param>
        /// <example>db.Orders.Audit(o => o.OrderID).AuditAssociation&lt;Order_Details&gt;(od => od.Order_DetailID, od => od.OrderID);</example>
        public AuditDefinition<TEntity> AuditAssociation<TAssociation>(Expression<Func<TAssociation, object>> pkSelector, Expression<Func<TAssociation, object>> fkSelector)
        {
            var relationship = new AuditAssociation<TAssociation>(this);
            relationship.PkSelector = pkSelector;
            relationship.FkSelector = fkSelector;
            Relationships.Add(relationship);

            return this;
        }

        /// <summary>
        /// Include an association (relationship) table to audit along with the parent table. 
        /// Use this overload if the parent table has an IEnumerable property representing the associated entity.
        /// </summary>
        /// <typeparam name="TEntity">Type of Entity that the parent table represents</typeparam>
        /// <typeparam name="TAssociation">Type of associted Entity to audit with the parent</typeparam>
        /// <param name="relatedEntity">A lambda expression that returns an IEnumerable property representing the associated table</param>
        /// <example>db.Orders.Audit(o => o.OrderID).AuditAssociation(o => o.Order_Details);</example>
        public AuditDefinition<TEntity> AuditAssociation<TAssociation>(Expression<Func<TEntity, IEnumerable<TAssociation>>> relatedEntity) where TAssociation : class
        {
            var relationship = new AuditAssociation<TAssociation>(this);
            relationship.PkSelector = Context.GetEntityPkProperty<TAssociation>();

            relationship.FkSelector = Context.GetPropertySelector<TAssociation>(Context.GetEntityRelationshipKeyName<TEntity, TAssociation>());

            Relationships.Add(relationship);

            return this;
        }

    }

}
