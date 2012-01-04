using System.Data.Entity;

namespace DoddleAudit.Sample.Web.Models
{
    public class ECommerceDbInitializer : DropCreateDatabaseIfModelChanges<ECommerceDb>
    {
        protected override void Seed(ECommerceDb context)
        {
            var beverages = new Category {CategoryName = "Beverages"};
            context.Categories.Add(beverages);

            context.Products.Add(new Product { Category = beverages, ProductName = "Chai"});
        }
    }
}