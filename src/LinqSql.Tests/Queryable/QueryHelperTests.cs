using System;
using System.Collections.Generic;
using System.Data.Common;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    [TestClass]
    public class QueryHelperTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void QueryHelper_SelectRecordItems_ArgumentNullException()
        {
            QueryHelper.SelectRecordItems(null);
        }

        [TestMethod]
        public void QueryHelper_SelectRecordItems()
        {
            // Prepare the test data
            DbConnection connection = ConnectionTestHelper.CreatePopulatedConnection();
            SqlQueryable query = new SqlQueryable(connection, "Course", new string[] { "Id", "Name" });

            // Perform the test operation
            RecordItem[] items = query
                .SelectRecordItems()
                .ToArray();

            // Check the test result
            Assert.AreEqual(ConnectionTestHelper.CountCourses, items.Length);
            foreach (RecordItem item in items)
            {
                Assert.IsTrue(item.ContainsKey("Id"));
                Assert.IsTrue(item.ContainsKey("Name"));
                Assert.IsTrue(typeof(long).IsAssignableFrom(item["Id"].GetType()));
                Assert.IsInstanceOfType(item["Name"], typeof(string));
            }
        }

        [TestMethod]
        public void QueryHelper_SelectRecordItems_Table_Exceptions()
        {
            Assert.ThrowsException<ArgumentNullException>(() => QueryHelper.SelectRecordItems(null, "TableName"));
            Assert.ThrowsException<ArgumentException>(() => QueryHelper.SelectRecordItems(new Record[0], ""));
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void QueryHelper_SelectRecordItems_Table_KeyNotFoundException()
        {
            // Prepare the test data
            DbConnection connection = ConnectionTestHelper.CreatePopulatedConnection();
            SqlQueryable query = new SqlQueryable(connection, "Course", new string[] { "Id", "Name" });

            // Perform the test operation
            query
                .SelectRecordItems("NotATable")
                .ToArray();
        }

        [TestMethod]
        public void QueryHelper_SelectRecordItems_Table()
        {
            // Prepare the test data
            DbConnection connection = ConnectionTestHelper.CreatePopulatedConnection();
            SqlQueryable query = new SqlQueryable(connection, "Course", new string[] { "Id", "Name" });

            // Perform the test operation
            RecordItem[] items = query
                .SelectRecordItems("Course")
                .ToArray();

            // Check the test result
            Assert.AreEqual(ConnectionTestHelper.CountCourses, items.Length);
            foreach (RecordItem item in items)
            {
                Assert.IsTrue(item.ContainsKey("Id"));
                Assert.IsTrue(item.ContainsKey("Name"));
                Assert.IsTrue(typeof(long).IsAssignableFrom(item["Id"].GetType()));
                Assert.IsInstanceOfType(item["Name"], typeof(string));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void QueryHelper_Flatten_ArgumentNullException()
        {
            QueryHelper.Flatten(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void QueryHelper_Flatten_Collision()
        {
            // Prepare the test data
            Record[] records = new Record[]
            {
                new Record(new Dictionary<string, RecordItem>()
                {
                    {
                        "TableA",
                        new RecordItem("TableA", new Dictionary<string, object>()
                        {
                            { "Id", 1 },
                            { "Name", "some name 1" }
                        })
                    },
                    {
                        "TableB",
                        new RecordItem("TableB", new Dictionary<string, object>()
                        {
                            { "Id", 2 },
                            { "Name", "some name 2" }
                        })
                    }
                })
            };

            // Perform the test operation
            records
                .Flatten()
                .ToArray();
        }

        [TestMethod]
        public void QueryHelper_Flatten()
        {
            // Prepare the test data
            Record[] records = new Record[]
            {
                new Record(new Dictionary<string, RecordItem>()
                {
                    {
                        "TableA",
                        new RecordItem("TableA", new Dictionary<string, object>()
                        {
                            { "A", 1 },
                            { "B", "some name 1" }
                        })
                    },
                    {
                        "TableB",
                        new RecordItem("TableB", new Dictionary<string, object>()
                        {
                            { "C", 2 },
                            { "D", "some name 2" }
                        })
                    }
                })
            };

            // Perform the test operation
            Dictionary<string, object>[] lookups = records
                .Flatten()
                .ToArray();

            // Check the test result
            KeyValuePair<string, object>[] expected = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>("A", 1),
                new KeyValuePair<string, object>("B", "some name 1"),
                new KeyValuePair<string, object>("C", 2),
                new KeyValuePair<string, object>("D", "some name 2")
            };
            Assert.AreEqual(1, lookups.Length);
            CollectionAssert.AreEquivalent(expected, lookups.FirstOrDefault());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void QueryHelper_GetScalar_ArgumentNullException()
        {
            QueryHelper.GetScalar<int>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void QueryHelper_GetScalar_InvalidOperationException_RecordCount()
        {
            QueryHelper.GetScalar<int>(new Record[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void QueryHelper_GetScalar_InvalidOperationException_RecordItemCount()
        {
            // Prepare the test data
            Record[] records = new Record[]
            {
                new Record(new Dictionary<string, RecordItem>())
            };

            // Perform the test operation
            QueryHelper.GetScalar<int>(records);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void QueryHelper_GetScalar_InvalidOperationException_ColumnCount()
        {
            // Prepare the test data
            Record[] records = new Record[]
            {
                new Record(new Dictionary<string, RecordItem>()
                {
                    {
                        "TableA",
                        new RecordItem("TableA", new Dictionary<string, object>())
                    }
                })
            };

            // Perform the test operation
            QueryHelper.GetScalar<int>(records);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void QueryHelper_GetScalar_InvalidOperationException_NullVaueType()
        {
            // Prepare the test data
            Record[] records = new Record[]
            {
                new Record(new Dictionary<string, RecordItem>()
                {
                    {
                        "TableA",
                        new RecordItem("TableA", new Dictionary<string, object>()
                        {
                            { "A", null }
                        })
                    }
                })
            };

            // Perform the test operation
            QueryHelper.GetScalar<int>(records);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void QueryHelper_GetScalar_Exception_Conversion()
        {
            // Prepare the test data
            Record[] records = new Record[]
            {
                new Record(new Dictionary<string, RecordItem>()
                {
                    {
                        "TableA",
                        new RecordItem("TableA", new Dictionary<string, object>()
                        {
                            { "A", "Hello World!" }
                        })
                    }
                })
            };

            // Perform the test operation
            QueryHelper.GetScalar<int>(records);
        }

        [TestMethod]
        public void QueryHelper_GetScalar_Null()
        {
            // Prepare the test data
            Record[] records = new Record[]
            {
                new Record(new Dictionary<string, RecordItem>()
                {
                    {
                        "TableA",
                        new RecordItem("TableA", new Dictionary<string, object>()
                        {
                            { "A", null }
                        })
                    }
                })
            };

            // Perform the test operation
            string result = QueryHelper.GetScalar<string>(records);

            // Check the test result
            Assert.IsNull(result);
        }

        [TestMethod]
        public void QueryHelper_GetScalar()
        {
            // Prepare the test data
            Record[] records = new Record[]
            {
                new Record(new Dictionary<string, RecordItem>()
                {
                    {
                        "TableA",
                        new RecordItem("TableA", new Dictionary<string, object>()
                        {
                            { "A", "some thing 1" },
                            { "B", "some name 1" }
                        })
                    },
                    {
                        "TableB",
                        new RecordItem("TableB", new Dictionary<string, object>()
                        {
                            { "C", 2 },
                            { "D", "some name 2" }
                        })
                    }
                })
            };

            // Perform the test operation
            string result = records.GetScalar<string>();

            // Check the test result
            Assert.AreEqual("some thing 1", result);
        }
    }
}
