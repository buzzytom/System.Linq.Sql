using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Expressions.Tests
{
    using Queryable;

    [TestClass]
    public class WhereExpressionTests
    {
        private APredicateExpression predicate = null;
        private readonly TableExpression table = new TableExpression("Table", "Alias", new string[] { "FieldA", "FieldB" });
        private WhereExpression expression = null;

        [TestInitialize]
        public void TestInitialize()
        {
            expression = new WhereExpression(table, predicate);
        }

        [TestMethod]
        public void WhereExpression_Constructor_Exceptions()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new WhereExpression(null, predicate));
            Assert.ThrowsException<ArgumentNullException>(() => new WhereExpression(table, null));
        }

        [TestMethod]
        public void WhereExpression_Properties()
        {
            Assert.AreEqual(ExpressionType.Extension, expression.NodeType);
            Assert.AreEqual(typeof(IQueryable<Record>), expression.Type);
            Assert.AreSame(table, expression.Source);
            Assert.AreSame(predicate, expression.Predicate);
            Assert.AreSame(table.Fields, expression.Fields);
        }

        [TestMethod]
        public void WhereExpression_Accept()
        {
            // Setup test
            MockExpressionVisitor visitor = new MockExpressionVisitor();

            // Perform the test operation
            visitor.Visit(expression);

            // Check test result
            Assert.IsTrue(visitor.WhereVisited);
        }
    }
}
