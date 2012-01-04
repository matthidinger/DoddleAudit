using System;
using System.Linq;

namespace DoddleAudit
{
    public class RelationshipEntityAuditor : EntityAuditor
    {
        private readonly IRelatationshipConfiguration _relatationship;

        public RelationshipEntityAuditor(IAuditableContext context, IRelatationshipConfiguration relatationship) 
            : base(relatationship.EntityType, relatationship.GetEntityPrimaryKey(context))
        {
            _relatationship = relatationship;
            Configuration = relatationship.Configuration;
        }

        public override AuditedEntity AuditEntity(IAuditableContext context, object entity, AuditAction action)
        {
            Action<AuditedEntity> updateKeys = audited =>
                                                   {
                                                       var pkProperty = _relatationship.GetEntityPrimaryKey(context);
                                                       var pk = pkProperty.GetValue(audited.Entity, null);

                                                       audited.EntityKey = new EntityKey(pk);

                                                       var field = audited.ModifiedProperties.SingleOrDefault(f => f.PropertyName == pkProperty.Name);
                                                       if (field != null)
                                                           field.NewValue = pk.ToString();


                                                       var fkProperty = _relatationship.GetParentEntityPrimaryKey(context);
                                                       var fk = fkProperty.GetValue(audited.Entity, null);

                                                       audited.ParentKey = new EntityKey(fk);

                                                       var parentField = audited.ModifiedProperties.SingleOrDefault(f => f.PropertyName == fkProperty.Name);
                                                       if (parentField != null)
                                                           parentField.NewValue = fk.ToString();

                                                       //var fkProperty = _relatationship.FkSelector == null
                                                       //    ? context.GetEntityPrimaryKey(_relatationship.ParentEntityType)
                                                       //    : _relatationship.PkSelector.ToPropertyInfo();

                                                       //var fk = fkProperty.GetValue(audited.Entity, null);

                                                       //audited.AssociationTableKey = new EntityKey(pk);
                                                       //if (_relatationship.FkSelector == null)
                                                       //{
                                                       //    var relationshipKeyName = context.GetForeignKeyPropertyName(_relatationship.ParentEntityType, _relatationship.EntityType);
                                                       //    object fk = context.GetPropertySelector(_relatationship.EntityType, relationshipKeyName);
                                                       //    audited.EntityTableKey = new EntityKey(fk);
                                                       //}
                                                       //else
                                                       //{
                                                       //    object fk = _relatationship.FkSelector.Compile().DynamicInvoke(audited.Entity);
                                                       //    audited.EntityTableKey = new EntityKey(fk);
                                                       //}
                                                   };

            var auditedEntity = new AuditedEntity(entity, action, updateKeys)
                                    {
                                        EntityType = context.GetEntityType(_relatationship.EntityType),
                                        ParentEntityType = context.GetEntityType(_relatationship.ParentEntityType),
                                    };

            AddModifiedPropertiesToRecord(context, auditedEntity);
            return auditedEntity;
        }
    }
}