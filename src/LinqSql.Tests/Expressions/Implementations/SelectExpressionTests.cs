using System.Collections.Generic;
using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    [TestClass]
    public class SelectExpressionTests
    {
        private static readonly string[] fields = new string[] { "FieldA", "FieldB" };
        private TableExpression source = new TableExpression("Table", "Alias", fields);
        private Ordering[] orderings = null;
        private SelectExpression expression = null;

        [TestInitialize]
        public void TestInitialize()
        {
            orderings = new[] { new Ordering(source.Fields.First(), OrderType.Descending) };
            expression = new SelectExpression(source, source.Fields, 10, 20, orderings);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ordering_Constructor_ArgumentNullException()
        {
            new Ordering(null, OrderType.Ascending);
        }

        [TestMethod]
        public void Ordering_Properties()
        {
            // Prepare the test data.
            Ordering ordering = new Ordering(source.Fields.First(), OrderType.Descending);

            // Check the test result.
            Assert.AreEqual(source.Fields.First(), ordering.Field);
            Assert.AreEqual(OrderType.Descending, ordering.OrderType);
        }

        [TestMethod]
        public void SelectExpression_Constructor_Exceptions()
        {
            // Prepare the test data.
            FieldExpression field = new FieldExpression(new LiteralExpression(42), "Table", "Alias");

            // Perform the tests operations.
            Assert.ThrowsException<ArgumentNullException>(() => new SelectExpression(source, null));
            Assert.ThrowsException<ArgumentException>(() => new SelectExpression(source, new FieldExpression[0]));
            Assert.ThrowsException<KeyNotFoundException>(() => new SelectExpression(source, -1, 0, new[] { new Ordering(field, OrderType.Ascending) }));
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

            // Check the fields are correct
            Assert.AreEqual(fields.Length, expression.Fields.Count());
            foreach (FieldExpression field in expression.Fields)
            {
                Assert.AreEqual(source.Alias, field.TableName);
                Assert.IsTrue(fields.Contains(field.FieldName));
            }

            // Check the orderings are correct
            Assert.AreEqual(orderings.Length, expression.Orderings.Count());
            foreach (Ordering ordering in expression.Orderings)
            {
                Ordering expected = orderings.FirstOrDefault(x => x.Field == ordering.Field);
                Assert.IsNotNull(expected);
                Assert.AreEqual(expected.OrderType, ordering.OrderType);
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
