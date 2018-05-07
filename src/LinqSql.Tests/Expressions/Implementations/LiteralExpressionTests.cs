using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Expressions.Tests
{
    [TestClass]
    public class LiteralExpressionTests
    {
        private static readonly string value = "Hello World!";
        private LiteralExpression expression = new LiteralExpression(value);
        
        [TestMethod]
        public void LiteralExpression_Constructor_Exceptions()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new LiteralExpression(null));
        }

        [TestMethod]
        public void LiteralExpression_Properties()
        {
            Assert.AreEqual(ExpressionType.Extension, expression.NodeType);
            Assert.AreEqual(value.GetType(), expression.Type);
            Assert.AreSame(value, expression.Value);
        }

        [TestMethod]
        public void LiteralExpression_Accept()
        {
            // Setup test
            MockExpressionVisitor visitor = new MockExpressionVisitor();

            // Perform the test operation
            visitor.Visit(expression);

            // Check test result
            Assert.IsTrue(visitor.LiteralVisited);
        }
    }
}
