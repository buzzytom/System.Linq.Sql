using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Expressions.Tests
{
    [TestClass]
    public class CompositeExpressionTests
    {
        private readonly APredicateExpression left = null;
        private readonly APredicateExpression right = null;
        private CompositeExpression expression = null;

        [TestInitialize]
        public void TestInitialize()
        {
            expression = new CompositeExpression(left, right, CompositeOperator.Or);
        }

        [TestMethod]
        public void CompositeExpression_Constructor_Exceptions()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new CompositeExpression(null, right));
            Assert.ThrowsException<ArgumentNullException>(() => new CompositeExpression(left, null));
        }

        [TestMethod]
        public void CompositeExpression_Properties()
        {
            Assert.AreEqual(ExpressionType.Extension, expression.NodeType);
            Assert.AreEqual(typeof(object), expression.Type);
            Assert.AreEqual(CompositeOperator.Or, expression.Operator);
            Assert.AreSame(left, expression.Left);
            Assert.AreSame(right, expression.Right);
        }

        [TestMethod]
        public void CompositeExpression_Accept()
        {
            // Setup test
            MockExpressionVisitor visitor = new MockExpressionVisitor();

            // Perform the test operation
            visitor.Visit(expression);

            // Check test result
            Assert.IsTrue(visitor.CompositeVisited);
        }
    }
}
