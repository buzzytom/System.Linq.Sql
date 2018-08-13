using System.Data.Common;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    using Sqlite;

    [TestClass]
    public class SqlQueryableTests
    {
        private readonly DbConnection connection = ConnectionTestHelper.CreatePopulatedConnection();

        [TestMethod]
        public void SqlQueryable_Properties()
        {
            // Prepare the test data
            SqlQueryable item = new SqliteQueryable(connection, "Table", "Alias", new string[] { "FieldA" });

            // Check the test result
            Assert.AreEqual(typeof(Record), item.ElementType);
            Assert.IsInstanceOfType(item.Provider, typeof(SqlQueryableProvider));
            Assert.IsInstanceOfType(item.Expression, typeof(TableExpression));
        }

        [TestMethod]
        public void SqlQueryable_Simple()
        {
            // Prepare the test data
            string[] fields = new[] { "Id", "Name" };
            IQueryable<Record> queryable = new SqliteQueryable(connection, "Course", "Alias", fields);

            // Perform the test operation
            Record[] records = queryable.ToArray();

            // Check the test result
            Assert.AreEqual(ConnectionTestHelper.CountCourses, records.Length);
        }

        [TestMethod]
        public void SqlQueryable_NullSource()
        {
            // Prepare the test data
            FieldExpression literal = new FieldExpression(new LiteralExpression(42), "Table", "Literal");
            FieldExpression boolean = new FieldExpression(new BooleanExpression(true), "Table", "Boolean");
            SelectExpression expression = new SelectExpression(null, new[] { literal, boolean });
            SqliteQueryableProvider provider = new SqliteQueryableProvider(connection);

            // Perform the test operation
            Record[] result = provider
                .CreateQuery<Record>(expression)
                .ToArray();

            // Check the test result
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(42L, result[0]["Table"]["Literal"]);
            Assert.AreEqual(1L, result[0]["Table"]["Boolean"]);
        }
    }
}
