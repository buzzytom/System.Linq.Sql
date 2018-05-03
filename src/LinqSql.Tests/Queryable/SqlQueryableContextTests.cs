using System;
using System.Data.Common;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqSql.Queryable.Tests
{
    using Expressions;

    [TestClass]
    public class SqlQueryableContextTests
    {

        [TestMethod]
        public void SqlQueryableContext_Simple()
        {
            // Prepare test data
            DbConnection connection = ConnectionTestHelper.CreatePopulatedConnection();
            TableExpression expression = new TableExpression("Student", "Alias", new string[] { "Id", "FirstName", "LastName" });
            SqlExpressionVisitor visitor = new SqlExpressionVisitor();

            // Perform test operation
            Record[] records = SqlQueryableContext
                .ExecuteQuery(connection, expression, visitor)
                .ToArray();

            // Check the test result
            Assert.AreEqual(ConnectionTestHelper.CountStudents, records.Length);
            foreach (Record record in records)
            {
                Assert.AreEqual(1, record.Count);

                RecordItem item = record["Alias"];
                Assert.AreEqual(3, item.Count);
                Assert.IsTrue(item.ContainsKey("Id"));
                Assert.IsTrue(item.ContainsKey("FirstName"));
                Assert.IsTrue(item.ContainsKey("LastName"));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SqlQueryableContext_InvalidOperationException()
        {
            // Prepare test data
            DbConnection connection = ConnectionTestHelper.CreatePopulatedConnection();
            TableExpression expression = new TableExpression("Student", "Alias", new string[] { "Id", "FirstName", "LastName" });
            SqlExpressionVisitor visitor = new SqlExpressionVisitor();

            // Perform the test operation
            connection.Close();
            SqlQueryableContext.ExecuteQuery(connection, expression, visitor);
        }
    }
}
