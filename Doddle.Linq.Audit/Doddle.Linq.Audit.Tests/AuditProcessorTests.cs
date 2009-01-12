using System;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Doddle.Linq.Audit.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Doddle.Linq.Audit.Tests
{
    [TestClass]
    public class AuditProcessorTests
    {
        [TestMethod]
        public void AuditProcessor_Inserts_Audit_Records_In_AuditableContext()
        {
            IAuditableContext context = GetContext();

            AuditProcessor processor = new AuditProcessor(context);
            processor.Process();

            context.AssertWasCalled(c => c.InsertAuditRecord(Arg<EntityAuditRecord>.Is.Anything));
        }

        private IAuditableContext GetContext()
        {
            IAuditableContext context = MockRepository.GenerateMock<FakeAuditableContext>();

            context.Audit<Product>(p => p.ID);
            return context;
        }
    }
}
