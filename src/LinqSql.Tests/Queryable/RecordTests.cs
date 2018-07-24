using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    [TestClass]
    public class RecordTests
    {
        [TestMethod]
        public void Record_Properties()
        {
            // Prepare test data
            Dictionary<string, RecordItem> data = new Dictionary<string, RecordItem>()
            {
                { "Table", new RecordItem("Table") }
            };
            Record record = new Record(data);

            // Perform the test operation
            CollectionAssert.AreEqual(data, record);
        }
    }
}
