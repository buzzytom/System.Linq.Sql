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

        [TestMethod]
        public void SqlQueryable_Where_Boolean()
        {
            // Prepare the test data
            string[] fields = new[] { "Id", "Name" };
            IQueryable<Record> queryable = new SqliteQueryable(connection, "Course", "Alias", fields);

            // Perform the test operation
            Record[] records = queryable
                .Where(x => true)
                .ToArray();

            // Check the test result
            Assert.AreEqual(4, records.Length);
        }

        [TestMethod]
        public void SqlQueryable_Where_Comparison()
        {
            // Prepare the test data
            string[] fields = new[] { "Id", "Name" };
            IQueryable<Record> queryable = new SqliteQueryable(connection, "Course", "Alias", fields);

            // Perform the test operation
            Record[] records = queryable
                .Where(x => (int)x["Alias"]["Id"] == 1)
                .ToArray();

            // Check the test result
            Assert.AreEqual(1, records.Length);
        }

        [TestMethod]
        public void SqlQueryable_Contains_IntArray()
        {
            // Prepare the test data
            int[] values = new[] { 1, 3 };
            string[] fields = new[] { "Id", "Name" };
            IQueryable<Record> queryable = new SqliteQueryable(connection, "Course", "Alias", fields);

            // Perfor the test operation
            Record[] records = queryable
                .Where(x => values.Contains((int)x["Alias"]["Id"]))
                .ToArray();

            // Check the test result
            Assert.AreEqual(values.Length, records.Length);
            foreach (Record record in records)
                Assert.IsTrue(values.Contains(Convert.ToInt32(record["Alias"]["Id"])));
        }

        [TestMethod]
        public void SqlQueryable_Contains_StringArray()
        {
            // Prepare the test data
            string[] values = new[] { "1", "3" };
            string[] fields = new[] { "Id", "Name" };
            IQueryable<Record> queryable = new SqliteQueryable(connection, "Course", "Alias", fields);

            // Perfor the test operation
            Record[] records = queryable
                .Where(x => values.Contains((string)x["Alias"]["Id"]))
                .ToArray();

            // Check the test result
            Assert.AreEqual(values.Length, records.Length);
            foreach (Record record in records)
                Assert.IsTrue(values.Contains(Convert.ToInt32(record["Alias"]["Id"]).ToString()));
        }

        [TestMethod]
        public void SqlQueryable_Contains_SubQuery()
        {
            // Prepare the test data
            IQueryable<Record> outer = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });
            IQueryable<Record> inner = new SqliteQueryable(connection, "CourseStudent", new[] { "Id", "CourseId" });

            // Perfor the test operation
            Record[] records = outer
                .Where(x => inner.Contains(y => y["CourseStudent"]["CourseId"], x["Course"]["Id"]))
                .ToArray();

            // Check the test result
            Assert.AreEqual(ConnectionTestHelper.CountCourses, records.Length);
        }

        [TestMethod]
        public void SqlQueryable_Contains_Chained()
        {
            // Prepare the test data
            IQueryable<Record> query = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });

            // Perfor the test operation
            bool result = query.Contains(x => x["Course"]["Id"], 1);

            // Check the test result
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void SqlQueryable_Join_SelectBoth()
        {
            // Prepare the test data
            IQueryable<Record> outer = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });
            IQueryable<Record> inner = new SqliteQueryable(connection, "CourseStudent", new[] { "Id", "CourseId", "StudentId" });

            // Perform the test operation
            Record[] records = outer
                .Join(inner, x => x["Course"]["Id"], x => x["CourseStudent"]["CourseId"], (o, i) => o | i)
                .ToArray();

            // Check the test result
            Assert.AreEqual(8, records.Length);
            foreach (Record record in records)
            {
                Assert.AreEqual(2, record.Count);
                Assert.IsTrue(record["Course"].ContainsKey("Id"));
                Assert.IsTrue(record["Course"].ContainsKey("Name"));
                Assert.IsTrue(record["CourseStudent"].ContainsKey("Id"));
                Assert.IsTrue(record["CourseStudent"].ContainsKey("CourseId"));
                Assert.IsTrue(record["CourseStudent"].ContainsKey("StudentId"));
                Assert.AreEqual(record["Course"]["Id"], record["CourseStudent"]["CourseId"]);
            }
        }

        [TestMethod]
        public void SqlQueryable_Join_SelectOuter()
        {
            // Prepare the test data
            IQueryable<Record> outer = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });
            IQueryable<Record> inner = new SqliteQueryable(connection, "CourseStudent", new[] { "Id", "CourseId", "StudentId" });

            // Perform the test operation
            Record[] records = outer
                .Join(inner, x => x["Course"]["Id"], x => x["CourseStudent"]["CourseId"], (o, i) => o)
                .ToArray();

            // Check the test result
            Assert.AreEqual(8, records.Length);
            foreach (Record record in records)
            {
                Assert.AreEqual(1, record.Count);
                Assert.IsTrue(record["Course"].ContainsKey("Id"));
                Assert.IsTrue(record["Course"].ContainsKey("Name"));
            }
        }

        [TestMethod]
        public void SqlQueryable_Join_SelectInner()
        {
            // Prepare the test data
            IQueryable<Record> outer = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });
            IQueryable<Record> inner = new SqliteQueryable(connection, "CourseStudent", new[] { "Id", "CourseId", "StudentId" });

            // Perform the test operation
            Record[] records = outer
                .Join(inner, x => x["Course"]["Id"], x => x["CourseStudent"]["CourseId"], (o, i) => i)
                .ToArray();

            // Check the test result
            Assert.AreEqual(8, records.Length);
            foreach (Record record in records)
            {
                Assert.AreEqual(1, record.Count);
                Assert.IsTrue(record["CourseStudent"].ContainsKey("Id"));
                Assert.IsTrue(record["CourseStudent"].ContainsKey("CourseId"));
                Assert.IsTrue(record["CourseStudent"].ContainsKey("StudentId"));
            }
        }

        [TestMethod]
        public void SqlQueryable_Join_Predicate_SelectBoth()
        {
            // Prepare the test data
            IQueryable<Record> outer = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });
            IQueryable<Record> inner = new SqliteQueryable(connection, "CourseStudent", new[] { "Id", "CourseId", "StudentId" });

            // Perform the test operation
            Record[] records = outer
                .Join(inner, (o, i) => o["Course"]["Id"] == i["CourseStudent"]["CourseId"], (o, i) => o | i)
                .ToArray();

            // Check the test result
            Assert.AreEqual(8, records.Length);
            foreach (Record record in records)
            {
                Assert.AreEqual(2, record.Count);
                Assert.IsTrue(record["Course"].ContainsKey("Id"));
                Assert.IsTrue(record["Course"].ContainsKey("Name"));
                Assert.IsTrue(record["CourseStudent"].ContainsKey("Id"));
                Assert.IsTrue(record["CourseStudent"].ContainsKey("CourseId"));
                Assert.IsTrue(record["CourseStudent"].ContainsKey("StudentId"));
                Assert.AreEqual(record["Course"]["Id"], record["CourseStudent"]["CourseId"]);
            }
        }

        [TestMethod]
        public void SqlQueryable_Join_Predicate_SelectOuter()
        {
            // Prepare the test data
            IQueryable<Record> outer = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });
            IQueryable<Record> inner = new SqliteQueryable(connection, "CourseStudent", new[] { "Id", "CourseId", "StudentId" });

            // Perform the test operation
            Record[] records = outer
                .Join(inner, (o, i) => o["Course"]["Id"] == i["CourseStudent"]["CourseId"], (o, i) => o)
                .ToArray();

            // Check the test result
            Assert.AreEqual(8, records.Length);
            foreach (Record record in records)
            {
                Assert.AreEqual(1, record.Count);
                Assert.IsTrue(record["Course"].ContainsKey("Id"));
                Assert.IsTrue(record["Course"].ContainsKey("Name"));
            }
        }

        [TestMethod]
        public void SqlQueryable_Join_Predicate_SelectInner()
        {
            // Prepare the test data
            IQueryable<Record> outer = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });
            IQueryable<Record> inner = new SqliteQueryable(connection, "CourseStudent", new[] { "Id", "CourseId", "StudentId" });

            // Perform the test operation
            Record[] records = outer
                .Join(inner, (o, i) => o["Course"]["Id"] == i["CourseStudent"]["CourseId"], (o, i) => i)
                .ToArray();

            // Check the test result
            Assert.AreEqual(8, records.Length);
            foreach (Record record in records)
            {
                Assert.AreEqual(1, record.Count);
                Assert.IsTrue(record["CourseStudent"].ContainsKey("Id"));
                Assert.IsTrue(record["CourseStudent"].ContainsKey("CourseId"));
                Assert.IsTrue(record["CourseStudent"].ContainsKey("StudentId"));
            }
        }

        [TestMethod]
        public void SqlQueryable_Skip()
        {
            // Prepare the test data
            IQueryable<Record> query = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });

            // Perform the test operation
            Record[] records = query
                .Skip(1)
                .ToArray();

            // Check the test result
            Assert.AreEqual(ConnectionTestHelper.CountCourses - 1, records.Length);
            foreach (Record record in records)
            {
                Assert.AreEqual(1, record.Count);
                Assert.IsTrue(record["Course"].ContainsKey("Id"));
                Assert.IsTrue(record["Course"].ContainsKey("Name"));
            }
        }

        [TestMethod]
        public void SqlQueryable_Take()
        {
            // Prepare the test data
            IQueryable<Record> query = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });

            // Perform the test operation
            Record[] records = query
                .Take(2)
                .ToArray();

            // Check the test result
            Assert.AreEqual(2, records.Length);
            foreach (Record record in records)
            {
                Assert.AreEqual(1, record.Count);
                Assert.IsTrue(record["Course"].ContainsKey("Id"));
                Assert.IsTrue(record["Course"].ContainsKey("Name"));
            }
        }

        [TestMethod]
        public void SqlQueryable_TakeSkip()
        {
            // Prepare the test data
            IQueryable<Record> query = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });

            // Perform the test operation
            Record[] records = query
                .Take(4)
                .Skip(1)
                .ToArray();

            // Check the test result
            Assert.AreEqual(3, records.Length);
            foreach (Record record in records)
            {
                Assert.AreEqual(1, record.Count);
                Assert.IsTrue(record["Course"].ContainsKey("Id"));
                Assert.IsTrue(record["Course"].ContainsKey("Name"));
            }
        }

        [TestMethod]
        public void SqlQueryable_SkipTake()
        {
            // Prepare the test data
            IQueryable<Record> source = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });

            // Perform the test operation
            Record[] records = source
                .Skip(1)
                .Take(2)
                .ToArray();

            // Check the test result
            Assert.AreEqual(2, records.Length);
            foreach (Record record in records)
            {
                Assert.AreEqual(1, record.Count);
                Assert.IsTrue(record["Course"].ContainsKey("Id"));
                Assert.IsTrue(record["Course"].ContainsKey("Name"));
            }
        }

        [TestMethod]
        public void SqlQueryable_Average()
        {
            // Prepare the test data
            IQueryable<Record> query = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });

            // Perform the test operations
            int averageInt = query.Average(x => (int)x["Course"]["Id"]);
            long averageLong = query.Average(x => (long)x["Course"]["Id"]);
            decimal averageDecimal = query.Average(x => (decimal)x["Course"]["Id"]);
            float averageFloat = query.Average(x => (float)x["Course"]["Id"]);
            double averageDouble = query.Average(x => (double)x["Course"]["Id"]);

            // Check the test results
            Assert.AreEqual(2, averageInt);
            Assert.AreEqual(2L, averageLong);
            Assert.AreEqual(2.5m, averageDecimal);
            Assert.AreEqual(2.5f, averageFloat);
            Assert.AreEqual(2.5, averageDouble);
        }

        [TestMethod]
        public void SqlQueryable_Count()
        {
            // Prepare the test data
            IQueryable<Record> query = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });

            // Perform the test operation
            int count = query.Count();

            // Check the test result
            Assert.AreEqual(ConnectionTestHelper.CountCourses, count);
        }

        [TestMethod]
        public void SqlQueryable_Count_Predicate()
        {
            // Prepare the test data
            IQueryable<Record> query = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });

            // Perform the test operation
            int count = query.Count(x => (int)x["Course"]["Id"] == 2 || (int)x["Course"]["Id"] == 3);

            // Check the test result
            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public void SqlQueryable_Max()
        {
            // Prepare the test data
            IQueryable<Record> query = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });

            // Perform the test operation
            int count = query.Max(x => (int)x["Course"]["Id"]);

            // Check the test result
            Assert.AreEqual(4, count);
        }

        [TestMethod]
        public void SqlQueryable_Min()
        {
            // Prepare the test data
            IQueryable<Record> query = new SqliteQueryable(connection, "Course", new[] { "Id", "Name" });

            // Perform the test operation
            int count = query.Min(x => (int)x["Course"]["Id"]);

            // Check the test result
            Assert.AreEqual(1, count);
        }
    }
}
