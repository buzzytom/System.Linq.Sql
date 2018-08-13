using System.Data.Common;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    using Sqlite;

    [TestClass]
    public class SqlQueryableFirstOrDefaultTests
    {
        private readonly DbConnection connection = ConnectionTestHelper.CreatePopulatedConnection();

        [TestMethod]
        public void SqlQueryable_FirstOrDefault()
        {
            // Prepare the test data
            string[] fields = new[] { "Id", "Name" };
            IQueryable<Record> queryable = new SqliteQueryable(connection, "Course", "Alias", fields);

            // Perform the test operation
            Record record = queryable.FirstOrDefault();

            // Check the test result
            Assert.AreEqual(1L, record["Alias"]["Id"]);
        }

        [TestMethod]
        public void SqlQueryable_FirstOrDefault_Predicate()
        {
            // Prepare the test data
            string[] fields = new[] { "Id", "Name" };
            IQueryable<Record> queryable = new SqliteQueryable(connection, "Course", "Alias", fields);

            // Perform the test operation
            Record record = queryable.FirstOrDefault(x => (int)x["Alias"]["Id"] == 2);

            // Check the test result
            Assert.AreEqual(2L, record["Alias"]["Id"]);
        }
    }
}
