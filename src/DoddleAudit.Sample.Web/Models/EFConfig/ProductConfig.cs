using System.Data.Entity.ModelConfiguration;

namespace DoddleAudit.Sample.Web.Models
{
    public class ProductConfig : EntityTypeConfiguration<Product>
    {
        public ProductConfig()
        {
            Property(m => m.ProductName);
            HasMany(m => m.Promotions).WithRequired(m => m.Product).HasForeignKey(m => m.ProductId);
        }
    }
}