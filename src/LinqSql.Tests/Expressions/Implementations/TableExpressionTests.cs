using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
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
            Assert.AreEqual(ExpressionType.Extension, expression.NodeType);
            Assert.AreEqual(typeof(IQueryable<Record>), expression.Type);
            CollectionAssert.AreEquivalent(new ASourceExpression[0], expression.Expressions.ToArray());
            Assert.AreEqual("Table", expression.Table);
            Assert.AreEqual("Alias", expression.Alias);
            Assert.AreEqual(fields.Length, expression.Fields.Count());
            foreach (FieldExpression field in expression.Fields)
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
