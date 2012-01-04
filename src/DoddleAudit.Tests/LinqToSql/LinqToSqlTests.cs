using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoddleAudit.Tests.LinqToSql
{
    [TestClass]
    public class LinqToSqlTests : RolledBackDbTests
    {
        [TestMethod]
        public void AuditsInsertedRecords()
        {
            var context = new ECommerceDataContext();
            var product = new Product
                              {
                                  CategoryId = 1,
                                  ProductName = "Test"
                              };

            context.Products.InsertOnSubmit(product);
            context.SubmitChanges();

            Assert.AreEqual(1, context.AuditRecords.Count());
        }

        [TestMethod]
        public void AuditsModifiedProperties()
        {
            var context = new ECommerceDataContext();
            var product = new Product
            {
                CategoryId = 1,
                ProductName = "Test"
            };

            context.Products.InsertOnSubmit(product);
            context.SubmitChanges();

            Assert.AreEqual(1, context.AuditRecords.Count());
        }

        [TestMethod]
        public void CanGetPrimaryKeyPropertyFromEntity()
        {
            var context = new ECommerceDataContext();

            var auditableContext = context as IAuditableContext;
            var pi = auditableContext.GetPrimaryKeyProperty(typeof (Product));
            Assert.AreEqual("Id", pi.Name);
        }

        [TestMethod]
        public void CanGetForeignKeyPropertyName()
        {
            var context = new ECommerceDataContext();
            var auditableContext = context as IAuditableContext;
            var propertyName = auditableContext.GetForeignKeyPropertyName(typeof(Promotion), typeof(Product));
            Assert.AreEqual("ProductId", propertyName);
        }
    }
}