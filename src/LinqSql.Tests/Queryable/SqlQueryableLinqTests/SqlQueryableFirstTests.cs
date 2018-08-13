using System.Data.Common;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    using Sqlite;

    [TestClass]
    public class SqlQueryableFirstTests
    {
        private readonly DbConnection connection = ConnectionTestHelper.CreatePopulatedConnection();

        [TestMethod]
        public void SqlQueryable_First()
        {
            // Prepare the test data
            string[] fields = new[] { "Id", "Name" };
            IQueryable<Record> queryable = new SqliteQueryable(connection, "Course", "Alias", fields);

            // Perform the test operation
            Record record = queryable.First();

            // Check the test result
            Assert.AreEqual(1L, record["Alias"]["Id"]);
        }

        [TestMethod]
        public void SqlQueryable_First_Predicate()
        {
            // Prepare the test data
            string[] fields = new[] { "Id", "Name" };
            IQueryable<Record> queryable = new SqliteQueryable(connection, "Course", "Alias", fields);

            // Perform the test operation
            Record record = queryable.First(x => (int)x["Alias"]["Id"] == 2);

            // Check the test result
            Assert.AreEqual(2L, record["Alias"]["Id"]);
        }
    }
}
