using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    [TestClass]
    public class JoinExpressionTests
    {
        private readonly ASourceExpression outer = new TableExpression("Outer", "OuterAlias", new string[] { "OuterField" });
        private readonly ASourceExpression inner = new TableExpression("Inner", "InnerAlias", new string[] { "InnerField" });
        private readonly APredicateExpression predicate = new BooleanExpression(true);
        private JoinExpression expression = null;

        [TestInitialize]
        public void TestInitialize()
        {
            expression = new JoinExpression(outer, inner, predicate, JoinType.Right);
        }

        [TestMethod]
        public void JoinExpression_Exceptions()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new JoinExpression(null, inner));
            Assert.ThrowsException<ArgumentNullException>(() => new JoinExpression(outer, null));
        }

        [TestMethod]
        public void JoinExpression_Properties()
        {
            Assert.AreEqual(ExpressionType.Extension, expression.NodeType);
            Assert.AreEqual(typeof(IQueryable<Record>), expression.Type);
            CollectionAssert.AreEquivalent(new[] { outer, inner }, expression.Expressions.ToArray());
            Assert.AreSame(outer, expression.Outer);
            Assert.AreSame(inner, expression.Inner);
            Assert.AreSame(predicate, expression.Predicate);
            Assert.AreEqual(JoinType.Right, expression.JoinType);
        }

        [TestMethod]
        public void JoinExpression_Accept()
        {
            // Setup test
            MockExpressionVisitor visitor = new MockExpressionVisitor();

            // Perform the test operation
            visitor.Visit(expression);

            // Check test result
            Assert.IsTrue(visitor.JoinVisited);
        }
    }
}
