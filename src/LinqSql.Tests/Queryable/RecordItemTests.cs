using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Queryable.Tests
{
    [TestClass]
    public class RecordItemTests
    {
        [TestMethod]
        public void RecordItem_Properties()
        {
            // Prepare test data
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "Id", 1 },
                { "Name", "Some Name" }
            };
            RecordItem item = new RecordItem("Table", data);

            // Perform the test operation
            Assert.AreEqual("Table", item.Key);
            CollectionAssert.AreEqual(data, item);
        }
    }
}
