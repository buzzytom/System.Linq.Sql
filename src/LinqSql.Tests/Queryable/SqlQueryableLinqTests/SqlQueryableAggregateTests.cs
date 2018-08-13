using System.Collections.Generic;
using System.Data.Common;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    using Sqlite;

    [TestClass]
    public class SqlQueryableAggregateTests
    {
        private readonly DbConnection connection = ConnectionTestHelper.CreatePopulatedConnection();

        [TestMethod]
        public void SqlQueryable_Average()
        {
            // Prepare the test data
            IQueryable<Record> query = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });

            // Perform the test operations
            int averageInt = query.Average(x => (int)x["Course"]["Id"]);
            long averageLong = query.Average(x => (long)x["Course"]["Id"]);
            decimal averageDecimal = query.Average(x => (decimal)x["Course"]["Id"]);
            float averageFloat = query.Average(x => (float)x["Course"]["Id"]);
            double averageDouble = query.Average(x => (double)x["Course"]["Id"]);

            // Check the test results
            Assert.AreEqual(2, averageInt);
            Assert.AreEqual(2L, averageLong);
            Assert.AreEqual(2.5m, averageDecimal);
            Assert.AreEqual(2.5f, averageFloat);
            Assert.AreEqual(2.5, averageDouble);
        }

        [TestMethod]
        public void SqlQueryable_Count()
        {
            // Prepare the test data
            IQueryable<Record> query = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });

            // Perform the test operation
            int count = query.Count();

            // Check the test result
            Assert.AreEqual(ConnectionTestHelper.CountCourses, count);
        }

        [TestMethod]
        public void SqlQueryable_Count_Predicate()
        {
            // Prepare the test data
            IQueryable<Record> query = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });

            // Perform the test operation
            int count = query.Count(x => (int)x["Course"]["Id"] == 2 || (int)x["Course"]["Id"] == 3);

            // Check the test result
            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public void SqlQueryable_Max()
        {
            // Prepare the test data
            IQueryable<Record> query = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });

            // Perform the test operation
            int count = query.Max(x => (int)x["Course"]["Id"]);

            // Check the test result
            Assert.AreEqual(4, count);
        }

        [TestMethod]
        public void SqlQueryable_Min()
        {
            // Prepare the test data
            IQueryable<Record> query = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });

            // Perform the test operation
            int count = query.Min(x => (int)x["Course"]["Id"]);

            // Check the test result
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void SqlQueryable_Sum()
        {
            // Prepare the test data
            IQueryable<Record> query = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });

            // Perform the test operations
            int sumInt = query.Sum(x => (int)x["Course"]["Id"]);
            long sumLong = query.Sum(x => (long)x["Course"]["Id"]);
            decimal sumDecimal = query.Sum(x => (decimal)x["Course"]["Id"]);
            float sumFloat = query.Sum(x => (float)x["Course"]["Id"]);
            double sumDouble = query.Sum(x => (double)x["Course"]["Id"]);

            // Check the test results
            Assert.AreEqual(10, sumInt);
            Assert.AreEqual(10L, sumLong);
            Assert.AreEqual(10m, sumDecimal);
            Assert.AreEqual(10f, sumFloat);
            Assert.AreEqual(10, sumDouble);
        }
    }
}
