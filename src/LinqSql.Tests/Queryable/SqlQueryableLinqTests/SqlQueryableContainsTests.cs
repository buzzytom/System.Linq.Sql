using System.Data.Common;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    using Sqlite;

    [TestClass]
    public class SqlQueryableContainsTests
    {
        private readonly DbConnection connection = ConnectionTestHelper.CreatePopulatedConnection();

        [TestMethod]
        public void SqlQueryable_Contains_IntArray()
        {
            // Prepare the test data
            int[] values = new[] { 1, 3 };
            string[] fields = new[] { "Id", "Name" };
            IQueryable<Record> queryable = new SqliteQueryable(connection, "Course", "Alias", fields);

            // Perfor the test operation
            Record[] records = queryable
                .Where(x => values.Contains((int)x["Alias"]["Id"]))
                .ToArray();

            // Check the test result
            Assert.AreEqual(values.Length, records.Length);
            foreach (Record record in records)
                Assert.IsTrue(values.Contains(Convert.ToInt32(record["Alias"]["Id"])));
        }

        [TestMethod]
        public void SqlQueryable_Contains_StringArray()
        {
            // Prepare the test data
            string[] values = new[] { "1", "3" };
            string[] fields = new[] { "Id", "Name" };
            IQueryable<Record> queryable = new SqliteQueryable(connection, "Course", "Alias", fields);

            // Perfor the test operation
            Record[] records = queryable
                .Where(x => values.Contains((string)x["Alias"]["Id"]))
                .ToArray();

            // Check the test result
            Assert.AreEqual(values.Length, records.Length);
            foreach (Record record in records)
                Assert.IsTrue(values.Contains(Convert.ToInt32(record["Alias"]["Id"]).ToString()));
        }

        [TestMethod]
        public void SqlQueryable_Contains_SubQuery()
        {
            // Prepare the test data
            IQueryable<Record> outer = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });
            IQueryable<Record> inner = new SqliteQueryable(connection, "CourseStudent", new[] { "Id", "CourseId" });

            // Perfor the test operation
            Record[] records = outer
                .Where(x => inner.Contains(y => y["CourseStudent"]["CourseId"], x["Course"]["Id"]))
                .ToArray();

            // Check the test result
            Assert.AreEqual(ConnectionTestHelper.CountCourses, records.Length);
        }

        [TestMethod]
        public void SqlQueryable_Contains_Chained()
        {
            // Prepare the test data
            IQueryable<Record> query = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });

            // Perfor the test operation
            bool result = query.Contains(x => x["Course"]["Id"], 1);

            // Check the test result
            Assert.IsTrue(result);
        }
    }
}
