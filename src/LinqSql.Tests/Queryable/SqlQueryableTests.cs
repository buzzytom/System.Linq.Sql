using System.Data.Common;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqSql.Queryable.Tests
{
    [TestClass]
    public class SqlQueryableTests
    {
        private static readonly string[] fields = new string[] { "FieldA", "FieldB" };
        private readonly DbConnection connection = null;

        [TestInitialize]
        public void TestInitialize()
        {
            // TODO - Setup connection
        }

        [TestMethod]
        public void SqlQueryable_Simple()
        {
            // Prepare test data
            IQueryable<Record> queryable = new SqlQueryable(connection, "Table", "Alias", fields);

            // Perform the test operation
            Record[] records = queryable.ToArray();

            // Check test result
            Assert.Fail();
        }
    }
}
