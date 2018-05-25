using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    [TestClass]
    public class SelectExpressionTests
    {
        private static readonly string[] fields = new string[] { "FieldA", "FieldB" };
        private TableExpression source = new TableExpression("Table", "Alias", fields);
        private SelectExpression expression = null;

        [TestInitialize]
        public void TestInitialize()
        {
            expression = new SelectExpression(source, source.Fields, 10, 20);
        }

        [TestMethod]
        public void SelectExpression_Constructor_Exceptions()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new SelectExpression(source, null));
            Assert.ThrowsException<ArgumentException>(() => new SelectExpression(source, new FieldExpression[0]));
        }

        [TestMethod]
        public void SelectExpression_Properties()
        {
            Assert.AreEqual(ExpressionType.Extension, expression.NodeType);
            Assert.AreEqual(typeof(IQueryable<Record>), expression.Type);
            Assert.AreEqual(10, expression.Take);
            Assert.AreEqual(20, expression.Skip);
            CollectionAssert.AreEquivalent(new[] { source }, expression.Expressions.ToArray());
            Assert.AreSame(source, expression.Source);
            foreach (FieldExpression field in expression.Fields)
            {
                Assert.AreEqual(source.Alias, field.TableName);
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
