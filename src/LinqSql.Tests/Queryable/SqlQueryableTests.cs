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

        [TestMethod]
        public void SqlQueryable_Join()
        {
            // Prepare the test data
            IQueryable<Record> outer = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });
            IQueryable<Record> inner = new SqliteQueryable(connection, "CourseStudent", new[] { "Id", "CourseId", "StudentId" });

            // Perform the test operation
            Record[] records = outer
                .Join(inner, x => x["Course"]["Id"], x => x["CourseStudent"]["CourseId"], (o, i) => o | i)
                .ToArray();

            // Check the test result
            Assert.AreEqual(8, records.Length);
            foreach (Record record in records)
            {
                Assert.AreEqual(2, record.Count);
                Assert.IsTrue(record["Course"].ContainsKey("Id"));
                Assert.IsTrue(record["Course"].ContainsKey("Name"));
                Assert.IsTrue(record["CourseStudent"].ContainsKey("Id"));
                Assert.IsTrue(record["CourseStudent"].ContainsKey("CourseId"));
                Assert.IsTrue(record["CourseStudent"].ContainsKey("StudentId"));
                Assert.AreEqual(record["Course"]["Id"], record["CourseStudent"]["CourseId"]);
            }
        }

        [TestMethod]
        public void SqlQueryable_Join_Predicate()
        {
            // Prepare the test data
            IQueryable<Record> outer = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });
            IQueryable<Record> inner = new SqliteQueryable(connection, "CourseStudent", new[] { "Id", "CourseId", "StudentId" });

            // Perform the test operation
            Record[] records = outer
                .Join(inner, (o, i) => o["Course"]["Id"] == i["CourseStudent"]["CourseId"], (o, i) => o | i)
                .ToArray();

            // Check the test result
            Assert.AreEqual(8, records.Length);
            foreach (Record record in records)
            {
                Assert.AreEqual(2, record.Count);
                Assert.IsTrue(record["Course"].ContainsKey("Id"));
                Assert.IsTrue(record["Course"].ContainsKey("Name"));
                Assert.IsTrue(record["CourseStudent"].ContainsKey("Id"));
                Assert.IsTrue(record["CourseStudent"].ContainsKey("CourseId"));
                Assert.IsTrue(record["CourseStudent"].ContainsKey("StudentId"));
                Assert.AreEqual(record["Course"]["Id"], record["CourseStudent"]["CourseId"]);
            }
        }
    }
}
