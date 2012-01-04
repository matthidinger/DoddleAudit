using DoddleAudit.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DoddleAudit.Tests
{
    public class BasicAuditingTests
    {
        [TestClass]
        public class AuditPendingDataModificationsTests
        {
            [TestMethod]
            public void SavesInsertsUpdatesAndDeletes()
            {
                var context = GetContext();
                context.Object.AddInsert(new Product());
                context.Object.AddUpdate(new Product());
                context.Object.AddDelete(new Product());

                var auditor = new ContextAuditor(context.Object);
                auditor.AuditAndSaveChanges();

                context.Verify(c => c.SaveAuditedEntity(It.Is<AuditedEntity>(entity => entity.Action == AuditAction.Insert)));
                context.Verify(c => c.SaveAuditedEntity(It.Is<AuditedEntity>(entity => entity.Action == AuditAction.Update)));
                context.Verify(c => c.SaveAuditedEntity(It.Is<AuditedEntity>(entity => entity.Action == AuditAction.Delete)));
            }

            private static Mock<FakeAuditableContext> GetContext()
            {
                var context = new Mock<FakeAuditableContext>();
                context.Object.Audit<Product>(p => p.ID);
                return context;
            }
        }
    }
}
