using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Sqlite.Tests
{
#if DEBUG
    [TestClass]
    public class SqliteExpressionVisitorTests
    {
        private readonly SqliteQueryVisitor visitor = new SqliteQueryVisitor();

        [TestMethod]
        public void SqliteExpressionVisitor_Visit_ArgumentNullExceptions()
        {
            Assert.ThrowsException<ArgumentNullException>(() => visitor.VisitBoolean(null));
        }

        [TestMethod]
        public void SqliteExpressionVisitor_VisitBoolean()
        {
            // Prepare the test data
            BooleanExpression a = new BooleanExpression(true);
            BooleanExpression b = new BooleanExpression(false);

            // Perform the test operation
            visitor.VisitBoolean(a);
            visitor.VisitBoolean(b);

            // Check the test result
            Assert.AreEqual("10", visitor.SqlState);
        }
    }
#endif
}
