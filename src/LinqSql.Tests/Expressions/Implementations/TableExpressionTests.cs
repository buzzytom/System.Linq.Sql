using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqSql.Expressions.Tests
{
    [TestClass]
    public class TableExpressionTests
    {
        private static readonly string[] fields = new string[] { "FieldA", "FieldB" };
        private TableExpression expression = new TableExpression("Table", "Alias", fields);

        [TestMethod]
        public void TableExpression_Constructor_Exceptions()
        {
            Assert.ThrowsException<ArgumentException>(() => new TableExpression("", "Alias", fields));
            Assert.ThrowsException<ArgumentException>(() => new TableExpression("Table", "", fields));
            Assert.ThrowsException<ArgumentNullException>(() => new TableExpression("Table", "Alias", null));
        }

        [TestMethod]
        public void TableExpression_Properties()
        {
            FieldExpression[] expressions = expression.Fields.ToArray();
            Assert.AreEqual("Table", expression.Table);
            Assert.AreEqual("Alias", expression.Alias);
            Assert.AreEqual(fields.Length, expressions.Length);
            foreach (FieldExpression field in expressions)
            {
                Assert.AreSame(expression, field.Source);
                Assert.IsTrue(fields.Contains(field.Field));
            }
        }

        [TestMethod]
        public void TableExpression_Accept()
        {
            // Setup test
            MockExpressionVisitor visitor = new MockExpressionVisitor();

            // Perform the test operation
            expression.Accept(visitor);

            // Check test result
            Assert.IsTrue(visitor.TableVisited);
        }
    }
}
