using System.Data.Common;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    using Sqlite;

    [TestClass]
    public class SqlQueryableLimitTests
    {
        private readonly DbConnection connection = ConnectionTestHelper.CreatePopulatedConnection();

        [TestMethod]
        public void SqlQueryable_Skip()
        {
            // Prepare the test data
            IQueryable<Record> query = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });

            // Perform the test operation
            Record[] records = query
                .Skip(1)
                .ToArray();

            // Check the test result
            Assert.AreEqual(ConnectionTestHelper.CountCourses - 1, records.Length);
            foreach (Record record in records)
            {
                Assert.AreEqual(1, record.Count);
                Assert.IsTrue(record["Course"].ContainsKey("Id"));
                Assert.IsTrue(record["Course"].ContainsKey("Name"));
            }
        }

        [TestMethod]
        public void SqlQueryable_Take()
        {
            // Prepare the test data
            IQueryable<Record> query = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });

            // Perform the test operation
            Record[] records = query
                .Take(2)
                .ToArray();

            // Check the test result
            Assert.AreEqual(2, records.Length);
            foreach (Record record in records)
            {
                Assert.AreEqual(1, record.Count);
                Assert.IsTrue(record["Course"].ContainsKey("Id"));
                Assert.IsTrue(record["Course"].ContainsKey("Name"));
            }
        }

        [TestMethod]
        public void SqlQueryable_TakeSkip()
        {
            // Prepare the test data
            IQueryable<Record> query = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });

            // Perform the test operation
            Record[] records = query
                .Take(4)
                .Skip(1)
                .ToArray();

            // Check the test result
            Assert.AreEqual(3, records.Length);
            foreach (Record record in records)
            {
                Assert.AreEqual(1, record.Count);
                Assert.IsTrue(record["Course"].ContainsKey("Id"));
                Assert.IsTrue(record["Course"].ContainsKey("Name"));
            }
        }

        [TestMethod]
        public void SqlQueryable_SkipTake()
        {
            // Prepare the test data
            IQueryable<Record> source = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });

            // Perform the test operation
            Record[] records = source
                .Skip(1)
                .Take(2)
                .ToArray();

            // Check the test result
            Assert.AreEqual(2, records.Length);
            foreach (Record record in records)
            {
                Assert.AreEqual(1, record.Count);
                Assert.IsTrue(record["Course"].ContainsKey("Id"));
                Assert.IsTrue(record["Course"].ContainsKey("Name"));
            }
        }
    }
}
