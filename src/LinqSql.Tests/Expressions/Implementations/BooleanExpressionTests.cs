using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    [TestClass]
    public class BooleanExpressionTests
    {
        private readonly BooleanExpression expression = new BooleanExpression(true);

        [TestMethod]
        public void BooleanExpression_Properties()
        {
            Assert.AreEqual(ExpressionType.Extension, expression.NodeType);
            Assert.AreEqual(typeof(bool), expression.Type);
            Assert.IsTrue(expression.Value);
        }

        [TestMethod]
        public void BooleanExpression_Accept()
        {
            // Setup test
            MockExpressionVisitor visitor = new MockExpressionVisitor();

            // Perform the test operation
            visitor.Visit(expression);

            // Check test result
            Assert.IsTrue(visitor.BooleanVisited);
        }
    }
}
