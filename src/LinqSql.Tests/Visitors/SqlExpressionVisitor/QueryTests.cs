using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    [TestClass]
    public class QueryTests
    {
        private static readonly Dictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "p0", "Hello" },
            { "p1", "World" }
        };
        private Query item = new Query("sql", parameters);

        [TestMethod]
        public void Query_Properties()
        {
            Assert.AreEqual("sql", item.Sql);
            CollectionAssert.AreEqual(parameters.ToArray(), item.Parameters.ToArray());
        }
    }
}
