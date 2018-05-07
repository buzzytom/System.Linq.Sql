using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    [TestClass]
    public class SqlVisitorContextTests
    {
        private SqlVisitorContext context = new SqlVisitorContext();

        [TestMethod]
        public void SqlVisitorContext_CreateParameter()
        {
            // Prepare test data
            object a = new object();
            object b = new object();

            // Perform test operation
            string keyA = context.CreateParameter(a);
            string keyB = context.CreateParameter(b);
            string keyC = context.CreateParameter(a);

            // Check test result
            Assert.AreEqual("p0", keyA);
            Assert.AreEqual("p1", keyB);
            Assert.AreEqual(keyA, keyC);
            Assert.AreEqual(a, context.Parameters[keyA]);
            Assert.AreEqual(b, context.Parameters[keyB]);
        }

        [TestMethod]
        public void SqlVisitorContext_Clear()
        {
            // Prepare test data
            object a = new object();
            context.CreateParameter(a);
            Assert.AreEqual(1, context.Parameters.Count);

            // Perform test operation
            context.Clear();

            // Check test result
            Assert.AreEqual(0, context.Parameters.Count);
        }

        [TestMethod]
        public void SqlVisitorContext_GetSource()
        {
            // Prepare test data
            TableExpression expressionA = new TableExpression("Table", "Alias", new string[] { "FieldA" });
            TableExpression expressionB = new TableExpression("Table", "Alias", new string[] { "FieldA" });

            // Perform the test operation
            string aliasA = context.GetSource(expressionA);
            string aliasB = context.GetSource(expressionB);

            // Check the test result
            Assert.AreEqual("t0", aliasA);
            Assert.AreEqual("t1", aliasB);
        }

        [TestMethod]
        public void SqlVisitorContext_GetSource_DuplicatedCall()
        {
            // Prepare test data
            TableExpression expression = new TableExpression("Table", "Alias", new string[] { "FieldA" });

            // Perform the test operation
            string aliasA = context.GetSource(expression);
            string aliasB = context.GetSource(expression);

            // Check the test result
            Assert.AreEqual(aliasA, aliasB);
        }
    }
}
