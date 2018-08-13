﻿using System.Collections.Generic;
using System.Data.Common;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    using Sqlite;

    [TestClass]
    public class SqlQueryableLastOrDefaultTests
    {
        private readonly DbConnection connection = ConnectionTestHelper.CreatePopulatedConnection();

        [TestMethod]
        public void SqlQueryable_LastOrDefault()
        {
            // Prepare the test data
            string[] fields = new[] { "Id", "Name" };
            IQueryable<Record> queryable = new SqliteQueryable(connection, "Course", "Alias", fields);

            // Perform the test operation
            Record record = queryable.LastOrDefault(x => x["Alias"]["Id"]);

            // Check the test result
            Assert.AreEqual((long)ConnectionTestHelper.CountCourses, record["Alias"]["Id"]);
        }

        [TestMethod]
        public void SqlQueryable_LastOrDefault_Predicate()
        {
            // Prepare the test data
            string[] fields = new[] { "Id", "Name" };
            IQueryable<Record> queryable = new SqliteQueryable(connection, "Course", "Alias", fields);

            // Perform the test operation
            Record record = queryable.LastOrDefault(x => x["Alias"]["Id"], x => (int)x["Alias"]["Id"] == 2);

            // Check the test result
            Assert.AreEqual(2L, record["Alias"]["Id"]);
        }
    }
}