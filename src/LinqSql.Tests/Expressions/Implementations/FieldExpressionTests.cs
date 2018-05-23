using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    [TestClass]
    public class FieldExpressionTests
    {
        private readonly TableExpression source = new TableExpression("Table", "Alias", new string[] { "Field" });
        private FieldExpression field = null;
        private FieldExpression expression = null;

        [TestInitialize]
        public void TestInitialize()
        {
            field = new FieldExpression(source, "TableNameB", "FieldNameB");
            expression = new FieldExpression(source, "TableName", "FieldName", field);
        }

        [TestMethod]
        public void FieldExpression_Constructor_Exceptions()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FieldExpression(null, "TableName", ""));
            Assert.ThrowsException<ArgumentException>(() => new FieldExpression(source, "", "FieldName"));
            Assert.ThrowsException<ArgumentException>(() => new FieldExpression(source, "TableName", ""));
        }

        [TestMethod]
        public void FieldExpression_Properties()
        {
            Assert.AreEqual(ExpressionType.Extension, expression.NodeType);
            Assert.AreEqual(typeof(object), expression.Type);
            Assert.AreSame(source, expression.ValueExpression);
            Assert.AreSame(field, expression.SourceExpression);
            Assert.AreEqual("TableName", expression.TableName);
            Assert.AreEqual("FieldName", expression.FieldName);
        }

        [TestMethod]
        public void FieldExpression_Accept()
        {
            // Setup test
            MockExpressionVisitor visitor = new MockExpressionVisitor();

            // Perform the test operation
            visitor.Visit(expression);

            // Check test result
            Assert.IsTrue(visitor.FieldVisited);
        }

        [TestMethod]
        public void FieldExpression_AcceptDeclarationSql()
        {
            // Setup test
            MockExpressionVisitor visitor = new MockExpressionVisitor();

            // Perform the test operation
            expression.AcceptDeclarationSql(visitor);

            // Check test result
            Assert.IsTrue(visitor.FieldDeclarationVisited);
        }
    }
}
