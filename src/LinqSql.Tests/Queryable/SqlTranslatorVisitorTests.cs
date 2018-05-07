using System.Data.Common;
using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    [TestClass]
    public class SqlTranslatorVisitorTests
    {
        private readonly DbConnection connection = ConnectionTestHelper.CreateConnection();
        private readonly SqlTranslatorVisitor visitor = new SqlTranslatorVisitor();

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void SqlTranslatorVisitor_Visit_NotSupportedException()
        {
            // Prepare the test data
            Expression<Action> expression = () => new Record[0]
                .AsQueryable()
                .Where(x => true);

            // Perform the test operation
            visitor.Visit(expression.Body);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void SqlTranslatorVisitor_VisitGeneric_NotSupportedException()
        {
            // Prepare the test data
            SqlQueryable queryable = new SqlQueryable(connection, "Table", new string[] { "Field" });
            Expression<Action> expression = () => queryable.Where(x => true);

            // Perform the test operation
            WhereExpression result = (WhereExpression)visitor.Visit(expression.Body);
        }
    }
}
