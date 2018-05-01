using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqSql.Expressions.Tests
{
    [TestClass]
    public class FieldExpressionTests
    {
        private ASourceExpression source = new TableExpression("Table", "Alias", new string[] { "FieldName" });
        private FieldExpression expression = null;

        [TestInitialize]
        public void TestInitialize()
        {
            expression = new FieldExpression(source, "FieldName", "FieldAlias");
        }

        [TestMethod]
        public void FieldExpression_Constructor_Exceptions()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FieldExpression(null, "FieldName", "FieldAlias"));
            Assert.ThrowsException<ArgumentException>(() => new FieldExpression(source, "", "FieldAlias"));
            Assert.ThrowsException<ArgumentException>(() => new FieldExpression(source, "FieldName", ""));
        }

        [TestMethod]
        public void FieldExpression_Properties()
        {
            Assert.AreEqual("FieldName", expression.Field);
            Assert.AreEqual("FieldAlias", expression.Alias);
            Assert.AreSame(source, expression.Source);
        }

        [TestMethod]
        public void FieldExpression_Accept()
        {
            // Setup test
            MockExpressionVisitor visitor = new MockExpressionVisitor();

            // Perform the test operation
            expression.Accept(visitor);

            // Check test result
            Assert.IsTrue(visitor.FieldVisited);
        }
    }
}
