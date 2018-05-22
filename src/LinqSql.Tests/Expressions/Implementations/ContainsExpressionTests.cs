using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    [TestClass]
    public class ContainsExpressionTests
    {
        private readonly AExpression values = new TableExpression("Table", "Alias", new[] { "Field" });
        private readonly AExpression value = new LiteralExpression(42);
        private ContainsExpression expression = null;

        [TestInitialize]
        public void TestInitialize()
        {
            expression = new ContainsExpression(values, value);
        }

        [TestMethod]
        public void ContainsExpression_Constructor_Exceptions()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ContainsExpression(null, value));
            Assert.ThrowsException<ArgumentNullException>(() => new ContainsExpression(values, null));
        }

        [TestMethod]
        public void ContainsExpression_Properties()
        {
            Assert.AreEqual(ExpressionType.Extension, expression.NodeType);
            Assert.AreEqual(typeof(object), expression.Type);
            Assert.AreSame(values, expression.Values);
            Assert.AreSame(value, expression.Value);
        }

        [TestMethod]
        public void ContainsExpression_Accept()
        {
            // Setup test
            MockExpressionVisitor visitor = new MockExpressionVisitor();

            // Perform the test operation
            visitor.Visit(expression);

            // Check test result
            Assert.IsTrue(visitor.ContainsVisited);
        }
    }
}
