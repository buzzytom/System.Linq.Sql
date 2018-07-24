using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    [TestClass]
    public class FieldExpressionsTests
    {
        private readonly TableExpression source = new TableExpression("Table", "Alias", new string[] { "Field" });

        [TestMethod]
        public void FieldExpressions_Constructor_Exceptions()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FieldExpressions(null));
            Assert.ThrowsException<ArgumentNullException>(() => new FieldExpressions(null, "Table", new string[0]));
            Assert.ThrowsException<ArgumentException>(() => new FieldExpressions(source, "", new string[0]));
            Assert.ThrowsException<ArgumentNullException>(() => new FieldExpressions(source, "Table", null));
            Assert.ThrowsException<ArgumentNullException>(() => new FieldExpressions(null, new FieldExpression[0]));
            Assert.ThrowsException<ArgumentNullException>(() => new FieldExpressions(source, null));
            Assert.ThrowsException<ArgumentException>(() => new FieldExpressions(source, "Table", new[] { "" }));
            Assert.ThrowsException<ArgumentException>(() => new FieldExpressions(source, new FieldExpression[] { null }));
        }

        [TestMethod]
        public void FieldExpressions_GetEnumerator_Empty()
        {
            // Prepare test data
            FieldExpressions expressions = new FieldExpressions();

            // Check test result
            Assert.AreEqual(0, expressions.Count());
            foreach (FieldExpression field in expressions)
                Assert.Fail();
        }

        [TestMethod]
        public void FieldExpressions_GetEnumerator_Populated()
        {
            // Prepare test data
            string[] fields = new string[] { "FieldA", "FieldB" };
            FieldExpressions expressions = new FieldExpressions(source, "Table", fields);

            // Check test result
            Assert.AreEqual(fields.Length, expressions.Count());
            int next = 0;
            foreach (FieldExpression field in expressions)
            {
                Assert.AreEqual("Table", field.TableName);
                Assert.IsTrue(fields.Any(x => x == field.FieldName));
                Assert.AreEqual($"f{next}", expressions.GetKey(field));
                next++;
            }
        }

        [TestMethod]
        public void FieldExpressions_Add_SameKey()
        {
            // Prepare test data
            FieldExpressions expressions = new FieldExpressions();
            FieldExpression expression = new FieldExpression(source, "Table", "Field");

            // Perform the test operation
            string a = expressions.Add(expression);
            string b = expressions.Add(expression);

            // Check test result
            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void FieldExpressions_GetKey_Exceptions()
        {
            FieldExpressions expressions = new FieldExpressions();
            Assert.ThrowsException<ArgumentNullException>(() => expressions.GetKey(null));
            Assert.ThrowsException<KeyNotFoundException>(() => expressions.GetKey(new FieldExpression(source, "Table", "Field")));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FieldExpressions_Add_Exception()
        {
            FieldExpressions expressions = new FieldExpressions();
            expressions.Add(null);
        }
    }
}
