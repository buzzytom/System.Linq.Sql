using System.Data.Common;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    using Sqlite;

    [TestClass]
    public class SqlQueryableTests
    {
        private readonly DbConnection connection = ConnectionTestHelper.CreatePopulatedConnection();

        [TestMethod]
        public void SqlQueryable_Properties()
        {
            // Prepare the test data
            SqlQueryable item = new SqliteQueryable(connection, "Table", "Alias", new string[] { "FieldA" });

            // Check the test result
            Assert.AreEqual(typeof(Record), item.ElementType);
            Assert.IsInstanceOfType(item.Provider, typeof(SqlQueryableProvider));
            Assert.IsInstanceOfType(item.Expression, typeof(TableExpression));
        }

        [TestMethod]
        public void SqlQueryable_Simple()
        {
            // Prepare the test data
            string[] fields = new string[] { "Id", "Name" };
            IQueryable<Record> queryable = new SqliteQueryable(connection, "Course", "Alias", fields);

            // Perform the test operation
            Record[] records = queryable.ToArray();

            // Check the test result
            Assert.AreEqual(ConnectionTestHelper.CountCourses, records.Length);
        }

        [TestMethod]
        public void SqlQueryable_WhereBoolean()
        {
            // Prepare the test data
            string[] fields = new string[] { "Id", "Name" };
            IQueryable<Record> queryable = new SqliteQueryable(connection, "Course", "Alias", fields);

            // Perform the test operation
            Record[] records = queryable
                .Where(x => true)
                .ToArray();

            // Check the test result
            Assert.AreEqual(4, records.Length);
        }

        [TestMethod]
        public void SqlQueryable_WhereComparison()
        {
            // Prepare the test data
            string[] fields = new string[] { "Id", "Name" };
            IQueryable<Record> queryable = new SqliteQueryable(connection, "Course", "Alias", fields);

            // Perform the test operation
            Record[] records = queryable
                .Where(x => (int)x["Alias"]["Id"] == 1)
                .ToArray();

            // Check the test result
            Assert.AreEqual(1, records.Length);
        }
    }
}
