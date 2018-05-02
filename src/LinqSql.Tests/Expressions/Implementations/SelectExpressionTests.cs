using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqSql.Expressions.Tests
{
    [TestClass]
    public class SelectExpressionTests
    {
        private static readonly string[] fields = new string[] { "FieldA", "FieldB" };
        private ASourceExpression source = new TableExpression("Table", "Alias", fields);
        private SelectExpression expression = null;

        [TestInitialize]
        public void TestInitialize()
        {
            expression = new SelectExpression(source, fields);
        }

        [TestMethod]
        public void SelectExpression_Constructor_Exceptions()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new SelectExpression(null, fields));
            Assert.ThrowsException<ArgumentNullException>(() => new SelectExpression(source, null));
            Assert.ThrowsException<ArgumentException>(() => new SelectExpression(source, new string[0]));
        }

        [TestMethod]
        public void SelectExpression_Properties()
        {
            FieldExpression[] expressions = expression.Fields.ToArray();
            Assert.AreSame(source, expression.Source);
            foreach (FieldExpression field in expressions)
            {
                Assert.AreSame(source, field.Source);
                Assert.IsTrue(fields.Contains(field.FieldName));
            }
        }

        [TestMethod]
        public void SelectExpression_Accept()
        {
            // Setup test
            MockExpressionVisitor visitor = new MockExpressionVisitor();

            // Perform the test operation
            visitor.Visit(expression);

            // Check test result
            Assert.IsTrue(visitor.SelectVisited);
        }
    }
}
