using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqSql.Expressions.Tests
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
            ASourceExpression source = new TableExpression("Table", "Alias", fields);
            FieldExpressions expressions = new FieldExpressions(source, fields);

            // Check test result
            Assert.AreEqual(fields.Length, expressions.Count());
            int next = 0;
            foreach (FieldExpression field in expressions)
            {
                Assert.AreSame(source, field.Source);
                Assert.IsTrue(fields.Any(x => x == field.FieldName));
                Assert.AreEqual($"f{next}", field.Alias);
                next++;
            }
        }
    }
}
