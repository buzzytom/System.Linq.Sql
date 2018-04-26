using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqSql.Expressions.Tests
{
    [TestClass]
    public class TableExpressionTests
    {
        private static readonly string[] fields = new string[] { "FieldA", "FieldB" };
        private TableExpression expression = new TableExpression(fields, "Table", "Alias");

        [TestMethod]
        public void TableExpression_Properties()
        {
            Assert.AreEqual("Table", expression.Table);
            Assert.AreEqual("Alias", expression.Alias);
            CollectionAssert.AreEquivalent(fields, expression.Fields.ToArray());
        }
    }
}
