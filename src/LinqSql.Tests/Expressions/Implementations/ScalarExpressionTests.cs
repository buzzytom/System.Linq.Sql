using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    [TestClass]
    public class ScalarExpressionTests
    {
        private static readonly string[] fields = new string[] { "FieldA" };
        private TableExpression source = new TableExpression("Table", "Alias", fields);
        private ScalarExpression expression = null;

        [TestInitialize]
        public void TestInitialize()
        {
            expression = new ScalarExpression(source);
        }

        [TestMethod]
        public void ScalarExpression_Constructor_Exceptions()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ScalarExpression(source, null));
            Assert.ThrowsException<InvalidOperationException>(() => new ScalarExpression(new TableExpression("Table", "Alias", new[] { "Field1", "Field2" })));
        }

        [TestMethod]
        public void ScalarExpression_Properties()
        {
            Assert.AreEqual(ExpressionType.Extension, expression.NodeType);
            Assert.AreEqual(typeof(IQueryable<Record>), expression.Type);
            CollectionAssert.AreEquivalent(new[] { source }, expression.Expressions.ToArray());
            Assert.AreSame(source, expression.Source);
            foreach (FieldExpression field in expression.Fields)
            {
                Assert.AreEqual(source.Alias, field.TableName);
                Assert.IsTrue(fields.Contains(field.FieldName));
            }
        }

        [TestMethod]
        public void ScalarExpression_Accept()
        {
            // Setup test
            MockExpressionVisitor visitor = new MockExpressionVisitor();

            // Perform the test operation
            visitor.Visit(expression);

            // Check test result
            Assert.IsTrue(visitor.ScalarVisited);
        }
    }
}
