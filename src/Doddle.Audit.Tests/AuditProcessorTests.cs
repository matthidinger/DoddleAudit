using Doddle.Audit.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Doddle.Audit.Tests
{
    [TestClass]
    public class AuditProcessorTests
    {
        [TestMethod]
        public void AuditProcessor_Inserts_Audit_Records_In_AuditableContext()
        {
            IAuditableContext context = GetContext();

            ContextAuditor auditor = new ContextAuditor(context);
            auditor.AuditPendingDataModifications();

            context.AssertWasCalled(c => c.InsertAuditRecord(Arg<AuditedEntity>.Is.Anything));
        }

        private IAuditableContext GetContext()
        {
            IAuditableContext context = MockRepository.GenerateMock<FakeAuditableContext>();

            context.Audit<Product>(p => p.ID);
            return context;
        }
    }
}
