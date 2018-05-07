using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Expressions.Tests
{
    [TestClass]
    public class FieldExpressionsTests
    {
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
            FieldExpressions expressions = new FieldExpressions("Table", fields);

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
            FieldExpression expression = new FieldExpression("Table", "Field");

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
            Assert.ThrowsException<KeyNotFoundException>(() => expressions.GetKey(new FieldExpression("Table", "Field")));
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
