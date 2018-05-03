using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqSql.Queryable.Tests
{
#if DEBUG
    [TestClass]
    public class CommandFieldTests
    {
        [TestMethod]
        public void CommandField_Properties()
        {
            // Prepare test data
            CommandField item = new CommandField("Table", "Field", 2);

            // Perform the test operation
            Assert.AreEqual("Table", item.Table);
            Assert.AreEqual("Field", item.FieldName);
            Assert.AreEqual(2, item.Ordinal);
        }
    }
#endif
}
