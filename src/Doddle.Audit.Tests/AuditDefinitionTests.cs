using Doddle.Audit.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Doddle.Audit.Tests
{
    [TestClass]
    public class AuditDefinitionTests
    {
        [TestMethod]
        public void Audit_Definition_Gets_Added()
        {
            IAuditableContext context = GetContext();
            context.Audit<Product>(p => p.ID);

            Assert.AreEqual(1, context.AuditDefinitions.Count);
        }

        [TestMethod]
        public void Audit_Definition_Can_Resolve_Primary_Key()
        {
            IAuditableContext context = MockRepository.GenerateMock<FakeAuditableContext>();

            context.Expect(c => c.GetEntityPrimaryKey<Product>()).Return(typeof(Product).GetProperty("ID"));
            
            context.Audit<Product>();
            
            context.VerifyAllExpectations();
        }

        [TestMethod]
        public void Association_Gets_Added_To_AuditDefinition()
        {
            IAuditableContext context = GetContext();
            context.Audit<Category>(c => c.ID).AuditAssociation<Product>(p => p.ID, p => p.CategoryID);

            Assert.AreEqual(1, context.AuditDefinitions[0].Relationships.Count);
        }


        private IAuditableContext GetContext()
        {
            return new FakeAuditableContext();
        }
    }
}
