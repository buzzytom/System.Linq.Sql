﻿using System.Data.Common;
using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    [TestClass]
    public class SqlQueryableHelperTests
    {
        private readonly DbConnection connection = ConnectionTestHelper.CreateConnection();

        [TestMethod]
        public void SqlQueryableHelper_Join_ArgumentExceptions()
        {
            // Prepare the test data
            SqlQueryable outer = new SqlQueryable(connection, "OuterTable", new[] { "OuterField" });
            SqlQueryable inner = new SqlQueryable(connection, "InnerTable", new[] { "InnerField" });

            // Perform the test operations
            Assert.ThrowsException<ArgumentNullException>(() => SqlQueryableHelper.Join(null, inner, (o, i) => true, (o, i) => i, JoinType.Inner));
            Assert.ThrowsException<ArgumentNullException>(() => SqlQueryableHelper.Join(outer, null, (o, i) => true, (o, i) => i, JoinType.Inner));
            Assert.ThrowsException<ArgumentNullException>(() => SqlQueryableHelper.Join(outer, inner, null, (o, i) => i, JoinType.Inner));
            Assert.ThrowsException<ArgumentNullException>(() => SqlQueryableHelper.Join(outer, inner, (o, i) => true, null, JoinType.Inner));
        }

        [TestMethod]
        public void SqlQueryableHelper_Join()
        {
            // Prepare the test data
            SqlQueryable outer = new SqlQueryable(connection, "OuterTable", new[] { "OuterField" });
            SqlQueryable inner = new SqlQueryable(connection, "InnerTable", new[] { "InnerField" });

            // Perform the test operations
            IQueryable<Record> query = SqlQueryableHelper.Join(outer, inner, (o, i) => true, (o, i) => i);

            // Check the test result
            Assert.AreSame(outer.Provider, query.Provider);
        }

        [TestMethod]
        public void SqlQueryableHelper_Contains_ArgumentExceptions()
        {
            // Prepare the test data
            SqlQueryable source = new SqlQueryable(connection, "Table", new[] { "Field" });
            Expression<Func<Record, object>> selector = record => record["key"];

            // Perform the test operations
            Assert.ThrowsException<ArgumentNullException>(() => SqlQueryableHelper.Contains(null, selector, 42));
            Assert.ThrowsException<ArgumentNullException>(() => SqlQueryableHelper.Contains(source, null, 42));
        }

        [TestMethod]
        public void SqlQueryableHelper_Count_ArgumentExceptions()
        {
            // Prepare the test data
            SqlQueryable source = new SqlQueryable(connection, "Table", new[] { "Field" });

            // Perform the test operation
            Assert.ThrowsException<ArgumentNullException>(() => SqlQueryableHelper.Count(null, x => true));
        }
    }
}
