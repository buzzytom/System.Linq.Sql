using System.Data.Common;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqSql.Queryable.Tests
{
    [TestClass]
    public class SqlQueryableTests
    {
        private readonly DbConnection connection = ConnectionTestHelper.CreatePopulatedConnection();

        [TestMethod]
        public void SqlQueryable_Simple()
        {
            // Prepare test data
            string[] fields = new string[] { "Id", "Name" };
            IQueryable<Record> queryable = new SqlQueryable(connection, "Course", "Alias", fields);

            // Perform the test operation
            Record[] records = queryable.ToArray();

            // Check test result
            Assert.AreEqual(ConnectionTestHelper.CountCourses, records.Length);
        }
    }
}
