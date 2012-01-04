using System.Data.Entity;
using System.Linq;
using DoddleAudit.Sample.Web.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoddleAudit.Tests.EFDbContext
{
    [TestClass]
    public class DbContextTests : RolledBackDbTests
    {
        [TestMethod]
        public void AuditsInsertedRecords()
        {
            Database.SetInitializer(new ECommerceDbInitializer());
            var context = new ECommerceDb();

            var product = new Product
            {
                CategoryId = 1,
                ProductName = "Test"
            };

            context.Products.Add(product);
            context.SaveChanges();

            Assert.AreEqual(1, context.AuditRecords.Count());
        }
    }
}
