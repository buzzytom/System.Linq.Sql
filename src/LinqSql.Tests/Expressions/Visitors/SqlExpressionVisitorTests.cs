using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqSql.Expressions.Tests
{
    [TestClass]
    public class SqlExpressionVisitorTests
    {
        private SqlExpressionVisitor visitor = new SqlExpressionVisitor();

        [TestMethod]
        public void SqlExpressionVisitor_GenerateSql()
        {
            // Prepare the test data
            string[] fields = new string[] { "FieldA", "FieldB" };
            TableExpression table = new TableExpression("Table", "Alias", fields);
            SelectExpression expression = new SelectExpression(table);

            // Performs the test operation
            Query query = visitor.GenerateQuery(expression);

            // Check the result
            Assert.AreEqual("select * from (select [Alias].[FieldA]as[f0],[Alias].[FieldB]as[f1] from [Table] as [Alias])as[t0]", query.Sql);
        }

        [TestMethod]
        public void SqlExpressionVisitor_VisitTable()
        {
            // Prepare the test data
            string[] fields = new string[] { "FieldA", "FieldB" };
            TableExpression expression = new TableExpression("Table", "Alias", fields);

            // Perform the test operation
            visitor.VisitTable(expression);

            // Check the result
            Assert.AreEqual("[Table] as [Alias]", visitor.SqlState);
        }

        [TestMethod]
        public void SqlExpressionVisitor_VisitSelect()
        {
            // Prepare the test data
            string[] fields = new string[] { "FieldA", "FieldB" };
            TableExpression table = new TableExpression("Table", "Alias", fields);
            SelectExpression expression = new SelectExpression(table);

            // Performs the test operation
            visitor.VisitSelect(expression);

            // Check the result
            Assert.AreEqual("(select [Alias].[FieldA]as[f0],[Alias].[FieldB]as[f1] from [Table] as [Alias])as[t0]", visitor.SqlState);
        }
    }
}
