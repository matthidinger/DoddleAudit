using System;

namespace DoddleAudit.Tests.EFObjectContext
{
    public partial class ECommerceEntities
    {
        partial void OnContextCreated()
        {
            Audit(Products);
            AuditConfiguration.OnlyAuditPropertiesIf((property, entity) => !property.Name.EndsWith("Id"));
        }

        public override void SaveAuditedEntity(AuditedEntity auditedEntity)
        {
            var audit = new AuditRecord
            {
                Action = (byte)auditedEntity.Action,
                AuditDate = DateTime.Now,
                ParentTable = auditedEntity.ParentEntityType.Name,
                ParentKey = auditedEntity.ParentKey,
                Table = auditedEntity.EntityType.Name,
                TableKey = auditedEntity.EntityKey,
                UserName = "Integration Test",
            };

            foreach (var modifiedProperty in auditedEntity.ModifiedProperties)
            {
                audit.AuditRecordProperties.Add(
                    new AuditRecordProperty
                        {
                            PropertyName = modifiedProperty.DisplayName,
                            OldValue = modifiedProperty.OldValue,
                            NewValue = modifiedProperty.NewValue
                        });
            }
          
            if (audit.AuditRecordProperties.Count > 0)
            {
                AuditRecords.AddObject(audit);
            }

        }
    }
}