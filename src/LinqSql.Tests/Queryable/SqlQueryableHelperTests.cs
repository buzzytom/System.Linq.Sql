using System.Data.Common;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    [TestClass]
    public class SqlQueryableHelperTests
    {
        private readonly DbConnection connection = ConnectionTestHelper.CreateConnection();
        private APredicateExpression predicate = new BooleanExpression(true);
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
            Assert.ThrowsException<ArgumentNullException>(() => SqlQueryableHelper.Join(null, inner, (i, o) => true, (i, o) => i, JoinType.Inner));
            Assert.ThrowsException<ArgumentNullException>(() => SqlQueryableHelper.Join(outer, null, (i, o) => true, (i, o) => i, JoinType.Inner));
            Assert.ThrowsException<ArgumentNullException>(() => SqlQueryableHelper.Join(outer, inner, null, (i, o) => i, JoinType.Inner));
            Assert.ThrowsException<ArgumentNullException>(() => SqlQueryableHelper.Join(outer, inner, (i, o) => true, null, JoinType.Inner));
        }
    }
}
