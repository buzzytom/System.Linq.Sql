using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data.Common;

namespace System.Linq.Sql.Tests
{
    using Sqlite;
    using System.Linq.Expressions;

    [TestClass]
    public class SqlQueryableUpdateTests
    {
        private readonly DbConnection connection = ConnectionTestHelper.CreatePopulatedConnection();

        [TestMethod]
        public void SqlQueryable_Update_ArgumentExceptions()
        {
            // Prepare the test data
            IQueryable<Record> queryable = new SqliteQueryable(connection, "Course", "Alias", new[] { "Id" });
            Expression<Func<Record, Dictionary<string, object>>> mapper = record => null;

            // Perform the test operations
            Assert.ThrowsException<ArgumentNullException>(() => SqlQueryableHelper.Update(null, "table", mapper));
            Assert.ThrowsException<ArgumentException>(() => SqlQueryableHelper.Update(queryable, "", mapper));
            Assert.ThrowsException<ArgumentNullException>(() => SqlQueryableHelper.Update(queryable, "table", null));
        }

        [TestMethod]
        public void SqlQueryable_Update()
        {
            // Prepare the test data
            string[] fields = new[] { "Id", "Name" };
            IQueryable<Record> queryable = new SqliteQueryable(connection, "Course", "Alias", fields)
                .Where(x => (int)x["Alias"]["Id"] == 1 || (int)x["Alias"]["Id"] == 2);

            // Preform the test operation
            int updated = queryable
                .Update("Alias", record => new Dictionary<string, object>
                {
                    { "Name", "Updated Name " + record["Alias"]["Id"] }
                })
                .GetScalar<int>();

            // Check the test result
            Assert.AreEqual(2, updated);
            Record[] items = queryable.ToArray();
            Assert.AreEqual(items.Length, updated);
            for (int i = 0; i < items.Length; i++)
                Assert.AreEqual($"Updated Name {i + 1}", items[i]["Alias"]["Name"]);
        }
    }
}
