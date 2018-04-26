using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqSql.Expressions.Tests
{
    [TestClass]
    public class SqlStandardExpressionVisitorTests
    {
        private SqlStandardExpressionVisitor visitor = new SqlStandardExpressionVisitor();

        [TestMethod]
        public void VisitTable()
        {
            // Prepare the test data
            string[] fields = new string[] { "FieldA", "FieldB" };
            TableExpression expression = new TableExpression(fields, "Table", "Alias");

            // Perform the test operation
            visitor.VisitTable(expression);

            // Check the result
            Assert.AreEqual("Table as [Alias]", visitor.SqlState);
        }
    }
}
