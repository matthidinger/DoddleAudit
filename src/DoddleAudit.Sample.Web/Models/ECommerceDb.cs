using System;
using System.Data.Entity;
using System.Linq;
using DoddleAudit.EntityFramework;

namespace DoddleAudit.Sample.Web.Models
{
    public class ECommerceDb : AuditableDbContext
    {
        public IDbSet<Category> Categories { get; set; }
        public IDbSet<Product> Products { get; set; }
        public IDbSet<AuditRecord> AuditRecords { get; set; }

        public ECommerceDb()
        {
            Audit(Categories);
            Audit(Products).WithConfiguration<ProductAuditConfig>();

            AuditConfiguration.EmptyPropertyMode = EmptyPropertyMode.AlwaysAudit;
            AuditConfiguration.OnlyAuditPropertiesIf((property, entity) => !property.Name.EndsWith("Id"));
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ProductConfig());
        }

        
        public override void SaveAuditedEntity(AuditedEntity auditedEntity)
        {
            var userName = "matt"; // HttpContext.Current.User.Identity.Name;

            var audit = new AuditRecord
            {
                Action = (byte)auditedEntity.Action,
                AuditDate = DateTime.Now,
                ParentTable = auditedEntity.ParentEntityType.Name,
                ParentKey = auditedEntity.ParentKey,
                Table = auditedEntity.EntityType.Name,
                TableKey = auditedEntity.EntityKey,
                UserName = userName,
                ModifiedProperties = auditedEntity.ModifiedProperties
                    .Select(auditedProperty => new AuditRecordProperty
                    {
                        PropertyName = auditedProperty.DisplayName,
                        OldValue = auditedProperty.OldValue,
                        NewValue = auditedProperty.NewValue
                    })
                    .ToList()
            };

            if (audit.ModifiedProperties.Count > 0)
            {
                AuditRecords.Add(audit);
            }
        }
    }


    public class ProductAuditConfig : EntityAuditConfiguration<Product>
    {
        public ProductAuditConfig()
        {
            AuditProperty(m => m.ProductName)
                .GetValueFrom(m => m)
                .WithPropertyName("ProdName");

            AuditProperty(m => m.Category)
                .GetValueFrom(m => m.CategoryName)
                .WithPropertyName("Category");

            AuditMany(m => m.Promotions)
                .WithForeignKey(m => m.ProductId);
        }
    }
}