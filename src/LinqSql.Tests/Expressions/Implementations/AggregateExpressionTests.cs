using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    [TestClass]
    public class AggregateExpressionTests
    {
        private readonly TableExpression source = new TableExpression("Table", "Alias", new[] { "Field" });
        private AggregateExpression expression = null;

        [TestInitialize]
        public void TestInitialize()
        {
            expression = new AggregateExpression(source, source.Fields.First(), AggregateFunction.Sum);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AggregateExpression_Constructor_Exceptions()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new AggregateExpression(null, source.Fields.First(), AggregateFunction.Top));
            Assert.ThrowsException<ArgumentNullException>(() => new AggregateExpression(source, null, AggregateFunction.Top));
        }

        [TestMethod]
        public void AggregateExpression_Properties()
        {
            Assert.AreEqual(ExpressionType.Extension, expression.NodeType);
            Assert.AreEqual(typeof(object), expression.Type);
            Assert.AreSame(source, expression.Source);
            Assert.AreSame(source.Fields.First(), expression.SourceField);
            Assert.AreEqual(AggregateFunction.Sum, expression.Function);
        }

        [TestMethod]
        public void AggregateExpression_Accept()
        {
            // Setup test
            MockExpressionVisitor visitor = new MockExpressionVisitor();

            // Perform the test operation
            visitor.Visit(expression);

            // Check test result
            Assert.IsTrue(visitor.AggregateVisited);
        }
    }
}
