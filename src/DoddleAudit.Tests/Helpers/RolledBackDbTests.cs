using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoddleAudit.Tests
{
    [TestClass]
    public class RolledBackDbTests
    {
        private TransactionScope _transScope;

        [TestInitialize]
        public void MyTestInitialize()
        {
            _transScope = new TransactionScope();
        }

        [TestCleanup]
        public void MyTestCleanup()
        {
            _transScope.Dispose();
        } 

    }
}