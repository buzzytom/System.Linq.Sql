using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    [TestClass]
    public class SqlQueryableProviderTests
    {
        private readonly DbConnection connection = ConnectionTestHelper.CreatePopulatedConnection();
        private SqlQueryableProvider provider = null;

        [TestInitialize]
        public void TestInitialize()
        {
            provider = new SqlQueryableProvider(connection);
        }

        [TestMethod]
        public void SqlQueryableProvider_CreateQuery()
        {
            // Prepare test data
            TableExpression expression = new TableExpression("Table", "Alias", new string[] { "Id" });

            // Perform test operation
            IQueryable<Record> query = provider.CreateQuery<Record>(expression);

            // Check the test result
            Assert.IsInstanceOfType(query, typeof(SqlQueryable));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void SqlQueryableProvider_CreateQuery_InvalidCastException()
        {
            // Prepare test data
            TableExpression expression = new TableExpression("Table", "Alias", new string[] { "Id" });

            // Perform test operation
            provider.CreateQuery<int>(expression);
        }

        [TestMethod]
        public void SqlQueryableProvider_Execute()
        {
            // Prepare test data
            TableExpression expression = new TableExpression("Student", "Alias", new string[] { "Id" });

            // Perform test operation
            Record[] records = provider
                .Execute<IEnumerable<Record>>(expression)
                .ToArray();

            // Check the test result
            Assert.AreEqual(ConnectionTestHelper.CountStudents, records.Length);
            foreach (Record record in records)
            {
                Assert.AreEqual(1, record.Count);

                RecordItem item = record["Alias"];
                Assert.AreEqual(1, item.Count);
                Assert.IsTrue(item.ContainsKey("Id"));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void SqlQueryableProvider_Execute_InvalidCastException()
        {
            // Prepare test data
            TableExpression expression = new TableExpression("Student", "Alias", new string[] { "Id" });

            // Perform test operation
            provider.Execute<int>(expression);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void SqlQueryableProvider_Execute_NotSupportedException()
        {
            // Prepare test data
            Expression expression = Expression.Constant(this);

            // Perform test operation
            provider.Execute(expression);
        }
    }
}
