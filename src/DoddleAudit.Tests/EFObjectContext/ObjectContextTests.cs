using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoddleAudit.Tests.EFObjectContext
{
    [TestClass]
    public class ObjectContextTests : RolledBackDbTests
    {
        [TestMethod]
        public void AuditableObjectContextAuditsCorrectly()
        {
            var context = new ECommerceEntities();

            var product = new Product
            {
                CategoryId = 1,
                ProductName = "Test"
            };

            context.Products.AddObject(product);
            context.SaveChanges();

            Assert.AreEqual(1, context.AuditRecords.Count());

            //Product edited = context.ProductSet.First(p => p.ProductID == 1);
            //edited.ProductName = "Edited product";
            //edited.Categories = category;


            //Product deleted = context.ProductSet.Include("Categories").First(p => p.ProductID == 110);
            //context.DeleteObject(deleted);

            context.SaveChanges();
        }
    }
}
