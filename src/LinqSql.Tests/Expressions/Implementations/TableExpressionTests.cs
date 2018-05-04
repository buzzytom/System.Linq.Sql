using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Expressions.Tests
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
                Assert.AreEqual(expression.Alias, field.TableName);
                Assert.IsTrue(fields.Contains(field.FieldName));
            }
        }

        [TestMethod]
        public void TableExpression_Accept()
        {
            // Setup test
            MockExpressionVisitor visitor = new MockExpressionVisitor();

            // Perform the test operation
            visitor.Visit(expression);

            // Check test result
            Assert.IsTrue(visitor.TableVisited);
        }
    }
}
