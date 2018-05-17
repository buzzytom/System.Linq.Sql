using System.Data.Common;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    [TestClass]
    public class SqlQueryableHelperTests
    {
        private readonly DbConnection connection = ConnectionTestHelper.CreateConnection();
        private SqlQueryable outer = null;
        private SqlQueryable inner = null;

        [TestInitialize]
        public void TestInitialize()
        {
            outer = new SqlQueryable(connection, "OuterTable", new[] { "OuterField" });
            inner = new SqlQueryable(connection, "InnerTable", new[] { "InnerField" });
        }

        [TestMethod]
        public void SqlQueryableHelper_Join_ArgumentExceptions()
        {
            Assert.ThrowsException<ArgumentNullException>(() => SqlQueryableHelper.Join(null, inner, (o, i) => true, (o, i) => i, JoinType.Inner));
            Assert.ThrowsException<ArgumentNullException>(() => SqlQueryableHelper.Join(outer, null, (o, i) => true, (o, i) => i, JoinType.Inner));
            Assert.ThrowsException<ArgumentNullException>(() => SqlQueryableHelper.Join(outer, inner, null, (o, i) => i, JoinType.Inner));
            Assert.ThrowsException<ArgumentNullException>(() => SqlQueryableHelper.Join(outer, inner, (o, i) => true, null, JoinType.Inner));
        }

        [TestMethod]
        public void SqlQueryableHelper_Join()
        {
            // Perform the test operation
            IQueryable<Record> query = SqlQueryableHelper.Join(outer, inner, (o, i) => true, (o, i) => i);

            // Check the test result
            Assert.AreSame(outer.Provider, query.Provider);
        }
    }
}
