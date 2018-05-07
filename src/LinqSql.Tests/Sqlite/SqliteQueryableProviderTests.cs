using System.Data.Common;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Sqlite.Tests
{
    [TestClass]
    public class SqliteQueryableProviderTests
    {
        private readonly DbConnection connection = ConnectionTestHelper.CreatePopulatedConnection();
        private SqlQueryableProvider provider = null;

        [TestInitialize]
        public void TestInitialize()
        {
            provider = new SqliteQueryableProvider(connection);
        }

        [TestMethod]
        public void SqliteQueryableProvider_CreateQuery()
        {
            // Prepare test data
            TableExpression expression = new TableExpression("Table", "Alias", new string[] { "Id" });

            // Perform test operation
            IQueryable<Record> query = provider.CreateQuery<Record>(expression);

            // Check the test result
            Assert.IsInstanceOfType(query, typeof(SqliteQueryable));
        }
    }
}
