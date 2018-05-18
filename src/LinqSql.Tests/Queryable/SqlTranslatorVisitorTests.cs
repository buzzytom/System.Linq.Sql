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
            // The test has to use something that the visitor won't translate within something it will.
            // This will visit the binary expression to make a composite expression, then fail to create an AExpression from BlockExpression.
            BinaryExpression expression = Expression.And(Expression.Block(Expression.Constant(42)), Expression.Block(Expression.Constant(42)));

            // Perform the test operation
            visitor.Visit(expression);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void SqlTranslatorVisitor_DecodeJoinSelector_NotSupported()
        {
            // Prepare the test data
            SqlQueryable outer = new SqlQueryable(connection, "Table", new string[] { "Field" });
            SqlQueryable inner = new SqlQueryable(connection, "Table", new string[] { "Field" });
            Expression<Action> expression = () => outer
                .Join(inner, (o, i) => true, (o, i) => null, JoinType.Inner);

            // Perform the test operation
            visitor.Visit(expression.Body);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SqlTranslatorVisitor_DecodeJoinSelector_DuplicatedSelection()
        {
            // Prepare the test data
            SqlQueryable outer = new SqlQueryable(connection, "Table", new string[] { "Field" });
            SqlQueryable inner = new SqlQueryable(connection, "Table", new string[] { "Field" });
            Expression<Action> expression = () => outer
                .Join(inner, (o, i) => true, (o, i) => i | i, JoinType.Inner);

            // Perform the test operation
            visitor.Visit(expression.Body);
        }
    }
}
