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
    }
}
