using System.Linq;
using DoddleAudit.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DoddleAudit.Tests
{
    public class ProductAuditResolver : AuditPropertyResolver<Product>
    {
        protected override void CustomizeProperties()
        {
            CustomizeProperty(p => p.CategoryID, cid => "Beverages", "Category");
        }
    }

    [TestClass]
    public class AuditPropertyResolverTests
    {
        [TestMethod]
        public void Able_To_Get_Resolver_From_Attribute()
        {
            Assert.IsInstanceOfType(AuditPropertyResolver.GetResolver<Product>(), typeof(ProductAuditResolver));
        }

        [TestMethod]
        public void Custom_AuditResolver_Can_Customize_Properties()
        {
            IAuditableContext context = GetContext();

            context.Expect(c => c.InsertAuditRecord(Arg<AuditedEntity>.Matches(e => e.ModifiedFields.Any(p => (string)p.NewValue == "Beverages"))));

            ContextAuditor auditor = new ContextAuditor(context);
            auditor.AuditPendingDataModifications();

            context.VerifyAllExpectations();
        }

        [TestMethod]
        public void Custom_AuditResolver_Can_Customize_Property_Name()
        {
            IAuditableContext context = GetContext();

            context.Expect(c => c.InsertAuditRecord(Arg<AuditedEntity>.Matches(e => e.ModifiedFields.Any(p => p.FieldName == "Category"))));

            ContextAuditor auditor = new ContextAuditor(context);
            auditor.AuditPendingDataModifications();

            context.VerifyAllExpectations();
        }

        private IAuditableContext GetContext()
        {
            IAuditableContext context = MockRepository.GenerateMock<FakeAuditableContext>();

            context.Audit<Product>(p => p.ID);
            return context;
        }
    }
}
