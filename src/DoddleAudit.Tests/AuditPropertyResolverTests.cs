//using System.Linq;
//using DoddleAudit.Tests.Fakes;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Rhino.Mocks;

using DoddleAudit.Tests.Fakes;

namespace DoddleAudit.Tests
{

    using DoddleAudit;


    public class ProductAuditResolver : EntityAuditConfiguration<Product>
    {
        public ProductAuditResolver()
        {
            //CustomizeProperty(p => p.CategoryID, cid => "Beverages", "Category");
        }
    }
}

//    [TestClass]
//    public class AuditPropertyResolverTests
//    {

//        [TestMethod]
//        public void Custom_AuditResolver_Can_Customize_Properties()
//        {
//            IAuditableContext context = GetContext();

//            context.Expect(c => c.SaveAuditedEntity(Arg<AuditedEntity>.Matches(e => e.ModifiedFields.Any(p => (string)p.NewValue == "Beverages"))));

//            ContextAuditor auditor = new ContextAuditor(context);
//            auditor.AuditAndSaveChanges();

//            context.VerifyAllExpectations();
//        }

//        [TestMethod]
//        public void Custom_AuditResolver_Can_Customize_Property_Name()
//        {
//            IAuditableContext context = GetContext();

//            context.Expect(c => c.SaveAuditedEntity(Arg<AuditedEntity>.Matches(e => e.ModifiedFields.Any(p => p.FieldName == "Category"))));

//            ContextAuditor auditor = new ContextAuditor(context);
//            auditor.AuditAndSaveChanges();

//            context.VerifyAllExpectations();
//        }

//        private IAuditableContext GetContext()
//        {
//            IAuditableContext context = MockRepository.GenerateMock<FakeAuditableContext>();

//            context.Audit<Product>(p => p.ID);
//            return context;
//        }
//    }
//}
