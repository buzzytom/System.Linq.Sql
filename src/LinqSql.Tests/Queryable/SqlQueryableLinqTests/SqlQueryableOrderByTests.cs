using System.Collections.Generic;
using System.Data.Common;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    using Sqlite;

    [TestClass]
    public class SqlQueryableOrderByTests
    {
        private readonly DbConnection connection = ConnectionTestHelper.CreatePopulatedConnection();

        [TestMethod]
        public void SqlQueryable_OrderBy()
        {
            // Prepare the test data
            IQueryable<Record> query = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });

            // Perform the test operation
            Dictionary<string, object>[] result = query
                .OrderBy(x => x["Course"]["Id"])
                .Flatten()
                .ToArray();

            // Check the test result
            Assert.AreEqual(ConnectionTestHelper.CountCourses, result.Length);
            long last = 0;
            foreach (Dictionary<string, object> record in result)
            {
                Assert.IsTrue((long)record["Id"] > last);
                last = (long)record["Id"];
            }
        }

        [TestMethod]
        public void SqlQueryable_OrderByDescending()
        {
            // Prepare the test data
            IQueryable<Record> query = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });

            // Perform the test operation
            Dictionary<string, object>[] result = query
                .OrderByDescending(x => x["Course"]["Id"])
                .Flatten()
                .ToArray();

            // Check the test result
            Assert.AreEqual(ConnectionTestHelper.CountCourses, result.Length);
            long last = ConnectionTestHelper.CountCourses + 1;
            foreach (Dictionary<string, object> record in result)
            {
                Assert.IsTrue((long)record["Id"] < last);
                last = (long)record["Id"];
            }
        }

        [TestMethod]
        public void SqlQueryable_ThenBy()
        {
            // Prepare the test data
            IQueryable<Record> query = new SqliteQueryable(connection, "SortTest", new[] { "Id", "Alpha", "Beta" });

            // Perform the test operation
            Dictionary<string, object>[] result = query
                .OrderBy(x => x["SortTest"]["Alpha"])
                .ThenBy(x => x["SortTest"]["Beta"])
                .Flatten()
                .ToArray();

            // Check the test result
            Assert.AreEqual(ConnectionTestHelper.CountSortTestAlphas * ConnectionTestHelper.CountSortTestBetas, result.Length);
            Dictionary<string, object> last = result[0];
            for (int i = 1; i < result.Length; i++)
            {
                Dictionary<string, object> record = result[i];
                Assert.IsTrue((long)record["Alpha"] > (long)last["Alpha"] || (long)record["Beta"] > (long)last["Beta"]);
                last = record;
            }
        }

        [TestMethod]
        public void SqlQueryable_ThenByDescending()
        {
            // Prepare the test data
            IQueryable<Record> query = new SqliteQueryable(connection, "SortTest", new[] { "Id", "Alpha", "Beta" });

            // Perform the test operation
            Dictionary<string, object>[] result = query
                .OrderBy(x => x["SortTest"]["Alpha"])
                .ThenByDescending(x => x["SortTest"]["Beta"])
                .Flatten()
                .ToArray();

            // Check the test result
            Assert.AreEqual(ConnectionTestHelper.CountSortTestAlphas * ConnectionTestHelper.CountSortTestBetas, result.Length);
            Dictionary<string, object> last = result[0];
            for (int i = 1; i < result.Length; i++)
            {
                Dictionary<string, object> record = result[i];
                Assert.IsTrue((long)record["Alpha"] > (long)last["Alpha"] || (long)record["Beta"] < (long)last["Beta"]);
                last = record;
            }
        }
    }
}
